using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithms
{
    public class InverseDFTModel: IDisposable
    {
        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                }
            }
            this.disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            //GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Check the each schema and return the fx value from based on the pattern it can be accomdated to
        /// </summary>
        /// <param name="schemaInstance"></param>
        /// <param name="patternList"></param>
        /// <returns></returns>
        public double GetFxByWildcardCharacterCheck(string schemaInstance, List<string> patternList, string classLabel)
        {
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
        public Dictionary<string, double> CalculateFxByPatternDirectly(List<string> uniqueSchemaList, List<string> patternList, string classLabel)
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
        public double GetCoeffInverseDft(string xVector, List<string> jPatterns, Dictionary<string, double> coeffArray)
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
        public Dictionary<string, double> GetFxByInverseDFT(List<string> allSchemaSxClass0, List<string> sjVectors, Dictionary<string, double> coeffDft)
        {
            var fxs = new Dictionary<string, double>();
            foreach (string x in allSchemaSxClass0)
            {
                double coeff = GetCoeffInverseDft(x, sjVectors, coeffDft);
                fxs[x] = coeff;
            }
            return fxs;
        }



    }
}
