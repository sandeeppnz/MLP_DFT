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
            string outputDataPath = @"D:\ANN_Project_AUT_Sem3\Microsoft\BackPropProgram\OutputData\";
            string resultsDataPath = @"D:\ANN_Project_AUT_Sem3\Microsoft\BackPropProgram\ResultsData\";
            string rScriptPath = @"D:\ANN_Project_AUT_Sem3\Microsoft\BackPropProgram\HotellingTTest\";

            bool isShuffleOn = false;
            int dftEnergyThresholdingLimit = 4;


            decimal partitionSize = 0.8M;
            const decimal partitionIncrement = 0.01M;
            const double hotellingTestThreshold = 0.05;


            MLPModel mlpModel = null;


            //Setup File
            InputSpecification inputSpec = new ElectricityExtended();

            // 1.Set input file and load dataset
            FileProcessor fp = new FileProcessor(inputDataPath, outputDataPath, resultsDataPath, rScriptPath, inputSpec);
            fp.LoadCSV();
            RRunner rn = new RRunner();


            // 2. Create MLP model    
            mlpModel = new MLPModel(numHidden, numOutput, fp, rn);
            mlpModel.SplitTrainTestData(partitionSize, seed);

            //mlpModel.RunHotellingTTest(mlpModel.TrainingFileName, mlpModel.TestingFileName, rScripFileName, rBin);

            mlpModel.TrainByNN(maxEpochs, learnRate, momentum);

            mlpModel.Dispose();
            mlpModel = null;
        }
    }
}