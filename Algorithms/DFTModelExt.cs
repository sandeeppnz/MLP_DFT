using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithms
{
    public class DFTModelExt
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


                    EnergyCoeffsTrain = null;
                    JVectorsTrain = null;

                    SchemaTrain = null;

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

        //public int NumTotalInstancesXClass0Train { get; set; }
        //public int NumTotalInstancesXClass1Train { get; set; }
        //public int NumTotalInstancesXClass0Test { get; set; }
        //public int NumTotalInstancesXClass1Test { get; set; }


        public Dictionary<string, SchemaStat> SchemaTrain { get; set; }
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
        public int MaxOrder { get; set; }


        public DFTModelExt()
        { }

        public DFTModelExt(NeuralNetwork nn, float[][] trainData,
            float[][] testData, bool featureSelection, bool autoEnergyCal,
            decimal energyThreshold, int maxOrder,
            double[] rankArray = null)
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
            MaxOrder = maxOrder;

            SchemaTrain = new Dictionary<string, SchemaStat>();

            EnergyCoeffsTrain = new Dictionary<string, double>();
            JVectorsTrain = new HashSet<string>();

            var redundantSchema = RemoveDuplcateSchemaInstances();
            //var convertedArray = MakeArrayBasedSchema(redundantSchema);

        }


        public double CalculateFxByInveseDftEquation(string xVector, HashSet<string> jVectors, Dictionary<string, double> coeffArray)
        {
            double fx = 0;
            foreach (string j in jVectors)
            {
                double dotProduct = Helper.CalculateDotProduct(j, xVector);
                double coeff = coeffArray[j];
                fx += dotProduct * coeff;
            }

            if (fx < 0.5)
            {
                fx = 0; //even of the instance vector
            }
            else
            {
                fx = 1;
            }

            return fx;
        }



        public SchemaStat DirectRetrieveFxByCaching(Dictionary<string, SchemaStat> list, string instancePattern)
        {
            if (list.Count > 0)
            {
                return list[instancePattern];
            }

            return null;
        }


        //public Dictionary<string, double> ValidateFxByClusterPatternMatching(List<string> uniqueSchemaList, List<string> patternList, ref long misClassficationCount, int classLabel)
        //{
        //    var fxArray = new Dictionary<string, double>();
        //    foreach (string schemaInstance in uniqueSchemaList)
        //    {
        //        double fx = GetFxByWildcardCharacterCheck(schemaInstance, patternList, classLabel);
        //        if (classLabel != fx)
        //        {
        //            misClassficationCount++;
        //        }

        //        fxArray[schemaInstance] = fx;
        //    }
        //    return fxArray;
        //}



        public string CheckIfInstanceCanBeMatchedToPattern(string xVector, Dictionary<string, SchemaStat> schemaPatternList)
        {
            //Checks for a matching pattern for the schema instance, returns the 
            double fx = -1;
            foreach (string pattern in schemaPatternList.Keys)
            {
                if (pattern.Equals(xVector))
                {
                    var o = schemaPatternList[xVector];
                    return o.ClassLabelClassifiedByMLP.ToString();
                    //return true;
                    //return double.Parse(classLabel.ToString());
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
                                //return false;
                                return fx.ToString();
                                //return fx;
                            }
                            else
                            {
                                //pattern[j] == schemaInstance[j]
                            }
                        }
                    }

                    if (isMatch)
                    {
                        var o = schemaPatternList[xVector];
                        return o.ClassLabelClassifiedByMLP.ToString();

                        //return true;
                        //return double.Parse(classLabel.ToString());
                    }
                }
            }


            return fx.ToString();
            //return fx;
        }



        public SchemaStat CheckIfInstanceCanBeMatchedToPatternSchemaStat(string xVector, Dictionary<string, SchemaStat> schemaPatternList)
        {
            //Checks for a matching pattern for the schema instance, returns the 
            double fx = -1;
            foreach (string pattern in schemaPatternList.Keys)
            {
                if (pattern.Equals(xVector))
                {
                    var o = schemaPatternList[xVector];
                    return o;
                    //return true;
                    //return double.Parse(classLabel.ToString());
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
                                //return false;
                                //return fx.ToString();
                                //return fx;
                            }
                            else
                            {
                                //pattern[j] == schemaInstance[j]
                            }
                        }
                    }

                    if (isMatch)
                    {
                        var o = schemaPatternList[pattern];
                        return o;

                        //return true;
                        //return double.Parse(classLabel.ToString());
                    }
                }
            }

            return null;
            //return fx.ToString();
            //return fx;
        }


        public double GetCurrentFxFromList(Dictionary<string, SchemaStat> list, string instanceSchema)
        {
            if (list.Count > 0)
            {
                if (list.ContainsKey(instanceSchema))
                {
                    return Double.Parse(list[instanceSchema].ClassLabelCalculatedByInvDft);
                }
                else
                {
                    return -1;
                }
            }

            return -1;
        }

        //public void UpdateCoeffcientArray(string j, List<string> patterns)
        //{
        //    double dCoefficientValue_Contribution_ABBA = 0.0;//reset contribution form set AB or BA class change schemas to refinemnet
        //    double dCoefficientValue_Contribution_0A0B = 0.0; //reset contribution form set 0A or 0B class change schemas to refinemnet    
        //    double correctionFactor = 0.0;
        //    double AveragedcorrectionFactor = 0.0;

        //    foreach (string x in patterns)
        //    {
        //        double dDotProduct = (double) Helper.CalculateDotProduct(j, x); // j*x
        //        if (dDotProduct != 0)
        //        {
        //            dCoefficientValue_Contribution_ABBA = dCoefficientValue_Contribution_ABBA + (x * dDotProduct); //sum over Xs of set ABBA
        //        }

        //        if (dDotProduct != -1.0 && dDotProduct != 1.0)
        //        {

        //        }
        //    }

        //}


        //public void UpdateRefineDftModel()
        //{
        //    double dCoefficientValue_Contribution_ABBA = 0.0;//reset contribution form set AB or BA class change schemas to refinemnet
        //    double dCoefficientValue_Contribution_0A0B = 0.0; //reset contribution form set 0A or 0B class change schemas to refinemnet    
        //    double correctionFactor = 0.0;
        //    double AveragedcorrectionFactor = 0.0;

        //    #region calculate contribution form set AB or BA class change schemas to refinemnet of this coefficient

        //    foreach (KeyValuePair<string, double> kv6 in schemaClass_classChanged_ABBA)//key is the X vector
        //    {
        //        if (kv6.Value != 0)//if the class diffrence is not 0, if 0 whatever the dot product , it is zero
        //        {
        //            double dDotProduct = (double) Helper.CalculateDotProduct(kv3.Key, kv6.Key); // j*x
        //                                                                                        //double dDotProduct = (double)FourierConversion.CalculateDotProduct(kv3.Key, kv6.Key);
        //            if (dDotProduct != 0)
        //            {

        //                dCoefficientValue_Contribution_ABBA = dCoefficientValue_Contribution_ABBA + ((kv6.Value) * dDotProduct); //sum over Xs of set ABBA
        //            }

        //            if (dDotProduct != -1.0 && dDotProduct != 1.0)
        //            {

        //            }
        //        }
        //    }

        //    #endregion calculate contribution form set AB or BA class change schemas to refinemnet of this coefficient


        //    #region calculate contribution form set 0A or 0B class change schemas to refinemnet of this coefficient

        //    foreach (KeyValuePair<string, double> kv7 in schemaClass_classChanged_0A0B)//key is the X vector
        //    {
        //        if (kv7.Value != 0)//if the class difference is not 0
        //        {
        //            double dDotProduct = (double) Helper.CalculateDotProduct(kv3.Key, kv7.Key); // j*x
        //                                                                                        //double dDotProduct = (double)FourierConversion.CalculateDotProduct(kv3.Key, kv7.Key);
        //            if (dDotProduct != 0)
        //            {

        //                dCoefficientValue_Contribution_0A0B = dCoefficientValue_Contribution_0A0B + ((kv7.Value) * dDotProduct); //sum over Xs of set 0A0B
        //            }

        //            if (dDotProduct != -1.0 && dDotProduct != 1.0)
        //            {

        //            }
        //        }
        //    }

        //    #endregion calculate contribution form set 0A or 0B class change schemas to refinemnet of this coefficient

        //    //correctionFactor = dCoefficientValue_Contribution_ABBA + (Alpha * dCoefficientValue_Contribution_0A0B);
        //    correctionFactor = dCoefficientValue_Contribution_ABBA + dCoefficientValue_Contribution_0A0B;
        //    //AveragedcorrectionFactor = correctionFactor / System.Convert.ToDouble(reservoirInstanceCount);

        //    //AveragedcorrectionFactor = correctionFactor / denominator;
        //    AveragedcorrectionFactor = correctionFactor / distinctSchemaWithinInterval.Count();
        //    //AveragedcorrectionFactor = (correctionFactor * changeFrequencyCountInThisInterval) / intervalSizeForRefinement;


        //    //AveragedcorrectionFactor = correctionFactor / denominator;
        //    kv3.Value.coefficientValue = kv3.Value.coefficientValue + AveragedcorrectionFactor;

        //    if (kv3.Value.coefficientValue == 0)
        //    {
        //        zeroCoefficientcount++;
        //    }

        //    if (zeroCoefficientcount == winnerTree.Pattern.Count())
        //    {
        //        //trouble---- solution below

        //        //find new winner 
        //        //delete this current winner from pool
        //        //rest winner reservoir
        //        //reset matching count
        //    }
        //}




        public HashSet<string> RemoveDuplcateSchemaInstances()
        {
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

            return fullHashSet;
        }


        public List<int> RedundantFeaturesByEnergyCoffsArray(int order)
        {
            //sort from lowest distribution to the highest in the energy array
            Dictionary<string, double> coeffsWithAttributes = EnergyCoeffsTrain;
            List<int> redundantAttributeList = new List<int>();

            var myList = coeffsWithAttributes.ToList();

            myList.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));

            int counter = 0;
            double sum = 0;
            foreach (var k in myList)
            {
                sum += k.Value;

                counter++;
                if (sum < 0.95)
                {

                }
                else
                {
                    redundantAttributeList.Add(counter);
                    //                   counter++;
                }
            }

            //redundantAttributeList.Add(counter);

            List<int> attributePositionList = new List<int>();

            foreach (var k in redundantAttributeList)
            {
                string key = myList[k].Key;

                for (int j = 0; j < key.Length; j++)
                {
                    if (key[j] == '1')
                    {
                        attributePositionList.Add(j + 1);
                    }
                }
            }


            return attributePositionList;

        }


        public void ExtractUniqueInstanceSchemasTrain(out HashSet<string> class0, out HashSet<string> class1)
        {

            Dictionary<string, SchemaStat> list = new Dictionary<string, SchemaStat>();

            Console.WriteLine("...ExtractUniqueInstanceSchemasTrain");

            // percentage correct using winner-takes all
            double[] xValues = new double[NumAttributes]; // inputs
            double[] tValues = new double[NumOutputs]; // targets
            double[] yValues; // computed Y

            //List<string> dataList = new List<string>();
            int binaryResult = 0;

            class0 = new HashSet<string>();
            class1 = new HashSet<string>();

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

                var sList = xValues.ToList();
                string s = string.Empty;

                foreach (var k in sList)
                {
                    s += k.ToString();
                }

                if (binaryResult == 0)
                {
                    class0.Add(s);
                }
                else
                {
                    class1.Add(s);
                }
            }

            //return list;
        }

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
            double actSize = Math.Pow(2.0, (double) NumAttributes);
            JVectorsTrain = jVectorArray;
            EnergyCoefficientOrderNum = order;

            Console.WriteLine("...GenerateJVectorByEnegryThresholdingLimit {0} vectors upto the order {1}, full coeffcient size:{2}", JVectorsTrain.Count, order, actSize);
            return JVectorsTrain;
        }

        public List<int> FindRedundantAttributeFromPatterns(Dictionary<string, SchemaStat> clusteredSchemaXVectorsClass1)
        {
            if (clusteredSchemaXVectorsClass1.Count <= 0)
                return new List<int>();

            string p1 = clusteredSchemaXVectorsClass1.ElementAt(0).Key;

            List<int> redundantIndexList = new List<int>();
            List<int> redudantIdenxLocalList = null;

            for (int i = 1; i < clusteredSchemaXVectorsClass1.Count(); i++)
            {
                string s = clusteredSchemaXVectorsClass1.ElementAt(i).Key;
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

        public double CalculateDynamicEnergy(Dictionary<string, double> coeffArray)
        {
            double total = 0;
            foreach (var coeff in coeffArray)
            {
                total += Math.Pow(coeff.Value, 2.0);
            }

            return total;
        }

        public Dictionary<string, double> CalculateDftEnergyCoeffs(Dictionary<string, SchemaStat> clusteredSchemaXVectorsClass1)
        {

            Stopwatch sw = new Stopwatch();
            sw.Start();

            Dictionary<string, double> coeffArray = new Dictionary<string, double>();
            double coeff = 0;
            double? checkRedundantCoeffVal = null;

            var redundantAttIndex = FindRedundantAttributeFromPatterns(clusteredSchemaXVectorsClass1);

            //Console.WriteLine("...GetCoefficientValue for each j vector..");

            long numEnergyZerojVectors = 0;


            //Convert clusteredSchemaXVectorsClass1 to a List
            List<string> clusteredSchemaXVectorsClass1List = new List<string>();
            foreach (var k in clusteredSchemaXVectorsClass1)
            {
                clusteredSchemaXVectorsClass1List.Add(k.Key);
            }


            foreach (string j in JVectorsTrain)
            {
                //Optimisation of Energy Calculation
                checkRedundantCoeffVal = EvaluateIfRedudantInstanceSchemas(redundantAttIndex, j);
                if (checkRedundantCoeffVal == null)
                {
                    coeff = GetCoefficientValue(j, clusteredSchemaXVectorsClass1List);
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

                while (ratio <= EnergyThresholdLimit && currOrder < MaxOrder)
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
                        coeff = GetCoefficientValue(s, clusteredSchemaXVectorsClass1List);
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


        public Dictionary<string, SchemaStat> GenerateClusteredSchemaPatterns(HashSet<string> class0, HashSet<string> class1)
        {
            Dictionary<string, SchemaStat> list = new Dictionary<string, SchemaStat>();
            Dictionary<string, SchemaStat> listClass1 = new Dictionary<string, SchemaStat>();

            Console.WriteLine("...GenerateClusteredSchemaPatterns for class 0");
            Dictionary<string, SchemaStat> ClusteredSchemaXVectorClass0Train = GetSchemaClustersWithWildcardChars(class0, class1, true);
            //Console.WriteLine("No of Class 0 XVector patterns: {0}", ClusteredSchemaXVectorClass0.Count);
            //string s1 = string.Empty;
            //s1 = PrintClusterClass2Patterns(s1);
            Console.WriteLine("...GenerateClusteredSchemaPatterns for class 1");
            Dictionary<string, SchemaStat> ClusteredSchemaXVectorClass1Train = GetSchemaClustersWithWildcardChars(class1, class0, false);
            //s1 = PrintClusterClass1Patterns();

            foreach (var s in ClusteredSchemaXVectorClass0Train)
            {
                list.Add(s.Key, s.Value);
            }
            foreach (var s in ClusteredSchemaXVectorClass1Train)
            {
                list.Add(s.Key, s.Value);
                listClass1.Add(s.Key, s.Value);
            }

            SchemaTrain = list;


            return listClass1;
        }

        public Dictionary<string, SchemaStat> GetSchemaClustersWithWildcardChars(HashSet<string> instanceSchemas, HashSet<string> otherInstanceSchemas, bool isClass0)
        {
            var clusterPool = new List<string>();
            Dictionary<string, SchemaStat> clusterPoolDictionary = new Dictionary<string, SchemaStat>();
            //Dictionary<string, SchemaStat> clusterPool1 = new Dictionary<string, SchemaStat>();



            //Zero Class Value
            for (int i = 0; i < instanceSchemas.Count; i++)
            {
                if (i == 0)
                {
                    //create first cluster
                    clusterPool.Add(instanceSchemas.ElementAt(i));

                    if (isClass0)
                    {
                        clusterPoolDictionary.Add(instanceSchemas.ElementAt(i), new SchemaStat(instanceSchemas.ElementAt(i), instanceSchemas.ElementAt(i), "0", string.Empty));
                    }
                    else
                    {
                        clusterPoolDictionary.Add(instanceSchemas.ElementAt(i), new SchemaStat(instanceSchemas.ElementAt(i), instanceSchemas.ElementAt(i), "1", string.Empty));
                    }
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

                        string s = instanceSchemas.ElementAt(i);
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

                                if (isClass0)
                                {
                                    clusterPoolDictionary.Add(s, new SchemaStat(s, s, "0", string.Empty));
                                }
                                else
                                {
                                    clusterPoolDictionary.Add(s, new SchemaStat(s, s, "1", string.Empty));
                                }


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

                                    if (isClass0)
                                    {
                                        clusterPoolDictionary.Remove(clusterLabel);
                                        clusterPoolDictionary.Add(newLabel, new SchemaStat(s, newLabel, "0", string.Empty));
                                    }
                                    else
                                    {
                                        clusterPoolDictionary.Remove(clusterLabel);
                                        clusterPoolDictionary.Add(newLabel, new SchemaStat(s, newLabel, "1", string.Empty));
                                    }




                                }
                                break;
                            }
                            else
                            {
                                if (clusterIt >= clusterPool.Count())
                                {
                                    clusterPool.Add(s);

                                    if (isClass0)
                                    {
                                        clusterPoolDictionary.Add(s, new SchemaStat(s, s, "0", string.Empty));
                                    }
                                    else
                                    {
                                        clusterPoolDictionary.Add(s, new SchemaStat(s, s, "1", string.Empty));
                                    }



                                    break;
                                }
                            }

                        }
                    }
                }
            }

            return clusterPoolDictionary;
        }


        private bool CheckIfTheNewLabelIsNewAndValid(HashSet<string> otherInstanceSchemas, string newLabel)
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

        private bool IsAllNonWildcardChar(string code)
        {

            int len = code.ToString().Count();
            if (!string.IsNullOrEmpty(code) && code.Replace("*", string.Empty).Trim().Length < len)
            {
                return false;
            }

            return true;
        }




    }
}
