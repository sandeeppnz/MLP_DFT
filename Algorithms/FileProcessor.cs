using BackPropProgram;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithms
{
    public interface IFileProcessor
    {
        IInputSpecification GetInputSpecification();
        string GetDataPath();
        string GetRScriptPath();
        void LoadCSV();
        string WriteMLPInputDataset(int numCols, float[][] data, string fileName);
        string WriteRawMLPInputDataset(int numCols, float[][] data, string fileName);
        float[][] GetRawDataset();
        void Dispose();
    }


    public class FileProcessor : IFileProcessor, IDisposable
    {
        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    RawDataset = null;
                    InputSpecification = null;
                    //ClassValFromInputProcessed = null;
                }
            }
            this.disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            //GC.SuppressFinalize(this);
        }


        public float[][] RawDataset { get; set; }
        //public float[][] ClassValFromInputProcessed { get; set; } //ToDo: To be removed

        public string Header { get; set; }



        public string DataPath { get; set; }
        public string RScriptPath { get; set; }

        public IInputSpecification InputSpecification { get; set; }

        public string GetDataPath() { return DataPath; }

        public string GetRScriptPath() { return RScriptPath; }

        public IInputSpecification GetInputSpecification() { return InputSpecification; }



        public FileProcessor(string dataPath, string rScriptPath, InputSpecification inputSpecification)
        {
            DataPath = dataPath;
            RScriptPath = rScriptPath;
            InputSpecification = inputSpecification;
        }

        public void LoadCSV()
        {
            Console.WriteLine("=====================================================");
            Console.WriteLine("Loading data from CSV...");


            int numInputs = InputSpecification.GetNumAttributes();
            int numRows = InputSpecification.GetNumRows();
            string inputFilePathName = InputSpecification.GetFileName();

            Console.WriteLine("Data folder:\t{0}", DataPath);
            Console.WriteLine("File name:\t{0}", inputFilePathName);
            Console.WriteLine("Attributes:\t{0}", numInputs);
            Console.WriteLine("Instances:\t{0}", numRows);


            String line = String.Empty;
            System.IO.StreamReader file = new System.IO.StreamReader(DataPath + inputFilePathName);
            RawDataset = new float[numRows][];

            //ClassValFromInputProcessed = new float[numRows][];

            for (int i = 0; i < numRows; i++)
            {
                //RawDataset[i] = new float[numInputs + 1];
                //Save attributes, class, calculated output (2 state), class output(2 state) 
                RawDataset[i] = new float[numInputs + 1 + 2 + 2];
            }

            int r = 0;
            int headerPassed = 0; //assumes first line is a header
            while ((line = file.ReadLine()) != null)
            {
                if (headerPassed == 0)
                {
                    headerPassed = 1;
                    Header = line;
                    continue;
                }

                String[] parts_of_line = line.Split(',');
                for (int i = 0; i < parts_of_line.Length; i++)
                {
                    parts_of_line[i] = parts_of_line[i].Trim();
                }
                // do with the parts of the line whatever you like
                //TODO
                for (int i = 0; i < numInputs + 1; i++)
                {
                    RawDataset[r][i] = float.Parse(parts_of_line[i]);
                }
                //tValueFile[r] = float.Parse(parts_of_line[11]);
                r++;
            }

            ////TODO: remove after
            //for (int i = 0; i < numRows; i++)
            //    ClassValFromInputProcessed[i] = new float[2];

            for (int i = 0; i < numRows; i++)
            {
                //if the class is 0, set the first element as 1 (kinda like 0 is activated)
                if (RawDataset[i][numInputs] == 0)
                {
                    //ClassValFromInputProcessed[i][0] = 1;
                    //ClassValFromInputProcessed[i][1] = 0;

                    RawDataset[i][numInputs + 3] = 1;
                    RawDataset[i][numInputs + 4] = 0;
                }
                else
                {
                    //ClassValFromInputProcessed[i][1] = 1;
                    //ClassValFromInputProcessed[i][0] = 0;

                    RawDataset[i][numInputs + 3] = 0;
                    RawDataset[i][numInputs + 4] = 1;
                }
            }

            Console.WriteLine("Done...\n");

        }



        //public void ReadInputDatasetCSVOther(int numInputs, int numRows, out float[][] fullDataset, out float[][] tValueFile, string inputFilePathName)
        //{
        //    String line = String.Empty;
        //    //System.IO.StreamReader file = new System.IO.StreamReader(@"d:\Data.csv");
        //    //System.IO.StreamReader file = new System.IO.StreamReader(@"d:\Data_withY-CS.csv");
        //    //System.IO.StreamReader file = new System.IO.StreamReader(@"d:\irisCSV.csv");
        //    System.IO.StreamReader file = new System.IO.StreamReader(DataPath + inputFilePathName);
        //    fullDataset = new float[numRows][];
        //    tValueFile = new float[numRows][];

        //    for (int i = 0; i < numRows; i++)
        //        fullDataset[i] = new float[numInputs + 1];
        //    int r = 0;

        //    while ((line = file.ReadLine()) != null)
        //    {
        //        String[] parts_of_line = line.Split(',');
        //        for (int i = 0; i < parts_of_line.Length; i++)
        //        {
        //            try
        //            {
        //                parts_of_line[i] = parts_of_line[i].Trim();

        //            }
        //            catch (Exception ex)
        //            {
        //                continue;
        //            }
        //        }
        //        // do with the parts of the line whatever you like
        //        //TODO
        //        for (int i = 0; i < numInputs + 1; i++)
        //        {
        //            float val = float.Parse(parts_of_line[i]);
        //            if (val == 1)
        //                fullDataset[r][i] = 0;
        //            else if (val == 2)
        //                fullDataset[r][i] = 1;


        //        }
        //        //tValueFile[r] = float.Parse(parts_of_line[11]);
        //        r++;
        //    }

        //    //NEW
        //    for (int i = 0; i < numRows; i++)
        //        tValueFile[i] = new float[2];

        //    for (int i = 0; i < numRows; i++)
        //    {
        //        //TODO: 
        //        if (fullDataset[i][numInputs] == 0)
        //        {
        //            tValueFile[i][1] = 0;
        //            tValueFile[i][0] = 1;
        //        }
        //        else
        //        {
        //            tValueFile[i][0] = 0;
        //            tValueFile[i][1] = 1;

        //        }
        //    }
        //}


        public string WriteMLPInputDataset(int numCols, float[][] data, string fileName)
        {
            string fileNameAndPath = GetDataPath() + "Output-" + fileName + ".csv";

            if (File.Exists(fileNameAndPath))
            {
                File.Delete(fileNameAndPath);
            }

            var sw = new StreamWriter(fileNameAndPath, true);


            sw.Write(Header);
            sw.Write("\r\n");

            //if (header)
            //{
            //    sw.Write("SchemaFull,fx-MLP,");
            //    for (int j = 0; j < numCols; j++)
            //    {
            //        sw.Write(",X" + j);
            //    }
            //    sw.Write(",fx-MLP");

            //    sw.Write(",,fx-InvDFT");
            //    sw.Write(",,fx-Shortcut");
            //    sw.Write("\r\n");
            //}

            for (int row = 0; row < data.Length; row++)
            {
                for (int col = 0; col < numCols; col++)
                {
                    sw.Write(data[row][col].ToString());
                    sw.Write(",");

                    //if (col != numCols - 1)
                    //    sw.Write(",");
                    //else
                    //    sw.Write("\r\n");

                }

                if (data[row][numCols + 2] == 1) //class value zero is activated
                    sw.Write(0);
                else
                    sw.Write(1);
                sw.Write("\r\n");
            }

            sw.Flush();
            sw.Close();
            sw = null;

            return "Output-" + fileName + ".csv";
        }


        public string WriteRawMLPInputDataset(int numCols, float[][] data, string fileName)
        {
            string fileNameAndPath = GetDataPath() + "Output-" + fileName + ".csv";

            if (File.Exists(fileNameAndPath))
            {
                File.Delete(fileNameAndPath);
            }

            var sw = new StreamWriter(fileNameAndPath, true);


            sw.Write(Header);
            sw.Write("\r\n");

            //if (header)
            //{
            //    sw.Write("SchemaFull,fx-MLP,");
            //    for (int j = 0; j < numCols; j++)
            //    {
            //        sw.Write(",X" + j);
            //    }
            //    sw.Write(",fx-MLP");

            //    sw.Write(",,fx-InvDFT");
            //    sw.Write(",,fx-Shortcut");
            //    sw.Write("\r\n");
            //}

            for (int row = 0; row < data.Length; row++)
            {
                for (int col = 0; col < numCols + 1; col++)
                {
                    sw.Write(data[row][col].ToString());
                    //sw.Write(",");

                    if (col != numCols)
                        sw.Write(",");
                    else
                        sw.Write("\r\n");

                }

                //if (data[row][numCols + 2] == 1) //class value zero is activated
                //    sw.Write(0);
                //else
                //    sw.Write(1);
                //sw.Write("\r\n");
            }

            sw.Flush();
            sw.Close();
            sw = null;

            return "Output-" + fileName + ".csv";
        }


        public static void WriteCSVOutput(int numCols, List<string> instanceArray, string classFromMLP,
            Dictionary<string, double> invDFT_fx, Dictionary<string, double> patternVerfication_fx, bool header)
        {
            var sw = new StreamWriter(@"D:\MLP.csv", true);
            if (header)
            {
                sw.Write("SchemaFull,fx-MLP,");
                for (int j = 0; j < numCols; j++)
                {
                    sw.Write(",X" + j);
                }
                sw.Write(",fx-MLP");

                sw.Write(",,fx-InvDFT");
                sw.Write(",,fx-Shortcut");
                sw.Write("\r\n");
            }
            foreach (var s in instanceArray)
            {
                sw.Write(s.ToString());
                sw.Write(",");
                sw.Write(classFromMLP);
                sw.Write(",");
                sw.Write(",");
                for (int j = 0; j < numCols; j++)
                {

                    sw.Write(s[j].ToString());
                    sw.Write(",");
                }
                sw.Write(classFromMLP);
                sw.Write(",");
                sw.Write(",");

                double fx = invDFT_fx[s];
                sw.Write(fx);
                sw.Write(",");
                sw.Write(",");

                fx = patternVerfication_fx[s];
                sw.Write(fx);

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

        public static void WriteCoeffArraToCsv(Dictionary<string, double> coeffs)
        {
            var sw = new StreamWriter(@"D:\DftCoeffs.csv", true);

            sw.Write('"');
            foreach (var s in coeffs)
            {
                sw.Write(s.Key.ToString());
                sw.Write(':');
                sw.Write(s.Value.ToString());
                sw.Write(',');
                sw.Write("\r\n");
            }
            sw.Flush();
            sw.Close();
        }

        public static void WritesXVectorsToCsv(List<string> array)
        {
            var sw = new StreamWriter(@"D:\sxVectors1.csv", true);

            sw.Write('"');
            foreach (var s in array)
            {
                sw.Write(s.ToString());
                sw.Write(',');
            }
            sw.Write('"');
            sw.Flush();
            sw.Close();
        }

        public float[][] GetRawDataset()
        {
            return RawDataset;
        }
    }
}
