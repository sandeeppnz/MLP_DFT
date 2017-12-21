using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithms
{
    public class SchemaStat
    {
        public string ClusterPattern { get; set; }
        public int TrainingClassValue { get; set; } //value from the MLP training
        public int InvDftClassValue { get; set; }

        public int CurrClassValue { get; set; } //new class value based on winner at each interval
        public int PrevClassValue { get; set; } //new class value based on winner at each interval

        public int CurrABBA { get; set; }
        public int PrevABBA { get; set; }

        public int Curr0A0B { get; set; }
        public int Prev0A0B { get; set; }

        public int ACount { get; set; }
        public int BCount { get; set; }


        //public int AAChangeCurr { get; set; }
        //public int AAChangePrev { get; set; }


        //used
        //public int PrevClassA { get; set; }
        //public int PrevClassB { get; set; }

        //public int CurrClassA { get; set; }
        //public int CurrClassB { get; set; }

        //public int CurrClass0A { get; set; }
        //public int CurrClass0B { get; set; }

        //public SchemaStat(string schemaInstance, string clusterPattern, string classLabelClassifiedByMLP, string classLabelCalculatedByInvDft, int prevMajorityClassInterval, int currMajorityClassInterval)
        public SchemaStat(string clusterPattern, int classLabelClassifiedByMLP, int classLabelCalculatedByInvDft)
        {
            ClusterPattern = clusterPattern;
            TrainingClassValue = classLabelClassifiedByMLP;
            InvDftClassValue = classLabelCalculatedByInvDft;

            CurrClassValue = classLabelClassifiedByMLP; //initialze with MLP class value
            PrevClassValue = classLabelClassifiedByMLP; //initialze with MLP class value

            CurrABBA = 0;
            PrevABBA = 0;

            Curr0A0B = 0;
            Prev0A0B = 0;

            //PrevMajorityClassInterval = prevMajorityClassInterval;
            //CurrMajorityClassInterval = currMajorityClassInterval;
        }

        public void ABChange()
        {
            CurrABBA += 1;
        }

        public void Add_ACount()
        {
            ACount++;
        }
        public void Add_BCount()
        {
            BCount++;
        }


        public void BAChange()
        {
            CurrABBA -= 1;
        }


        public void _0AChange()
        {
            Curr0A0B -= 1;
        }

        public void _0BChange()
        {
            Curr0A0B += 1;
        }


        public int GetDirectionABBA()
        {
            if (CurrABBA == PrevABBA)
                return 0;

            if (CurrABBA == 0 && PrevABBA == 0)
                return 0;

            if (PrevABBA <= 0 && CurrABBA >= 0)
                return 1;

            if (PrevABBA >= 0 && CurrABBA <= 0)
                return -1;

            return 0;
        }




        public double GetCurrMajority()
        {
            //if (CurrClassA > CurrClassB)
            //{
            //    return 0.0;
            //}
            //else
            //{
            //    return 1.0;
            //}
            return 0;
        }

        public double GetPrevMajority()
        {
            //if (PrevClassA > PrevClassB)
            //{
            //    return 0.0;
            //}
            //else
            //{
            //    return 1.0;
            //}

            return 0;
        }

        public void UpdateSchema()
        {
            int res = GetDirectionABBA();


            if (ACount != 0 && BCount != 0)
            {
                if (ACount != BCount)
                {
                    if (res == -1)
                    {
                        TrainingClassValue = 0;
                        CurrClassValue = 0;
                    }
                    else if (res == 1)
                    {
                        TrainingClassValue = 1;
                        CurrClassValue = 1;
                    }

                    if (Curr0A0B < 0)
                    {
                        TrainingClassValue = 0;
                        CurrClassValue = 0;
                    }
                    else if (Curr0A0B > 0)
                    {
                        TrainingClassValue = 1;
                        CurrClassValue = 1;
                    }

                }
            }
            else
            {
                if (res == -1)
                {
                    TrainingClassValue = 0;
                    CurrClassValue = 0;
                }
                else if (res == 1)
                {
                    TrainingClassValue = 1;
                    CurrClassValue = 1;
                }

                if (Curr0A0B < 0)
                {
                    TrainingClassValue = 0;
                    CurrClassValue = 0;
                }
                else if (Curr0A0B > 0)
                {
                    TrainingClassValue = 1;
                    CurrClassValue = 1;
                }

            }


        }




        public void CopyCurrToPrev()
        {
            Prev0A0B = Curr0A0B;
            Curr0A0B = 0;

            PrevABBA = CurrABBA;
            CurrABBA = 0;

            PrevClassValue = CurrClassValue;
            CurrClassValue = 0;

            ACount = 0;
            BCount = 0;

        }



    }
}
