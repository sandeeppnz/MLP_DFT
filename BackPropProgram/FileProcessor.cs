﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackPropProgram
{
    public class FileProcessor
    {

        public static void InputDatasetCSV(int numInputs, int numRows, out float[][] fullDataset, out float[][] tValueFile)
        {
            String line = String.Empty;
            //System.IO.StreamReader file = new System.IO.StreamReader(@"d:\Data.csv");
            //System.IO.StreamReader file = new System.IO.StreamReader(@"d:\Data_withY-CS.csv");
            //System.IO.StreamReader file = new System.IO.StreamReader(@"d:\irisCSV.csv");
            System.IO.StreamReader file = new System.IO.StreamReader(@"d:\Data_withYEle.csv");
            fullDataset = new float[numRows][];
            tValueFile = new float[numRows][];
            for (int i = 0; i < numRows; i++)
                fullDataset[i] = new float[numInputs + 1];
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
                for (int i = 0; i < numInputs + 1; i++)
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
                if (fullDataset[i][numInputs] == 0)
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
        }



        public static void WriteOutputToCsv(int numCols, List<string> instanceArray, string classFromMLP,
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


    }
}