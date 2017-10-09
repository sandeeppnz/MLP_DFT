using Algorithms;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

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

            fileList.Add(new ElectricitySmall());
            //fileList.Add(new ElectricityExtended());


            //fileList.Add(new SensorSmall());
            //fileList.Add(new SensorExtended());


            //fileList.Add(new CoverTypeSmall());
            //fileList.Add(new CoverTypeIntemediate());
            //fileList.Add(new CoverTypeExtended());



            //fileList.Add(new FlightExtended());
            //fileList.Add(new FlightSmall());

            //fileList.Add(new RBF());

            //fileList.Add(new RH());


            foreach (var file in fileList)
            {
                decimal currPartitionSize = 0.01M;
                decimal partitionLimit = 0.01M;
                int dftEnergyThresholdingLimit = -1;
                bool autoCoefficientCalculation = true;
                decimal energyThresholdLimit = 0.90M;
                SplitType split = SplitType.LinearSequence;
                List<ResultsStatistics> stats = new List<ResultsStatistics>();

                //Setup File
                InputSpecification inputSpec = (InputSpecification)file;

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
                            mlpModel.LinearSeqTrainTestSplit(currPartitionSize, seed);
                            mlpModel.RunHotellingTTest(mlpModel.TrainingFileName, mlpModel.TestingFileName, rScripFileName, rBin, hotellingTestThreshold, currPartitionSize, split);
                        }
                        else
                        {
                            //Select the optimum the fold (with a curr partition size) that would give the best accuracy
                            // by using Hotelling T-Test
                            mlpModel.OptimizeSplit(currPartitionSize, seed, split, rScripFileName, rBin, hotellingTestThreshold);
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
                        DFTModel dftModel = new DFTModel(mlpModel.GetNeuralNetwork(), mlpModel.TrainData, mlpModel.TestData, isFSOn,autoCoefficientCalculation, energyThresholdLimit, null); //TODO: Add rank array
                        dftModel.SpliteInstanceSchemasByClassValueTrain();
                        dftModel.SpliteInstanceSchemasByClassValueTest();
                        dftModel.GenerateClusteredSchemaPatterns();
                        dftModel.GenerateJVectorByEnegryThresholdingLimit(dftEnergyThresholdingLimit); //concept of energy thresholding and order
                        dftModel.CalculateDftEnergyCoeffs(dftModel.ClusteredSchemaXVectorClass1Train);
                        #endregion


                        #region 
                        InverseDFTModel inverseDftModel = new InverseDFTModel(mlpModel, dftModel);
                        inverseDftModel.Validate(split, currPartitionSize);
                        #endregion


                        ResultsStatistics results = CreateStats(currPartitionSize, inputSpec, mlpModel, dftModel);
                        stats.Add(results);


                        //Increase the partition size
                        currPartitionSize = currPartitionSize < 0.1M ? currPartitionSize += 0.01M : currPartitionSize += 0.1M;


                        dftModel.Dispose();
                        dftModel = null;

                        inverseDftModel.Dispose();
                        inverseDftModel = null;

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

            //Console.ReadKey();

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
                string header = "FileName,NumAttribute,TotalSize,PerSplit,TrainingFile,TrainSize,TestFile,TestSize,MLPTrainingAccuracy,MLPTestingAccuracy,MLPTrainingTime,MLPTestingTime,NumTotalInstancesXClass0,NumTotalInstancesXClass1,NumResolvedUniqueSchemaInstancesXClass0,ResolvedUniqueSchemaInstancesXClass0,NumResolvedUniqueSchemaInstancesXClass1,ResolvedUniqueSchemaInstancesXClass1,NumPatternsXClass0,PatternsXClass0,NumPatternsXClass1,PatternsXClass1,NumEnergyCoefficients,EnergyCoefficientGenerationTime,PVal,HotellingTestTime,DFTModelTestDataAccuracy,DFTModelTestDataTime,Shortcut_ClusterPatternMachingTestDataAccuracy,Shortcut_ClusterPatternMachingTestDataTime,DFTModelTrainDataAccuracy,DFTModelTrainDataTime,Shortcut_ClusterPatternMachingTrainDataAccuracy,Shortcut_ClusterPatternMachingTrainDataTime,SelectedOrder";
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


                    sw.Write("\r\n");


                }
                sw.Flush();
                sw.Close();
                sw = null;

                Console.WriteLine("end....");
                Console.WriteLine("zipping files....");
                string startPath = path + folderName;//folder to add
                ZipFile.CreateFromDirectory(startPath, zipPath, CompressionLevel.Fastest, true);
                Console.WriteLine("deleting folder....");
                if (Directory.Exists(path + folderName))
                {
                    Directory.Delete(path + folderName, true);
                }
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
