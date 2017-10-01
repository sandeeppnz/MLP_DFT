using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackPropProgram
{
    public interface ILogger
    {
        void Log();
    }

    public class Logger : ILogger
    {
        public void Log()
        {
            throw new NotImplementedException();
        }
    }
}
