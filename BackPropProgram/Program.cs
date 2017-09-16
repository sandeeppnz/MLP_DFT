using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.FileIO;
using System.Collections;

namespace BackPropProgram
{
    /*
     * https://visualstudiomagazine.com/Articles/2015/04/01/Back-Propagation-Using-C.aspx?Page=3 
     * 
     * Dataset: 10000
     * Train: 8000
     * Test: 2000
     * Features: 11
     * Class: 0,1
     * 
     * 1) Input the file
     * 2) 
     * 
     */



    class Program
    {
        const int NUMINPUT = 7;//11 // number features
        const int NUMHIDDEN = 8;
        const int NUMOUTPUT = 2; // number of classes for Y
        const bool ISFEATURESELECTION = false;

        static void Main(string[] args)
        {
            //int numInput = 4; // number features
            //int numHidden = 5;
            //int numOutput = 3; // number of classes for Y
            //int numRows = 1000;
            //int seed = 1; // gives nice demo



            //int numOutput = 2; // number of classes for Y
            int numRows = 45312; //10000; //// // //
            int seed = 1; // gives nice demo


            //int numInput = 11; // number features
            //int numHidden = 9;
            //int numOutput = 2; // number of classes for Y
            //int numRows = 10000;//10000; // //
            //int seed = 1; // gives nice demo


            int maxEpochs = 100;
            double learnRate = 0.3;
            double momentum = 0.2;
            //double learnRate = 0.3;
            //double momentum = 0.2;


            String line = String.Empty;
            //System.IO.StreamReader file = new System.IO.StreamReader(@"d:\Data.csv");

            //System.IO.StreamReader file = new System.IO.StreamReader(@"d:\Data_withY-CS.csv");

            //System.IO.StreamReader file = new System.IO.StreamReader(@"d:\irisCSV.csv");

            System.IO.StreamReader file = new System.IO.StreamReader(@"d:\Data_withYEle.csv");


            float[][] fullDataset = new float[numRows][];

            float[][] tValueFile = new float[numRows][];


            for (int i = 0; i < numRows; i++)
                fullDataset[i] = new float[NUMINPUT + 1];

            int r = 0;
            while ((line = file.ReadLine()) != null)
            {
                String[] parts_of_line = line.Split(',');
                for (int i = 0; i < parts_of_line.Length; i++)
                {
                    parts_of_line[i] = parts_of_line[i].Trim();
                }

                // do with the parts of the line whatever you like
                //TODO
                for (int i = 0; i < NUMINPUT + 1; i++)
                {
                    fullDataset[r][i] = float.Parse(parts_of_line[i]);
                }
                //tValueFile[r] = float.Parse(parts_of_line[11]);

                r++;
            }

            //NEW
            for (int i = 0; i < numRows; i++)
                tValueFile[i] = new float[2];


            for (int i = 0; i < numRows; i++)
            {
                //TODO: 
                if (fullDataset[i][NUMINPUT] == 0)
                {
                    tValueFile[i][1] = 0;
                    tValueFile[i][0] = 1;
                }
                else
                {
                    tValueFile[i][0] = 0;
                    tValueFile[i][1] = 1;

                }
            }


            Console.WriteLine("\nBegin neural network back-propagation demo");


            Console.WriteLine("\nGenerating " + numRows +
              " artificial data items with " + NUMINPUT + " features");



            double[][] allData = MakeAllDataDataFile(NUMINPUT, NUMHIDDEN, NUMOUTPUT,
              numRows, seed, fullDataset, tValueFile);



            Console.WriteLine("Done");

            //ShowMatrix(allData, allData.Length, 2, true);

            Console.WriteLine("\nCreating train (80%) and test (20%) matrices");
            double[][] trainData;
            double[][] testData;

            SplitTrainTest(allData, 0.8, seed, out trainData, out testData);
            Console.WriteLine("Done\n");

            Console.WriteLine("Training data:");
            ShowMatrix(trainData, 4, 2, true);

            Console.WriteLine("Test data:");
            ShowMatrix(testData, 4, 2, true);

            Console.WriteLine("Creating a " + NUMINPUT + "-" + NUMHIDDEN +
              "-" + NUMOUTPUT + " neural network");
            NeuralNetwork nn = new NeuralNetwork(NUMINPUT, NUMHIDDEN, NUMOUTPUT, ISFEATURESELECTION);



            Console.WriteLine("\nSetting maxEpochs = " + maxEpochs);
            Console.WriteLine("Setting learnRate = " + learnRate.ToString("F2"));
            Console.WriteLine("Setting momentum  = " + momentum.ToString("F2"));

            Console.WriteLine("\nStarting training");

            List<string> yZero = new List<string>();
            List<string> yOne = new List<string>();


            #region MLP
            double[] weights = nn.Train(trainData, maxEpochs, learnRate, momentum);



            Console.WriteLine("Done");
            Console.WriteLine("\nFinal neural network model weights and biases:\n");
            ShowVector(weights, 2, 10, true);

            double[] inputNodeTotalWeightsArray = ShowVectorWInputMy(weights, 2);

            double[] rankArray = null;
            if (ISFEATURESELECTION)
            {
                rankArray = GenerateRankArrayMy(inputNodeTotalWeightsArray);
                weights = UpdateWeightsArrayByRankMy(weights, rankArray);
                inputNodeTotalWeightsArray = ShowVectorWInputMy(weights, 2);
                nn.SetWeights(weights);
            }

            double trainAcc = nn.Accuracy(trainData, rankArray);
            Console.WriteLine("\nFinal accuracy on training data = " + trainAcc.ToString("F4"));

            bool[,] inputTable = GenerateTruthTableMy(NUMINPUT);
            bool[] answer1 = new bool[inputTable.GetLength(0)];

            if (ISFEATURESELECTION)
            {
                inputTable = SetIrrelevantVariablesMy(inputTable, rankArray);
            }

            double testAcc = nn.Accuracy(testData, rankArray);
            Console.WriteLine("Final accuracy on test data     = " + testAcc.ToString("F4"));

            var redundantSchema = nn.GetUniqueRedudantSchemaMy(trainData, rankArray); //unique combinations
            var convertedArray = nn.MakeArrayBasedSchemaMy(redundantSchema);


            nn.CalculateAccuracyAndAppendYValMy(trainData, rankArray, out yZero, out yOne);
            #region write to csv
            WriteToCSV(NUMINPUT,yZero,"0");
            WriteToCSV(NUMINPUT, yOne,"1");

            #endregion

            #endregion


            //1*1
            //yZero = new List<string>()
            //{
            //    "001",
            //    "011",
            //};

            //**0, 1*1
            //yOne = new List<string>()
            //{
            //    "000",
            //    "010",
            //    "100",
            //    "110",
            //    "101",
            //    "111",
            //};


            ////001
            //yZero = new List<string>()
            //{
            //    "001",
            //};

            ////011, **0, 1 * 1
            //yOne = new List<string>()
            //{
            //    "101",
            //    "111",
            //    "000",
            //    "110",
            //    "010",
            //    "100",
            //    "011"
            //};

            //////sample
            //yZero = new List<string>()
            //{
            //    "001",
            //    "011",
            //};

            ////011, **0, 1 * 1
            //yOne = new List<string>()
            //{
            //    "000",
            //    "100",
            //    "110",
            //    "010",

            //    "101",
            //    "111",
            //};



            var SxClusterLabel_ClassZero = nn.SetWildcard(yZero, yOne);
            var SxClusterLabel_ClassOne = nn.SetWildcard(yOne, yZero);

            //SxClusterLabel_ClassOne = new List<string> {
            //    "*1*",
            //    "1**",
            //    "**0",
            //    "**0",
            //};
            //SxClusterLabel_ClassOne = new List<string> {
            //    "*10",
            //    "**1",
            //    "1*1",
            //};




            var redundantIndexList = nn.FindRedundantAttributeFromPatterns(SxClusterLabel_ClassOne);




            int NUMINPUT_Temp = 7;
            bool[,] sjVectorArray = GenerateTruthTableMy(NUMINPUT_Temp);
            List<string> sjVectorList = new List<string>();
            int arrayLength = (int) Math.Pow(2, (double) NUMINPUT_Temp);

            for (int i = 0; i < arrayLength; i++)
            {
                bool state;
                string sjString = string.Empty;
                for (int j = 0; j < NUMINPUT_Temp; j++)
                {
                    if (sjVectorArray[i, j] == false)
                        sjString += '0';
                    else
                        sjString += '1';
                    //state = sjVectorArray[i, j];
                }
                sjVectorList.Add(sjString);
            }

            Dictionary<string, double> coeffArray = new Dictionary<string, double>();


            foreach (string j in sjVectorList)
            {
                double coeff = getCoefficientValue(j, SxClusterLabel_ClassOne);
                coeffArray[j] = coeff;
            }









            Console.WriteLine("\nEnd back-propagation demo\n");
            Console.ReadLine();
        }

