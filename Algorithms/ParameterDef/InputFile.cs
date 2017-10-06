using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackPropProgram
{

    public interface IInputFile
    {
        string GetFileName();
        int GetNumAttributes();
        int GetNumRows();
    }

    public class InputFile: IInputFile
    {
        public int NumRows { get; set; }
        public int NumAttributes { get; set; }
        public string InputDatasetFileName { get; set; }

        public string GetFileName()
        {
            return InputDatasetFileName;
        }

        public int GetNumAttributes()
        {
            return NumAttributes;
        }

        public int GetNumRows()
        {
            return NumRows;
        }
    }
}
