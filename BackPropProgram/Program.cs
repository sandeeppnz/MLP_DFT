using Algorithms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace BackPropProgram
{
    class Program
    {
        static void Main(string[] args)
        {
            //TODO:
            //Exit if the data is continuous
            //Rank Array
            //TODO: Parition logic
            //FS


            const int seed = 123;
            const int maxEpochs = 100;
            const double learnRate = 0.3;
            const double momentum = 0.2;
            const decimal partitionIncrement = 0.01M;
            const double hotellingTestThreshold = 0.05;
            const int numHidden = 8;
            const int numOutput = 2; // number of classes for Y
            const bool isFSOn = false;
            const string rBin = @"C:\Program Files\R\R-3.4.1\bin\rscript.exe";
            //const string rScripFileName = "hotellingttest3.r";
            const string rScripFileName = "Script.r";
            string dataPath = @"D:\ANN_Project_AUT_Sem3\Microsoft\BackPropProgram\Data\";
            string rScriptPath = @"D:\ANN_Project_AUT_Sem3\Microsoft\BackPropProgram\HotellingTTest\";
            bool isShuffleOn = false;
            int dftEnergyThresholdingLimit = 4;

            List<ResultsStatistics> stats = new List<ResultsStatistics>();


            //Setup File
            InputSpecification inputSpec = new ElectricitySmall();

            //Unoptimized Manual partitional selection and classfication
            // 1.Set input file and load dataset
            FileProcessor fp = new FileProcessor(dataPath, rScriptPath, inputSpec);
            fp.LoadCSV();
            RRunner rn = new RRunner();

            // 2. Create MLP model    
            MLPModel mlpModel = new MLPModel(numHidden, numOutput, fp, rn);
            decimal partitionSize = 0.1M;
            mlpModel.RawSplitTrainTest(partitionSize, seed);
            //mlpModel.PrintTrain();
            //mlpModel.PrintTest();

            //mlpModel.GenerateArtificalDataUsingNN(numInput, numHidden, numOutput);
            //mlpModel.PrintWeights(2, 10, true);
            //mlpModel.RunHotellingTTest(mlpModel.TrainingFileName, mlpModel.TestingFileName, rScripFileName, rBin);


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



            mlpModel.NewTrainMLP(maxEpochs, learnRate, momentum);
            //mlpModel.PrintWeights(2, 10, true);

            ////TODO: not used
            //bool[,] inputTable = DFT.GenerateTruthTable(NUMINPUT);
            //bool[] answer1 = new bool[inputTable.GetLength(0)];

            ////TODO: not used
            //if (ISFEATURESELECTION)
            //{
            //    inputTable = DFT.SetIrrelevantVariables(NUMINPUT, inputTable, rankArray);
            //}




            #region Main DFT processing begins

            //Add rank array
            DFTModel dftModel = new DFTModel(mlpModel.GetNeuralNetwork(), mlpModel.TrainData, isFSOn, null);
            dftModel.SpliteInstanceSchemasByClassValue();
            dftModel.GenerateClusteredSchemaPatterns();
            dftModel.GenerateJVectors(dftEnergyThresholdingLimit);//concept of energy thresholding and order
            var energyCoffs = dftModel.CalculateDftEnergyCoeffs(dftModel.ClusteredSchemaXVectorClass1);

            //#region Find redundant attributes from patterns

            ////TODO: not used
            //var redundantAttibuteIndexList = DFT.FindRedundantAttributeFromPatterns(clusteredSchemaSxClass1);
            //#endregion
            Console.WriteLine("..................");

            #endregion



            #region 
            //Calculate f(x) directly by looking at the pattern
            InverseDFTModel inverseDftModel = new InverseDFTModel();
            var fxShortcutClass0 = inverseDftModel.CalculateFxByPatternDirectly(dftModel.AllSchemaXVectorClass0, dftModel.ClusteredSchemaXVectorClass0, "0");
            var fxShortcutClass1 = inverseDftModel.CalculateFxByPatternDirectly(dftModel.AllSchemaXVectorClass1, dftModel.ClusteredSchemaXVectorClass1, "1");


            //Calculate f(x) by Inverse DFT 
            var fxClass0ByInvDFT = inverseDftModel.GetFxByInverseDFT(dftModel.AllSchemaXVectorClass0, dftModel.jVectors, energyCoffs);
            var fxClass1ByInvDFT = inverseDftModel.GetFxByInverseDFT(dftModel.AllSchemaXVectorClass1, dftModel.jVectors, energyCoffs);

            ////FileProcessor.WriteCoeffArraToCsv(coeffsDFT);
            ////FileProcessor.WritesXVectorsToCsv(allSchemaSxClass1);
            ////FileProcessor.WriteCoeffArraToCsv(coeffsDFT);


            ////Console.ReadLine();
            #endregion




            Console.ReadLine();
        }













        #region commented
        //private static DataTable GetDataTabletFromCSVFile(string csv_file_path)
        //{
        //    DataTable csvData = new DataTable();

        //    try
        //    {

        //        using (TextFieldParser csvReader = new TextFieldParser(csv_file_path))
        //        {
        //            csvReader.SetDelimiters(new string[] { "," });
        //            csvReader.HasFieldsEnclosedInQuotes = true;
        //            string[] colFields = csvReader.ReadFields();
        //            foreach (string column in colFields)
        //            {
        //                DataColumn datecolumn = new DataColumn(column);
        //                datecolumn.AllowDBNull = true;
        //                csvData.Columns.Add(datecolumn);
        //            }

        //            while (!csvReader.EndOfData)
        //            {
        //                string[] fieldData = csvReader.ReadFields();
        //                //Making empty value as null
        //                for (int i = 0; i < fieldData.Length; i++)
        //                {
        //                    if (fieldData[i] == "")
        //                    {
        //                        fieldData[i] = null;
        //                    }
        //                }
        //                csvData.Rows.Add(fieldData);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //    return csvData;
        //}




        //static double[][] MakeAllDataUsingDataFile(int numInput, int numHidden,
        // int numOutput, int numRows, int seed, IEnumerable<string[]> fileArray)
        //{



        //    Random rnd = new Random(seed);
        //    int numWeights = (numInput * numHidden) + numHidden +
        //      (numHidden * numOutput) + numOutput;

        //    double[] weights = new double[numWeights]; // actually weights & biases
        //    for (int i = 0; i < numWeights; ++i)
        //        weights[i] = 20.0 * rnd.NextDouble() - 10.0; // [-10.0 to 10.0]

        //    Console.WriteLine("Generating weights and biases:");
        //    ShowVector(weights, 2, 10, true);

        //    double[][] result = new double[numRows][]; // allocate return-result
        //    for (int i = 0; i < numRows; ++i)
        //        result[i] = new double[numInput + numOutput]; // 1-of-N in last column

        //    NeuralNetwork gnn =
        //      new NeuralNetwork(numInput, numHidden, numOutput); // generating NN
        //    gnn.SetWeights(weights);

        //    for (int r = 0; r < numRows; ++r) // for each row
        //    {
        //        //MOD read the file input cols
        //        double[] inputs = new double[numInput];
        //        for (int i = 0; i < numInput; ++i)
        //            inputs[i] = fileArray[r][i]; // [-10.0 to -10.0]

        //        // compute outputs
        //        double[] outputs = gnn.ComputeOutputs(inputs);

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
        //    } // each row
        //    return result;
        //} // MakeAllData
        #endregion

        public static void ShowMatrix(double[][] matrix, int numRows,
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


        //public static void ShowVector(double[] vector, int decimals,
        //  int lineLen, bool newLine)
        //{
        //    for (int i = 0; i < vector.Length; ++i)
        //    {
        //        if (i > 0 && i % lineLen == 0) Console.WriteLine("");
        //        if (vector[i] >= 0) Console.Write(" ");
        //        Console.Write(vector[i].ToString("F" + decimals) + " ");
        //    }
        //    if (newLine == true)
        //        Console.WriteLine("");
        //}


        #region Mods



        //public static double[] Ranker(double[] weightsArray, double[] rankerArray)
        //{
        //    int numInput = 11;
        //    int numHidden = 8;
        //    int numOutput = 2;

        //    double maxVal = 0;
        //    int rank = 1;
        //    for (int j = 0; j < numInput; ++j)
        //    {
        //        if (maxVal < weightsArray[j])
        //            maxVal = weightsArray[j];
        //    }

        //    for (int j = 0; j < numInput; ++j)
        //    {
        //        maxVal = GetMaxValInArray(weighsArray);

        //        if (maxVal == weightsArray[j])
        //            rankerArray[j] = rank;

        //    }


        //    //Top 5
        //    //Console.WriteLine("Unsorted");
        //    //for (int j = 0; j < numInput; ++j)
        //    //{
        //    //}

        //    double[] probInputArray = new double[numInput];
        //    double[] rankArray = new double[numInput];

        //    for (int j = 0; j < numInput; ++j)
        //    {
        //        rankArray[j] = 0;
        //    }

        //    double currMin = 100;
        //    int rank = numInput;
        //    for (int j = 0; j < numInput; ++j)
        //    {
        //        if (currMin > weightsArray[j])
        //        {
        //            maxVal = weightsArray[j];
        //        }
        //    }


        //    double totalAbsInputNode = 0;


        //    for (int j = 0; j < numInput; ++j)
        //    {
        //        Console.WriteLine(weightsArray[j]);
        //        totalAbsInputNode += weightsArray[j];
        //    }

        //    for (int j = 0; j < numInput; ++j)
        //    {
        //        probInputArray[j] = (double) weightsArray[j] / (double) totalAbsInputNode;
        //        Console.WriteLine(probInputArray[j]);
        //    }




        //    //Array.Sort(probInputArray, (a, b) => b.CompareTo(a));

        //    //Console.WriteLine("Reverse");
        //    //for (int j = 0; j < numInput; ++j)
        //    //{
        //    //    Console.WriteLine(probInputArray[j]);
        //    //}



        //    return null;
        //}


        #endregion


        //static double[][] GenerateInitialMLPModel(int numInput, int numHidden,
        //      int numOutput, int numRows, int seed, float[][] datafile, float[][] tValueFile)
        //{
        //    Random rnd = new Random(seed);
        //    int numWeights = (numInput * numHidden) + numHidden +
        //      (numHidden * numOutput) + numOutput;

        //    double[] weights = new double[numWeights]; // actually weights & biases

        //    for (int i = 0; i < numWeights; ++i)
        //        weights[i] = 20.0 * rnd.NextDouble() - 10.0; // [-10.0 to 10.0]

        //    Console.WriteLine("Generating weights and biases:");
        //    ShowVector(weights, 2, 10, true);

        //    double[][] result = new double[numRows][]; // allocate return-result
        //    for (int i = 0; i < numRows; ++i)
        //        //TODO: added to save the tvalues
        //        result[i] = new double[numInput + numOutput + 2]; // 1-of-N in last column

        //    NeuralNetwork gnn =
        //      new NeuralNetwork(numInput, numHidden, numOutput, ISFEATURESELECTION); // generating NN

        //    gnn.SetWeights(weights);

        //    for (int r = 0; r < numRows; ++r) // for each row
        //    {
        //        // generate random inputs
        //        double[] inputs = new double[numInput];

        //        //for (int i = 0; i < numInput; ++i)
        //        //    inputs[i] = 20.0 * rnd.NextDouble() - 10.0; // [-10.0 to -10.0]

        //        //read file
        //        for (int i = 0; i < numInput; ++i)
        //            inputs[i] = datafile[r][i]; // [-10.0 to -10.0]

        //        // compute outputs
        //        double[] outputs = gnn.ComputeOutputs(inputs);

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
        //            result[r][c++] = tValueFile[r][i];



        //    } // each row
        //    return result;
        //} // MakeAllData



        //static double[][] MakeAllData(int numInput, int numHidden,
        //  int numOutput, int numRows, int seed)
        //{
        //    Random rnd = new Random(seed);
        //    int numWeights = (numInput * numHidden) + numHidden +
        //      (numHidden * numOutput) + numOutput;

        //    double[] weights = new double[numWeights]; // actually weights & biases
        //    for (int i = 0; i < numWeights; ++i)
        //        weights[i] = 20.0 * rnd.NextDouble() - 10.0; // [-10.0 to 10.0]

        //    Console.WriteLine("Generating weights and biases:");
        //    ShowVector(weights, 2, 10, true);

        //    double[][] result = new double[numRows][]; // allocate return-result
        //    for (int i = 0; i < numRows; ++i)
        //        result[i] = new double[numInput + numOutput]; // 1-of-N in last column

        //    NeuralNetwork gnn =
        //      new NeuralNetwork(numInput, numHidden, numOutput, ISFEATURESELECTION); // generating NN
        //    gnn.SetWeights(weights);

        //    for (int r = 0; r < numRows; ++r) // for each row
        //    {
        //        // generate random inputs
        //        double[] inputs = new double[numInput];
        //        for (int i = 0; i < numInput; ++i)
        //            inputs[i] = 20.0 * rnd.NextDouble() - 10.0; // [-10.0 to -10.0]

        //        // compute outputs
        //        double[] outputs = gnn.ComputeOutputs(inputs);

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
        //    } // each row
        //    return result;
        //} // MakeAllData


    }



}
