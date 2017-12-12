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
        public int ClassLabelClassifiedByMLP { get; set; }
        public int ClassLabelCalculatedByInvDft { get; set; }
        //public int TotalClass0 { get; set; }
        //public int TotalClass1 { get; set; }

        //not used
        //public int PrevMajorityClassInterval { get; set; }
        //public int CurrMajorityClassInterval { get; set; }


        public int AAChangeCurr { get; set; }
        public int AAChangePrev { get; set; }


        //used
        public int PrevClassA { get; set; }
        public int PrevClassB { get; set; }

        public int CurrClassA { get; set; }
        public int CurrClassB { get; set; }

        public int CurrClass0A { get; set; }
        public int CurrClass0B { get; set; }



        //public int CumulativeCurrClassChange { get; set; }
        //public int CumulativePrevClassChange { get; set; }



        //public int CurrChangeA { get; set; }
        //public int CurrChangeB { get; set; }

        //public int CurrNoChangeA { get; set; }
        //public int CurrNoChangeB { get; set; }


        //public int PrevChangeA { get; set; }
        //public int PrevChangeB { get; set; }

        //public int PrevNoChangeA { get; set; }
        //public int PrevNoChangeB { get; set; }



        //public SchemaStat(string schemaInstance, string clusterPattern, string classLabelClassifiedByMLP, string classLabelCalculatedByInvDft, int prevMajorityClassInterval, int currMajorityClassInterval)
        public SchemaStat(string schemaInstance, string clusterPattern, int classLabelClassifiedByMLP, int classLabelCalculatedByInvDft)
        {
            SchemaInstance = schemaInstance;
            ClusterPattern = clusterPattern;
            ClassLabelClassifiedByMLP = classLabelClassifiedByMLP;
            ClassLabelCalculatedByInvDft = classLabelCalculatedByInvDft;
            //PrevMajorityClassInterval = prevMajorityClassInterval;
            //CurrMajorityClassInterval = currMajorityClassInterval;
        }

        public bool IsAtoBChange()
        {
            if (AAChangePrev <= 0 && AAChangeCurr > 0)
            {
                return true;
            }

            return false;
        }


        public bool IsOtoAChange()
        {
            if (CurrClass0A > 0)
            {
                return true;
            }

            return false;
        }
        public bool IsOtoBChange()
        {
            if (CurrClass0B > 0)
            {
                return true;
            }

            return false;
        }



        public bool IsBtoAChange()
        {
            if (AAChangePrev > 0 && AAChangeCurr < 0)
            {
                return true;
            }

            return false;
        }



        public void AToBChangeCurr()
        {
            AAChangeCurr += 1;
        }
        public void BToAChangeCurr()
        {
            AAChangeCurr += -1;
        }

        public void AddCurrClass0A()
        {
            CurrClass0A++;
        }

        public void AddCurrClass0B()
        {
            CurrClass0B++;
        }


        public void AddCurrClassA()
        {
            CurrClassA++;
        }

        public void AddCurrClassB()
        {
            CurrClassB++;
        }

        //public void CurrAddAToB()
        //{
        //    CurrChangeB += 1;
        //}

        //public void CurrAddBToA()
        //{
        //    CurrChangeA -= 1;
        //}

        //public void CurrNoAddA()
        //{
        //    CurrNoChangeB += 1;
        //}

        //public void CurrNoAddB()
        //{
        //    CurrNoChangeA -= 1;
        //}



        //public void PrevAddAToB()
        //{
        //    PrevChangeB += 1;
        //}

        //public void PrevAddBToA()
        //{
        //    PrevChangeA -= 1;
        //}

        //public void PrevNoAddA()
        //{
        //    PrevNoChangeB += 1;
        //}

        //public void PrevNoAddB()
        //{
        //    PrevNoChangeA -= 1;
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

            AAChangePrev = AAChangeCurr;
            AAChangeCurr = 0;
        }



    }
}
