using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithms
{
    public class ResultsStatistics
    {
        //MLP
        public int NumAttribute { get; set; }
        public int TotalSize { get; set; }
        public decimal PerSplit { get; set; }

        public string FileName { get; set; }
        public string TrainingFile { get; set; }
        public int TrainSize { get; set; }

        public string TestFile { get; set; }
        public int TestSize { get; set; }

        public double TestingAccuracy { get; set; }
        public double TrainingAccuracy { get; set; }
        public double TrainingTime { get; set; }
        public double TestingTime { get; set; }

        //DFT
        public int NumTotalInstancesXClass0 { get; set; }
        public int NumTotalInstancesXClass1 { get; set; }


        public List<string> ResolvedUniqueSchemaInstancesXClass0 { get; set; }
        public int NumResolvedUniqueSchemaInstancesXClass0 { get; set; }

        public List<string> ResolvedUniqueSchemaInstancesXClass1 { get; set; }
        public int NumResolvedUniqueSchemaInstancesXClass1 { get; set; }


        public List<string> PatternsXClass0 { get; set; }
        public int NumPatternsXClass0 { get; set; }

        public List<string> PatternsXClass1 { get; set; }
        public int NumPatternsXClass1 { get; set; }

        public int NumEnergyCoefficients { get; set; }
        public Dictionary<string, double> EnergyCoefficients { get; set; }

        public double EnergyCoefficientTime { get; set; }

        public Dictionary<decimal,double> PVal { get; set; }

        public double HotellingTestTime { get; set; }


        public ResultsStatistics()
        {
            PVal = new Dictionary<decimal, double>();
        }
    }
}
