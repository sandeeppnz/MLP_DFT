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
        string GetInputDataPath();
        string GetOutputDataPath();
        //void WriteResultsToCSV(List<ResultsStatistics> list, string path, string fileName);


        string GetRScriptPath();
        void LoadCSV();
        //string WriteMLPInputDataset(int numCols, float[][] data, string fileName);
        string OutputDatasetToCSV(int numCols, float[][] data, string fileName, SplitType splitType);
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
        public string DatasetHeader { get; set; }
        public string InputDataPath { get; set; }
        public string OutputDataPath { get; set; }
        public string ResultsDataPath { get; set; }

        public string RScriptPath { get; set; }
        public IInputSpecification InputSpecification { get; set; }
        public string GetInputDataPath() { return InputDataPath; }
        public string GetOutputDataPath() { return OutputDataPath; }

        public string GetRScriptPath() { return RScriptPath; }
        public IInputSpecification GetInputSpecification() { return InputSpecification; }
        public FileProcessor(string inputDataPath, string outputDataPath, string resultsDataPath, string rScriptPath, InputSpecification inputSpecification)
        {
            InputDataPath = inputDataPath;
            OutputDataPath = outputDataPath;
            RScriptPath = rScriptPath;
            InputSpecification = inputSpecification;
        }

        public float[][] GetRawDataset()
        {
            return RawDataset;
        }
        public void LoadCSV()
        {
            Console.WriteLine("=====================================================");
            Console.WriteLine("Loading data from CSV...");

            int numInputs = InputSpecification.GetNumAttributes();
            int numRows = InputSpecification.GetNumRows();
            string inputFilePathName = InputSpecification.GetFileName();

            Console.WriteLine("Data folder:\t{0}", InputDataPath);
            Console.WriteLine("File name:\t{0}", inputFilePathName);
            Console.WriteLine("Attributes:\t{0}", numInputs);
            Console.WriteLine("Instances:\t{0}", numRows);

            String line = String.Empty;
            System.IO.StreamReader file = new System.IO.StreamReader(InputDataPath + inputFilePathName);
            RawDataset = new float[numRows][];

            for (int i = 0; i < numRows; i++)
            {
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
                    DatasetHeader = line;
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
                r++;
            }

            for (int i = 0; i < numRows; i++)
            {
                //if the class is 0, set the first element as 1 (kinda like 0 is activated)
                if (RawDataset[i][numInputs] == 0)
                {
                    RawDataset[i][numInputs + 3] = 1;
                    RawDataset[i][numInputs + 4] = 0;
                }
                else
                {
                    RawDataset[i][numInputs + 3] = 0;
                    RawDataset[i][numInputs + 4] = 1;
                }
            }

            Console.WriteLine("Done...\n");

        }

        //public string WriteMLPInputDataset(int numCols, float[][] data, string fileName)
        //{
        //    string fileNameAndPath = GetInputDataPath() + "Output-" + fileName + ".csv";

        //    if (File.Exists(fileNameAndPath))
        //    {
        //        File.Delete(fileNameAndPath);
        //    }

        //    var sw = new StreamWriter(fileNameAndPath, true);


        //    sw.Write(Header);
        //    sw.Write("\r\n");

        //    //if (header)
        //    //{
        //    //    sw.Write("SchemaFull,fx-MLP,");
        //    //    for (int j = 0; j < numCols; j++)
        //    //    {
        //    //        sw.Write(",X" + j);
        //    //    }
        //    //    sw.Write(",fx-MLP");

        //    //    sw.Write(",,fx-InvDFT");
        //    //    sw.Write(",,fx-Shortcut");
        //    //    sw.Write("\r\n");
        //    //}

        //    for (int row = 0; row < data.Length; row++)
        //    {
        //        for (int col = 0; col < numCols; col++)
        //        {
        //            sw.Write(data[row][col].ToString());
        //            sw.Write(",");

        //            //if (col != numCols - 1)
        //            //    sw.Write(",");
        //            //else
        //            //    sw.Write("\r\n");

        //        }

        //        if (data[row][numCols + 2] == 1) //class value zero is activated
        //            sw.Write(0);
        //        else
        //            sw.Write(1);
        //        sw.Write("\r\n");
        //    }

        //    sw.Flush();
        //    sw.Close();
        //    sw = null;

        //    return "Output-" + fileName + ".csv";
        //}


        public string OutputDatasetToCSV(int numCols, float[][] data, string fileName, SplitType splitType)
        {
            string folderName = splitType.ToString() + "_" + InputSpecification.GetFileName();
            int index = folderName.IndexOf('.');
            if (index > 0)
            {
                folderName = folderName.Substring(0, index);
            }


            if (!Directory.Exists(GetOutputDataPath() + folderName))
            {
                Directory.CreateDirectory(GetOutputDataPath() + folderName);
            }

            string fileNameAndPath = GetOutputDataPath() + folderName + "\\" + fileName + ".csv";

            if (File.Exists(fileNameAndPath))
            {
                File.Delete(fileNameAndPath);
            }

            var sw = new StreamWriter(fileNameAndPath, true);


            sw.Write(DatasetHeader);
            sw.Write("\r\n");

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

            }

            sw.Flush();
            sw.Close();
            sw = null;

            return fileName + ".csv";
        }


        public void OutputModelValidationToCSV(int numCols, List<string> instanceArray, string classFromMLP,
            Dictionary<string, double> invDFT_fx, Dictionary<string, double> patternVerfication_fx, bool header, string fileName, SplitType splitType, bool append = false)
        {
            string folderName = splitType.ToString() + "_" + InputSpecification.GetFileName();
            int index = folderName.IndexOf('.');
            if (index > 0)
            {
                folderName = folderName.Substring(0, index);
            }


            if (!Directory.Exists(GetOutputDataPath() + folderName))
            {
                Directory.CreateDirectory(GetOutputDataPath() + folderName);
            }

            string fileNameAndPath = GetOutputDataPath() + folderName + "\\" + fileName + ".csv";

            if (!append)
            {
                if (File.Exists(fileNameAndPath))
                {
                    File.Delete(fileNameAndPath);
                }
            }
            var sw = new StreamWriter(fileNameAndPath, true);


            if (header)
            {
                sw.Write("SchemaFull");
                for (int j = 0; j < numCols; j++)
                {
                    sw.Write(",X" + j);
                }
                sw.Write(",fx-MLP");

                sw.Write(",fx-InvDFT");
                sw.Write(",fx-Shortcut");
                sw.Write("\r\n");
            }

            foreach (var s in instanceArray)
            {
                sw.Write(s.ToString());
                sw.Write(",");
                for (int j = 0; j < numCols; j++)
                {
                    sw.Write(s[j].ToString());
                    sw.Write(",");
                }
                sw.Write(classFromMLP);
                sw.Write(",");

                double fx = invDFT_fx[s];
                sw.Write(fx);
                sw.Write(",");

                fx = patternVerfication_fx[s];
                sw.Write(fx);

                sw.Write("\r\n");
            }
            sw.Flush();
            sw.Close();
        }


        public void OutputEnergyCoeffsToCSV(Dictionary<string, double> coeffs, SplitType splitType, string fileName)
        {
            string folderName = splitType.ToString() + "_" + InputSpecification.GetFileName();
            int index = folderName.IndexOf('.');
            if (index > 0)
            {
                folderName = folderName.Substring(0, index);
            }


            if (!Directory.Exists(GetOutputDataPath() + folderName))
            {
                Directory.CreateDirectory(GetOutputDataPath() + folderName);
            }

            string fileNameAndPath = GetOutputDataPath() + folderName + "\\" + fileName + ".csv";


            if (File.Exists(fileNameAndPath))
            {
                File.Delete(fileNameAndPath);
            }
            var sw = new StreamWriter(fileNameAndPath, true);
            foreach (var s in coeffs)
            {
                sw.Write(s.Key.ToString());
                sw.Write(',');
                sw.Write(s.Value.ToString());
                sw.Write("\r\n");
            }
            sw.Flush();
            sw.Close();
        }

        public void ReserviorToCSV(Dictionary<string, SchemaStat> coeffs, SplitType splitType, string fileName)
        {
            string folderName = splitType.ToString() + "_" + InputSpecification.GetFileName();
            int index = folderName.IndexOf('.');
            if (index > 0)
            {
                folderName = folderName.Substring(0, index);
            }


            if (!Directory.Exists(GetOutputDataPath() + folderName))
            {
                Directory.CreateDirectory(GetOutputDataPath() + folderName);
            }

            string fileNameAndPath = GetOutputDataPath() + folderName + "\\" + fileName + ".csv";


            if (File.Exists(fileNameAndPath))
            {
                File.Delete(fileNameAndPath);
            }
            var sw = new StreamWriter(fileNameAndPath, true);
            foreach (var s in coeffs.Values)
            {
                sw.Write(s.ClusterPattern.ToString());
                sw.Write(',');
                //sw.Write(s.CurrClassValue.ToString());
                sw.Write(s.TrainingClassValue.ToString());
                //                sw.Write(',');
                //                sw.Write(s.ClassLabelCalculatedByInvDft.ToString());
                sw.Write("\r\n");
            }
            sw.Flush();
            sw.Close();
        }

        public void TestInstanceMatchingToCSV(string testInstance, string clusterInstance, SplitType splitType, string fileName, bool erase)
        {
            string folderName = splitType.ToString() + "_" + InputSpecification.GetFileName();
            int index = folderName.IndexOf('.');
            if (index > 0)
            {
                folderName = folderName.Substring(0, index);
            }


            if (!Directory.Exists(GetOutputDataPath() + folderName))
            {
                Directory.CreateDirectory(GetOutputDataPath() + folderName);
            }

            string fileNameAndPath = GetOutputDataPath() + folderName + "\\" + fileName + ".csv";

            if (erase)
            {
                if (File.Exists(fileNameAndPath))
                {
                    File.Delete(fileNameAndPath);
                }
            }

            var sw = new StreamWriter(fileNameAndPath, true);
            sw.Write(testInstance.ToString());
            sw.Write(',');
            sw.Write(clusterInstance.ToString());
            sw.Write("\r\n");


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

    }
}
