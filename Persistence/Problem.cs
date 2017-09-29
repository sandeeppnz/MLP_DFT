using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence
{
    public class Problem
    {
        int NumAttributes { get; set; }
        //int NumAttributes { get; set; }

        string InputFilePathAndName { get; set; }
        float[][] FullDataset;
        float[][] TValueFile;

    }
}
