using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithms
{
    //https://stackoverflow.com/questions/14021055/r-script-form-c-passing-arguments-running-script-retrieving-results

    public interface IRRunner
    {
        string RunFromCmd(string rCodeFilePath, string rScriptExecutablePath, string args, string filePathAndName);
    }

    public class RRunner: IRRunner
    {
        //public RScript()
        //{  }

        // Runs an R script from a file using Rscript.exe.
        /// 
        /// Example: 
        ///
        ///   RScriptRunner.RunFromCmd(curDirectory +         @"\ImageClustering.r", "rscript.exe", curDirectory.Replace('\\','/'));
        ///   
        /// Getting args passed from C# using R:
        ///
        ///   args = commandArgs(trailingOnly = TRUE)
        ///   print(args[1]);
        ///  
        ///   
        /// rCodeFilePath          - File where your R code is located.
        /// rScriptExecutablePath  - Usually only requires "rscript.exe"
        /// args                   - Multiple R args can be seperated by spaces.
        /// Returns                - a string with the R responses.



        //public static bool Run(string filename, int year = 0)
        //{
        //    return false;
        //}


        public string RunFromCmd(string rCodeFilePath, string rScriptExecutablePath, string args, string filePathAndName)
        {
            string file = rCodeFilePath;
            string result = string.Empty;

            try
            {

                var info = new ProcessStartInfo();
                info.FileName = rScriptExecutablePath;
                info.WorkingDirectory = Path.GetDirectoryName(rScriptExecutablePath);
                info.Arguments = rCodeFilePath + " " + args + " " + filePathAndName;

                info.RedirectStandardInput = false;
                info.RedirectStandardOutput = true;
                info.UseShellExecute = false;
                info.CreateNoWindow = true;

                using (var proc = new Process())
                {
                    proc.StartInfo = info;
                    proc.Start();
                    result = proc.StandardOutput.ReadToEnd();
                }

                //proc.CloseMainWindow();
                //proc.Close();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("R Script failed: " + result, ex);
            }
        }
    }
}
