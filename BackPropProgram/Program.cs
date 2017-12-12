using Algorithms;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace BackPropProgram
{
    class Program
    {
        static void Main(string[] args)
        {
            //TODO:
            //Exit if the data is continuous
            //Rank Array, FP in MLP, Unit testing
            const int seed = 123;
            const int maxEpochs = 100;
            const double learnRate = 0.3;
            const double momentum = 0.2;
            const double hotellingTestThreshold = 0.05;
            const double energyThreshold = 0.95;

            const int numHidden = 8;
            const int numOutput = 2; // number of classes for Y
            const bool isFSOn = false;
            const string rBin = @"C:\Program Files\R\R-3.4.1\bin\rscript.exe";
            const string rScripFileName = "Script.r";

            string inputDataPath = @"D:\ANN_Project_AUT_Sem3\Microsoft\BackPropProgram\Data\";
            string outputDataPath = @"D:\ANN_Project_AUT_Sem3\Microsoft\BackPropProgram\OutputData\";
            string resultsDataPath = @"D:\ANN_Project_AUT_Sem3\Microsoft\BackPropProgram\ResultsData\";
            string rScriptPath = @"D:\ANN_Project_AUT_Sem3\Microsoft\BackPropProgram\HotellingTTest\";


            Console.WriteLine("------ Manual Parition Selection --------");


            List<IInputSpecification> fileList = new List<IInputSpecification>();
            //fileList.Add(new OccupancySmall());
            //fileList.Add(new OccupancyExtented());

            //fileList.Add(new ElectricitySmall());
            //fileList.Add(new ElectricitySmallTest());
            fileList.Add(new ElectricitySmallTest3());
            //fileList.Add(new ElectricityExtended());


            //fileList.Add(new SensorSmall()); //TORUN
            //fileList.Add(new SensorExtended());


            ////fileList.Add(new CoverTypeSmall());
            ////fileList.Add(new CoverTypeIntemediate());
            ////fileList.Add(new CoverTypeExtended());

            //fileList.Add(new RBF());
            //fileList.Add(new RH());

            //fileList.Add(new FlightSmall());
            //fileList.Add(new FlightExtended());


            foreach (var file in fileList)
            {
                decimal currPartitionSize = 0.3M;
                decimal partitionLimit = 0.3M;


                int presetOrderNum = 3;
                int maxAutOrderLimit = 4;
                bool autoCoefficientCalculation = true;
                decimal energyThresholdLimit = 0.90M;
                SplitType split = SplitType.FixSplit;
                List<ResultsStatistics> stats = new List<ResultsStatistics>();

                //Setup File
                InputSpecification inputSpec = (InputSpecification) file;

                //fix split
                decimal trainingDataSize = 5M;
                ////


                try
                {
                    //Use the settings below to limit the iterations
                    //Use slip type settings
                    // 1.Set input file and load dataset
                    FileProcessor fp = new FileProcessor(inputDataPath, outputDataPath, resultsDataPath, rScriptPath, inputSpec);
                    fp.LoadCSV();


                    RRunner rn = new RRunner();

                    while (currPartitionSize <= partitionLimit)
                    {
                        // 2. Create MLP model    
                        MLPModel mlpModel = new MLPModel(numHidden, numOutput, fp, rn);
                        if (split == SplitType.LinearSequence)
                        {
                            mlpModel.LinearSeqTrainTestSplit(currPartitionSize, seed, false);
                        }
                        else if (split == SplitType.FixSplit)
                        {
                            mlpModel.FixTrainTestSplit(currPartitionSize, trainingDataSize, seed, false);
                        }
                        else if (split == SplitType.FixedSizeOptimumSet)
                        {
                            //Select the optimum the fold (with a curr partition size) that would give the best accuracy
                            // by using Hotelling T-Test
                            mlpModel.OptimizeSplit(currPartitionSize, seed, split, rScripFileName, rBin, hotellingTestThreshold);
                        }
                        else if (split == SplitType.Partition)
                        {
                            //mlpModel.SameSizeSplit2(currPartitionSize, seed, split, rScripFileName, rBin, hotellingTestThreshold);
                            mlpModel.ProcessTrainingDataset(currPartitionSize, seed, split, rScripFileName, rBin, hotellingTestThreshold);

                        }

                        #region TODO
                        //mlpModel.PrintTrain();
                        //mlpModel.PrintTest();
                        //mlpModel.GenerateArtificalDataUsingNN(numInput, numHidden, numOutput);
                        //mlpModel.PrintWeights(2, 10, true);

                        ////TODO: not used
                        // NOT MIGRATED
                        //double[] inputNodeTotalWeightsArray = DFT.ShowVectorWInput(NUMINPUT, NUMHIDDEN, NUMOUTPUT, weights, 2);

                        ////TODO: not used
                        //double[] rankArray = null;
                        //if (ISFEATURESELECTION)
                        //{
                        //    rankArray = DFT.GenerateRankArray(NUMINPUT, inputNodeTotalWeightsArray);
                        //    weights = DFT.UpdateWeightsArrayByRank(NUMINPUT, NUMHIDDEN, weights, rankArray);
                        //    inputNodeTotalWeightsArray = DFT.ShowVectorWInput(NUMINPUT, NUMHIDDEN, NUMOUTPUT, weights, 2);
                        //    neuralNetwork.SetWeights(weights);
                        //}
                        #endregion

                        mlpModel.TrainByNN(maxEpochs, learnRate, momentum);
                        //Find redundant attributes


                        #region TODO
                        //mlpModel.PrintWeights(2, 10, true);
                        ////TODO: not used
                        //bool[,] inputTable = DFT.GenerateTruthTable(NUMINPUT);
                        //bool[] answer1 = new bool[inputTable.GetLength(0)];

                        ////TODO: not used
                        //if (ISFEATURESELECTION)
                        //{
                        //    inputTable = DFT.SetIrrelevantVariables(NUMINPUT, inputTable, rankArray);
                        //}
                        #endregion

                        #region Main DFT model creation begins
                        //DFTModel dftModel = new DFTModel(mlpModel.GetNeuralNetwork(), mlpModel.TestDataFull, mlpModel.TestData, isFSOn, autoCoefficientCalculation, energyThresholdLimit, maxAutOrderLimit, null); //TODO: Add rank array
                        //dftModel.SpliteInstanceSchemasByClassValueTrain();
                        ////dftModel.SpliteInstanceSchemasByClassValueTest();
                        //dftModel.GenerateClusteredSchemaPatterns();
                        //dftModel.GenerateJVectorByEnegryThresholdingLimit(presetOrderNum); //concept of energy thresholding and order
                        //dftModel.CalculateDftEnergyCoeffs(dftModel.ClusteredSchemaXVectorClass1Train);

                        #endregion


                        //DFTModelExt dftModel = new DFTModelExt(
                        //    mlpModel.GetNeuralNetwork(),
                        //    mlpModel.TestDataFull,
                        //    mlpModel.TestData,
                        //    isFSOn,
                        //    autoCoefficientCalculation,
                        //    energyThresholdLimit,
                        //    maxAutOrderLimit, null);


                        DFTModelExt dftModel = new DFTModelExt(
                                                    mlpModel.GetNeuralNetwork(),
                                                    mlpModel.TrainData,
                                                    mlpModel.TestData,
                                                    isFSOn,
                                                    false,
                                                    energyThresholdLimit,
                                                    maxAutOrderLimit, null);


                        HashSet<string> class0 = new HashSet<string>();
                        HashSet<string> class1 = new HashSet<string>();
                        Dictionary<string, SchemaStat> listClass1 = new Dictionary<string, SchemaStat>();

                        Dictionary<int, double> bottomToTop = new Dictionary<int, double>();
                        string allZeroVectorOfDim = string.Empty;
                        double sum = 0;

                        //Generate the initial Energy Coeff array with redundant list
                        //using the training set
                        dftModel.ExtractUniqueInstanceSchemasTrain(out class0, out class1);
                        listClass1 = dftModel.GenerateClusteredSchemaPatterns(class0, class1);
                        dftModel.JVectorsTrain = dftModel.GenerateJVectorByEnegryThresholdingLimit(presetOrderNum);
                        Dictionary<string, double> coeff_a = dftModel.CalculateDftEnergyCoeffs(listClass1);
                        fp.OutputEnergyCoeffsToCSV(dftModel.EnergyCoeffsTrain, split, currPartitionSize + "_EnergyCoeffs_Tr");



                        //var redundantAttributesList = dftModel.RedundantFeaturesByEnergyCoffsArray(presetOrderNum);

                        #region Adaptation using the Testset

                        // adaptation using test set
                        // intialise the testing environment
                        var currSchemaReservior = dftModel.SchemaTrain;
                        fp.ReserviorToCSV(currSchemaReservior, split, currPartitionSize + "_currSchemaReservior_Tr");




                        int triggeredInterval = 1;
                        int cumulativeChanges = 0;


                        int numAtt = inputSpec.NumAttributes;
                        //float[][] FullTestSet = mlpModel.TestDataFull;
                        float[][] FullTestSet = mlpModel.TestData;


                        int testDataSize = FullTestSet.Length;
                        int intervalSize = 5;// FullTestSet.Length;
                        int numIterations = testDataSize / intervalSize;
                        //numIterations++;

                        // 1.  get a batch size of test instances
                        for (int i = 0; i < numIterations; i++)
                        {
                            // 2.  process each batch by
                            double cachedFx = -1;

                            for (int k = 0; k < intervalSize; k++)
                            {
                                int index = (i * intervalSize) + k;

                                string testInstance;
                                double actualClassValue;
                                double fxTestInstance;

                                GetTestInstanceString(numAtt, FullTestSet, index, out testInstance, out actualClassValue);

                                //Check with the cache
                                // 2.1 get the f(x) using inverse or cache

                                SchemaStat cachedSchemaStat = dftModel.CheckIfInstanceCanBeMatchedToPatternSchemaStat(testInstance, currSchemaReservior);
                                //fp.ReserviorToCSV(currSchemaReservior, split, currPartitionSize + "_currSchemaReservior");

                                bool newApperance = false;

                                int cachedFxString = -99;
                                if (cachedSchemaStat != null)
                                {
                                    cachedFxString = cachedSchemaStat.ClassLabelClassifiedByMLP;
                                }
                                else
                                {
                                    newApperance = true;
                                }
                                //string cachedFxString = dftModel.CheckIfInstanceCanBeMatchedToPattern(testInstance, currSchemaReservior);

                                cachedFx = Convert.ToDouble(cachedFxString);
                                double invFx = -99;
                                if (cachedFx == -99 || newApperance == true)
                                {
                                    invFx = dftModel.CalculateFxByInveseDftEquation(testInstance, dftModel.JVectorsTrain, dftModel.EnergyCoeffsTrain);
                                }

                                if (invFx != -99)
                                {
                                    fxTestInstance = invFx;
                                }
                                else
                                {
                                    fxTestInstance = cachedFx;
                                }

                                //2.2 update the stats of the curr
                                //get the reservoir, update the stat
                                if (cachedSchemaStat != null)
                                {
                                    //increase A or B
                                    if (fxTestInstance == 0 && actualClassValue == 0)
                                    {
                                        //A->A
                                        cachedSchemaStat.AddCurrClassA();
                                    }
                                    else if (fxTestInstance == 0 && actualClassValue == 1)
                                    {
                                        //A->B 
                                        cachedSchemaStat.AddCurrClassB();
                                        cachedSchemaStat.AToBChangeCurr();
                                    }
                                    else if (fxTestInstance == 1 && actualClassValue == 1)
                                    {
                                        //B->B //same
                                        cachedSchemaStat.AddCurrClassB();
                                    }
                                    else if (fxTestInstance == 1 && actualClassValue == 0)
                                    {
                                        //B->A //same
                                        cachedSchemaStat.AddCurrClassA();
                                        cachedSchemaStat.BToAChangeCurr();
                                    }
                                }
                                else
                                {
                                    //new instance appear
                                    cachedSchemaStat = new SchemaStat(testInstance, testInstance, int.Parse(actualClassValue.ToString()), int.Parse(fxTestInstance.ToString()));
                                    //if (fxTestInstance == 0)
                                        if (actualClassValue == 0)
                                        {
                                            //0->A
                                            cachedSchemaStat.AddCurrClass0A();
                                    }
                                    else if (actualClassValue == 1)
                                    //else if (fxTestInstance == 1)
                                    {
                                        //0->B
                                        cachedSchemaStat.AddCurrClass0B();
                                    }

                                    //add the new pattern to reservior// TODO: add pattern instead
                                    currSchemaReservior.Add(testInstance, cachedSchemaStat);

                                }

                                fp.TestInstanceMatchingToCSV(testInstance, cachedSchemaStat.ClusterPattern, split, currPartitionSize + "_testInstanceMatchingToCSV_" + i, false);

                            }

                            // 3.  copy the change starts to previous and update the coefficient array
                            //if (i != 0)
                            //{
                                bool triggerUpdate = CalculateTrigger(currSchemaReservior, triggeredInterval, intervalSize, cumulativeChanges);
                                if (triggerUpdate)
                                {
                                    triggeredInterval = 1;
                                    cumulativeChanges = 0;
                                    //if (i > 1)
                                    //{
                                    dftModel.EnergyCoeffsTrain = dftModel.RefineIterator(currSchemaReservior, coeff_a);
                                    //}
                                    CopyStatsCurrToPrev(currSchemaReservior);
                                    fp.ReserviorToCSV(currSchemaReservior, split, currPartitionSize + "_currSchemaReservior_" + i);
                                    fp.OutputEnergyCoeffsToCSV(dftModel.EnergyCoeffsTrain, split, currPartitionSize + "_EnergyCoeffs_" + i);
                                }
                                else
                                {
                                    UpdateCummulativeCount(currSchemaReservior, out cumulativeChanges);
                                    triggeredInterval++;
                                }
                                //bool triggerUpdate = CheckTriggerStatusCurrToPrev(currSchemaReservior, out triggeredInterval);
                            //}

                        }




                        #endregion







                        #region 
                        //InverseDFTModel inverseDftModel = new InverseDFTModel(mlpModel, dftModel);
                        //inverseDftModel.Validate(split, currPartitionSize);
                        #endregion


                        #region adaptaton

                        //int intervalRecordSet = 1000;
                        //float[][] testdata = mlpModel.TestDataFull;


                        //var schemaArrayTests = new Dictionary<string, schemaClasfcnStat>();
                        //try
                        //{
                        //    for (int r = 0; r < intervalRecordSet; r++)
                        //    {
                        //        HashSet<string> testXVectors = new HashSet<string>();
                        //        float[] sin = testdata[r];
                        //        string str = string.Empty;
                        //        int classVal = 0;
                        //        for (int k = 0; k <= numAtt; k++)
                        //        {
                        //            if (numAtt != k)
                        //            {
                        //                str += sin[k].ToString();
                        //            }
                        //            else
                        //            {
                        //                classVal = (int) sin[k];
                        //            }
                        //        }
                        //        testXVectors.Add(str);
                        //        var ans = inverseDftModel.ProcessTesting(testXVectors, classVal);

                        //        if (!schemaArrayTests.ContainsKey(str))
                        //        {
                        //            schemaArrayTests.Add(str, new schemaClasfcnStat(ans, classVal.ToString()));
                        //        }

                        //    }
                        //}
                        //catch (Exception ex)
                        //{
                        //    throw;
                        //}



                        //var redIndex = dftModel.RedundantIndexListTrain;
                        //var schemaPatterns0 = dftModel.ClusteredSchemaXVectorClass0Train;
                        //var schemaPatterns1 = dftModel.ClusteredSchemaXVectorClass1Train;
                        //var schemaArray = new Dictionary<string, schemaClasfcnStat>();
                        //foreach (string s in schemaPatterns0)
                        //{
                        //    schemaClasfcnStat c = new schemaClasfcnStat(0.0, "0");
                        //    schemaArray.Add(DFTModel.ReplaceSchemaInstance(s), c);
                        //}

                        //foreach (string s in schemaPatterns1)
                        //{
                        //    schemaClasfcnStat c = new schemaClasfcnStat(1.0, "1");
                        //    schemaArray.Add(DFTModel.ReplaceSchemaInstance(s), c);
                        //}

                        #endregion


                        //inverseDftModel.RefineDFTModel();





                        //ResultsStatistics results = CreateStats(currPartitionSize, inputSpec, mlpModel, dftModel);
                        //stats.Add(results);


                        //Increase the partition size
                        currPartitionSize = currPartitionSize < 0.1M ? currPartitionSize += 0.01M : currPartitionSize += 0.1M;


                        dftModel.Dispose();
                        dftModel = null;

                        //inverseDftModel.Dispose();
                        //inverseDftModel = null;

                        mlpModel.Dispose();
                        mlpModel = null;

                    }

                    WriteResultsToCSV(stats, outputDataPath, inputSpec.InputDatasetFileName, split);
                }
                catch (Exception ex)
                {
                    string msg = file.GetFileName() + ", partition size (one after): " + currPartitionSize + ", split: " + split.ToString() + ", error: " + ex.Message;
                    LogMessage(outputDataPath, "Error", msg);
                    //throw;
                }

            }


            //foreach (var file in fileList)
            //{
            //    decimal currPartitionSize = 0.01M;
            //    decimal partitionLimit = 0.8M;
            //    int presetOrderNum = -1;
            //    int maxAutOrderLimit = 4;
            //    bool autoCoefficientCalculation = true;
            //    decimal energyThresholdLimit = 0.90M;
            //    SplitType split = SplitType.FixedSizeOptimumSet;
            //    List<ResultsStatistics> stats = new List<ResultsStatistics>();

            //    //Setup File
            //    InputSpecification inputSpec = (InputSpecification)file;

            //    try
            //    {
            //        //Use the settings below to limit the iterations
            //        //Use slip type settings
            //        // 1.Set input file and load dataset
            //        FileProcessor fp = new FileProcessor(inputDataPath, outputDataPath, resultsDataPath, rScriptPath, inputSpec);
            //        fp.LoadCSV();

            //        RRunner rn = new RRunner();

            //        while (currPartitionSize <= partitionLimit)
            //        {
            //            // 2. Create MLP model    
            //            MLPModel mlpModel = new MLPModel(numHidden, numOutput, fp, rn);
            //            if (split == SplitType.LinearSequence)
            //            {
            //                mlpModel.LinearSeqTrainTestSplit(currPartitionSize, seed);
            //                mlpModel.RunHotellingTTest(mlpModel.TrainingFileName, mlpModel.TestingFileName, rScripFileName, rBin, hotellingTestThreshold, currPartitionSize, split);
            //            }
            //            else
            //            {
            //                //Select the optimum the fold (with a curr partition size) that would give the best accuracy
            //                // by using Hotelling T-Test
            //                mlpModel.OptimizeSplit(currPartitionSize, seed, split, rScripFileName, rBin, hotellingTestThreshold);
            //            }

            //            #region TODO
            //            //mlpModel.PrintTrain();
            //            //mlpModel.PrintTest();
            //            //mlpModel.GenerateArtificalDataUsingNN(numInput, numHidden, numOutput);
            //            //mlpModel.PrintWeights(2, 10, true);

            //            ////TODO: not used
            //            // NOT MIGRATED
            //            //double[] inputNodeTotalWeightsArray = DFT.ShowVectorWInput(NUMINPUT, NUMHIDDEN, NUMOUTPUT, weights, 2);

            //            ////TODO: not used
            //            //double[] rankArray = null;
            //            //if (ISFEATURESELECTION)
            //            //{
            //            //    rankArray = DFT.GenerateRankArray(NUMINPUT, inputNodeTotalWeightsArray);
            //            //    weights = DFT.UpdateWeightsArrayByRank(NUMINPUT, NUMHIDDEN, weights, rankArray);
            //            //    inputNodeTotalWeightsArray = DFT.ShowVectorWInput(NUMINPUT, NUMHIDDEN, NUMOUTPUT, weights, 2);
            //            //    neuralNetwork.SetWeights(weights);
            //            //}
            //            #endregion


            //            mlpModel.TrainByNN(maxEpochs, learnRate, momentum);

            //            #region TODO
            //            //mlpModel.PrintWeights(2, 10, true);
            //            ////TODO: not used
            //            //bool[,] inputTable = DFT.GenerateTruthTable(NUMINPUT);
            //            //bool[] answer1 = new bool[inputTable.GetLength(0)];

            //            ////TODO: not used
            //            //if (ISFEATURESELECTION)
            //            //{
            //            //    inputTable = DFT.SetIrrelevantVariables(NUMINPUT, inputTable, rankArray);
            //            //}
            //            #endregion

            //            #region Main DFT model creation begins
            //            DFTModel dftModel = new DFTModel(mlpModel.GetNeuralNetwork(), mlpModel.TrainData, mlpModel.TestData, isFSOn, autoCoefficientCalculation, energyThresholdLimit, maxAutOrderLimit, null); //TODO: Add rank array
            //            dftModel.SpliteInstanceSchemasByClassValueTrain();
            //            dftModel.SpliteInstanceSchemasByClassValueTest();
            //            dftModel.GenerateClusteredSchemaPatterns();
            //            dftModel.GenerateJVectorByEnegryThresholdingLimit(presetOrderNum); //concept of energy thresholding and order
            //            dftModel.CalculateDftEnergyCoeffs(dftModel.ClusteredSchemaXVectorClass1Train);
            //            #endregion


            //            #region 
            //            InverseDFTModel inverseDftModel = new InverseDFTModel(mlpModel, dftModel);
            //            inverseDftModel.Validate(split, currPartitionSize);
            //            #endregion


            //            ResultsStatistics results = CreateStats(currPartitionSize, inputSpec, mlpModel, dftModel);
            //            stats.Add(results);


            //            //Increase the partition size
            //            currPartitionSize = currPartitionSize < 0.1M ? currPartitionSize += 0.01M : currPartitionSize += 0.1M;


            //            dftModel.Dispose();
            //            dftModel = null;

            //            inverseDftModel.Dispose();
            //            inverseDftModel = null;

            //            mlpModel.Dispose();
            //            mlpModel = null;

            //        }

            //        WriteResultsToCSV(stats, outputDataPath, inputSpec.InputDatasetFileName, split);
            //    }
            //    catch (Exception ex)
            //    {
            //        string msg = file.GetFileName() + ", partition size (one after): " + currPartitionSize + ", split: " + split.ToString() + ", error: " + ex.Message;
            //        LogMessage(outputDataPath, "Error", msg);
            //        //throw;
            //    }

            //}

            //Console.ReadKey();

        }

        //private static bool CheckTriggerStatusCurrToPrev2(Dictionary<string, SchemaStat> schemaPatternList)
        //{
        //    bool trigger = false;
        //    foreach (var i in schemaPatternList.Values)
        //    {
        //        if (i.GetPrevMajority() == 0.0 && i.GetCurrMajority() == 1.0)
        //        {
        //            trigger = true; //A->B
        //        }
        //        else if (i.GetPrevMajority() == 1.0 && i.GetCurrMajority() == 0.0)
        //        {
        //            trigger = true; //B->A
        //        }
        //    }
        //    return trigger;
        //}
        private static void UpdateCummulativeCount(Dictionary<string, SchemaStat> schemaPatternList, out int cumulativeCount)
        {
            cumulativeCount = 0;
            foreach (var i in schemaPatternList.Values)
            {
                cumulativeCount += i.AAChangeCurr;
            }
        }


        private static bool CalculateTrigger(Dictionary<string, SchemaStat> schemaPatternList, int interval, int intervalSize, int cummulativeChanges)
        {
            bool trigger = false;

            int totalNumber = 0;

            foreach (var i in schemaPatternList.Values)
            {
                totalNumber += i.AAChangeCurr;
            }

            totalNumber += cummulativeChanges;

            double ratio = (double) Math.Abs(totalNumber) / (double) (interval * intervalSize);

            if (ratio >= (1 - 0.95))
            {
                trigger = true;
            }
            else
            {
                trigger = false;
            }

            return trigger;

        }



        private static bool CheckTriggerStatusCurrToPrev(Dictionary<string, SchemaStat> schemaPatternList)
        {
            bool trigger = false;
            foreach (var i in schemaPatternList.Values)
            {
                if (i.GetPrevMajority() == 0.0 && i.GetCurrMajority() == 1.0)
                {
                    trigger = true; //A->B
                }
                else if (i.GetPrevMajority() == 1.0 && i.GetCurrMajority() == 0.0)
                {
                    trigger = true; //B->A
                }
            }
            return trigger;
        }

        private static void CopyStatsCurrToPrev(Dictionary<string, SchemaStat> schemaPatternList)
        {
            foreach (var i in schemaPatternList.Values)
            {
                i.CopyCurrToPrev();
            }
        }



        private static void GetTestInstanceString(int numAtt, float[][] FullTestSet, int index, out string testInstance, out double actualClassValue)
        {
            //extracting test instance
            float[] sin = FullTestSet[index];
            testInstance = string.Empty;
            actualClassValue = 0;
            for (int k = 0; k <= numAtt; k++)
            {
                if (numAtt != k)
                {
                    testInstance += sin[k].ToString();
                }
                else
                {
                    actualClassValue = (double) sin[k];
                }
            }
        }

        private static ResultsStatistics CreateStats(decimal currPartitionSize, InputSpecification inputSpec, MLPModel mlpModel, DFTModel dftModel)
        {
            ResultsStatistics results = new ResultsStatistics();

            results.FileName = inputSpec.InputDatasetFileName;
            results.NumAttribute = inputSpec.NumAttributes;
            results.TotalSize = inputSpec.NumRows;

            results.TrainingFile = mlpModel.TrainingFileName;
            results.TrainSize = mlpModel.TrainData.Length;
            results.TrainingTime = mlpModel.TrainingTime;

            results.TestFile = mlpModel.TestingFileName;
            results.TestSize = mlpModel.TestData.Length;
            results.TestingTime = mlpModel.TestingTime;

            results.PerSplit = currPartitionSize; //????

            results.TrainingAccuracy = mlpModel.TrainAcc;
            results.TestingAccuracy = mlpModel.TestAcc;
            results.TrainingTime = mlpModel.TrainingTime;

            results.NumTotalInstancesXClass0 = dftModel.NumTotalInstancesXClass0Train;
            results.NumTotalInstancesXClass1 = dftModel.NumTotalInstancesXClass1Train;

            results.ResolvedUniqueSchemaInstancesXClass0 = dftModel.AllSchemaXVectorClass0Train;
            results.ResolvedUniqueSchemaInstancesXClass1 = dftModel.AllSchemaXVectorClass1Train;

            results.NumResolvedUniqueSchemaInstancesXClass0 = dftModel.AllSchemaXVectorClass0Train.Count;
            results.NumResolvedUniqueSchemaInstancesXClass1 = dftModel.AllSchemaXVectorClass1Train.Count;

            results.PatternsXClass0 = dftModel.ClusteredSchemaXVectorClass0Train;
            results.NumPatternsXClass0 = dftModel.ClusteredSchemaXVectorClass0Train.Count;

            results.PatternsXClass1 = dftModel.ClusteredSchemaXVectorClass1Train;
            results.NumPatternsXClass1 = dftModel.ClusteredSchemaXVectorClass1Train.Count;

            //results.EnergyCoefficients = dftModel.EnergyCoeffsTrain;
            results.NumEnergyCoefficients = dftModel.EnergyCoeffsTrain.Count;
            results.EnergyCoefficientTime = dftModel.CoefficientGenerationTime;
            results.EnergyCoefficientOrderNum = dftModel.EnergyCoefficientOrderNum;
            results.AutoEnergyThresholding = dftModel.AutoEnergyThresholding;
            results.EnergyThresholdLimit = dftModel.EnergyThresholdLimit;

            results.PVal.Add(currPartitionSize, mlpModel.PValue);
            results.HotellingTestTime = mlpModel.HotellingTestTime;

            results.DFTModelTestDataAccuracy = dftModel.DFTModelTestDataAccuracy;
            results.DFTModelTestDataTime = dftModel.DFTModelTestDataTime;
            results.Shortcut_ClusterPatternMachingTestDataAccuracy = dftModel.Shortcut_ClusterPatternMachingTestDataAccuracy;
            results.Shortcut_ClusterPatternMachingTestDataTime = dftModel.Shortcut_ClusterPatternMachingTestDataTime;

            results.DFTModelTrainDataAccuracy = dftModel.DFTModelTrainDataAccuracy;
            results.DFTModelTrainDataTime = dftModel.DFTModelTrainDataTime;
            results.Shortcut_ClusterPatternMachingTrainDataAccuracy = dftModel.Shortcut_ClusterPatternMachingTrainDataAccuracy;
            results.Shortcut_ClusterPatternMachingTrainDataTime = dftModel.Shortcut_ClusterPatternMachingTrainDataTime;


            return results;
        }

        public static void WriteResultsToCSV(List<ResultsStatistics> list, string path, string fileName, SplitType split)
        {
            try
            {
                Console.WriteLine("==========================");
                Console.WriteLine("Writing to file {0}", fileName);

                string folderName = split.ToString() + "_" + fileName;
                int index = folderName.IndexOf('.');
                if (index > 0)
                {
                    folderName = folderName.Substring(0, index);
                }

                ////Delete an existing zip file
                string zipPath = path + folderName + ".zip";
                if (File.Exists(zipPath))
                {
                    File.Delete(zipPath);
                }


                if (!Directory.Exists(path + folderName))
                {
                    Directory.CreateDirectory(path + folderName);
                }
                string fileNameAndPath = path + folderName + "\\" + fileName + ".csv";
                if (File.Exists(fileNameAndPath))
                {
                    File.Delete(fileNameAndPath);
                }
                var sw = new StreamWriter(fileNameAndPath, true);
                string header = "FileName,NumAttribute,TotalSize,PerSplit,TrainingFile,TrainSize,TestFile,TestSize,MLPTrainingAccuracy,MLPTestingAccuracy,MLPTrainingTime,MLPTestingTime,NumTotalInstancesXClass0,NumTotalInstancesXClass1,NumResolvedUniqueSchemaInstancesXClass0,ResolvedUniqueSchemaInstancesXClass0,NumResolvedUniqueSchemaInstancesXClass1,ResolvedUniqueSchemaInstancesXClass1,NumPatternsXClass0,PatternsXClass0,NumPatternsXClass1,PatternsXClass1,NumEnergyCoefficients,EnergyCoefficientGenerationTime,PVal,HotellingTestTime,DFTModelTestDataAccuracy,DFTModelTestDataTime,Shortcut_ClusterPatternMachingTestDataAccuracy,Shortcut_ClusterPatternMachingTestDataTime,DFTModelTrainDataAccuracy,DFTModelTrainDataTime,Shortcut_ClusterPatternMachingTrainDataAccuracy,Shortcut_ClusterPatternMachingTrainDataTime,AutoEnergyCoeff,SelectedOrder,EnergyThresholdLimit";
                sw.Write(header);
                sw.Write("\r\n");
                string patternSep = "#";
                foreach (var s in list)
                {
                    sw.Write(s.FileName);
                    sw.Write(",");
                    sw.Write(s.NumAttribute);
                    sw.Write(",");
                    sw.Write(s.TotalSize);
                    sw.Write(",");
                    sw.Write(s.PerSplit);
                    sw.Write(",");
                    sw.Write(s.TrainingFile);
                    sw.Write(",");
                    sw.Write(s.TrainSize);
                    sw.Write(",");
                    sw.Write(s.TestFile);
                    sw.Write(",");
                    sw.Write(s.TestSize);
                    sw.Write(",");
                    sw.Write(s.TrainingAccuracy);
                    sw.Write(",");
                    sw.Write(s.TestingAccuracy);
                    sw.Write(",");
                    sw.Write(s.TrainingTime);
                    sw.Write(",");
                    sw.Write(s.TestingTime);
                    sw.Write(",");

                    sw.Write(s.NumTotalInstancesXClass0);
                    sw.Write(",");

                    sw.Write(s.NumTotalInstancesXClass1);
                    sw.Write(",");

                    sw.Write(s.NumResolvedUniqueSchemaInstancesXClass0);
                    sw.Write(",");
                    string p = string.Empty;
                    foreach (var i in s.ResolvedUniqueSchemaInstancesXClass0)
                    {
                        p += i.ToString() + patternSep;
                    }
                    sw.Write(p);
                    sw.Write(",");



                    sw.Write(s.NumResolvedUniqueSchemaInstancesXClass1);
                    sw.Write(",");
                    p = string.Empty;
                    foreach (var i in s.ResolvedUniqueSchemaInstancesXClass1)
                    {
                        p += i.ToString() + patternSep;
                    }
                    sw.Write(p);
                    sw.Write(",");



                    sw.Write(s.NumPatternsXClass0);
                    sw.Write(",");
                    p = string.Empty;
                    foreach (var i in s.PatternsXClass0)
                    {
                        p += i.ToString() + patternSep;
                    }
                    sw.Write(p);
                    sw.Write(",");



                    sw.Write(s.NumPatternsXClass1);
                    sw.Write(",");
                    p = string.Empty;
                    foreach (var i in s.PatternsXClass1)
                    {
                        p += i.ToString() + patternSep;
                    }
                    sw.Write(p);
                    sw.Write(",");


                    sw.Write(s.NumEnergyCoefficients);
                    sw.Write(",");


                    p = string.Empty;

                    //if (s.NumAttribute <= 5)
                    //{
                    //    foreach (var i in s.EnergyCoefficients)
                    //    {
                    //        p += i.Key.ToString() + ":" + i.Value.ToString() + patternSep;
                    //    }
                    //}
                    //sw.Write(p);
                    //sw.Write(",");


                    sw.Write(s.EnergyCoefficientTime);
                    sw.Write(",");


                    p = string.Empty;
                    foreach (var i in s.PVal)
                    {
                        p += i.Value.ToString();
                    }
                    sw.Write(p);

                    sw.Write(",");

                    sw.Write(s.HotellingTestTime);
                    sw.Write(",");


                    sw.Write(s.DFTModelTestDataAccuracy);
                    sw.Write(",");
                    sw.Write(s.DFTModelTestDataTime);
                    sw.Write(",");
                    sw.Write(s.Shortcut_ClusterPatternMachingTestDataAccuracy);
                    sw.Write(",");
                    sw.Write(s.Shortcut_ClusterPatternMachingTestDataTime);


                    sw.Write(",");
                    sw.Write(s.DFTModelTrainDataAccuracy);
                    sw.Write(",");
                    sw.Write(s.DFTModelTrainDataTime);
                    sw.Write(",");
                    sw.Write(s.Shortcut_ClusterPatternMachingTrainDataAccuracy);
                    sw.Write(",");
                    sw.Write(s.Shortcut_ClusterPatternMachingTrainDataTime);

                    sw.Write(",");
                    sw.Write(s.AutoEnergyThresholding);

                    sw.Write(",");
                    sw.Write(s.EnergyCoefficientOrderNum);

                    sw.Write(",");
                    sw.Write(s.EnergyThresholdLimit);

                    sw.Write("\r\n");


                }
                sw.Flush();
                sw.Close();
                sw = null;

                Console.WriteLine("end....");
                //Console.WriteLine("zipping files....");
                //string startPath = path + folderName;//folder to add
                //ZipFile.CreateFromDirectory(startPath, zipPath, CompressionLevel.Fastest, true);
                //Console.WriteLine("deleting folder....");
                //if (Directory.Exists(path + folderName))
                //{
                //    Directory.Delete(path + folderName, true);
                //}
                Console.WriteLine("done....");


            }
            catch (Exception ex)
            {
                //LogMessage(path, "LogFile", ex.Message);
                throw;
                //return;
            }
        }



        public static void LogMessage(string mainPath, string fileName, string msg, bool append = true)
        {
            string fileNameAndPath = mainPath + fileName + ".log";

            if (!append)
            {
                if (File.Exists(fileNameAndPath))
                {
                    File.Delete(fileNameAndPath);
                }
            }
            var sw = new StreamWriter(fileNameAndPath, true);
            sw.Write(msg.ToString());
            sw.Write("\r\n");
            sw.Flush();
            sw.Close();
        }



    }



}
