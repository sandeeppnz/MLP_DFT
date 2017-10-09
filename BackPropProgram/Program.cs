using Algorithms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

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
            //fileList.Add(new ElectricityExtended());


            //fileList.Add(new SensorSmall());
            fileList.Add(new SensorExtended());


            //fileList.Add(new CoverTypeSmall());
            //fileList.Add(new CoverTypeIntemediate());
            //fileList.Add(new CoverTypeExtended());



            //fileList.Add(new FlightExtended());
            //fileList.Add(new FlightSmall());

            //fileList.Add(new RBF());

            //fileList.Add(new RH());

            foreach (var file in fileList)
            {
                //Use the settings below to limit the iterations
                //Use slip type settings
                decimal currPartitionSize = 0.01M;
                decimal partitionLimit = 0.8M;
                int dftEnergyThresholdingLimit = 4;

                SplitType split = SplitType.LinearSequence;
                List<ResultsStatistics> stats = new List<ResultsStatistics>();

                //Setup File
                InputSpecification inputSpec = (InputSpecification)file;
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

                    #region Main DFT processing begins
                    DFTModel dftModel = new DFTModel(mlpModel.GetNeuralNetwork(), mlpModel.TrainData, isFSOn, null); //TODO: Add rank array
                    dftModel.SpliteInstanceSchemasByClassValue();
                    dftModel.GenerateClusteredSchemaPatterns();
                    var jVectors = dftModel.GenerateJVectorByEnegryThresholdingLimit(dftEnergyThresholdingLimit); //concept of energy thresholding and order
                    var energyCoffs = dftModel.CalculateDftEnergyCoeffs(dftModel.ClusteredSchemaXVectorClass1);
                    #endregion

                    ResultsStatistics results = CreateStats(currPartitionSize, inputSpec, mlpModel, dftModel);
                    stats.Add(results);

                    #region 
                    InverseDFTModel inverseDftModel = new InverseDFTModel(mlpModel,dftModel);
                    inverseDftModel.Validate(split, currPartitionSize);
                    #endregion



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

            results.NumTotalInstancesXClass0 = dftModel.NumTotalInstancesXClass0;
            results.NumTotalInstancesXClass1 = dftModel.NumTotalInstancesXClass1;

            results.ResolvedUniqueSchemaInstancesXClass0 = dftModel.AllSchemaXVectorClass0;
            results.ResolvedUniqueSchemaInstancesXClass1 = dftModel.AllSchemaXVectorClass1;

            results.NumResolvedUniqueSchemaInstancesXClass0 = dftModel.AllSchemaXVectorClass0.Count;
            results.NumResolvedUniqueSchemaInstancesXClass1 = dftModel.AllSchemaXVectorClass1.Count;

            results.PatternsXClass0 = dftModel.ClusteredSchemaXVectorClass0;
            results.NumPatternsXClass0 = dftModel.ClusteredSchemaXVectorClass0.Count;

            results.PatternsXClass1 = dftModel.ClusteredSchemaXVectorClass1;
            results.NumPatternsXClass1 = dftModel.ClusteredSchemaXVectorClass1.Count;

            results.EnergyCoefficients = dftModel.EnergyCoeffs;
            results.NumEnergyCoefficients = dftModel.EnergyCoeffs.Count;
            results.EnergyCoefficientTime = dftModel.CoefficientGenerationTime;


            results.PVal.Add(currPartitionSize, mlpModel.PValue);
            results.HotellingTestTime = mlpModel.HotellingTestTime;
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
                string header = "FileName,NumAttribute,TotalSize,PerSplit,TrainingFile,TrainSize,TestFile,TestSize,TrainingAccuracy,TestingAccuracy,TrainingTime,TestingTime,NumTotalInstancesXClass0,NumTotalInstancesXClass1,NumResolvedUniqueSchemaInstancesXClass0,ResolvedUniqueSchemaInstancesXClass0,NumResolvedUniqueSchemaInstancesXClass1,ResolvedUniqueSchemaInstancesXClass1,NumPatternsXClass0,PatternsXClass0,NumPatternsXClass1,PatternsXClass1,NumEnergyCoefficients,EnergyCoefficients,EnergyCoefficientTime,PVal,HotellingTestTime";
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

                    if (s.NumAttribute <= 5)
                    {
                        foreach (var i in s.EnergyCoefficients)
                        {
                            p += i.Key.ToString() + ":" + i.Value.ToString() + patternSep;
                        }
                    }
                    sw.Write(p);
                    sw.Write(",");


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

                    sw.Write("\r\n");


                }
                sw.Flush();
                sw.Close();

                Console.WriteLine("end....");

                sw = null;

            }
            catch (Exception)
            {
                throw;
            }
        }


    }



}
