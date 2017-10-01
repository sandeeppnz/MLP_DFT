using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackPropProgram
{
    public static class Helper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xValues"></param>
        /// <param name="rankArray"></param>
        /// <param name="numInput"></param>
        /// <returns></returns>
        public static double[] SetXValueToZeroByRankCheck(double[] xValues, double[] rankArray, int numInput)
        {
            //Set zero weights if for bottom 5 weights
            int k = 0;
            for (int j = 0; j < numInput; ++j)
            {
                if (rankArray[j] > 5)
                {

                    xValues[k] = 0;
                }
                k++;
            }
            return xValues;
        }

        public static int MaxIndex(double[] vector) // helper for Accuracy()
        {
            // index of largest value
            int bigIndex = 0;
            double biggestVal = vector[0];
            for (int i = 0; i < vector.Length; ++i)
            {
                if (vector[i] > biggestVal)
                {
                    biggestVal = vector[i];
                    bigIndex = i;
                }
            }
            return bigIndex;
        }

    }
}
