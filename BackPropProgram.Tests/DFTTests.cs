using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace BackPropProgram.Tests
{
    [TestFixture]
    public class FindRedundantAttributeFromPatterns
    {

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
            CollectionAssert.AreEquivalent(expectedList, actualList);
        }


        [TestCase("**0,**1,1*1", "2")]
        [TestCase("1**,**0,**0", "2")]
        [TestCase("1*0,1**,**0", "2")]
        [TestCase("*11,***,**0", "1")]
        [TestCase("1**,***,*0*", "3")]
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
        [TestCase("000,100,110,010,101,111","000:0.75,001:0.25,010:0,011:0,100:-0.25,101:0.25,110:0,111:0")]
        public static void CalculateDFTCoeffs_Return_CorrectCoeffs(params string[] inputString)
        {
            //arrange
            var clusteredSchemaSxClass1 = inputString[0]
                                        .Split(',')
                                        .ToList();

            //act
            List<string> sjVectors = null;
            var actualList = DFT.CalculateDFTCoeffs(3, clusteredSchemaSxClass1, out sjVectors);

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


    //[TestFixture]
    //public class GetFxByInverseDFT
    //{
    //    [TestCase("000,100,110,010,101,111", "000:0.75,001:0.25,010:0,011:0,100:-0.25,101:0.25,110:0,111:0")]
    //    public static void GetFxByInverseDFT_Return(params string[] inputString)
    //    {
    //        //arrange
    //        var allSchemaSxClass0 = inputString[0]
    //                                    .Split(',')
    //                                    .ToList();

    //        var sjVectors = inputString[0]
    //                                    .Split(',')
    //                                    .ToList();

    //        var coeffsDFT = null;

    //        //act
    //        var actualList = DFT.GetFxByInverseDFT(allSchemaSxClass0, sjVectors, coeffsDFT);

    //        var expectedList = new Dictionary<string, double>();
    //        var fullExp = new List<string>();
    //        if (!string.IsNullOrEmpty(inputString[1]))
    //        {
    //            fullExp = inputString[1]
    //                            .Split(',')
    //                            .ToList();

    //            foreach (var s in fullExp)
    //            {
    //                string[] entry = s.Split(':').ToArray();
    //                expectedList.Add(entry[0].ToString(), double.Parse(entry[1].ToString()));
    //            }
    //        }

    //        //assert
    //        CollectionAssert.AreEquivalent(expectedList, actualList);
    //    }

    //}

}
