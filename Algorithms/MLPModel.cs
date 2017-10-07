using BackPropProgram;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Algorithms
{
    public class MLPModel : IDisposable
    {
        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    //AllData = null;
                    TrainData = null;
                    TestData = null;
                    //_nn.Dispose(true); //TODO remove maybe
                    _nn = null;

                    //_fileProcessor.Dispose();
                    _fileProcessor = null;
                    _rRunner = null;
                }
            }
            this.disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            //GC.SuppressFinalize(this);
        }

        IFileProcessor _fileProcessor;
        INeuralNetwork _nn;
        IRRunner _rRunner;


        public NeuralNetwork GetNeuralNetwork()
        {
            return (NeuralNetwork)_nn;
        }

        public float[][] TrainData;
        public string TrainingFileName { get; set; }

        public float[][] TestData;
        public string TestingFileName { get; set; }

        public double TrainAcc { get; set; }
        public double TestAcc { get; set; }

        public string TestingTime { get; set; }
        public string TrainingTime { get; set; }

        public void WriteResultsToCSV(List<ResultsStatistics> list, string path, string fileName)
        {
            _fileProcessor.WriteResultsToCSV(list, path, fileName);
        }

        public MLPModel(int numHidden, int numOutput, FileProcessor fp, RRunner rRunner, bool isFSActivated = false)
        {
            //Initialize NN
            _nn = new NeuralNetwork(fp.InputSpecification.GetNumAttributes(), numHidden, numOutput, isFSActivated);
            _fileProcessor = fp;
            _rRunner = rRunner;
        }

        public void TrainByNN(int maxEpochs, double learnRate, double momentum)
        {
            Console.WriteLine("=====================================================");
            Console.WriteLine("Neural Network Classification...");
            Console.WriteLine("MaxEpochs = " + maxEpochs);
            Console.WriteLine("LearnRate = " + learnRate.ToString("F2"));
            Console.WriteLine("Momentum  = " + momentum.ToString("F2"));

            Console.WriteLine("Creating a " + _nn.GetNumInputNodes() + "-" + _nn.GetNumHiddenNodes() +
                  "-" + _nn.GetNumOutputNodes() + " neural network");

            //Reinitialize the exisiting NN
            //_nn = new NeuralNetwork(numInput, numHidden, numOutput, featureSelection);



            Stopwatch sw = new Stopwatch();
            sw.Start();

            double[] weights = _nn.NewTrain(TrainData, maxEpochs, learnRate, momentum);

            sw.Stop();
            TrainingTime = sw.Elapsed.ToString();

            //Console.WriteLine("\nFinal neural network model weights and biases:\n");

            _nn.SetAllWeights(weights);
            //ShowVector(weights, 2, 10, true);

            //trainAcc = _nn.Accuracy(TrainData, rankArray);
            TrainAcc = _nn.NewAccuracy(TrainData);

            Stopwatch sw2 = new Stopwatch();
            sw2.Start();
            TestAcc = _nn.NewAccuracy(TestData);
            sw2.Stop();
            TestingTime = sw2.Elapsed.ToString();

            Console.WriteLine("Training accuracy =\t{0} ", TrainAcc.ToString("F4"));
            Console.WriteLine("Testing accuracy =\t{0} ", TestAcc.ToString("F4"));
            Console.WriteLine("Training time =\t\t{0}", TrainingTime);
            Console.WriteLine("Testing time =\t\t{0}", TestingTime);
            Console.WriteLine("Done...\n");

        }



        //public void RunHotellingTTest(string trainingFile, string testingFile, string rScriptFile, string rBin)
        //{
        //    Stopwatch sw = new Stopwatch();
        //    Console.WriteLine("Hotelling Test...");
        //    sw.Start();
        //    while (HotellingTestPValue <= HotellingTestThreshold)
        //    {
        //        var result = _rRunner.RunFromCmd(this._fileProcessor.GetRScriptPath() + rScriptFile, rBin, Partition.ToString(), this._fileProcessor.GetDataPath() + trainingFile, this._fileProcessor.GetDataPath() + testingFile);

        //        if (string.IsNullOrEmpty(result))
        //        {
        //            Partition += PartitionIncrement;
        //            continue;
        //        }

        //        var res2 = result.Substring(result.IndexOf(']') + 1);
        //        var res3 = res2.Substring(res2.IndexOf(']') + 1);
        //        var res = Regex.Split(res3, @"[^0-9\.]+");


        //        foreach (string s in res)
        //        {
        //            if (!string.IsNullOrEmpty(s))
        //            {
        //                HotellingTestPValue = double.Parse(s);
        //                break;
        //            }
        //        }
        //        Console.WriteLine("Partiion: {0} PValue: {1}", Partition, HotellingTestPValue);


        //        if (HotellingTestPValue <= HotellingTestThreshold)
        //        {
        //            Partition += PartitionIncrement;
        //        }
        //    }
        //    sw.Stop();
        //    Console.WriteLine("Elapsed={0}", sw.Elapsed);
        //}


        //public void GenerateArtificalDataUsingNN(int numInput, int numHidden, int numOutput)
        //{
        //    Console.WriteLine("\nGenerating " + NumRows + " artificial data items with " + NumAttributes + " features");

        //    Random rnd = new Random(Seed);

        //    double[] weights = new double[_nn.GetTotalWeights()]; // actually weights & biases

        //    for (int i = 0; i < _nn.GetTotalWeights(); ++i)
        //        weights[i] = 20.0 * rnd.NextDouble() - 10.0; // [-10.0 to 10.0]

        //    Console.WriteLine("Generating weights and biases:");

        //    _nn.SetAllWeights(weights);
        //    //ShowVector(weights, 2, 10, true);

        //    double[][] result = new double[NumRows][]; // allocate return-result
        //    for (int i = 0; i < NumRows; ++i)
        //        //TODO: added to save the tvalues
        //        result[i] = new double[numInput + numOutput + 2]; // 1-of-N in last column

        //    //gnn =
        //    //  new NeuralNetwork(numInput, numHidden, numOutput, ISFEATURESELECTION); // generating NN

        //    _nn.SetWeights(weights);

        //    for (int r = 0; r < NumRows; ++r) // for each row
        //    {
        //        // generate random inputs
        //        double[] inputs = new double[numInput];

        //        //for (int i = 0; i < numInput; ++i)
        //        //    inputs[i] = 20.0 * rnd.NextDouble() - 10.0; // [-10.0 to -10.0]

        //        //read file
        //        for (int i = 0; i < numInput; ++i)
        //            inputs[i] = RawFullDataset[r][i]; // [-10.0 to -10.0]

        //        // compute outputs
        //        double[] outputs = _nn.ComputeOutputs(inputs);

        //        //double[] outputs = new double[numOutput];
        //        //for (int i = numInput; i < numOutput + numInput; ++i)
        //        //    outputs[i] = datafile[r][i]; // [-10.0 to -10.0]

        //        // translate outputs to 1-of-N
        //        double[] oneOfN = new double[numOutput]; // all 0.0

        //        int maxIndex = 0;
        //        double maxValue = outputs[0];
        //        for (int i = 0; i < numOutput; ++i)
        //        {
        //            if (outputs[i] > maxValue)
        //            {
        //                maxIndex = i;
        //                maxValue = outputs[i];
        //            }
        //        }
        //        oneOfN[maxIndex] = 1.0;

        //        // place inputs and 1-of-N output values into curr row
        //        int c = 0; // column into result[][]
        //        for (int i = 0; i < numInput; ++i) // inputs
        //            result[r][c++] = inputs[i];
        //        for (int i = 0; i < numOutput; ++i) // outputs
        //            result[r][c++] = oneOfN[i];

        //        //Add the target values
        //        for (int i = 0; i < 2; ++i) // outputs
        //            result[r][c++] = TValueFile[r][i];



        //    } // each row


        //    AllData = result;
        //    Console.WriteLine("Done");

        //    //return result;

        //}

        public void PrintTrain()
        {
            Console.WriteLine("Training data:");

            ShowMatrix(TrainData, 4, 2, true);
        }

        public void PrintTest()
        {
            //            ShowMatrix(testData, 4, 2, true);

            Console.WriteLine("Test data:");
            ShowMatrix(TestData, 4, 2, true);

        }

        //public void CreateAndTrainMLP(int numInput, int numHidden, int numOutput, bool featureSelection, int maxEpochs, double learnRate, double momentum)
        //{
        //    Console.WriteLine("Creating a " + numInput + "-" + numHidden +
        //          "-" + numOutput + " neural network");
        //    //Reinitialize the exisiting NN
        //    _nn = new NeuralNetwork(numInput, numHidden, numOutput, featureSelection);

        //    Console.WriteLine("\nSetting maxEpochs = " + maxEpochs);
        //    Console.WriteLine("Setting learnRate = " + learnRate.ToString("F2"));
        //    Console.WriteLine("Setting momentum  = " + momentum.ToString("F2"));
        //    Console.WriteLine("\nStarting training");

        //    Stopwatch sw = new Stopwatch();
        //    sw.Start();

        //    double[] weights = _nn.Train(TrainData, maxEpochs, learnRate, momentum);

        //    sw.Stop();
        //    TrainingTime = sw.Elapsed.ToString();
        //    Console.WriteLine("Elapsed={0}", sw.Elapsed);


        //    Console.WriteLine("Done");
        //    Console.WriteLine("\nFinal neural network model weights and biases:\n");

        //    _nn.SetAllWeights(weights);
        //    //ShowVector(weights, 2, 10, true);

        //    //trainAcc = _nn.Accuracy(TrainData, rankArray);
        //    TrainAcc = _nn.Accuracy(TrainData);
        //    TestAcc = _nn.Accuracy(TestData);


        //}

        public void PrintWeights(int decimals,
            int lineLen, bool newLine)
        {
            var vector = _nn.GetAllWeights();

            for (int i = 0; i < vector.Length; ++i)
            {
                if (i > 0 && i % lineLen == 0) Console.WriteLine("");
                if (vector[i] >= 0) Console.Write(" ");
                Console.Write(vector[i].ToString("F" + decimals) + " ");
            }
            if (newLine == true)
                Console.WriteLine("");

        }

        public void ShowMatrix(double[][] matrix, int numRows,
            int decimals, bool indices)
        {
            int len = matrix.Length.ToString().Length;
            for (int i = 0; i < numRows; ++i)
            {
                if (indices == true)
                    Console.Write("[" + i.ToString().PadLeft(len) + "]  ");
                for (int j = 0; j < matrix[i].Length; ++j)
                {
                    double v = matrix[i][j];
                    if (v >= 0.0)
                        Console.Write(" "); // '+'
                    Console.Write(v.ToString("F" + decimals) + "  ");
                }
                Console.WriteLine("");
            }

            if (numRows < matrix.Length)
            {
                Console.WriteLine(". . .");
                int lastRow = matrix.Length - 1;
                if (indices == true)
                    Console.Write("[" + lastRow.ToString().PadLeft(len) + "]  ");
                for (int j = 0; j < matrix[lastRow].Length; ++j)
                {
                    double v = matrix[lastRow][j];
                    if (v >= 0.0)
                        Console.Write(" "); // '+'
                    Console.Write(v.ToString("F" + decimals) + "  ");
                }
            }
            Console.WriteLine("\n");
        }

        public void ShowMatrix(float[][] matrix, int numRows,
    int decimals, bool indices)
        {
            int len = matrix.Length.ToString().Length;
            for (int i = 0; i < numRows; ++i)
            {
                if (indices == true)
                    Console.Write("[" + i.ToString().PadLeft(len) + "]  ");
                for (int j = 0; j < matrix[i].Length; ++j)
                {
                    double v = matrix[i][j];
                    if (v >= 0.0)
                        Console.Write(" "); // '+'
                    Console.Write(v.ToString("F" + decimals) + "  ");
                }
                Console.WriteLine("");
            }

            if (numRows < matrix.Length)
            {
                Console.WriteLine(". . .");
                int lastRow = matrix.Length - 1;
                if (indices == true)
                    Console.Write("[" + lastRow.ToString().PadLeft(len) + "]  ");
                for (int j = 0; j < matrix[lastRow].Length; ++j)
                {
                    double v = matrix[lastRow][j];
                    if (v >= 0.0)
                        Console.Write(" "); // '+'
                    Console.Write(v.ToString("F" + decimals) + "  ");
                }
            }
            Console.WriteLine("\n");
        }


        //public void SplitTrainTest(decimal partitionSize, int seed, bool shuffle = false)
        //{
        //    Console.WriteLine("\nCreating train {0} and test {1} matrices", Partition, 1 - Partition);
        //    Random rnd = new Random(seed);
        //    //Partition = partition;
        //    int totRows = AllData.Length;
        //    int numTrainRows = (int)(totRows * partitionSize); // usually 0.80
        //    int numTestRows = totRows - numTrainRows;
        //    TrainData = new double[numTrainRows][];
        //    TestData = new double[numTestRows][];


        //    double[][] copy = new double[AllData.Length][]; // ref copy of data
        //    for (int i = 0; i < copy.Length; ++i)
        //    {
        //        copy[i] = AllData[i];
        //    }

        //    if (shuffle)
        //    {
        //        for (int i = 0; i < copy.Length; ++i) // scramble order
        //        {
        //            int r = rnd.Next(i, copy.Length); // use Fisher-Yates
        //            double[] tmp = copy[r];
        //            copy[r] = copy[i];
        //            copy[i] = tmp;
        //        }
        //    }

        //    for (int i = 0; i < numTrainRows; ++i)
        //        TrainData[i] = copy[i];

        //    for (int i = 0; i < numTestRows; ++i)
        //        TestData[i] = copy[i + numTrainRows];

        //    TrainingFileName = _fileProcessor.WriteMLPInputDataset(NumAttributes, TrainData, "Train-" + Partition.ToString());
        //    TestingFileName = _fileProcessor.WriteMLPInputDataset(NumAttributes, TestData, "Test-" + Partition.ToString());


        //    Console.WriteLine("Done\n");
        //} // SplitTrainTest


        public void SplitTrainTestData(decimal partitionSize, int seed, bool shuffle = false)
        {
            Console.WriteLine("=====================================================");
            Console.WriteLine("Generating Datasets...");
            Console.WriteLine("Random: {0}", shuffle);

            Random rnd = new Random(seed);
            //Partition = partition;
            float[][] rawData = _fileProcessor.GetRawDataset();
            int numAttributes = _fileProcessor.GetInputSpecification().GetNumAttributes();

            int totRows = rawData.Length;
            int numTrainRows = (int)(totRows * partitionSize); // usually 0.80
            int numTestRows = totRows - numTrainRows;

            Console.WriteLine("\nSplit ratio: Training({0}) and Testing({1})", partitionSize, 1 - partitionSize);
            Console.WriteLine("Training instances: {0}", numTrainRows);
            Console.WriteLine("Testing instances: {0}", numTestRows);
            Console.WriteLine("Total instances: {0}", totRows);


            TrainData = new float[numTrainRows][];
            TestData = new float[numTestRows][];


            float[][] copy = new float[rawData.Length][]; // ref copy of data
            for (int i = 0; i < copy.Length; ++i)
            {
                copy[i] = rawData[i];
            }

            if (shuffle)
            {
                for (int i = 0; i < copy.Length; ++i) // scramble order
                {
                    int r = rnd.Next(i, copy.Length); // use Fisher-Yates
                    float[] tmp = copy[r];
                    copy[r] = copy[i];
                    copy[i] = tmp;
                }
            }

            for (int i = 0; i < numTrainRows; ++i)
            {
                TrainData[i] = copy[i];
            }
            for (int i = 0; i < numTestRows; ++i)
            {
                TestData[i] = copy[i + numTrainRows];
            }

            TrainingFileName = _fileProcessor.OutputDatasetToCSV(numAttributes, TrainData, "RawTrain-" + partitionSize);
            TestingFileName = _fileProcessor.OutputDatasetToCSV(numAttributes, TestData, "RawTest-" + partitionSize);

            Console.WriteLine("\nGenerating files...");
            Console.WriteLine("Training file: {0}", TrainingFileName);
            Console.WriteLine("Testing file: {0}", TestingFileName);
            Console.WriteLine("Done....\n");
        }

    }
}
