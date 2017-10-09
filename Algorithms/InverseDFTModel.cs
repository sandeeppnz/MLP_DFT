using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithms
{
    public class InverseDFTModel : IDisposable
    {
        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    //_fileProcessor = null;
                    _mlpModel = null;
                    _dftModel = null;
                }
            }
            this.disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            //GC.SuppressFinalize(this);
        }


        //IFileProcessor _fileProcessor;
        MLPModel _mlpModel = null;
        DFTModel _dftModel = null;


        public InverseDFTModel(MLPModel mlpmodel, DFTModel dftModel)
        {
            //_fileProcessor = fp;
            _mlpModel = mlpmodel;
            _dftModel = dftModel;
        }


        /// <summary>
        /// Check the each schema and return the fx value from based on the pattern it can be accomdated to
        /// </summary>
        /// <param name="schemaInstance"></param>
        /// <param name="patternList"></param>
        /// <returns></returns>
        public double GetFxByWildcardCharacterCheck(string schemaInstance, List<string> patternList, int classLabel)
        {
            //Checks for a matching pattern for the schema instance, returns the 

            double fx = -1;
            foreach (string pattern in patternList)
            {
                if (pattern.Equals(schemaInstance))
                {
                    return double.Parse(classLabel.ToString());
                }
                else
                {
                    //it would contain wildcard character
                    bool isMatch = true; //resetting for each pattern

                    for (int j = 0; j < pattern.Length; j++)
                    {

                        if (pattern[j] != '*' && pattern[j] != schemaInstance[j])
                        {
                            isMatch = false;
                        }
                        else
                        {
                            if (pattern[j] == '*')
                            {
                                // can be matched
                            }
                            else if (pattern[j] != schemaInstance[j])
                            {
                                isMatch = false;
                                return fx;
                            }
                            else
                            {
                                //pattern[j] == schemaInstance[j]
                            }
                        }
                    }

                    if (isMatch)
                    {
                        return double.Parse(classLabel.ToString());
                    }
                }
            }
            return fx;
        }




        /// <summary>
        /// </summary>
        /// <param name="uniqueSchemaList"></param>
        /// <param name="patternList"></param>
        /// <returns></returns>
        public Dictionary<string, double> ValidateFxByClusterPatternMatching(List<string> uniqueSchemaList, List<string> patternList, ref long misClassficationCount, int classLabel)
        {
            var fxArray = new Dictionary<string, double>();
            foreach (string schemaInstance in uniqueSchemaList)
            {
                double fx = GetFxByWildcardCharacterCheck(schemaInstance, patternList, classLabel);
                if (classLabel != fx)
                {
                    misClassficationCount++;
                }

                fxArray[schemaInstance] = fx;
            }
            return fxArray;
        }


        /// <summary>
        /// Calculate the f(x) value i.e. Inverse of DFT
        /// </summary>
        /// <param name="xVector"></param>
        /// <param name="jPatterns"></param>
        /// <param name="coeffArray"></param>
        /// <returns></returns>
        public double CalculateFxByInveseDftEquation(string xVector, HashSet<string> jPatterns, Dictionary<string, double> coeffArray)
        {
            double fx = 0;
            foreach (string j in jPatterns)
            {
                double dotProduct = Helper.CalculateDotProduct(j, xVector);
                double coeff = coeffArray[j];
                fx += dotProduct * coeff;
            }
            return fx;
        }



        /// <summary>
        // Calculate Inverse DFT for each sXVectvor
        // Input: CoffArray, AllSjVectors,  AllSxVectors in the patterns
        // Output: f(x)  
        /// </summary>
        /// <param name="allSchemaSxClass0"></param>
        /// <param name="sjVectors"></param>
        /// <param name="coeffsDFT"></param>
        /// <returns></returns>
        public Dictionary<string, double> ValidateFxByInverseDFT(List<string> allSchemaSxClass0, HashSet<string> sjVectors, Dictionary<string, double> coeffDft, ref long numMisclassfication, int classValue)
        {
            //Calculate the f(x) by inverse dft equation
            var fxs = new Dictionary<string, double>();
            foreach (string x in allSchemaSxClass0)
            {
                double fx = CalculateFxByInveseDftEquation(x, sjVectors, coeffDft);

                //approximation of class 
                if (fx < 0.5)
                {
                    fx = 0; //even of the instance vector
                }
                else
                {
                    fx = 1;
                }

                if (classValue != fx)
                {
                    numMisclassfication++;
                }

                fxs[x] = fx;
            }
            return fxs;
        }



        //public void Validate(List<string> allSchemaXVectorClass0, List<string> allSchemaXVectorClass1, List<string> clusteredSchemaXVectorClass0, List<string> clusteredSchemaXVectorClass1, Dictionary<string,double> energyCoffs, List<string> jVectors, )
        public void Validate(SplitType splitType, decimal partitionSize)
        {
            Console.WriteLine("Calculate f(x) directly by looking at the pattern...");

            long misClassficationCount = 0;
            Console.WriteLine("Train");
            Stopwatch sw3 = new Stopwatch();
            sw3.Start();
            var fxShortcutClass0Train = ValidateFxByClusterPatternMatching(_dftModel.AllSchemaXVectorClass0Train, _dftModel.ClusteredSchemaXVectorClass0Train, ref misClassficationCount, 0);
            var fxShortcutClass1Train = ValidateFxByClusterPatternMatching(_dftModel.AllSchemaXVectorClass1Train, _dftModel.ClusteredSchemaXVectorClass1Train, ref misClassficationCount, 1);
            sw3.Stop();
            long totalSchemas = fxShortcutClass0Train.Count + fxShortcutClass1Train.Count;
            double error = (double)misClassficationCount / (double)totalSchemas;
            error = (double)misClassficationCount / (double)totalSchemas;
            _dftModel.Shortcut_ClusterPatternMachingTrainDataAccuracy = 1.0 - error;
            _dftModel.Shortcut_ClusterPatternMachingTrainDataTime = sw3.Elapsed.TotalSeconds;




            Console.WriteLine("Test");
            Stopwatch sw2 = new Stopwatch();
            sw2.Start();
            misClassficationCount = 0;
            var fxShortcutClass0Test = ValidateFxByClusterPatternMatching(_dftModel.AllSchemaXVectorClass0Test, _dftModel.ClusteredSchemaXVectorClass0Train, ref misClassficationCount, 0);
            var fxShortcutClass1Test = ValidateFxByClusterPatternMatching(_dftModel.AllSchemaXVectorClass1Test, _dftModel.ClusteredSchemaXVectorClass1Train, ref misClassficationCount, 1);
            sw2.Stop();
            totalSchemas = fxShortcutClass0Test.Count + fxShortcutClass1Test.Count;
            error = (double)misClassficationCount / (double)totalSchemas;
            error = (double)misClassficationCount / (double)totalSchemas;
            _dftModel.Shortcut_ClusterPatternMachingTestDataAccuracy = 1.0 - error;
            _dftModel.Shortcut_ClusterPatternMachingTestDataTime = sw2.Elapsed.TotalSeconds;


            //
            Console.WriteLine("Calculate f(x) by Inverse DFT ...");
            Console.WriteLine("Train");
            Stopwatch sw4 = new Stopwatch();
            sw4.Start();
            misClassficationCount = 0;
            var fxClass0ByInvDFTTrain = ValidateFxByInverseDFT(_dftModel.AllSchemaXVectorClass0Train, _dftModel.JVectorsTrain, _dftModel.EnergyCoeffsTrain, ref misClassficationCount, 0);
            var fxClass1ByInvDFTTrain = ValidateFxByInverseDFT(_dftModel.AllSchemaXVectorClass1Train, _dftModel.JVectorsTrain, _dftModel.EnergyCoeffsTrain, ref misClassficationCount, 1);
            sw4.Stop();
            totalSchemas = fxClass0ByInvDFTTrain.Count + fxClass1ByInvDFTTrain.Count;
            error = (double)misClassficationCount / (double)totalSchemas;
            _dftModel.DFTModelTrainDataAccuracy = 1.0 - error;
            _dftModel.DFTModelTrainDataTime = sw4.Elapsed.TotalSeconds;



            Console.WriteLine("Test");
            Stopwatch sw = new Stopwatch();
            sw.Start();
            misClassficationCount = 0;
            var fxClass0ByInvDFTTest = ValidateFxByInverseDFT(_dftModel.AllSchemaXVectorClass0Test, _dftModel.JVectorsTrain, _dftModel.EnergyCoeffsTrain, ref misClassficationCount, 0);
            var fxClass1ByInvDFTTest = ValidateFxByInverseDFT(_dftModel.AllSchemaXVectorClass1Test, _dftModel.JVectorsTrain, _dftModel.EnergyCoeffsTrain, ref misClassficationCount, 1);
            sw.Stop();
            totalSchemas = fxClass0ByInvDFTTest.Count + fxClass1ByInvDFTTest.Count;
            error = (double)misClassficationCount / (double)totalSchemas;
            _dftModel.DFTModelTestDataAccuracy = 1.0 - error;
            _dftModel.DFTModelTestDataTime = sw.Elapsed.TotalSeconds;


            Console.WriteLine("done inverse...");

            FileProcessor fp = _mlpModel.GetFileProcessor();
            int numInput = fp.GetInputSpecification().GetNumAttributes();

            //fp.OutputModelValidationToCSV(numInput, _dftModel.AllSchemaXVectorClass0Train, "0", fxClass0ByInvDFTTrain, fxShortcutClass0Train, true, partitionSize + "_DFTValidation_Train", splitType, false);
            //fp.OutputModelValidationToCSV(numInput, _dftModel.AllSchemaXVectorClass1Train, "1", fxClass1ByInvDFTTrain, fxShortcutClass1Train, false, partitionSize + "_DFTValidation_Train", splitType, true);
            
            //fp.OutputModelValidationToCSV(numInput, _dftModel.AllSchemaXVectorClass0Test, "0", fxClass0ByInvDFTTest, fxShortcutClass0Test, true, partitionSize + "_DFTValidation_Test", splitType, false);
            //fp.OutputModelValidationToCSV(numInput, _dftModel.AllSchemaXVectorClass1Test, "1", fxClass1ByInvDFTTest, fxShortcutClass1Test, false, partitionSize + "_DFTValidation_Test", splitType, true);

            //fp.OutputEnergyCoeffsToCSV(_dftModel.EnergyCoeffsTrain, splitType, partitionSize + "_EnergyCoeffs");




        }
    }
}
