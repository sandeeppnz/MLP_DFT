using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithms
{
    public class schemaClasfcnStat
    {
        public string classifiedClassAs = null;
        public double calculatedClassValue = 0.0;
        //public int previousMajorityErrorOrCorrectPlus = 0; 
        //public  int currentMajorityErrorOrCorrectPlus = 0;//if majority correct , the final result is possitive
        public int inThisIntervalMajorityErrorOrCorrectPlus = 0;
        public int inThisIntervalMajorityTrueClassAPlus = 0;
        public int previousMajorityTrueClassAplus = 0;
        //public  int currentMajorityTrueClassAplus = 0;//if majority true class is A, trhis is plus , else minus
        public int frequencycount = 0;
        public bool hasSchemaContributedToRefinement = false;
        public bool hasFxUpdatedAfterRefinement = true;

        //This is for the winner's statistics
        public schemaClasfcnStat(double _calculatedClassValue, string _classifiedClassAs, int _ErrorOrCorrectPlusInThisOccurance, int _trueClassAplusInThisOccurance, int _previousMajorityTrueClassAplus, int _frequencycount, bool _hasFxUpdatedAfterRefinement)
        {
            calculatedClassValue = _calculatedClassValue;
            classifiedClassAs = _classifiedClassAs;
            //inThisIntervalMajorityErrorOrCorrectPlus = _majorityErrorOrCorrectPlusInThisInterval;
            //previousMajorityErrorOrCorrectPlus = _previousMajorityErrorOrCorrectPlus;
            previousMajorityTrueClassAplus = _previousMajorityTrueClassAplus;
            //majorityTrueClassAPlus = _majorityTrueClassAplus;
            //hasSchemaContributedToRefinement = _contributionFlag;
            hasFxUpdatedAfterRefinement = _hasFxUpdatedAfterRefinement;

            frequencycount = frequencycount + _frequencycount;


            if (_ErrorOrCorrectPlusInThisOccurance < 0)//missclassification
            {
                inThisIntervalMajorityErrorOrCorrectPlus = inThisIntervalMajorityErrorOrCorrectPlus - 1;
            }
            else if (_ErrorOrCorrectPlusInThisOccurance > 0)//correct classification
            {
                inThisIntervalMajorityErrorOrCorrectPlus = inThisIntervalMajorityErrorOrCorrectPlus + 1;
            }
            else
            {
                //nothing
            }


            if (_trueClassAplusInThisOccurance < 0)//class B is true
            {
                inThisIntervalMajorityTrueClassAPlus = inThisIntervalMajorityTrueClassAPlus - 1;
            }
            else if (_trueClassAplusInThisOccurance > 0)//class A is true
            {
                inThisIntervalMajorityTrueClassAPlus = inThisIntervalMajorityTrueClassAPlus + 1;
            }
            else
            {
                //nothing
            }

        }

        //non-winner trees' statistics
        public schemaClasfcnStat(double _calculatedClassValue, string _classifiedClassAs)
        {
            calculatedClassValue = _calculatedClassValue;
            classifiedClassAs = _classifiedClassAs;
            previousMajorityTrueClassAplus = 0;
            //hasFxUpdatedAfterRefinement = _hasFxUpdatedAfterRefinement;
            //hasSchemaContributedToRefinement =false;
            hasFxUpdatedAfterRefinement = true;
            //calculatedClassValue = 0.0;            
            frequencycount = 0;
            inThisIntervalMajorityErrorOrCorrectPlus = 0;
            inThisIntervalMajorityTrueClassAPlus = 0;

        }

    }
}
