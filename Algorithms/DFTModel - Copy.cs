using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Algorithms
{
    public class DFT
    {
        /// <summary>
        // Calculate Inverse DFT for each sXVectvor
        // Input: CoffArray, AllSjVectors,  AllSxVectors in the patterns
        // Output: f(x)  
        /// </summary>
        /// <param name="allSchemaSxClass0"></param>
        /// <param name="sjVectors"></param>
        /// <param name="coeffsDFT"></param>
        /// <returns></returns>
        public static Dictionary<string, double> GetFxByInverseDFT(List<string> allSchemaSxClass0, List<string> sjVectors, Dictionary<string, double> coeffDft)
        {
            var fxs = new Dictionary<string, double>();
            foreach (string x in allSchemaSxClass0)
            {
                double coeff = DFT.GetCoeffInverseDft(x, sjVectors, coeffDft);
                fxs[x] = coeff;
            }
            return fxs;
        }


        /// <summary>
        /// </summary>
        /// <param name="uniqueSchemaList"></param>
        /// <param name="patternList"></param>
        /// <returns></returns>
        public static Dictionary<string, double> CalculateFxByPatternDirectly(List<string> uniqueSchemaList, List<string> patternList, string classLabel)
        {
            var fxArray = new Dictionary<string, double>();
            foreach (string schemaInstance in uniqueSchemaList)
            {
                double coeff = DFT.GetFxByWildcardCharacterCheck(schemaInstance, patternList, classLabel);
                fxArray[schemaInstance] = coeff;
            }
            return fxArray;
        }


        /// <summary>
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="decimals"></param>
        /// <returns></returns>
        public static double[] ShowVectorWInput(int numInput, int numHidden, int numOutput, double[] vector, int decimals)
        {
            int nodeCount = 1;
            int k = 0;

            double[] inputNodeHiddenTotalArray = new double[numInput];

            for (int i = 0; i < vector.Length; ++i)
            {
                for (int j = 0; j < numInput; ++j)
                {
                    Console.Write("Input-Node " + j + ": ");

                    for (int m = 0; m < numHidden; ++m)
                    {
                        double val = vector[k++];
                        Console.Write(val.ToString("F" + decimals) + " ");
                        inputNodeHiddenTotalArray[j] += Math.Abs(Math.Round(val, 2));
                    }
                    Console.WriteLine();
                }

                for (int j = 0; j < numHidden; ++j)
                {
                    Console.Write("Bias Hidden-Node " + j + ": ");
                    Console.Write(vector[k++].ToString("F" + decimals) + " ");
                    Console.WriteLine();
                }

                for (int j = 0; j < numHidden; ++j)
                {
                    Console.Write("Output-Node " + j + ": ");
                    for (int m = 0; m < numOutput; ++m)
                    {
                        Console.Write(vector[k++].ToString("F" + decimals) + " ");
                    }
                    Console.WriteLine();
                }


                for (int j = 0; j < numOutput; ++j)
                {
                    Console.Write("Bias Output-Node " + j + ": ");
                    Console.Write(vector[k++].ToString("F" + decimals) + " ");
                    Console.WriteLine();
                }
                break;
            }
            return inputNodeHiddenTotalArray;
        }



        /// <summary>
        /// </summary>
        /// <param name="weightsArray"></param>
        /// <param name="rankArray"></param>
        /// <returns></returns>
        public static double[] UpdateWeightsArrayByRank(int numInput, int numHidden, double[] weightsArray, double[] rankArray)
        {
            int k = 0;
            for (int i = 0; i < weightsArray.Length; ++i)
            {
                for (int j = 0; j < numInput; ++j)
                {
                    for (int m = 0; m < numHidden; ++m)
                    {
                        if (rankArray[j] > 5)
                        {

                            weightsArray[k] = 0;
                        }

                        k++;
                    }

                }
                break;
            }
            return weightsArray;
        }




        /// <summary>
        /// </summary>
        /// <param name="table"></param>
        /// <param name="rankArray"></param>
        /// <returns></returns>
        public static bool[,] SetIrrelevantVariables(int numInput, bool[,] table, double[] rankArray)
        {
            int k = 0;
            for (int i = 0; i < table.GetLength(0); i++)
            {
                for (int j = 0; j < numInput; j++)
                {
                    if (rankArray[j] >= 5)
                    {

                        table[i, j] = false;
                    }

                    k++;
                }
            }

            return table;
        }



        /// <summary>
        /// </summary>
        /// <param name="numInput"></param>
        /// <param name="totalArray"></param>
        /// <returns></returns>
        public static double[] GenerateRankArray(int numInput, double[] totalArray)
        {
            double max = -1;
            int rank = 1;

            double[] rankArray = new double[numInput];

            for (int i = 0; i < numInput; ++i)
            {
                rankArray[i] = 0;
            }

            for (int i = 0; i < numInput; ++i)
            {
                for (int j = 0; j < numInput; ++j)
                {
                    if ((totalArray[j] > max) && (rankArray[j] == 0))
                    {
                        max = totalArray[j];
                    }
                }

                //Update the rank
                for (int j = 0; j < numInput; ++j)
                {
                    if (totalArray[j] == max)
                    {
                        rankArray[j] = rank++;
                    }
                }
                max = -1;
            }

            Console.WriteLine("Weights");
            for (int i = 0; i < numInput; ++i)
            {
                Console.WriteLine(totalArray[i] + " " + rankArray[i]);
            }

            return rankArray;
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="redundantSchema"></param>
        /// <returns></returns>
        public static double[][] MakeArrayBasedSchema(int numInput, HashSet<string> redundantSchema)
        {
            /*
             * Translate schema to send to the Evaluator
             * 
             */

            double[][] array = new double[redundantSchema.Count][];

            int i = 0;

            foreach (string s in redundantSchema)
            {
                double[] wordArray = new double[numInput];

                for (int j = 0; j < numInput; j++)
                {
                    double val = s[j].ToString() == "0" ? 0 : 1;
                    wordArray[j] = val;
                }

                array[i] = wordArray;
                i++;
            }
            return array;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="xValues"></param>
        /// <param name="rankArray"></param>
        /// <param name="numInput"></param>
        /// <returns></returns>
        public static double[] SetXValueToZeroByRankCheck(double[] xValues, double[] rankArray, int numInput)
        {
            //Set zero weights if for bottom 5 weights
            int k = 0;
            for (int j = 0; j < numInput; ++j)
            {
                if (rankArray[j] > 5)
                {

                    xValues[k] = 0;
                }
                k++;
            }
            return xValues;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="rankArray"></param>
        /// <returns></returns>
        public static HashSet<string> GetUniqueRedudantSchema(int numInput, bool isFeatureSelection, double[][] data, double[] rankArray)
        {
            /*
             * Get unique schema entries
             * https://www.dotnetperls.com/bitarray
             * 
             */

            double[] xValues = new double[numInput]; // inputs
                                                     //List<string> fullList = new List<string>();
            HashSet<string> fullHashSet = new HashSet<string>();
            List<BitArray> fullList = new List<BitArray>();
            double[] yValues; // computed Y

            for (int i = 0; i < data.Length; i++)
            {
                //Ideally, pass bool[] to the BitArray()
                Array.Copy(data[i], xValues, numInput); // get x-values

                //TODO: rank array comment
                if (isFeatureSelection)
                {
                    xValues = SetXValueToZeroByRankCheck(xValues, rankArray, numInput);
                }
                //BitArray n = new BitArray(numInput);
                string s = null;
                for (int j = 0; j < numInput; j++)
                {
                    s += xValues[j].ToString();
                }
                fullHashSet.Add(s);
            }
            return fullHashSet;
        }


        /// <summary>
        /// Optimisation:
        /// If each pattern has a recurring wildcharacter in same position, the position correpsonding to the attribute can be considered a redundant feature
        /// e.g. if the 2nd attribute is redudant, and the following patterns are derived initially
        /// (**0,0*1,1*1), then w(*1*) = 0 but w(*1*) is not zero 
        /// </summary>
        /// <param name="fullPatternList"></param>
        /// <returns> Returns redundant attributes (i.e. attributes that have a wild card character) in each pattern
        /// </returns>
        public static List<int> FindRedundantAttributeFromPatterns(List<string> fullPatternList)
        {
            string p1 = fullPatternList[0];
            List<int> redundantIndexList = new List<int>();
            List<int> redudantIdenxLocalList = null;

            for (int i = 1; i < fullPatternList.Count(); i++)
            {
                string s = fullPatternList[i];
                redudantIdenxLocalList = new List<int>();

                for (int j = 0; j < s.Length; j++)
                {
                    if ((p1[j] == '*') && (s[j] == '*'))
                    {
                        redudantIdenxLocalList.Add(j + 1);

                    }
                }

                if (i != 1) // the global list will be empty in the beginning
                {
                    redundantIndexList = redundantIndexList.Intersect(redudantIdenxLocalList).ToList();
                }
                else
                {
                    redundantIndexList = redudantIdenxLocalList;
                }
            }
            return redundantIndexList;
        }





        /// <summary>
        /// GenerateTruthTable
        /// </summary>
        /// <param name="col"></param>
        /// <returns></returns>
        public static bool[,] GenerateTruthTable(int col)
        {
            bool[,] table;
            int row = (int)Math.Pow(2, col);

            table = new bool[row, col];

            int divider = row;

            // iterate by column
            for (int c = 0; c < col; c++)
            {
                divider /= 2;
                bool cell = false;
                // iterate every row by this column's index:
                for (int r = 0; r < row; r++)
                {
                    table[r, c] = cell;
                    if ((divider == 1) || ((r + 1) % divider == 0))
                    {
                        cell = !cell;
                    }
                }
            }

            return table;
        }



        /// <summary>
        /// Check the each schema and return the fx value from based on the pattern it can be accomdated to
        /// </summary>
        /// <param name="schemaInstance"></param>
        /// <param name="patternList"></param>
        /// <returns></returns>
        public static double GetFxByWildcardCharacterCheck(string schemaInstance, List<string> patternList, string classLabel)
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


        ///// <summary>
        ///// SkipCalculatingDftCofficient
        ///// </summary>
        ///// <param name="redundantAttributes"></param>
        ///// <returns></returns>
        //public static List<string> GetRedudantInstanceSchemas(int bitStringLength, List<int> positions)
        //{
        //    var tblMatrix = GenerateTruthTable(bitStringLength);
        //    var table = DeriveSjVectors(bitStringLength, tblMatrix);
        //    var redundantInstances = new List<string>();

        //    string emptyString = string.Empty;
        //    string selectionString = emptyString.PadLeft(bitStringLength, '0');


        //    foreach (int position in positions)
        //    {
        //        StringBuilder sb = new StringBuilder(selectionString);
        //        sb[position] = '1';
        //        selectionString = sb.ToString();
        //    }


        //    foreach (string s in table)
        //    {


        //        if(s == selectionString)
        //        redundantInstances.Add(s);

        //    }


        //    return redundantInstances;
        //}


        /// <summary>
        /// GetRedudantInstanceSchemas
        /// </summary>
        /// <param name="redundantAttributes"></param>
        /// <returns></returns>
        public static List<string> GetRedudantInstanceSchemas(int bitStringLength, int position)
        {
            var tblMatrix = GenerateTruthTable(bitStringLength);
            var table = DeriveSjVectors(bitStringLength, tblMatrix);
            var redundantInstances = new List<string>();

            foreach (var s in table)
            {
                if (s[position] == '1')
                {
                    redundantInstances.Add(s);
                }
            }

            return redundantInstances;
        }





        /// <summary>
        /// 
        /// </summary>
        /// <param name="instanceSchemas"></param>
        /// <param name="otherInstanceSchemas"></param>
        /// <returns></returns>
        public static List<string> GetSchemaClustersWithWildcardChars(List<string> instanceSchemas, List<string> otherInstanceSchemas)
        {
            var clusterPool = new List<string>();

            //Zero Class Value
            for (int i = 0; i < instanceSchemas.Count; i++)
            {
                if (i == 0)
                {
                    //create first cluster
                    clusterPool.Add(instanceSchemas[i]);
                }
                else
                {
                    int clusterIt = 0;
                    for (int cl = clusterPool.Count() - 1; cl >= 0; cl--)
                    //for (int cl = 0; cl < clusterPool.Count(); cl++)
                    {

                        //string clusterLabel = clusterPool[cl];
                        //foreach (string clusterLabel in clusterPool)
                        //{
                        //Get ClusterString
                        string clusterLabel = clusterPool[cl];

                        string s = instanceSchemas[i];
                        string newLabel = null;
                        for (int j = 0; j < s.Length; j++)
                        {
                            if (s[j] == clusterLabel[j])
                            {
                                newLabel += s[j].ToString();
                            }
                            else
                            {
                                newLabel += '*';
                            }
                        }

                        clusterIt++;

                        if (!string.IsNullOrEmpty(newLabel) && newLabel.Replace("*", string.Empty).Trim().Length == 0)
                        {
                            //Open a new cluster, if there are no suitable existing clusters
                            // cluster selected based on sequence
                            if (clusterIt >= clusterPool.Count())
                            {
                                clusterPool.Add(s);
                                break;
                            }

                        }
                        else
                        {
                            bool isCheck = IsValidClassLabel(otherInstanceSchemas, newLabel);
                            if (isCheck)
                            {
                                if (clusterLabel != newLabel)
                                {
                                    clusterPool.Remove(clusterLabel);
                                    clusterPool.Add(newLabel);
                                }
                                break;
                            }
                            else
                            {
                                if (clusterIt >= clusterPool.Count())
                                {
                                    clusterPool.Add(s);
                                    break;
                                }
                            }

                        }
                    }
                }
            }
            return clusterPool;
        }



        /// <summary>
        /// Check??????
        /// </summary>
        /// <param name="zeroArrayClusterLabels"></param>
        /// <param name="oneClusterLabel"></param>
        /// <returns></returns>
        private static bool IsValidClassLabel(List<string> zeroArrayClusterLabels, string oneClusterLabel)
        {
            foreach (string zeroClusterLabel in zeroArrayClusterLabels)
            {
                ////TODO: Evalute if the algorithm needs to be iterated to each clusterlabel
                //if (!string.IsNullOrEmpty(zeroClusterLabel) && newLabel.Replace("*", string.Empty).Trim().Length == 0)
                //{
                //    return false;
                //}

                //Check if any wildcharacters
                int matchCount = 0;

                if (IsPure(zeroClusterLabel) && IsPure(oneClusterLabel))
                {
                    for (int i = 0; i < zeroClusterLabel.Length; i++)
                    {
                        if (zeroClusterLabel[i] == oneClusterLabel[i])
                        {
                            matchCount++;
                        }
                    }
                }
                else
                {
                    //if either is not pure
                    for (int i = 0; i < zeroClusterLabel.Length; i++)
                    {
                        if (oneClusterLabel[i].ToString() == "*" || zeroClusterLabel[i].ToString() == "*")
                        {
                            matchCount++;
                        }
                        else
                        {
                            if (zeroClusterLabel[i] == oneClusterLabel[i])
                            {
                                matchCount++;
                            }
                        }
                    }
                }

                if (matchCount == oneClusterLabel.Count())
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Check if the string is a complete wildcard character string e.g. '****'
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private static bool IsPure(string code)
        {

            int len = code.ToString().Count();
            if (!string.IsNullOrEmpty(code) && code.Replace("*", string.Empty).Trim().Length < len)
            {
                return false;
            }

            return true;
        }


        /// <summary>
        /// Calculate the coeffcients of DFT
        /// </summary>
        /// <param name="SxClusterLabel_ClassOne"></param>
        /// <param name="sjVectorList"></param>
        /// <returns></returns>
        public static Dictionary<string, double> CalculateDFTCoeffs(int numInput, List<string> SxClusterLabel_ClassOne, out List<string> sjVectorList)
        {
            bool[,] sjVectorArray = DFT.GenerateTruthTable(numInput);

            sjVectorList = DeriveSjVectors(numInput, sjVectorArray);

            Dictionary<string, double> coeffArray = new Dictionary<string, double>();

            List<int> redundantIndices = FindRedundantAttributeFromPatterns(sjVectorList);
            redundantIndices.Sort();

            foreach (string j in sjVectorList)
            {
                double coeff = DFT.GetCoefficientValue(j, SxClusterLabel_ClassOne);
                coeffArray[j] = coeff;
            }

            return coeffArray;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="numInput"></param>
        /// <param name="sjVectorArray"></param>
        /// <returns></returns>
        public static List<string> DeriveSjVectors(int numInput, bool[,] sjVectorArray)
        {
            int arrayLength = (int)Math.Pow(2, (double)numInput);
            List<string> sjVectorList = new List<string>();
            for (int i = 0; i < arrayLength; i++)
            {
                string sjString = string.Empty;
                for (int j = 0; j < numInput; j++)
                {
                    if (sjVectorArray[i, j] == false)
                        sjString += '0';
                    else
                        sjString += '1';
                }
                sjVectorList.Add(sjString);
            }

            return sjVectorList;
        }


        /// <summary>
        /// Calculate the Coefficient value
        /// </summary>
        /// <param name="j"></param>
        /// <param name="patterns"></param>
        /// <returns></returns>
        private static double GetCoefficientValue(string j, List<string> patterns)
        {
            double denominator = Math.Pow(2, j.Length);
            double coefficientValue = 0.0;
            foreach (string x in patterns)
            {
                double dotProduct = DFT.CalculateDotProduct(j, x);
                if (dotProduct != 0)
                {
                    coefficientValue = coefficientValue + (dotProduct / denominator);
                }
            }
            return coefficientValue;
        }

        /// <summary>
        /// Calculate the f(x) value i.e. Inverse of DFT
        /// </summary>
        /// <param name="xVector"></param>
        /// <param name="jPatterns"></param>
        /// <param name="coeffArray"></param>
        /// <returns></returns>
        public static double GetCoeffInverseDft(string xVector, List<string> jPatterns, Dictionary<string, double> coeffArray)
        {
            double fx = 0.0;
            foreach (string j in jPatterns)
            {
                double dotProduct = DFT.CalculateDotProduct(j, xVector);
                double coeff = coeffArray[j];
                fx += dotProduct * coeff;
            }
            return fx;
        }



        // All DFT models
        //************************************************************************************************************************************
        //Calculates dot product between two binary strings with wild card characters. sjVector can not have wildcard characters
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sjVector"></param>
        /// <param name="sxVector"></param>
        /// <returns></returns>
        private static int CalculateDotProduct(string sjVector, string sxVector)
        {
            if (sjVector.Length != sxVector.Length)
            {
                //MessageBox.Show("In Calculating dot product the j vector length and x vector length are not equal.", "Error");
                //TODO: Error
            }

            CharEnumerator ojCharEnum = sjVector.GetEnumerator();
            CharEnumerator oxCharEnum = sxVector.GetEnumerator();

            int i11Count = 0;
            int i00Count = 0;
            int i1WildcardCount = 0;
            int i0WildcardCount = 0;

            while (ojCharEnum.MoveNext() && oxCharEnum.MoveNext())
            {
                if (oxCharEnum.Current == '*')
                {
                    if (ojCharEnum.Current == '1')
                    {
                        ++i1WildcardCount;
                        return 0;
                    }

                    else if (ojCharEnum.Current == '0')
                    {
                        ++i0WildcardCount;
                    }
                    else
                    {
                        Console.WriteLine("* * combination Impossible");
                    }
                }
                else
                {
                    if (ojCharEnum.Current == '1' && oxCharEnum.Current == '1')
                    {
                        ++i11Count;
                    }
                    else if (ojCharEnum.Current == '0' && oxCharEnum.Current == '0')
                    {
                        ++i00Count;
                    }
                }
            }
            if (i1WildcardCount != 0) //all are 1 * combinations or there exists a 1 * combination 
            {
                return 0;
            }
            else if (i0WildcardCount != 0)
            {
                if (i0WildcardCount == sjVector.Length) //all are 0 * combinations
                {
                    return (int)Math.Pow(2, i0WildcardCount);
                }
                else if (i0WildcardCount < sjVector.Length)
                {
                    if (i11Count % 2 == 0) //there exists some 1 1 combinations
                    {
                        return (int)Math.Pow(2, i0WildcardCount);
                    }
                    else
                    {
                        return -(int)Math.Pow(2, i0WildcardCount);
                    }
                }
            }
            else
            {
                if (i11Count % 2 == 0)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
            return 10000;
        }



        /// <summary>
        /// GetRedudantInstanceSchemas
        /// </summary>
        /// <param name="redundantAttributes"></param>
        /// <returns></returns>
        public static List<string> CalculateEnergyThresholding(int bitStringLength, int order)
        {
            var tblMatrix = GenerateTruthTable(bitStringLength);
            var table = DeriveSjVectors(bitStringLength, tblMatrix);
            var list = new List<string>();

            if (order == 1)
            {
                list.Add(table[0]);
            }

            foreach (var entry in table)
            {
                if (entry.Replace("0", string.Empty).Length == order)
                {
                    list.Add(entry);
                }
            }

            return list;
        }


        //public static string getBinaryString(int number, out int numberOfOnes)
        //{
        //    numberOfOnes = 0;
        //    string binaryString = "";
        //    if (number == 0)
        //    {
        //        binaryString = "0";
        //    }
        //    while (number > 0)
        //    {
        //        int value = number % 2;
        //        if (value == 1)
        //        {
        //            numberOfOnes++;
        //        }
        //        binaryString = binaryString + value.ToString();
        //        number = number / 2;
        //    }

        //    //TODO: redefine size
        //    while (binaryString.Length < 3)
        //    {
        //        binaryString = binaryString + "0";
        //    }

        //    char[] charArray = binaryString.ToArray();
        //    Array.Reverse(charArray);
        //    binaryString = new string(charArray);
        //    return binaryString;
        //}



    }
}
