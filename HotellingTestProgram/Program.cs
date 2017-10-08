using Algorithms;
using BackPropProgram;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotellingTestProgram
{
    class Program
    {

        /*
         
            Used as a testing 

             
             */

        static void Main(string[] args)
        {
            Console.WriteLine("++++++ Dynamic Parition Selection using Hotelling +++++++");
            //TODO: Parition logic
            const int seed = 123;
            const int maxEpochs = 100;
            const double learnRate = 0.3;
            const double momentum = 0.2;
            const int numHidden = 8;
            const int numOutput = 2; // number of classes for Y
            const bool isFSOn = false;
            const string rBin = @"C:\Program Files\R\R-3.4.1\bin\rscript.exe";
            //const string rScripFileName = "hotellingttest3.r";
            const string rScripFileName = "Script.r";

            string inputDataPath = @"D:\ANN_Project_AUT_Sem3\Microsoft\BackPropProgram\Data\";
            string outputDataPath = @"D:\ANN_Project_AUT_Sem3\Microsoft\BackPropProgram\OutputDataHotelling\";
            string resultsDataPath = @"D:\ANN_Project_AUT_Sem3\Microsoft\BackPropProgram\ResultsDataOutputDataHotelling\";
            string rScriptPath = @"D:\ANN_Project_AUT_Sem3\Microsoft\BackPropProgram\HotellingTTest\";

            bool isShuffleOn = false;
            int dftEnergyThresholdingLimit = 4;

            decimal startPartitionSize = 0.1M;
            decimal lastPartitionSize = 0.1M;

            const double hotellingTestThreshold = 0.05;


            InputSpecification inputSpec = new ElectricitySmall();
            FileProcessor fp = new FileProcessor(inputDataPath, outputDataPath, resultsDataPath, rScriptPath, inputSpec);
            fp.LoadCSV();
            RRunner rn = new RRunner();
            MLPModel mlpModel = null;

            Dictionary<string, bool> HotellingResult = new Dictionary<string, bool>();
            Dictionary<string, double> PValues = new Dictionary<string, double>();
            Dictionary<string, double> TrAcc = new Dictionary<string, double>();
            Dictionary<string, double> TsAcc = new Dictionary<string, double>();
            Dictionary<string, string> TrTime = new Dictionary<string, string>();

            int iter = 0;
            while (startPartitionSize <= lastPartitionSize)
            {
                //if (startPartitionSize < 0.01M)
                //{
                //    break;
                //}
                mlpModel = new MLPModel(numHidden, numOutput, fp, rn);

                int numTakeFolds = (int)(1.0M / startPartitionSize);

                //mlpModel.FixedSizeOptimumSetTrainTestSplit(startPartitionSize, seed, 9);


                for (int f = 0; f < numTakeFolds; f++)
                {
                    Console.WriteLine("******** fold {0} ****", f);

                    iter++;
                    mlpModel.FixedSizeOptimumSetTrainTestSplit(startPartitionSize, seed, f);


                    mlpModel.RunHotellingTTest(mlpModel.TrainingFileName, mlpModel.TestingFileName, rScripFileName, rBin, hotellingTestThreshold, startPartitionSize, SplitType.FixedSizeOptimumSet);

                    string keyA = startPartitionSize + "_" + f;

                    PValues.Add(keyA, mlpModel.PValue);

                    mlpModel.TrainByNN(maxEpochs, learnRate, momentum);


                    TsAcc.Add(keyA, mlpModel.TestAcc);
                    TrAcc.Add(keyA, mlpModel.TrainAcc);
                    TrTime.Add(keyA, mlpModel.TrainingTime.ToString());
                }

                if (startPartitionSize < 0.1M)
                {
                    startPartitionSize += 0.01M;
                }
                else
                {
                    startPartitionSize += 0.1M;
                }



            }

            for (int i = 0; i < iter; i++)
            {
                Console.WriteLine("%:{0}, TrAcc:{1}, TsAcc:{2}, PVal:{3}, TrTime:{4}", PValues.ElementAt(i).Key, TrAcc.ElementAt(i).Value, TsAcc.ElementAt(i).Value, PValues.ElementAt(i).Value, TrTime.ElementAt(i).Value);
            }



            mlpModel.Dispose();
            mlpModel = null;

            Console.ReadKey();
        }
    }
}