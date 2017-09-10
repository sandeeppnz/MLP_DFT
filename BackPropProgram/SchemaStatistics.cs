using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackPropProgram
{
    public class DepthElement
    {
        public int depth;
        public int index;
    }


    public class SchemaStatistics
    {
        string s_Class = null;
        string s_LeafNodeName = null;
        bool b_Label = false;
        public string Class { get { return s_Class; } }
        public string NodeName { get { return s_LeafNodeName; } }
        public bool Label { get { return b_Label; } }
        public bool b_Required = true;

        public int i_wildcardCount;
        public int i_oneCount;

        public double coefficientValue;
        public double energy;

        public int[] depths;
        public int attributeBinaryLength;
        public DepthElement[] depthElements;
        public SchemaStatistics(string _sClass, string _sLeafNodeName, int wildcardCount, int oneCount, double _coefficientValue, int[] _depths, int _attributeBinaryLength)
        {
            s_Class = _sClass;
            s_LeafNodeName = _sLeafNodeName;
            i_wildcardCount = wildcardCount;
            i_oneCount = oneCount;
            coefficientValue = _coefficientValue;
            energy = 0.0;
            depths = _depths;
            //createDepthElements();
            //if(_sClass == _leftMostClass)
            //{
            //    b_Label = true;
            //}
            //b_Label = _bLabel;
        }

        public SchemaStatistics(double _coefficientValue)
        {
            coefficientValue = _coefficientValue;
        }

        public SchemaStatistics(double _coefficientValue, int _oneCount)
        {
            coefficientValue = _coefficientValue;
            i_oneCount = _oneCount;
        }

        public void setEnergyValue(double _energyValue)
        {
            energy = _energyValue;
        }

        public String getDepths()
        {
            String depthsString = "";
            for (int index = 0; index < depths.Length; index++)
            {
                depthsString = depthsString + depths[index];
            }
            return depthsString;
        }

        public void createDepthElements()
        {
            for (int index = 0; index < depths.Length; index++)
            {
                depthElements[index] = new DepthElement();
                depthElements[index].depth = depths[index];
                depthElements[index].index = index;
            }
            QuickSort(depthElements);
        }

        public DepthElement[] getDepthElements()
        {
            return depthElements;
        }

        public static void QuickSort(DepthElement[] _array)
        {
            sort(_array, 0, _array.Length - 1);
        }

        public static void sort(DepthElement[] arr, int left, int right)
        {
            int pivot;
            int l_holder, r_holder;

            l_holder = left;
            r_holder = right;
            pivot = arr[left].depth;

            while (left < right)
            {
                while ((arr[right].depth >= pivot) && (left < right))
                {
                    right--;
                }

                if (left != right)
                {
                    arr[left] = arr[right];
                    left++;
                }

                while ((arr[left].depth <= pivot) && (left < right))
                {
                    left++;
                }

                if (left != right)
                {
                    arr[right] = arr[left];
                    right--;
                }
            }

            arr[left].depth = pivot;
            pivot = left;
            left = l_holder;
            right = r_holder;

            if (left < pivot)
            {
                sort(arr, left, pivot - 1);
            }

            if (right > pivot)
            {
                sort(arr, pivot + 1, right);
            }
        }


    }
}
