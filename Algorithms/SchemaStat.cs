using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithms
{
    public class SchemaStat
    {
        public string SchemaInstance { get; set; }
        public string ClusterPattern { get; set; }
        public string ClassLabelClassifiedByMLP { get; set; }
        public string ClassLabelCalculatedByInvDft { get; set; }
        //public int TotalClass0 { get; set; }
        //public int TotalClass1 { get; set; }

        //not used
        //public int PrevMajorityClassInterval { get; set; }
        //public int CurrMajorityClassInterval { get; set; }

        //used
        public int PrevClassA { get; set; }
        public int PrevClassB { get; set; }
        public int CurrClassA { get; set; }
        public int CurrClassB { get; set; }



        //public SchemaStat(string schemaInstance, string clusterPattern, string classLabelClassifiedByMLP, string classLabelCalculatedByInvDft, int prevMajorityClassInterval, int currMajorityClassInterval)
        public SchemaStat(string schemaInstance, string clusterPattern, string classLabelClassifiedByMLP, string classLabelCalculatedByInvDft)
        {
            SchemaInstance = schemaInstance;
            ClusterPattern = clusterPattern;
            ClassLabelClassifiedByMLP = classLabelClassifiedByMLP;
            ClassLabelCalculatedByInvDft = classLabelCalculatedByInvDft;
            //PrevMajorityClassInterval = prevMajorityClassInterval;
            //CurrMajorityClassInterval = currMajorityClassInterval;
        }

        public void AddCurrClassA()
        {
            CurrClassA++;
        }
        public void AddCurrClassB()
        {
            CurrClassB++;
        }

        //public void ReduceCurrClassA()
        //{
        //    CurrClassA--;
        //}
        //public void ReduceCurrClassB()
        //{
        //    CurrClassB--;
        //}

        public double GetCurrMajority()
        {
            if (CurrClassA > CurrClassB)
            {
                return 0.0;
            }
            else
            {
                return 1.0;
            }
        }

        public double GetPrevMajority()
        {
            if (PrevClassA > PrevClassB)
            {
                return 0.0;
            }
            else
            {
                return 1.0;
            }
        }

        public void CopyCurrToPrev()
        {

            PrevClassA = CurrClassA;
            CurrClassA = 0;

            PrevClassB = CurrClassB;
            CurrClassB = 0;



        }



    }
}
