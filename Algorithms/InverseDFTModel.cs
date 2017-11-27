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
                    _dftModelExt = null;
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
        DFTModelExt _dftModelExt = null;


        public InverseDFTModel(MLPModel mlpmodel, DFTModel dftModel)
        {
            _mlpModel = mlpmodel;
            _dftModel = dftModel;
        }

        public InverseDFTModel(MLPModel mlpmodel, DFTModelExt dftModelExt)
        {
            _mlpModel = mlpmodel;
            _dftModelExt = dftModelExt;
        }


        /// <summary>
        /// Check the each schema and return the fx value from based on the pattern it can be accomdated to
        /// </summary>
        /// <param name="xVector"></param>
        /// <param name="schemaPatternList"></param>
        /// <returns></returns>
        public double GetFxByWildcardCharacterCheck(string xVector, List<string> schemaPatternList, int classLabel)
        {
            //Checks for a matching pattern for the schema instance, returns the 

            double fx = -1;
            foreach (string pattern in schemaPatternList)
            {
                if (pattern.Equals(xVector))
                {
                    return double.Parse(classLabel.ToString());
                }
                else
                {
                    //it would contain wildcard character
                    bool isMatch = true; //resetting for each pattern

                    for (int j = 0; j < pattern.Length; j++)
                    {

                        if (pattern[j] != '*' && pattern[j] != xVector[j])
                        {
                            isMatch = false;
                        }
                        else
                        {
                            if (pattern[j] == '*')
                            {
                                // can be matched
                            }
                            else if (pattern[j] != xVector[j])
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
        /// <param name="jVectors"></param>
        /// <param name="coeffArray"></param>
        /// <returns></returns>
        public double CalculateFxByInveseDftEquation(string xVector, HashSet<string> jVectors, Dictionary<string, double> coeffArray)
        {
            double fx = 0;
            foreach (string j in jVectors)
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

        public double ProcessTesting(HashSet<string> xVector, int classVal)
        {
            long misClassficationCount = 0;
            misClassficationCount = 0;

            List<string> sss = xVector.ToList();

            var fxClass0ByInvDFTTest = ValidateFxByInverseDFT(sss, _dftModel.JVectorsTrain, _dftModel.EnergyCoeffsTrain, ref misClassficationCount, classVal);
            return fxClass0ByInvDFTTest[sss[0]];
        }

        public void SetABBA()
        {

        }

        public void Set0A0B()
        {

        }



        //public void RefineDFTModel()
        //{
        //    // Get the DFT model
        //    // 


        //    foreach (KeyValuePair<string, SchemaStatistics> kv3 in winnerTree.Pattern)//key is the j vector , only update coefficients which are exist in winner
        //    {
        //        //double denominator = Math.Pow(2, winnerTree.attributeIndexMapInitial.o_NodeIndexMap.Count());


        //        //test for significance of this schema in winner spectrum
        //        if (kv3.Value.isSignificant)//added on 18th Oct 2016 , after feature selection on coefficient
        //        {

        //            double dCoefficientValue_Contribution_ABBA = 0.0;//reset contribution form set AB or BA class change schemas to refinemnet
        //            double dCoefficientValue_Contribution_0A0B = 0.0; //reset contribution form set 0A or 0B class change schemas to refinemnet    
        //            double correctionFactor = 0.0;
        //            double AveragedcorrectionFactor = 0.0;


        //            #region calculate contribution form set AB or BA class change schemas to refinemnet of this coefficient

        //            foreach (KeyValuePair<string, double> kv6 in schemaClass_classChanged_ABBA)//key is the X vector
        //            {
        //                if (kv6.Value != 0)//if the class diffrence is not 0, if 0 whatever the dot product , it is zero
        //                {
        //                    double dDotProduct = (double) Helper.CalculateDotProduct(kv3.Key, kv6.Key); // j*x
        //                                                                                         //double dDotProduct = (double)FourierConversion.CalculateDotProduct(kv3.Key, kv6.Key);
        //                    if (dDotProduct != 0)
        //                    {

        //                        dCoefficientValue_Contribution_ABBA = dCoefficientValue_Contribution_ABBA + ((kv6.Value) * dDotProduct); //sum over Xs of set ABBA
        //                    }

        //                    if (dDotProduct != -1.0 && dDotProduct != 1.0)
        //                    {

        //                    }
        //                }
        //            }

        //            #endregion calculate contribution form set AB or BA class change schemas to refinemnet of this coefficient


        //            #region calculate contribution form set 0A or 0B class change schemas to refinemnet of this coefficient

        //            foreach (KeyValuePair<string, double> kv7 in schemaClass_classChanged_0A0B)//key is the X vector
        //            {
        //                if (kv7.Value != 0)//if the class difference is not 0
        //                {
        //                    double dDotProduct = (double) Helper.CalculateDotProduct(kv3.Key, kv7.Key); // j*x
        //                                                                                         //double dDotProduct = (double)FourierConversion.CalculateDotProduct(kv3.Key, kv7.Key);
        //                    if (dDotProduct != 0)
        //                    {

        //                        dCoefficientValue_Contribution_0A0B = dCoefficientValue_Contribution_0A0B + ((kv7.Value) * dDotProduct); //sum over Xs of set 0A0B
        //                    }

        //                    if (dDotProduct != -1.0 && dDotProduct != 1.0)
        //                    {

        //                    }
        //                }
        //            }

        //            #endregion calculate contribution form set 0A or 0B class change schemas to refinemnet of this coefficient

        //            //correctionFactor = dCoefficientValue_Contribution_ABBA + (Alpha * dCoefficientValue_Contribution_0A0B);
        //            correctionFactor = dCoefficientValue_Contribution_ABBA + dCoefficientValue_Contribution_0A0B;
        //            //AveragedcorrectionFactor = correctionFactor / System.Convert.ToDouble(reservoirInstanceCount);

        //            //AveragedcorrectionFactor = correctionFactor / denominator;
        //            AveragedcorrectionFactor = correctionFactor / distinctSchemaWithinInterval.Count();
        //            //AveragedcorrectionFactor = (correctionFactor * changeFrequencyCountInThisInterval) / intervalSizeForRefinement;


        //            //AveragedcorrectionFactor = correctionFactor / denominator;
        //            kv3.Value.coefficientValue = kv3.Value.coefficientValue + AveragedcorrectionFactor;

        //            if (kv3.Value.coefficientValue == 0)
        //            {
        //                zeroCoefficientcount++;
        //            }

        //            if (zeroCoefficientcount == winnerTree.Pattern.Count())
        //            {
        //                //trouble---- solution below

        //                //find new winner 
        //                //delete this current winner from pool
        //                //rest winner reservoir
        //                //reset matching count
        //            }


        //        }
        //        else
        //        {
        //            //insignificant coefficient schema of winner , update is not necesary , we do not use these for f(x) calculations
        //        }

        //    }

        //}





    }
}
