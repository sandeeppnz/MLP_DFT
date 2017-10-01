using Algorithms;
using Persistence;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BackPropProgram
{
    /*  
     * https://visualstudiomagazine.com/Articles/2015/04/01/Back-Propagation-Using-C.aspx?Page=3 
     * Dataset: 10000
     * Train: 8000
     * Test: 2000
     * Features: 11
     * Class: 0,1
     */



    class Program
    {
        const int NUMINPUT = 7; //11; // number features
        const int NUMHIDDEN = 8;
        const int NUMOUTPUT = 2; // number of classes for Y
        const bool ISFEATURESELECTION = false;


        static void Main(string[] args)
        {
            int seed = 1;
            int numRows = 45312; //10000;
            //float[][] fullDataset, tValueFile;
            int maxEpochs = 100;
            double learnRate = 0.3;
            double momentum = 0.2;
            string inputFilePathAndName = @"d:/ann_project_aut_sem3/microsoft/backpropprogram/hotellingr/Data_withYEle.csv";
            //string inputFilePathAndName = @"d:/ann_project_aut_sem3/microsoft/backpropprogram/hotellingr/Data_withY.csv";
            double p_val = 0;
            double partition = 0;


            #region sample test R
            //string path = @"D:\ANN_Project_AUT_Sem3\Microsoft\BackPropProgram\HotellingR";
            //var result = RScript.RunFromCmd(path + @"\rcodeTest.r", @"C:\Program Files\R\R-3.4.1\bin\rscript.exe", "3");
            #endregion

            while (p_val <= 0.05)
            {
                partition += 0.1;
                string path = @"D:\ANN_Project_AUT_Sem3\Microsoft\BackPropProgram\HotellingR";
                var result = RScript.RunFromCmd(path + @"\hotellingttest3.r", @"C:\Program Files\R\R-3.4.1\bin\rscript.exe", partition.ToString(), inputFilePathAndName);

                var res2 = result.Substring(result.IndexOf(']') + 1);
                var res3 = res2.Substring(res2.IndexOf(']') + 1);
                var res = Regex.Split(res3, @"[^0-9\.]+");


                foreach (string s in res)
                {
                    if (!string.IsNullOrEmpty(s))
                    {
                        p_val = double.Parse(s);
                        break;
                    }
                }
            }

            Problem problem = new Problem(new FileProcessor(), new Logger(), new NeuralNetwork(NUMINPUT, NUMHIDDEN, NUMOUTPUT, ISFEATURESELECTION),
                NUMINPUT, seed);

            problem.ReadDataset(inputFilePathAndName, numRows);
            //FileProcessor.ReadInputDatasetCSV(NUMINPUT, numRows, out fullDataset, out tValueFile, inputFilePathAndName);

            Console.WriteLine("\nBegin neural network back-propagation demo");
            problem.GenerateArtificalDataUsingNN(NUMINPUT, NUMHIDDEN, NUMOUTPUT);
            //double[][] allData = GenerateInitialMLPModel(NUMINPUT, NUMHIDDEN, NUMOUTPUT,
            //  problem.NumRows, seed, problem.RawFullDataset, problem.TValueFile);
            //NeuralNetwork.SplitTrainTest(allData, partition, seed, out trainData, out testData);
            problem.SplitTrainTest(partition, seed);
            problem.PrintTrain();
            problem.PrintTest();
            problem.CreateAndTrainMLP(NUMINPUT, NUMHIDDEN, NUMOUTPUT, ISFEATURESELECTION, maxEpochs, learnRate, momentum);



            #region MLP

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

            //Investigate Changes HERE
            //double trainAcc = neuralNetwork.Accuracy(trainData, rankArray);

            Console.WriteLine("\nFinal accuracy on training data = " + problem.TrainAcc.ToString("F4"));
            Console.WriteLine("Final accuracy on test data     = " + problem.TestAcc.ToString("F4"));


            ////TODO: not used
            //bool[,] inputTable = DFT.GenerateTruthTable(NUMINPUT);
            //bool[] answer1 = new bool[inputTable.GetLength(0)];

            ////TODO: not used
            //if (ISFEATURESELECTION)
            //{
            //    inputTable = DFT.SetIrrelevantVariables(NUMINPUT, inputTable, rankArray);
            //}


            #endregion




            //#region Main DFT processing begins
            //var redundantSchema = DFT.GetUniqueRedudantSchema(NUMINPUT, ISFEATURESELECTION, trainData, rankArray); //unique combinations
            //var convertedArray = DFT.MakeArrayBasedSchema(NUMINPUT, redundantSchema);

            //List<string> allSchemaSxClass0 = null;
            //List<string> allSchemaSxClass1 = null;
            //neuralNetwork.CalculateAccuracyAndAppendYValMy(trainData, rankArray, out allSchemaSxClass0, out allSchemaSxClass1);
            //#endregion


            //int numInputs_temp = NUMINPUT;
            ////int numInputs_temp = 3;
            //var clusteredSchemaSxClass0 = DFT.GetSchemaClustersWithWildcardChars(allSchemaSxClass0, allSchemaSxClass1);
            //var clusteredSchemaSxClass1 = DFT.GetSchemaClustersWithWildcardChars(allSchemaSxClass1, allSchemaSxClass0);


            //#region Calculate f(x) directly by looking at the pattern
            //var fxShortcutClass0 = DFT.CalculateFxByPatternDirectly(allSchemaSxClass0, clusteredSchemaSxClass0, "0");
            //var fxShortcutClass1 = DFT.CalculateFxByPatternDirectly(allSchemaSxClass1, clusteredSchemaSxClass1, "1");
            //#endregion

            //#region Find redundant attributes from patterns
            ////TODO: not used
            //var redundantAttibuteIndexList = DFT.FindRedundantAttributeFromPatterns(clusteredSchemaSxClass1);
            //#endregion

            //#region Calculate DFT coeffs
            //List<string> sjVectors = null;
            //var coeffsDFT = DFT.CalculateDFTCoeffs(numInputs_temp, clusteredSchemaSxClass1, out sjVectors);
            //#endregion

            //#region Calculate f(x) by Inverse DFT 
            //var fxClass0ByInvDFT = DFT.GetFxByInverseDFT(allSchemaSxClass0, sjVectors, coeffsDFT);
            //var fxClass1ByInvDFT = DFT.GetFxByInverseDFT(allSchemaSxClass1, sjVectors, coeffsDFT);
            //#endregion

            ////FileProcessor.WriteCoeffArraToCsv(coeffsDFT);
            ////FileProcessor.WritesXVectorsToCsv(allSchemaSxClass1);
            ////FileProcessor.WriteCoeffArraToCsv(coeffsDFT);


            ////Console.ReadLine();



            //#region write to csv
            //FileProcessor.WriteCSVOutput(numInputs_temp, allSchemaSxClass0, "0", fxClass0ByInvDFT, fxShortcutClass0, true);
            //FileProcessor.WriteCSVOutput(numInputs_temp, allSchemaSxClass1, "1", fxClass1ByInvDFT, fxShortcutClass1, false);
            //#endregion





            Console.WriteLine("\nEnd back-propagation demo\n");
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
