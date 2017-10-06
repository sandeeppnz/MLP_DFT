using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackPropProgram
{
    public class OccupancyExtented : InputFile
    {
        public OccupancyExtented()
        {
            NumRows = 102800;
            NumAttributes = 5;
            InputDatasetFileName = "10-occupancyData_Extended_102800_Instances_5Attributes_CBDT.csv";
        }

    }
    public class OccupancySmall : InputFile
    {
        public OccupancySmall()
        {
            NumRows = 20560;
            NumAttributes = 5;
            InputDatasetFileName = "09-occupancyData_20560_Instances_5Attributes_CBDT.csv";
        }

    }



    public class ElectricitySmall : InputFile
    {
        public ElectricitySmall()
        {
            NumRows = 45312;
            NumAttributes = 7;
            InputDatasetFileName = "07-electricitydiscretized-7Att_45312Ins_CBDT.csv";
        }

    }
    public class ElectricityExtended : InputFile
    {
        public ElectricityExtended()
        {
            //Takes a long time to train
            NumRows = 453120;
            NumAttributes = 7;
            InputDatasetFileName = "12-electricityData_Extended_453120Instances_7Att_CBDT.csv";
        }

    }



    public class SensorSmall : InputFile
    {
        public SensorSmall()
        {
            NumRows = 130073;
            NumAttributes = 5;
            InputDatasetFileName = "08-IntelLabSensorStream_5Att_130073Instances_two_majority_classes_29_and_31_CBDT.csv";
        }

    }

    public class SensorExtended : InputFile
    {
        public SensorExtended()
        {
            NumRows = 650365;
            NumAttributes = 5;
            InputDatasetFileName = "15-IntelLabSensorStreamExtended_5Att_650365Instances_added4MoreCycles_two_majority_classes_29_and_31_CBDT.csv";
        }

    }



    public class CoverTypeSmall : InputFile
    {
        public CoverTypeSmall()
        {
            NumRows = 49514;
            NumAttributes = 54;
            InputDatasetFileName = "11-Covertype_TwoClass_49514_Initial_Most_frequent_classesInstances_54Att_my_CBDT.csv";
        }
    }

    public class CoverTypeIntemediate : InputFile
    {
        public CoverTypeIntemediate()
        {
            NumRows = 495141;
            NumAttributes = 54;
            InputDatasetFileName = "13-Covertype_TwoClass_All_495141_Instances_54Att_my_CBDT.csv";
        }
    }

    public class CoverTypeExtended : InputFile
    {
        public CoverTypeExtended()
        {
            NumRows = 495141;
            NumAttributes = 54;
            InputDatasetFileName = "14-CovertypeExtended_TwoClass_495140_Initial10_ofMost_frequent_classesInstances_54Att_my_CBDT.csv";
        }
    }



    public class FlightExtended : InputFile
    {
        public FlightExtended()
        {
            NumRows = 250430;
            NumAttributes = 30;
            InputDatasetFileName = "17-FlightData_Extended_250430_Instances_entireDatafileRepeats9Moretimes_CBDT.csv";
        }
    }

    public class FlightSmall : InputFile
    {
        public FlightSmall()
        {
            NumRows = 25043;
            NumAttributes = 30;
            InputDatasetFileName = "16-Allflightdatadiscretized_30Att_25043Ins_CBDT.csv";
        }
    }


    public class RBF : InputFile
    {
        public RBF()
        {
            NumRows = 1370000;
            NumAttributes = 10;
            InputDatasetFileName = "18-RBF_10Att_1370000Instances_5to25DriftingCentroids_5OrgConcepts_5_Noise_CBDT.csv";
        }
    }

    public class RH : InputFile
    {
        public RH()
        {
            NumRows = 660000;
            NumAttributes = 10;
            InputDatasetFileName = "19-RH_10Att_660000Ins_0.03_0.04_0.05MagChange3Concpets10DftAtt10Sigma_20MoreCycles5Pc_Noise_0.09_0.08_0.07MagChng3NewConcpetsAtEnd_CBDT.csv";
        }
    }



}
