using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Algorithms
{

    public class DFTModel
    {
        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    TrainData = null;
                    RankArray = null;
                    TestData = null;

                    AllSchemaXVectorClass0Train = null;
                    AllSchemaXVectorClass1Train = null;

                    AllSchemaXVectorClass0Test = null;
                    AllSchemaXVectorClass1Test = null;


                    ClusteredSchemaXVectorClass0Train = null;
                    ClusteredSchemaXVectorClass1Train = null;

                    //ClusteredSchemaXVectorClass0Test = null;
                    //ClusteredSchemaXVectorClass1Test = null;

                    EnergyCoeffsTrain = null;
                    JVectorsTrain = null;


                    RedundantIndexListTrain = null;
                    RedundantSchemasTrain = null;

                    _nn.Dispose(true);
                }
            }
            this.disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            //GC.SuppressFinalize(this);
        }
        INeuralNetwork _nn;
        public int NumAttributes { get; set; }
        public int NumOutputs { get; set; }


        public double CoefficientGenerationTime { get; set; }
        public decimal EnergyThresholdLimit { get; set; }
        public int EnergyCoefficientOrderNum { get; set; }

        public float[][] TrainData;
        public float[][] TestData;
        public int NumTotalInstancesXClass0Train { get; set; }
        public int NumTotalInstancesXClass1Train { get; set; }
        public int NumTotalInstancesXClass0Test { get; set; }
        public int NumTotalInstancesXClass1Test { get; set; }
        public List<string> AllSchemaXVectorClass0Train { get; set; }
        public List<string> AllSchemaXVectorClass1Train { get; set; }
        public List<string> AllSchemaXVectorClass0Test { get; set; }
        public List<string> AllSchemaXVectorClass1Test { get; set; }
        public List<string> ClusteredSchemaXVectorClass0Train { get; set; }
        public List<string> ClusteredSchemaXVectorClass1Train { get; set; }
        //public List<string> ClusteredSchemaXVectorClass0Test { get; set; }
        //public List<string> ClusteredSchemaXVectorClass1Test { get; set; }
        public Dictionary<string, double> EnergyCoeffsTrain { get; set; }
        public HashSet<string> JVectorsTrain { get; set; }

        public List<int> RedundantIndexListTrain { get; set; }
        public List<string> RedundantSchemasTrain { get; set; }

        public bool IsFeatureSelection { get; set; }
        public double[] RankArray { get; set; }

        public double DFTModelTrainDataAccuracy { get; set; }
        public double DFTModelTrainDataTime { get; set; }
        public double Shortcut_ClusterPatternMachingTrainDataAccuracy { get; set; }
        public double Shortcut_ClusterPatternMachingTrainDataTime { get; set; }


        public double DFTModelTestDataAccuracy { get; set; }
        public double DFTModelTestDataTime { get; set; }
        public double Shortcut_ClusterPatternMachingTestDataAccuracy { get; set; }
        public double Shortcut_ClusterPatternMachingTestDataTime { get; set; }
        public bool AutoEnergyThresholding { get; set; }

        public DFTModel()
        { }

        public DFTModel(NeuralNetwork nn, float[][] trainData, float[][] testData, bool featureSelection, bool autoEnergyCal,  decimal energyThreshold,  double[] rankArray = null)
        {
            Console.WriteLine("=====================================================");
            Console.WriteLine("Discrete Fourier Transformation of training dataset...\n");

            _nn = nn;
            NumAttributes = nn.NumInputNodes;
            NumOutputs = nn.NumOutputNodes;
            TrainData = trainData;
            TestData = testData;
            IsFeatureSelection = featureSelection;
            RankArray = rankArray;

            AutoEnergyThresholding = autoEnergyCal;
            EnergyThresholdLimit = energyThreshold;

            AllSchemaXVectorClass0Train = new List<string>();
            AllSchemaXVectorClass1Train = new List<string>();

            AllSchemaXVectorClass0Test = new List<string>();
            AllSchemaXVectorClass1Test = new List<string>();


            ClusteredSchemaXVectorClass0Train = new List<string>();
            ClusteredSchemaXVectorClass1Train = new List<string>();


            EnergyCoeffsTrain = new Dictionary<string, double>();
            JVectorsTrain = new HashSet<string>();

            var redundantSchema = RemoveDuplcateSchemaInstances();
            //var convertedArray = MakeArrayBasedSchema(redundantSchema);

        }


        //public List<string> CalculateEnergyThresholding(int bitStringLength, int order)
        //{
        //    //var tblMatrix = GenerateTruthTable(bitStringLength);
        //    //var table = DeriveSjVectors(bitStringLength, tblMatrix);

        //    var table = GenerateSjVectors();

        //    var list = new List<string>();

        //    if (order == 1)
        //    {
        //        list.Add(table[0]);
        //    }

        //    foreach (var entry in table)
        //    {
        //        if (entry.Replace("0", string.Empty).Length == order)
        //        {
        //            list.Add(entry);
        //        }
        //    }

        //    return list;
        //}




        //public List<string> GenerateSjVectors()
        //{
        //    bool[,] sjVectorArray = GenerateTruthTable(NumAttributes);

        //    int arrayLength = (int)Math.Pow(2, (double)NumAttributes);
        //    for (int i = 0; i < arrayLength; i++)
        //    {
        //        string sjString = string.Empty;
        //        for (int j = 0; j < NumAttributes; j++)
        //        {
        //            if (sjVectorArray[i, j] == false)
        //                sjString += '0';
        //            else
        //                sjString += '1';
        //        }
        //        JVectorsTrain.Add(sjString);
        //    }

        //    return JVectorsTrain;
        //}



        //public bool[,] GenerateTruthTable(int NumAttributes)
        //{
        //    long row = (int)Math.Pow(2, NumAttributes);
        //    bool[,] table = new bool[row, NumAttributes];

        //    long divider = row;

        //    // iterate by column
        //    for (int c = 0; c < NumAttributes; c++)
        //    {
        //        divider /= 2;
        //        bool cell = false;
        //        // iterate every row by this column's index:
        //        for (int r = 0; r < row; r++)
        //        {
        //            table[r, c] = cell;
        //            if ((divider == 1) || ((r + 1) % divider == 0))
        //            {
        //                cell = !cell;
        //            }
        //        }
        //    }

        //    return table;
        //}

        public HashSet<string> GenerateJVectorByEnegryThresholdingLimit(int order)
        {
            if (AutoEnergyThresholding)
            {
                Console.WriteLine("...Auto jVectors");
                order = 1;
            }

            if (order == -1 && !AutoEnergyThresholding) //unset order
            {
                order = NumAttributes;
            }

            var jVectorArray = GenerateTruthTableOptimized(NumAttributes, order);
            double actSize = Math.Pow(2.0, (double)NumAttributes);
            JVectorsTrain = jVectorArray;
            EnergyCoefficientOrderNum = order;

            Console.WriteLine("...GenerateJVectorByEnegryThresholdingLimit {0} vectors upto the order {1}, full coeffcient size:{2}", JVectorsTrain.Count, order, actSize);
            return JVectorsTrain;
        }

        /// <summary>
        /// GenerateTruthTable
        /// </summary>
        /// <param name="NumAttributes"></param>
        /// <returns></returns>
        public HashSet<string> GenerateTruthTableOptimized(int NumAttributes, int maxOrder)
        {
            List<HashSet<string>> list = new List<HashSet<string>>();
            HashSet<string> final = new HashSet<string>();
            HashSet<string> firstOrder = new HashSet<string>();


            string original = string.Empty;
            original = original.PadLeft(NumAttributes, '0');

            firstOrder.Add(original); // 0th order

            //1st Order
            for (int i = NumAttributes - 1; i >= 0; i--)
            {
                StringBuilder sb = new StringBuilder(original);

                sb[i] = '1';
                firstOrder.Add(sb.ToString());
            }

            list.Add(firstOrder);


            for (int i = 2; i <= maxOrder; i++)
            {
                firstOrder = OrderIterator(NumAttributes, firstOrder, i);
                list.Add(firstOrder);
            }


            foreach (var itemList in list)
            {
                foreach (var item in itemList)
                {
                    final.Add(item);
                }
            }

            return final;
        }


        private static HashSet<string> OrderIterator(int NumAttributes, HashSet<string> firstOrder, int order)
        {
            HashSet<string> array = new HashSet<string>();
            for (int k = 0; k < firstOrder.Count(); k++)
            {
                string s = firstOrder.ElementAt(k);
                for (int i = NumAttributes - order; i >= 0; i--)
                {
                    StringBuilder sb = new StringBuilder(s);
                    sb[i] = '1';
                    array.Add(sb.ToString());
                }

            }
            return array;
        }


        /// <summary>
        /// Optimisation:
        /// If each pattern has a recurring wildcharacter in same position, the position correpsonding to the attribute can be considered a redundant feature
        /// e.g. if the 2nd attribute is redudant, and the following patterns are derived initially
        /// (**0,0*1,1*1), then w(*1*) = 0 but w(*1*) is not zero 
        /// </summary>
        /// <param name="clusteredSchemaXVectorsClass1"></param>
        /// <returns> Returns redundant attributes (i.e. attributes that have a wild card character) in each pattern
        /// </returns>
        public List<int> FindRedundantAttributeFromPatterns(List<string> clusteredSchemaXVectorsClass1)
        {
            if (clusteredSchemaXVectorsClass1.Count <= 0)
                return new List<int>();

            string p1 = clusteredSchemaXVectorsClass1[0];
            List<int> redundantIndexList = new List<int>();
            List<int> redudantIdenxLocalList = null;

            for (int i = 1; i < clusteredSchemaXVectorsClass1.Count(); i++)
            {
                string s = clusteredSchemaXVectorsClass1[i];
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

            redundantIndexList.Sort();

            //return redundantIndexList;

            Console.WriteLine("...FindRedundantAttributeFromPatterns from Class 1 clusters, {0} attributes found", redundantIndexList.Count);

            RedundantIndexListTrain = redundantIndexList;
            return redundantIndexList;
        }






        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="rankArray"></param>
        /// <returns></returns>
        public HashSet<string> RemoveDuplcateSchemaInstances()
        {
            /*
             * Get unique schema entries
             * https://www.dotnetperls.com/bitarray
             */

            Console.WriteLine("...RemoveDuplcateSchemaInstances");


            double[] xValues = new double[NumAttributes]; // inputs
            HashSet<string> fullHashSet = new HashSet<string>();
            List<BitArray> fullList = new List<BitArray>();

            for (int i = 0; i < TrainData.Length; i++)
            {
                //Ideally, pass bool[] to the BitArray()
                Array.Copy(TrainData[i], xValues, NumAttributes); // get x-values

                //TODO: rank array comment
                if (IsFeatureSelection)
                {
                    xValues = Helper.SetXValueToZeroByRankCheck(xValues, RankArray, NumAttributes);
                }
                //BitArray n = new BitArray(numInput);
                string s = null;
                for (int j = 0; j < NumAttributes; j++)
                {
                    s += xValues[j].ToString();
                }
                fullHashSet.Add(s);
            }

            //Console.WriteLine("Total instances: {0}", TrainData.Length);
            //Console.WriteLine("Unique instances: {0}\n", fullHashSet.Count);


            return fullHashSet;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="redundantSchema"></param>
        /// <returns></returns>
        public double[][] MakeArrayBasedSchema(HashSet<string> redundantSchema)
        {
            /*
             * Translate schema to send to the Evaluator
             * 
             */

            double[][] array = new double[redundantSchema.Count][];

            int i = 0;

            foreach (string s in redundantSchema)
            {
                double[] wordArray = new double[NumAttributes];

                for (int j = 0; j < NumAttributes; j++)
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
        /// Calculates Accuracy and Sets the AllSchemaSxClass1 and AllSchemaSxClass0
        /// </summary>
        /// <param name="TrainData"></param>
        /// <param name="RankArray"></param>
        /// <param name="AllSchemaSxClass0"></param>
        /// <param name="AllSchemaSxClass1"></param>
        public void SpliteInstanceSchemasByClassValueTrain()
        {

            Console.WriteLine("...SpliteInstanceSchemasByClassValueTrain");

            // percentage correct using winner-takes all
            double[] xValues = new double[NumAttributes]; // inputs
            double[] tValues = new double[NumOutputs]; // targets
            double[] yValues; // computed Y

            //List<string> dataList = new List<string>();
            int binaryResult = 0;

            AllSchemaXVectorClass0Train = new List<string>();
            AllSchemaXVectorClass1Train = new List<string>();

            var yZeroTemp = new List<string>();
            var yOneTemp = new List<string>();


            for (int i = 0; i < TrainData.Length; i++)
            {
                Array.Copy(TrainData[i], xValues, NumAttributes); // get x-values
                Array.Copy(TrainData[i], NumAttributes + 3, tValues, 0, NumOutputs); // get target values from array

                //Set X values to zero if ranking...

                //TODO: xValues comments
                if (IsFeatureSelection)
                {
                    xValues = Helper.SetXValueToZeroByRankCheck(xValues, RankArray, NumAttributes);
                }

                yValues = _nn.ComputeOutputs(xValues);
                binaryResult = Helper.MaxIndex(yValues); // which cell in yValues has largest value?

                //string s = binaryResult.ToString();
                string s = string.Empty;
                for (int j = 0; j < NumAttributes; j++)
                {
                    s += xValues[j].ToString(); //TODO: for continuous variables this would raise errors
                }

                if (binaryResult == 0)
                {
                    yZeroTemp.Add(s);
                }
                else
                {
                    yOneTemp.Add(s);
                }
            }

            NumTotalInstancesXClass0Train = yZeroTemp.Count;
            NumTotalInstancesXClass1Train = yOneTemp.Count;



            int k = 0;
            long[] outputZero = new long[yZeroTemp.Count];
            foreach (string s in yZeroTemp)
            {
                outputZero[k] = Convert.ToInt64(s, 2);
                k++;
            }
            Array.Sort(outputZero);
            var outputZeroDistinct = outputZero.Distinct().ToArray();

            k = 0;
            long[] outputOne = new long[yOneTemp.Count];
            foreach (string s in yOneTemp)
            {
                outputOne[k] = Convert.ToInt64(s, 2);
                k++;
            }


            Array.Sort(outputOne);
            var outputOneDistinct = outputOne.Distinct().ToArray();

            for (int i = 0; i < outputZeroDistinct.Count(); i++)
            {
                AllSchemaXVectorClass0Train.Add(Convert.ToString(outputZeroDistinct[i], 2).PadLeft(NumAttributes, '0'));
            }

            for (int i = 0; i < outputOneDistinct.Count(); i++)
            {
                AllSchemaXVectorClass1Train.Add(Convert.ToString(outputOneDistinct[i], 2).PadLeft(NumAttributes, '0'));
            }

        }

        public void SpliteInstanceSchemasByClassValueTest()
        {

            Console.WriteLine("...SpliteInstanceSchemasByClassValueTest");

            // percentage correct using winner-takes all
            double[] xValues = new double[NumAttributes]; // inputs
            double[] tValues = new double[NumOutputs]; // targets
            double[] yValues; // computed Y

            //List<string> dataList = new List<string>();
            int binaryResult = 0;

            AllSchemaXVectorClass0Test = new List<string>();
            AllSchemaXVectorClass1Test = new List<string>();

            var yZeroTemp = new List<string>();
            var yOneTemp = new List<string>();


            for (int i = 0; i < TestData.Length; i++)
            {
                Array.Copy(TestData[i], xValues, NumAttributes); // get x-values
                Array.Copy(TestData[i], NumAttributes + 3, tValues, 0, NumOutputs); // get target values from array

                //Set X values to zero if ranking...

                //TODO: xValues comments
                if (IsFeatureSelection)
                {
                    xValues = Helper.SetXValueToZeroByRankCheck(xValues, RankArray, NumAttributes);
                }

                yValues = _nn.ComputeOutputs(xValues); //deriving the correct classfication from NN model 
                binaryResult = Helper.MaxIndex(yValues); // which cell in yValues has largest value?

                //string s = binaryResult.ToString();
                string s = string.Empty;
                for (int j = 0; j < NumAttributes; j++)
                {
                    s += xValues[j].ToString(); //TODO: for continuous variables this would raise errors
                }

                if (binaryResult == 0)
                {
                    yZeroTemp.Add(s);
                }
                else
                {
                    yOneTemp.Add(s);
                }
            }

            NumTotalInstancesXClass0Test = yZeroTemp.Count;
            NumTotalInstancesXClass1Test = yOneTemp.Count;



            int k = 0;
            long[] outputZero = new long[yZeroTemp.Count];
            foreach (string s in yZeroTemp)
            {
                outputZero[k] = Convert.ToInt64(s, 2);
                k++;
            }
            Array.Sort(outputZero);
            var outputZeroDistinct = outputZero.Distinct().ToArray();

            k = 0;
            long[] outputOne = new long[yOneTemp.Count];
            foreach (string s in yOneTemp)
            {
                outputOne[k] = Convert.ToInt64(s, 2);
                k++;
            }


            Array.Sort(outputOne);
            var outputOneDistinct = outputOne.Distinct().ToArray();

            for (int i = 0; i < outputZeroDistinct.Count(); i++)
            {
                AllSchemaXVectorClass0Test.Add(Convert.ToString(outputZeroDistinct[i], 2).PadLeft(NumAttributes, '0'));
            }

            for (int i = 0; i < outputOneDistinct.Count(); i++)
            {
                AllSchemaXVectorClass1Test.Add(Convert.ToString(outputOneDistinct[i], 2).PadLeft(NumAttributes, '0'));
            }
        }


        /// <summary>
        /// Check if the string is a complete wildcard character string e.g. '****'
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private bool IsAllNonWildcardChar(string code)
        {

            int len = code.ToString().Count();
            if (!string.IsNullOrEmpty(code) && code.Replace("*", string.Empty).Trim().Length < len)
            {
                return false;
            }

            return true;
        }




        /// <summary>
        /// Check??????
        /// </summary>
        /// <param name="otherInstanceSchemas"></param>
        /// <param name="newLabel"></param>
        /// <returns></returns>
        private bool CheckIfTheNewLabelIsNewAndValid(List<string> otherInstanceSchemas, string newLabel)
        {
            foreach (string zeroClusterLabel in otherInstanceSchemas)
            {
                ////TODO: Evalute if the algorithm needs to be iterated to each clusterlabel
                //if (!string.IsNullOrEmpty(zeroClusterLabel) && newLabel.Replace("*", string.Empty).Trim().Length == 0)
                //{
                //    return false;
                //}

                //Check if any wildcharacters
                int matchCount = 0;

                if (IsAllNonWildcardChar(zeroClusterLabel) && IsAllNonWildcardChar(newLabel))
                {
                    for (int i = 0; i < zeroClusterLabel.Length; i++)
                    {
                        if (zeroClusterLabel[i] == newLabel[i])
                        {
                            matchCount++;
                        }
                    }
                }
                else
                {
                    //if either is not pure (has a wildcharacter atleast)
                    for (int i = 0; i < zeroClusterLabel.Length; i++)
                    {
                        if (newLabel[i].ToString() == "*" || zeroClusterLabel[i].ToString() == "*")
                        {
                            matchCount++;
                        }
                        else
                        {
                            if (zeroClusterLabel[i] == newLabel[i])
                            {
                                matchCount++;
                            }
                        }
                    }
                }

                if (matchCount == newLabel.Count())
                {
                    return false;
                }
            }

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="instanceSchemas"></param>
        /// <param name="otherInstanceSchemas"></param>
        /// <returns></returns>
        public List<string> GetSchemaClustersWithWildcardChars(List<string> instanceSchemas, List<string> otherInstanceSchemas)
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
                            bool isCheck = CheckIfTheNewLabelIsNewAndValid(otherInstanceSchemas, newLabel);
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

        public void GenerateClusteredSchemaPatterns()
        {
            Console.WriteLine("...GenerateClusteredSchemaPatterns for class 0");
            ClusteredSchemaXVectorClass0Train = GetSchemaClustersWithWildcardChars(AllSchemaXVectorClass0Train, AllSchemaXVectorClass1Train);

            //Console.WriteLine("No of Class 0 XVector patterns: {0}", ClusteredSchemaXVectorClass0.Count);
            //string s1 = string.Empty;
            //s1 = PrintClusterClass2Patterns(s1);

            Console.WriteLine("...GenerateClusteredSchemaPatterns for class 1");
            ClusteredSchemaXVectorClass1Train = GetSchemaClustersWithWildcardChars(AllSchemaXVectorClass1Train, AllSchemaXVectorClass0Train);
            //s1 = PrintClusterClass1Patterns();

        }

        private string PrintClusterClass2Patterns(string s1)
        {
            foreach (var x in ClusteredSchemaXVectorClass0Train)
            {
                s1 += x + ",";
            }
            Console.WriteLine(s1 + "\n");
            return s1;
        }

        private string PrintClusterClass1Patterns()
        {
            string s1;
            Console.WriteLine("No of Class 1 XVector patterns: {0}", ClusteredSchemaXVectorClass1Train.Count);
            s1 = string.Empty;
            foreach (var x in ClusteredSchemaXVectorClass1Train)
            {
                s1 += x + ",";
            }
            Console.WriteLine(s1 + "\n");
            return s1;
        }

        /// <summary>
        /// Calculate the Coefficient value
        /// </summary>
        /// <param name="j"></param>
        /// <param name="patterns"></param>
        /// <returns></returns>
        private double GetCoefficientValue(string j, List<string> patterns)
        {
            double denominator = Math.Pow(2, j.Length);
            double coefficientValue = 0.0;
            foreach (string x in patterns)
            {
                double dotProduct = Helper.CalculateDotProduct(j, x);
                if (dotProduct != 0)
                {
                    coefficientValue = coefficientValue + (dotProduct / denominator);
                }
            }
            return coefficientValue;
        }

        /// <summary>
        /// Calculate the coeffcients of DFT
        /// </summary>
        /// <param name="clusteredSchemaXVectorsClass1"></param>
        /// <param name="SjVectors"></param>
        /// <returns></returns>
        public Dictionary<string, double> CalculateDftEnergyCoeffs(List<string> clusteredSchemaXVectorsClass1)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            Dictionary<string, double> coeffArray = new Dictionary<string, double>();
            double coeff = 0;
            double? checkRedundantCoeffVal = null;

            var redundantAttIndex = FindRedundantAttributeFromPatterns(clusteredSchemaXVectorsClass1);

            //Console.WriteLine("...GetCoefficientValue for each j vector..");

            long numEnergyZerojVectors = 0;


            foreach (string j in JVectorsTrain)
            {
                //Optimisation of Energy Calculation
                checkRedundantCoeffVal = EvaluateIfRedudantInstanceSchemas(redundantAttIndex, j);
                if (checkRedundantCoeffVal == null)
                {
                    coeff = GetCoefficientValue(j, clusteredSchemaXVectorsClass1);
                    coeffArray[j] = coeff;
                }
                else
                {
                    numEnergyZerojVectors++;
                    coeffArray[j] = 0;
                }


            }

            if (AutoEnergyThresholding)
            {
                int currOrder = 1;
                //incrementally calculate and orders
                double orderZeroEnergy = coeffArray.ElementAt(0).Value;
                double energy = CalculateDynamicEnergy(coeffArray); //E = (w0^2+ sum(w^2 of 1 order)) / w0^2 
                decimal ratio = (decimal) (energy / orderZeroEnergy);
                //int startIndex = 

                while (ratio <= EnergyThresholdLimit)
                {
                    currOrder++;
                    HashSet<string> newjFullVectors = GenerateTruthTableOptimized(NumAttributes, currOrder);

                    int startSize = JVectorsTrain.Count;
                    foreach (var i in newjFullVectors)
                    {
                        JVectorsTrain.Add(i);
                    }

                    //int numNewJVetors = (int) PermutationsAndCombinations.nCr(NumAttributes, currOrder);

                    for (int idx = startSize; idx < JVectorsTrain.Count; idx++)
                    {
                        string s = JVectorsTrain.ElementAt(idx);
                        coeff = GetCoefficientValue(s, clusteredSchemaXVectorsClass1);
                        coeffArray[s] = coeff;

                    }

                    energy = CalculateDynamicEnergy(coeffArray); //E = (w0^2+ sum(w^2 of 1 order)) / w0^2 
                    ratio = (decimal) (energy / orderZeroEnergy);
                }

                EnergyCoefficientOrderNum = currOrder;


            }
            sw.Stop();
            CoefficientGenerationTime = sw.Elapsed.TotalSeconds;


            EnergyCoeffsTrain = coeffArray;
            Console.WriteLine("\nNo. of Energy Coeffcients: {0}", EnergyCoeffsTrain.Count);
            Console.WriteLine("Time taken: {0}", CoefficientGenerationTime);
            Console.WriteLine("No. of redundant attributes: {0}", numEnergyZerojVectors);
            //PrintEnergyCoeffs();

            Console.WriteLine("\nDone...");
            return coeffArray;
        }

        public double CalculateDynamicEnergy(Dictionary<string, double> energies)
        {
            double total = 0;
            foreach (var e in energies)
            {
                total += Math.Pow(e.Value, 2.0);
            }

            return total;
        }

        private void PrintEnergyCoeffs()
        {
            foreach (var item in EnergyCoeffsTrain)
            {
                Console.WriteLine(item.Key + " " + item.Value);
            }
        }
















        /// <summary>
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="decimals"></param>
        /// <returns></returns>
        public double[] ShowVectorWInput(int numInput, int numHidden, int numOutput, double[] vector, int decimals)
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
        public double[] UpdateWeightsArrayByRank(int numInput, int numHidden, double[] weightsArray, double[] rankArray)
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
        public bool[,] SetIrrelevantVariables(int numInput, bool[,] table, double[] rankArray)
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
        public double[] GenerateRankArray(int numInput, double[] totalArray)
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







        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="numInput"></param>
        ///// <param name="sjVectorArray"></param>
        ///// <returns></returns>
        //public static List<string> DeriveSjVectors(int numInput, bool[,] sjVectorArray)
        //{
        //    int arrayLength = (int)Math.Pow(2, (double)numInput);
        //    List<string> sjVectorList = new List<string>();
        //    for (int i = 0; i < arrayLength; i++)
        //    {
        //        string sjString = string.Empty;
        //        for (int j = 0; j < numInput; j++)
        //        {
        //            if (sjVectorArray[i, j] == false)
        //                sjString += '0';
        //            else
        //                sjString += '1';
        //        }
        //        sjVectorList.Add(sjString);
        //    }

        //    return sjVectorList;
        //}





        ///// <summary>
        ///// </summary>
        ///// <param name="GetRedudantInstanceSchemas"></param>
        ///// <returns></returns>
        //public List<string> GetRedudantInstanceSchemas(List<int> positions, List<string> sJvectors)
        //{
        //    //var tblMatrix = GenerateTruthTable(bitStringLength);
        //    //var table = DeriveSjVectors(bitStringLength, tblMatrix);

        //    //Make the comparison string
        //    string emptyString = string.Empty;
        //    string selectionString = emptyString.PadLeft(NumAttributes, '*');


        //    foreach (int position in positions)
        //    {
        //        StringBuilder sb = new StringBuilder(selectionString);
        //        sb[position] = '1';
        //        selectionString = sb.ToString();
        //    }

        //    //
        //    /*
        //     *1* => {010,011,110,111}
        //     1*1* => {1010,1110,1011,1111}
        //     */

        //    var redundantInstances = new List<string>();

        //    foreach (string s in SjVectors)
        //    {


        //        if (s == selectionString)
        //            redundantInstances.Add(s);

        //    }

        //    RedundantSchemas = redundantInstances;

        //    return redundantInstances;
        //}

        /// <summary>
        /// </summary>
        /// <param name="GetRedudantInstanceSchemas"></param>
        /// <returns></returns>
        public double? EvaluateIfRedudantInstanceSchemas(List<int> positions, string sJvector)
        {
            //
            /*
             *1* => {010,011,110,111}
             1*1* => {1010,1110,1011,1111}
             */


            if (positions.Count == 0) return null;

            int currMatches = 0;
            foreach (int position in positions)
            {
                if (sJvector[position - 1] == '1')
                {
                    currMatches++;
                }

            }
            if (currMatches == positions.Count)
            {
                return 0;
            }


            return null;
        }




        ///// <summary>
        ///// GetRedudantInstanceSchemas
        ///// </summary>
        ///// <param name="GetRedudantInstanceSchemas"></param>
        ///// <returns></returns>
        //public List<string> GetRedudantInstanceSchemas(int bitStringLength, int position)
        //{
        //    var table = GenerateSjVectors();

        //    //var tblMatrix = GenerateTruthTable(bitStringLength);
        //    //var table = DeriveSjVectors(bitStringLength, tblMatrix);
        //    var redundantInstances = new List<string>();

        //    foreach (var s in table)
        //    {
        //        if (s[position] == '1')
        //        {
        //            redundantInstances.Add(s);
        //        }
        //    }

        //    return redundantInstances;
        //}

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
