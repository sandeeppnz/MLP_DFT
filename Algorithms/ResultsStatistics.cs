using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithms
{
    public class ResultsStatistics
    {
        public string FileName { get; set; }
        public int TotalSize { get; set; }
        public int TrainSize { get; set; }
        public int TestSize { get; set; }
        public string TrainingTime { get; set; }
        public double TestingAccuracy { get; set; }
        public double TrainingAccuracy { get; set; }
        public decimal PerSplit { get; set; }
    }
}
