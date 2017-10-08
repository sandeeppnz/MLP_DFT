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

            decimal startPartitionSize = 0.01M;
            decimal lastPartitionSize = 0.1M;

            const double hotellingTestThreshold = 0.05;


            InputSpecification inputSpec = new ElectricitySmall();
            FileProcessor fp = new FileProcessor(inputDataPath, outputDataPath, resultsDataPath, rScriptPath, inputSpec);
            fp.LoadCSV();
            RRunner rn = new RRunner();
            MLPModel mlpModel = null;

            Dictionary<decimal, bool> HotellingResult = new Dictionary<decimal, bool>();
            Dictionary<decimal, double> PValues = new Dictionary<decimal, double>();
            Dictionary<decimal, double> TrAcc = new Dictionary<decimal, double>();
            Dictionary<decimal, double> TsAcc = new Dictionary<decimal, double>();
            Dictionary<decimal, string> TrTime = new Dictionary<decimal, string>();

            int iter = 0;
            while (startPartitionSize <= lastPartitionSize)
            {
                iter++;
                //if (startPartitionSize < 0.01M)
                //{
                //    break;
                //}
                mlpModel = new MLPModel(numHidden, numOutput, fp, rn);


                mlpModel.LinearSeqTrainTestSplit(startPartitionSize, seed);
                bool result = mlpModel.RunFoldingHotellingTTest(mlpModel.TrainingFileName, mlpModel.TestingFileName, rScripFileName, rBin, hotellingTestThreshold, startPartitionSize);

                PValues.Add(startPartitionSize, mlpModel.PValLinearSeqSplit);
                HotellingResult.Add(startPartitionSize, result);

                mlpModel.TrainByNN(maxEpochs, learnRate, momentum);
                TsAcc.Add(startPartitionSize, mlpModel.TestAcc);
                TrAcc.Add(startPartitionSize, mlpModel.TrainAcc);
                TrTime.Add(startPartitionSize, mlpModel.TrainingTime.ToString());


                if (startPartitionSize < 0.1M)
                {
                    startPartitionSize += 0.01M;
                }
                else
                {
                    startPartitionSize += 0.1M;
                }



            }

            for (int i = 0; i < iter - 1; i++)
            {
                Console.WriteLine("%:{0}, R:{1}, TrAcc:{2}, TsAcc:{3}, PVal:{4}, TrTime:{5}", PValues.ElementAt(i).Key, HotellingResult.ElementAt(i).Value, TrAcc.ElementAt(i).Value, TsAcc.ElementAt(i).Value, PValues.ElementAt(i).Value, TrTime.ElementAt(i).Value);
            }



            mlpModel.Dispose();
            mlpModel = null;

            Console.ReadKey();
        }
    }
}