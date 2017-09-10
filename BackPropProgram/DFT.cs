using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackPropProgram
{
    public class DFT
    {

        public static double getCoefficientValue(String coefficient, Dictionary<String, SchemaStatistics> Patterns)
        {
            double denominator = Math.Pow(2, coefficient.Length);
            double dCoefficientValue = 0.0;
            foreach (KeyValuePair<string, SchemaStatistics> pattern in Patterns)
            {
                //if (pattern.Value.Label == true) //skipping the leaf nodes labeled 0 (false)
                //{
                double dDotProduct = (double) CalculateDotProduct(coefficient, pattern.Key);
                if (dDotProduct != 0)
                {
                    dCoefficientValue = dCoefficientValue + (dDotProduct / denominator);
                }
                //}
            }
            return dCoefficientValue;
        }


        // All DFT models
        //************************************************************************************************************************************
        //Calculates dot product between two binary strings with wild card characters. sjVector can not have wildcard characters
        public static int CalculateDotProduct(string sjVector, string sxVector)
        {
            if (sjVector.Length != sxVector.Length)
            {
                //MessageBox.Show("In Calculating dot product the j vector length and x vector length are not equal.", "Error");
                //TODO: Error
            }

            CharEnumerator ojCharEnum = sjVector.GetEnumerator();
            CharEnumerator oxCharEnum = sxVector.GetEnumerator();

            int i11Count = 0;
            int i00Count = 0;
            int i1WildcardCount = 0;
            int i0WildcardCount = 0;

            while (ojCharEnum.MoveNext() && oxCharEnum.MoveNext())
            {
                if (oxCharEnum.Current == '*')
                {
                    if (ojCharEnum.Current == '1')
                    {
                        ++i1WildcardCount;
                        return 0;
                    }

                    else if (ojCharEnum.Current == '0')
                    {
                        ++i0WildcardCount;
                    }
                    else
                    {
                        Console.WriteLine("* * combination Impossible");
                    }
                }
                else
                {
                    if (ojCharEnum.Current == '1' && oxCharEnum.Current == '1')
                    {
                        ++i11Count;
                    }
                    else if (ojCharEnum.Current == '0' && oxCharEnum.Current == '0')
                    {
                        ++i00Count;
                    }
                }
            }
            if (i1WildcardCount != 0) //all are 1 * combinations or there exists a 1 * combination 
            {
                return 0;
            }
            else if (i0WildcardCount != 0)
            {
                if (i0WildcardCount == sjVector.Length) //all are 0 * combinations
                {
                    return (int) Math.Pow(2, i0WildcardCount);
                }
                else if (i0WildcardCount < sjVector.Length)
                {
                    if (i11Count % 2 == 0) //there exists some 1 1 combinations
                    {
                        return (int) Math.Pow(2, i0WildcardCount);
                    }
                    else
                    {
                        return -(int) Math.Pow(2, i0WildcardCount);
                    }
                }
            }
            else
            {
                if (i11Count % 2 == 0)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
            return 10000;
        }


    }
}
