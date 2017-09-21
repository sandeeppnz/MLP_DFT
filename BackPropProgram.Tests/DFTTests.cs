using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace BackPropProgram.Tests
{


    [TestFixture]
    public class FindRedundantAttributeFromPatterns
    {
        //[TestSetup]
        //public void Setup()
        //{

        //}

        [TearDown]
        public void CleanUp()
        {

        }


        //private TestContext _testContext;
        //public TestContext TestContext
        //{
        //    get { return _testContext; }
        //    set { _testContext = value;  }
        //}
        public TestContext TestContext { get; set; }

        [TestCase("111", "")]
        [TestCase("111,000,011", "")]
        [TestCase("111,000,011", "")]
        [TestCase("*10,**1,1*1", "")]
        [TestCase("**1,*11,111", "")]
        [TestCase("**1,**1,111", "")]
        [TestCase("**1,111,**1", "")]
        [TestCase("**1,111,1**", "")]
        [TestCase("**1,111,*1*", "")]
        [TestCase("*1*,1*1,*1*", "")]
        [TestCase("**1,1**,*11", "")]
        [TestCase("*1*,1**,**0,**0", "")]
        //[Ignore("Some reason")]
        public static void FindRedundantAttributeFromPatterns_Return_Empty(params string[] inputString)
        {
            //arrange
            var clusteredSchemaSxClass1 = inputString[0]
                                        .Split(',')
                                        .ToList();
            //act
            var actualList = DFT.FindRedundantAttributeFromPatterns(clusteredSchemaSxClass1);

            var expectedList = new List<int>();
            if (!string.IsNullOrEmpty(inputString[1]))
            {
                expectedList = inputString[1].Split(',')
                    .Select(int.Parse)
                    .ToList();
                expectedList.Sort();
            }

            //assert
            CollectionAssert.AreEquivalent(expectedList, actualList, "Failed: {0} ", inputString);
            //TestContext.WriteLine(TestContext.Test.Name.ToString());
        }


        [TestCase("**0,**1,1*1", "2")]
        [TestCase("1**,**0,**0", "2")]
        [TestCase("1*0,1**,**0", "2")]
        [TestCase("*11,***,**0", "1")]
        [TestCase("1**,***,*0*", "3")]
        [TestCase("**0,1*1", "2")]
        public static void FindRedundantAttributeFromPatterns_Return_OneElement(params string[] inputString)
        {
            //arrange
            var clusteredSchemaSxClass1 = inputString[0]
                                        .Split(',')
                                        .ToList();
            //act
            var actualList = DFT.FindRedundantAttributeFromPatterns(clusteredSchemaSxClass1);

            var expectedList = new List<int>();
            if (!string.IsNullOrEmpty(inputString[1]))
            {
                expectedList = inputString[1].Split(',')
                    .Select(int.Parse)
                    .ToList();
                expectedList.Sort();
            }
            //assert
            CollectionAssert.AreEquivalent(expectedList, actualList);
        }


        [TestCase("**1,***,**0", "2,1")]
        [TestCase("**1,***,**0", "1,2")] //swap
        [TestCase("**1,**1,**0", "2,1")]
        [TestCase("1**,1**,0**", "2,3")]
        [TestCase("*1*,*0*,*1*", "1,3")]
        [TestCase("*1*,***,*1*", "1,3")]
        [TestCase("***,*1*,*1*", "1,3")]
        [TestCase("*1*,*1*,***", "1,3")]
        public static void FindRedundantAttributeFromPatterns_Return_TwoElements(params string[] inputString)
        {
            //arrange
            var clusteredSchemaSxClass1 = inputString[0]
                                        .Split(',')
                                        .ToList();
            //act
            var actualList = DFT.FindRedundantAttributeFromPatterns(clusteredSchemaSxClass1);

            var expectedList = new List<int>();
            if (!string.IsNullOrEmpty(inputString[1]))
            {
                expectedList = inputString[1].Split(',')
                    .Select(int.Parse)
                    .ToList();
                expectedList.Sort();
            }
            //assert
            CollectionAssert.AreEquivalent(expectedList, actualList);
        }

        [TestCase("***,***,***", "2,1,3")]
        public static void FindRedundantAttributeFromPatterns_Return_ThreeElements(params string[] inputString)
        {
            //arrange
            var clusteredSchemaSxClass1 = inputString[0]
                                        .Split(',')
                                        .ToList();
            //act
            var actualList = DFT.FindRedundantAttributeFromPatterns(clusteredSchemaSxClass1);

            var expectedList = new List<int>();
            if (!string.IsNullOrEmpty(inputString[1]))
            {
                expectedList = inputString[1].Split(',')
                    .Select(int.Parse)
                    .ToList();
                expectedList.Sort();
            }

            //assert
            CollectionAssert.AreEquivalent(expectedList, actualList);
        }


    }

    [TestFixture]
    public class GetSchemaClustersWithWildcardChars
    {
        [TestCase("001", "101,111,000,110,010,100,011", "**0,1*1,011")] //class 1 testing
        [TestCase("101,111,000,110,010,100,011", "001", "001")] //class 0 testing
        [TestCase("001,011", "000,100,110,010,101,111", "**0,1*1")] //class 1 testing
        [TestCase("000,100,110,010,101,111", "001,011", "0*1")] //class 0 testing
        public static void GetSchemaClustersWithWildcardChars_Return_CorrectString(params string[] inputString)
        {
            //arrange
            var instanceSchemas = inputString[1]
                                        .Split(',')
                                        .ToList();
            var otherInstanceSchemas = inputString[0]
                                        .Split(',')
                                        .ToList();

            //act
            var actualList = DFT.GetSchemaClustersWithWildcardChars(instanceSchemas, otherInstanceSchemas);

            var expectedList = new List<string>();
            if (!string.IsNullOrEmpty(inputString[2]))
            {
                expectedList = inputString[2]
                                .Split(',')
                                .ToList();
            }

            //assert
            CollectionAssert.AreEquivalent(expectedList, actualList);
        }
    }

    [TestFixture]
    public class CalculateDFTCoeffs
    {
        [TestCase("000,100,110,010,101,111", "000:0.75,001:0.25,010:0,011:0,100:-0.25,101:0.25,110:0,111:0")]
        [TestCase("1*1,**0", "000:0.75,001:0.25,010:0,011:0,100:-0.25,101:0.25,110:0,111:0")]
        public static void CalculateDFTCoeffs_Return_CorrectCoeffs(params string[] inputString)
        {
            //arrange
            var clusteredSchemaSxClass1 = inputString[0]
                                        .Split(',')
                                        .ToList();

            //act
            List<string> sjVectors = null;
            int numInput = 3;
            var actualList = DFT.CalculateDFTCoeffs(numInput, clusteredSchemaSxClass1, out sjVectors);

            var expectedList = new Dictionary<string, double>();
            var fullExp = new List<string>();
            if (!string.IsNullOrEmpty(inputString[1]))
            {
                fullExp = inputString[1]
                                .Split(',')
                                .ToList();

                foreach (var s in fullExp)
                {
                    string[] entry = s.Split(':').ToArray();
                    expectedList.Add(entry[0].ToString(), double.Parse(entry[1].ToString()));
                }
            }

            //assert
            CollectionAssert.AreEquivalent(expectedList, actualList);
        }
    }

    [TestFixture]
    public class GetCoeffInverseDft
    {
    //    public static string[] CoeffArray = new string[] {
    //        "0000000:0.171875,0000001:-0.015625,0000010:0.03125,0000011:0,0000100:0.171875,0000101:-0.015625,0000110:0.03125,0000111:0,0001000:0.078125,0001001:0.015625,0001010:-0.03125,0001011:0,0001100:0.078125,0001101:0.015625,0001110:-0.03125,0001111:0,0010000:0.171875,0010001:-0.015625,0010010:0.03125,0010011:0,0010100:0.171875,0010101:-0.015625,0010110:0.03125,0010111:0,0011000:0.078125,0011001:0.015625,0011010:-0.03125,0011011:0,0011100:0.078125,0011101:0.015625,0011110:-0.03125,0011111:0,0100000:-0.03125,0100001:0,0100010:-0.015625,0100011:-0.015625,0100100:-0.03125,0100101:0,0100110:-0.015625,0100111:-0.015625,0101000:0.03125,0101001:0,0101010:0.015625,0101011:0.015625,0101100:0.03125,0101101:0,0101110:0.015625,0101111:0.015625,0110000:-0.03125,0110001:0,0110010:-0.015625,0110011:-0.015625,0110100:-0.03125,0110101:0,0110110:-0.015625,0110111:-0.015625,0111000:0.03125,0111001:0,0111010:0.015625,0111011:0.015625,0111100:0.03125,0111101:0,0111110:0.015625,0111111:0.015625,1000000:0,1000001:0,1000010:0.015625,1000011:-0.015625,1000100:0,1000101:0,1000110:0.015625,1000111:-0.015625,1001000:0,1001001:0,1001010:-0.015625,1001011:0.015625,1001100:0,1001101:0,1001110:-0.015625,1001111:0.015625,1010000:0,1010001:0,1010010:0.015625,1010011:-0.015625,1010100:0,1010101:0,1010110:0.015625,1010111:-0.015625,1011000:0,1011001:0,1011010:-0.015625,1011011:0.015625,1011100:0,1011101:0,1011110:-0.015625,1011111:0.015625,1100000:0.015625,1100001:-0.015625,1100010:0,1100011:0,1100100:0.015625,1100101:-0.015625,1100110:0,1100111:0,1101000:-0.015625,1101001:0.015625,1101010:0,1101011:0,1101100:-0.015625,1101101:0.015625,1101110:0,1101111:0,1110000:0.015625,1110001:-0.015625,1110010:0,1110011:0,1110100:0.015625,1110101:-0.015625,1110110:0,1110111:0,1111000:-0.015625,1111001:0.015625,1111010:0,1111011:0,1111100:-0.015625,1111101:0.015625,1111110:0,1111111:0"

    //};
        [Test, TestCaseSource("CoeffArray")]
        public static void GetCoeffInverseDft_ReturnFx1(string inputString)
        {
            string sxString1 = "0000000,0000001,0000010,0000011,0001001,0100000,0100001,0100010,0100011,0101000,0101001,1000000,1000001,1000010,1000011,1100000,1100001,1100010,1100011,1101000,1101001,1101011";

            //arrange
            var fullCoeffString = inputString.Split(',').ToList();
            var coeffArray = new Dictionary<string, double>();
            var sjVectors = new List<string>();

            var sxVectors1 = sxString1.Split(',').ToList();

            foreach (var s in fullCoeffString)
            {
                string[] entry = s.Split(':').ToArray();
                coeffArray.Add(entry[0].ToString(), double.Parse(entry[1].ToString()));
                sjVectors.Add(entry[0].ToString());
            }

            //act
            List<double> actualList1 = new List<double>();
            foreach (string sx in sxVectors1)
            {
                actualList1.Add(DFT.GetCoeffInverseDft(sx, sjVectors, coeffArray));
            }

            List<double> expectedList = new List<double>();
            for (int i = 0; i < actualList1.Count; i++)
            {
                expectedList.Add(1.0);
            }

            //Assert
            CollectionAssert.AreEquivalent(expectedList, actualList1);
        }


        [Test, TestCaseSource("CoeffArray")]
        public static void GetCoeffInverseDft_ReturnFx0(string inputString)
        {
            string sxString0 = "0001000,0001010,0001011,0011010,0101010,0101011,0101110,0111010,0111110,1001000,1001001,1001010,1001011,1010001,1011000,1011001,1011010,1011011,1101010,1110000,1110001,1111000,1111001,1111010,1111011";
            //arrange
            var fullCoeffString = inputString.Split(',').ToList();
            var coeffArray = new Dictionary<string, double>();
            var sjVectors = new List<string>();

            var sxVectors0 = sxString0.Split(',').ToList();

            foreach (var s in fullCoeffString)
            {
                string[] entry = s.Split(':').ToArray();
                coeffArray.Add(entry[0].ToString(), double.Parse(entry[1].ToString()));
                sjVectors.Add(entry[0].ToString());
            }

            //act
            List<double> actualList0 = new List<double>();
            foreach (string sx in sxVectors0)
            {
                actualList0.Add(DFT.GetCoeffInverseDft(sx, sjVectors, coeffArray));
            }

            List<double> expectedList = new List<double>();
            for (int i = 0; i < actualList0.Count; i++)
            {
                expectedList.Add(0.0);
            }

            //Assert
            CollectionAssert.AreEquivalent(expectedList, actualList0);

        }


    }

    [TestFixture]
    public class GetFxByWildcardCharacterCheck
    {
        [TestCase("0001000")]
        public static void GetFxByWildcardCharacterCheck_ReturnFx1(string x)
        {
            //arrange
            string patterns = "0**1*1*,*0010*0,10010**,1101010,1*1*0**";
            var patternList = patterns.Split(',').ToList();

            //act
            double actual = DFT.GetFxByWildcardCharacterCheck(x, patternList, "0");
            double expected = 0;

            //assert
            Assert.AreEqual(expected, actual);

        }

    }


    [TestFixture]
    public class CheckIfRedundantAttribute
    {
        [TestCase("010")]
        [TestCase("011")]
        [TestCase("110")]
        [TestCase("111")]
        public static void CheckRedundantAttribute_Return_True(string sxString)
        {
            //010,011,110,111
            //arrange
            //List<int> positions = new List<int>() { 1, 2 };

            //act
            var actual = DFT.GetRedudantInstanceSchemas(3, 1);
            var expected = new List<string>() { "010", "011", "110", "111" };

            //assert
            //Assert.IsTrue(actual);
            CollectionAssert.AreEqual(expected, actual);
        }

    }

    [TestFixture]
    public class CalculateEnergyThresholding
    {
        [TestCase("1*1,**0")]
        public static void CalculateEnergyThresholding_Return_True(string sxString)
        {
            //010,011,110,111
            //arrange
            //List<int> positions = new List<int>() { 1, 2 };

            //act
            var actual = DFT.CalculateEnergyThresholding(3, 3);
            var expected = new List<string>() { "010", "011", "110", "111" };

            //assert
            //Assert.IsTrue(actual);
            CollectionAssert.AreEquivalent(expected, actual);
        }

    }


}