        private static void WriteToCSV(int numCols, List<string> instanceArray, string classLabel)
        {
            StreamWriter sw = new StreamWriter(@"D:\MLP.csv", true);

            foreach (var s in instanceArray)
            {
                sw.Write(s.ToString());
                sw.Write(",");
                sw.Write(classLabel);
                sw.Write(",");
                sw.Write(",");
                for (int j = 0; j < numCols; j++)
                {

                    sw.Write(s[j].ToString());
                    sw.Write(",");
                }
                sw.Write(classLabel);
                sw.Write("\r\n");
            }




            //for (int i = 0; i < NUMINPUT; i++)
            //{
            //        sw.Write(i.ToString());
            //        sw.Write(",");
            //}
            //sw.Write(",");


            sw.Flush();
            sw.Close();
        }

        public static double getCoefficientValue(String j, List<string> patterns)
        {
            double denominator = Math.Pow(2, j.Length);
            double coefficientValue = 0.0;

            foreach (string x in patterns)
            {
                double dotProduct = DFT.CalculateDotProduct(j, x);
                if (dotProduct != 0)
                {
                    coefficientValue = coefficientValue + (dotProduct / denominator);
                }
            }


            return coefficientValue;

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

        public static string getBinaryString(int number, out int numberOfOnes)
        {
            numberOfOnes = 0;
            string binaryString = "";
            if (number == 0)
            {
                binaryString = "0";
            }
            while (number > 0)
            {
                int value = number % 2;
                if (value == 1)
                {
                    numberOfOnes++;
                }
                binaryString = binaryString + value.ToString();
                number = number / 2;
            }

            //TODO: redefine size
            while (binaryString.Length < 3)
            {
                binaryString = binaryString + "0";
            }

            char[] charArray = binaryString.ToArray();
            Array.Reverse(charArray);
            binaryString = new string(charArray);
            return binaryString;
        }



        private static void SortForClusters(List<string> list)
        {
            /*
             * Sort according to the fx val, then the string
             * Clustering algorithm 
             * 
             */

            var ascendingOrder = list.OrderBy(i => i);



        }



        private static bool[,] GenerateTruthTableMy(int col)
        {
            bool[,] table;
            int row = (int) Math.Pow(2, col);

            table = new bool[row, col];

            int divider = row;

            // iterate by column
            for (int c = 0; c < col; c++)
            {
                divider /= 2;
                bool cell = false;
                // iterate every row by this column's index:
                for (int r = 0; r < row; r++)
                {
                    table[r, c] = cell;
                    if ((divider == 1) || ((r + 1) % divider == 0))
                    {
                        cell = !cell;
                    }
                }
            }

            return table;
        }

        public static bool[,] SetIrrelevantVariablesMy(bool[,] table, double[] rankArray)
        {
            int k = 0;
            for (int i = 0; i < table.GetLength(0); i++)
            {
                for (int j = 0; j < NUMINPUT; j++)
                {
                    if (rankArray[j] >= 5)
                    {

                        table[i, j] = false;
                    }

                    k++;
                }
            }

            return table;
        }


        //public static double[][] GenerateSchemaMap()
        //{
        //    int maxRowSize = (int) Math.Pow(NUMINPUT, 2);

        //    double[][] schema = new double[maxRowSize][];
        //    for (int r = 0; r < schema.Length; ++r)
        //        schema[r] = new double[NUMINPUT];

        //    //for (int i = 0; i < NUMINPUT; i++)
        //    //{
        //    //    for (int j = 0; j < NUMINPUT; j++)
        //    //    {
        //    //        schema[i][j] = 0;
        //    //    }
        //    //}

        //    //for (int a = 0; a < 2; a++)
        //    //{
        //    //    for (int b = 0; b < 2; b++)
        //    //    {

        //    //        for (int c = 0; c < 2; c++)
        //    //        {

        //    //            for (int d = 0; d < 2; d++)
        //    //            {
        //    //                for (int e = 0; e < 2; e++)
        //    //                {
        //    //                    for (int f = 0; f < 2;f++)
        //    //                    {
        //    //                        for (int g = 0; g < 2; g++)
        //    //                        {
        //    //                            //schema[]
        //    //                        }

        //    //                    }

        //    //                }

        //    //            }

        //    //        }
        //    //    }
        //    //}


        //    return schema;



        //}

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


        public static void ShowVector(double[] vector, int decimals,
          int lineLen, bool newLine)
        {
            for (int i = 0; i < vector.Length; ++i)
            {
                if (i > 0 && i % lineLen == 0) Console.WriteLine("");
                if (vector[i] >= 0) Console.Write(" ");
                Console.Write(vector[i].ToString("F" + decimals) + " ");
            }
            if (newLine == true)
                Console.WriteLine("");
        }


        #region Mods



        public static double[] GenerateRankArrayMy(double[] totalArray)
        {
            double max = -1;
            int rank = 1;

            double[] rankArray = new double[NUMINPUT];

            for (int i = 0; i < NUMINPUT; ++i)
            {
                rankArray[i] = 0;
            }

            for (int i = 0; i < NUMINPUT; ++i)
            {
                for (int j = 0; j < NUMINPUT; ++j)
                {
                    if ((totalArray[j] > max) && (rankArray[j] == 0))
                    {
                        max = totalArray[j];
                    }
                }

                //Update the rank
                for (int j = 0; j < NUMINPUT; ++j)
                {
                    if (totalArray[j] == max)
                    {
                        rankArray[j] = rank++;
                    }
                }
                max = -1;
            }


            Console.WriteLine("Weights");
            for (int i = 0; i < NUMINPUT; ++i)
            {
                Console.WriteLine(totalArray[i] + " " + rankArray[i]);
            }


            return rankArray;
        }

        public static double[] UpdateWeightsArrayByRankMy(double[] weightsArray, double[] rankArray)
        {
            //Set zero weights if for bottom 5 weights
            //int NUMINPUT = 11;
            //int NUMHIDDEN = 8;
            int k = 0;

            for (int i = 0; i < weightsArray.Length; ++i)
            {
                for (int j = 0; j < NUMINPUT; ++j)
                {
                    for (int m = 0; m < NUMHIDDEN; ++m)
                    {
                        if (rankArray[j] > 5)
                        {

                            weightsArray[k] = 0;
                        }

                        k++;
                    }

                }


                break;

            }

            return weightsArray;
        }





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





        public static double[] ShowVectorWInputMy(double[] vector, int decimals)
        {
            int nodeCount = 1;
            int k = 0;

            double[] inputNodeHiddenTotalArray = new double[NUMINPUT];

            for (int i = 0; i < vector.Length; ++i)
            {
                for (int j = 0; j < NUMINPUT; ++j)
                {
                    Console.Write("Input-Node " + j + ": ");

                    for (int m = 0; m < NUMHIDDEN; ++m)
                    {
                        double val = vector[k++];
                        Console.Write(val.ToString("F" + decimals) + " ");
                        inputNodeHiddenTotalArray[j] += Math.Abs(Math.Round(val, 2));
                    }
                    Console.WriteLine();
                }

                for (int j = 0; j < NUMHIDDEN; ++j)
                {
                    Console.Write("Bias Hidden-Node " + j + ": ");
                    Console.Write(vector[k++].ToString("F" + decimals) + " ");
                    Console.WriteLine();
                }

                for (int j = 0; j < NUMHIDDEN; ++j)
                {
                    Console.Write("Output-Node " + j + ": ");
                    for (int m = 0; m < NUMOUTPUT; ++m)
                    {
                        Console.Write(vector[k++].ToString("F" + decimals) + " ");
                    }
                    Console.WriteLine();
                }


                for (int j = 0; j < NUMOUTPUT; ++j)
                {
                    Console.Write("Bias Output-Node " + j + ": ");
                    Console.Write(vector[k++].ToString("F" + decimals) + " ");
                    Console.WriteLine();
                }
                break;
            }
            return inputNodeHiddenTotalArray;
        }

        #endregion


        static double[][] MakeAllDataDataFile(int numInput, int numHidden,
              int numOutput, int numRows, int seed, float[][] datafile, float[][] tValueFile)
        {
            Random rnd = new Random(seed);
            int numWeights = (numInput * numHidden) + numHidden +
              (numHidden * numOutput) + numOutput;

            double[] weights = new double[numWeights]; // actually weights & biases

            for (int i = 0; i < numWeights; ++i)
                weights[i] = 20.0 * rnd.NextDouble() - 10.0; // [-10.0 to 10.0]

            Console.WriteLine("Generating weights and biases:");
            ShowVector(weights, 2, 10, true);

            double[][] result = new double[numRows][]; // allocate return-result
            for (int i = 0; i < numRows; ++i)
                //TODO: added to save the tvalues
                result[i] = new double[numInput + numOutput + 2]; // 1-of-N in last column

            NeuralNetwork gnn =
              new NeuralNetwork(numInput, numHidden, numOutput, ISFEATURESELECTION); // generating NN

            gnn.SetWeights(weights);

            for (int r = 0; r < numRows; ++r) // for each row
            {
                // generate random inputs
                double[] inputs = new double[numInput];

                //for (int i = 0; i < numInput; ++i)
                //    inputs[i] = 20.0 * rnd.NextDouble() - 10.0; // [-10.0 to -10.0]

                //read file
                for (int i = 0; i < numInput; ++i)
                    inputs[i] = datafile[r][i]; // [-10.0 to -10.0]

                // compute outputs
                double[] outputs = gnn.ComputeOutputs(inputs);

                //double[] outputs = new double[numOutput];
                //for (int i = numInput; i < numOutput + numInput; ++i)
                //    outputs[i] = datafile[r][i]; // [-10.0 to -10.0]

                // translate outputs to 1-of-N
                double[] oneOfN = new double[numOutput]; // all 0.0

                int maxIndex = 0;
                double maxValue = outputs[0];
                for (int i = 0; i < numOutput; ++i)
                {
                    if (outputs[i] > maxValue)
                    {
                        maxIndex = i;
                        maxValue = outputs[i];
                    }
                }
                oneOfN[maxIndex] = 1.0;

                // place inputs and 1-of-N output values into curr row
                int c = 0; // column into result[][]
                for (int i = 0; i < numInput; ++i) // inputs
                    result[r][c++] = inputs[i];
                for (int i = 0; i < numOutput; ++i) // outputs
                    result[r][c++] = oneOfN[i];

                //Add the target values
                for (int i = 0; i < 2; ++i) // outputs
                    result[r][c++] = tValueFile[r][i];



            } // each row
            return result;
        } // MakeAllData



        static double[][] MakeAllData(int numInput, int numHidden,
          int numOutput, int numRows, int seed)
        {
            Random rnd = new Random(seed);
            int numWeights = (numInput * numHidden) + numHidden +
              (numHidden * numOutput) + numOutput;

            double[] weights = new double[numWeights]; // actually weights & biases
            for (int i = 0; i < numWeights; ++i)
                weights[i] = 20.0 * rnd.NextDouble() - 10.0; // [-10.0 to 10.0]

            Console.WriteLine("Generating weights and biases:");
            ShowVector(weights, 2, 10, true);

            double[][] result = new double[numRows][]; // allocate return-result
            for (int i = 0; i < numRows; ++i)
                result[i] = new double[numInput + numOutput]; // 1-of-N in last column

            NeuralNetwork gnn =
              new NeuralNetwork(numInput, numHidden, numOutput, ISFEATURESELECTION); // generating NN
            gnn.SetWeights(weights);

            for (int r = 0; r < numRows; ++r) // for each row
            {
                // generate random inputs
                double[] inputs = new double[numInput];
                for (int i = 0; i < numInput; ++i)
                    inputs[i] = 20.0 * rnd.NextDouble() - 10.0; // [-10.0 to -10.0]

                // compute outputs
                double[] outputs = gnn.ComputeOutputs(inputs);

                // translate outputs to 1-of-N
                double[] oneOfN = new double[numOutput]; // all 0.0

                int maxIndex = 0;
                double maxValue = outputs[0];
                for (int i = 0; i < numOutput; ++i)
                {
                    if (outputs[i] > maxValue)
                    {
                        maxIndex = i;
                        maxValue = outputs[i];
                    }
                }
                oneOfN[maxIndex] = 1.0;

                // place inputs and 1-of-N output values into curr row
                int c = 0; // column into result[][]
                for (int i = 0; i < numInput; ++i) // inputs
                    result[r][c++] = inputs[i];
                for (int i = 0; i < numOutput; ++i) // outputs
                    result[r][c++] = oneOfN[i];
            } // each row
            return result;
        } // MakeAllData




        static void SplitTrainTest(double[][] allData, double trainPct,
          int seed, out double[][] trainData, out double[][] testData)
        {
            Random rnd = new Random(seed);
            int totRows = allData.Length;
            int numTrainRows = (int) (totRows * trainPct); // usually 0.80
            int numTestRows = totRows - numTrainRows;
            trainData = new double[numTrainRows][];
            testData = new double[numTestRows][];

            double[][] copy = new double[allData.Length][]; // ref copy of data
            for (int i = 0; i < copy.Length; ++i)
            {
                copy[i] = allData[i];
            }

            //TODO: shuffle
            for (int i = 0; i < copy.Length; ++i) // scramble order
            {
                int r = rnd.Next(i, copy.Length); // use Fisher-Yates
                double[] tmp = copy[r];
                copy[r] = copy[i];
                copy[i] = tmp;
            }
            for (int i = 0; i < numTrainRows; ++i)
                trainData[i] = copy[i];

            for (int i = 0; i < numTestRows; ++i)
                testData[i] = copy[i + numTrainRows];
        } // SplitTrainTest
    }



}
