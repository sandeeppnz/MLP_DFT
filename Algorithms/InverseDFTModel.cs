using System;
using System.Collections.Generic;
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
        public double GetFxByWildcardCharacterCheck(string schemaInstance, List<string> patternList, string classLabel)
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
        public Dictionary<string, double> ValidateFxByClusterPatternMatching(List<string> uniqueSchemaList, List<string> patternList, string classLabel)
        {
            var fxArray = new Dictionary<string, double>();
            foreach (string schemaInstance in uniqueSchemaList)
            {
                double coeff = GetFxByWildcardCharacterCheck(schemaInstance, patternList, classLabel);
                fxArray[schemaInstance] = coeff;
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
        public double CalculateFxByInveseDftEquation(string xVector, List<string> jPatterns, Dictionary<string, double> coeffArray)
        {
            double fx = 0.0;
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
        public Dictionary<string, double> ValidateFxByInverseDFT(List<string> allSchemaSxClass0, List<string> sjVectors, Dictionary<string, double> coeffDft)
        {
            //Calculate the f(x) by inverse dft equation
            var fxs = new Dictionary<string, double>();
            foreach (string x in allSchemaSxClass0)
            {
                double fx = CalculateFxByInveseDftEquation(x, sjVectors, coeffDft);
                fxs[x] = fx;
            }
            return fxs;
        }

        //public void Validate(List<string> allSchemaXVectorClass0, List<string> allSchemaXVectorClass1, List<string> clusteredSchemaXVectorClass0, List<string> clusteredSchemaXVectorClass1, Dictionary<string,double> energyCoffs, List<string> jVectors, )
        public void Validate(SplitType splitType, decimal partitionSize)
        {
            //Calculate f(x) directly by looking at the pattern
            var fxShortcutClass0 = ValidateFxByClusterPatternMatching(_dftModel.AllSchemaXVectorClass0, _dftModel.ClusteredSchemaXVectorClass0, "0");
            var fxShortcutClass1 = ValidateFxByClusterPatternMatching(_dftModel.AllSchemaXVectorClass1, _dftModel.ClusteredSchemaXVectorClass1, "1");


            //Calculate f(x) by Inverse DFT 
            var fxClass0ByInvDFT = ValidateFxByInverseDFT(_dftModel.AllSchemaXVectorClass0, _dftModel.jVectors, _dftModel.EnergyCoeffs);
            var fxClass1ByInvDFT = ValidateFxByInverseDFT(_dftModel.AllSchemaXVectorClass1, _dftModel.jVectors, _dftModel.EnergyCoeffs);

            FileProcessor fp = _mlpModel.GetFileProcessor();
            int numInput = fp.GetInputSpecification().GetNumAttributes();

            fp.OutputModelValidationToCSV(numInput, _dftModel.AllSchemaXVectorClass0, "0", fxClass0ByInvDFT, fxShortcutClass0, true, partitionSize + "_Validation", splitType, false);
            fp.OutputModelValidationToCSV(numInput, _dftModel.AllSchemaXVectorClass1, "1", fxClass1ByInvDFT, fxShortcutClass1, false, partitionSize + "_Validation", splitType, true);

            //FileProcessor.WritesXVectorsToCsv(allSchemaSxClass1);
            fp.OutputEnergyCoeffsToCSV(_dftModel.EnergyCoeffs,splitType, partitionSize + "_EnergyCoeffs");




        }
    }
}
