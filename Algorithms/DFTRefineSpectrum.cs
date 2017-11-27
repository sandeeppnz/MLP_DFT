using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithms
{
    public class DFTRefineSpectrum
    {
        public void refineWinnerSpectrumAtEveryIntervalForCurrentClassChanges()
        {
            int zeroCoefficientcount = 0;

            if (!winnerTree.refinedWithinThisConcept)//if the tree has not been refined already within this concept take a clone 
            {

                #region cloning winner tree's drift detector and pattern before the refinemnet start //do only once per particular initial winner

                Dictionary<string, SchemaStatistics> patternOfWinner = (winnerTree.Pattern).Clone() as Dictionary<string, SchemaStatistics>;//clone coefficient index and values 
                EDD driftDetectorOfWinner = (winnerTree.o_driftDetector).Clone() as EDD;//clone drift detector
                dmDSattribute splitAttOfWinner = (winnerTree.o_SplitAttribute).Clone() as dmDSattribute;
                AttributeIndexMap attInMapOfWinner = (winnerTree.attributeIndexMapInitial).Clone() as AttributeIndexMap;
                List<int> importantFeaturesOfWinner = (winnerTree.importantFeaturesForThisSpectrum).Clone() as List<int>;


                #endregion cloning winner tree's drift detector

                bool hasRefinedPrevious = false;

                if (winnerTree.hasRefined)//hasRefined true for the lifetime of  a spectrum , will be used to discard these refined spec terms at the end of stage 2
                {
                    hasRefinedPrevious = true;
                }
                else
                {
                    hasRefinedPrevious = false;
                }

                winnerBeforeRefinement = new dmArchievalTreeCBDT("NonRefinedWinner", patternOfWinner, winnerTree.InstancesSeen, splitAttOfWinner, driftDetectorOfWinner, winnerTree.d_initialAccuracy, this, attInMapOfWinner, hasRefinedPrevious, importantFeaturesOfWinner);
                //spectrum id will be the spectrum id+1                                      

                treeList.Add(winnerBeforeRefinement);//add newly constructed non refined winner tree to pool

                //resetMatchingClassCount();//this will reset all spectrums' (including newly created spectrum) matching class counts to zero
                //1 June 2017 -  this will reset just after t2 measures is taken , not here 

                #region transfering classified class details in reservoir to non refined copy of winner tree

                foreach (KeyValuePair<string, schemaClasfcnStat> specStat in winnerTree.schemaAndStatOfThisSpectrum)
                {
                    //insert that spectrum as non-winner spectrum     
                    double calculatedClassValue = specStat.Value.calculatedClassValue;
                    string classClassifiedOfWinner = (specStat.Value.classifiedClassAs).Clone() as string;//clone drift detector
                    winnerBeforeRefinement.schemaAndStatOfThisSpectrum.Add(specStat.Key, new schemaClasfcnStat(calculatedClassValue, classClassifiedOfWinner));

                }

                #endregion transfering classified class


                winnerTree.hasRefined = true;//this will be true for the lifetime of this spectrum , will be used to discard these refined spec trms at the end of stage 2


                #region reinitialization of winner stat in order to capture details of next interval

                winnerTree.schemaAndStatOfThisSpectrum.Clear();     //winner who is going to be refined will have empty reservoir                              

                #endregion reinitialization

            }
            else //WINNER TREEE has been refined within this concept , that means already cloned , yet we need new reservoir after this refinemnet
            {
                #region reinitialization of winner stat in order to capture details of next interval
                winnerTree.schemaAndStatOfThisSpectrum.Clear();//structure will be there , but zero items - 14 March 2017
                #endregion reinitialization
            }

            #region do refinemnet for each coefficient

            foreach (KeyValuePair<string, SchemaStatistics> kv3 in winnerTree.Pattern)//key is the j vector , only update coefficients which are exist in winner
            {
                //double denominator = Math.Pow(2, winnerTree.attributeIndexMapInitial.o_NodeIndexMap.Count());


                //test for significance of this schema in winner spectrum
                if (kv3.Value.isSignificant)//added on 18th Oct 2016 , after feature selection on coefficient
                {

                    double dCoefficientValue_Contribution_ABBA = 0.0;//reset contribution form set AB or BA class change schemas to refinemnet
                    double dCoefficientValue_Contribution_0A0B = 0.0; //reset contribution form set 0A or 0B class change schemas to refinemnet    
                    double correctionFactor = 0.0;
                    double AveragedcorrectionFactor = 0.0;


                    #region calculate contribution form set AB or BA class change schemas to refinemnet of this coefficient

                    foreach (KeyValuePair<string, double> kv6 in schemaClass_classChanged_ABBA)//key is the X vector
                    {
                        if (kv6.Value != 0)//if the class diffrence is not 0, if 0 whatever the dot product , it is zero
                        {
                            double dDotProduct = (double) Helper.CalculateDotProduct(kv3.Key, kv6.Key); // j*x
                                                                                                 //double dDotProduct = (double)FourierConversion.CalculateDotProduct(kv3.Key, kv6.Key);
                            if (dDotProduct != 0)
                            {

                                dCoefficientValue_Contribution_ABBA = dCoefficientValue_Contribution_ABBA + ((kv6.Value) * dDotProduct); //sum over Xs of set ABBA
                            }

                            if (dDotProduct != -1.0 && dDotProduct != 1.0)
                            {

                            }
                        }
                    }

                    #endregion calculate contribution form set AB or BA class change schemas to refinemnet of this coefficient


                    #region calculate contribution form set 0A or 0B class change schemas to refinemnet of this coefficient

                    foreach (KeyValuePair<string, double> kv7 in schemaClass_classChanged_0A0B)//key is the X vector
                    {
                        if (kv7.Value != 0)//if the class difference is not 0
                        {
                            double dDotProduct = (double) Helper.CalculateDotProduct(kv3.Key, kv7.Key); // j*x
                                                                                                 //double dDotProduct = (double)FourierConversion.CalculateDotProduct(kv3.Key, kv7.Key);
                            if (dDotProduct != 0)
                            {

                                dCoefficientValue_Contribution_0A0B = dCoefficientValue_Contribution_0A0B + ((kv7.Value) * dDotProduct); //sum over Xs of set 0A0B
                            }

                            if (dDotProduct != -1.0 && dDotProduct != 1.0)
                            {

                            }
                        }
                    }

                    #endregion calculate contribution form set 0A or 0B class change schemas to refinemnet of this coefficient

                    //correctionFactor = dCoefficientValue_Contribution_ABBA + (Alpha * dCoefficientValue_Contribution_0A0B);
                    correctionFactor = dCoefficientValue_Contribution_ABBA + dCoefficientValue_Contribution_0A0B;
                    //AveragedcorrectionFactor = correctionFactor / System.Convert.ToDouble(reservoirInstanceCount);

                    //AveragedcorrectionFactor = correctionFactor / denominator;
                    AveragedcorrectionFactor = correctionFactor / distinctSchemaWithinInterval.Count();
                    //AveragedcorrectionFactor = (correctionFactor * changeFrequencyCountInThisInterval) / intervalSizeForRefinement;


                    //AveragedcorrectionFactor = correctionFactor / denominator;
                    kv3.Value.coefficientValue = kv3.Value.coefficientValue + AveragedcorrectionFactor;

                    if (kv3.Value.coefficientValue == 0)
                    {
                        zeroCoefficientcount++;
                    }

                    if (zeroCoefficientcount == winnerTree.Pattern.Count())
                    {
                        //trouble---- solution below

                        //find new winner 
                        //delete this current winner from pool
                        //rest winner reservoir
                        //reset matching count
                    }


                }
                else
                {
                    //insignificant coefficient schema of winner , update is not necesary , we do not use these for f(x) calculations
                }

            }

            #region Winner tree's new important feature vector
            /*
                winnerTree.importantFeaturesForThisSpectrum.Clear();
                winnerTree.selectSignificantCoefficientsOfEachSpectra();
                winnerImportantFeaturesCount = winnerTree.importantFeaturesForThisSpectrum.Count();
            */
            #endregion Winner tree's new important feature vector


            #endregion do refinemnet for each coefficient




            #region check whether the refinement results meaningful spectrum

            if (zeroCoefficientcount == winnerTree.Pattern.Count()) // refinement results menaingless sectrum
            {
                //trouble

                //find new winner 
                //delete this current winner from pool
                //rest winner reservoir
                //reset matching count
                treeList.Remove(winnerTree);
                winnerTree = DetermineWinnerTree();

                resetMatchingClassificationCount();
                resetMatchingClassCount();

                denominator = Math.Pow(2, winnerTree.attributeIndexMapInitial.o_NodeIndexMap.Count());//19 May 2017 - important features from decision tree

                //will clear new winner's reservoir if it differs from previous
                winnerTree.schemaAndStatOfThisSpectrum.Clear();//new winner's treservoir is cleaned 

                instanceCountWinnerSinceRefinement = 0;//reset count for aggregation
                winnerBeforeRefinement = null;
                OriginalAndRefiningAgreement = 0.0;//chamari added this on 9 Jan 2017
                previousLambdaAccumulation.Clear();
                observedInstanceCountWithinThisConceptThisWinnerThisRefinedVersion = 0;
                AccProxyForChanges = 1.0;

                schemaClass_classChanged_ABBA.Clear();
                schemaClass_classChanged_0A0B.Clear();
                distinctSchemaWithinInterval.Clear();
                //instanceAndClassReservoir.Clear();

                //14 March 2017
                firstIntervalCompleted = false; // somecases, there may be drift( internal or concept ) at interval check = observations within this concept , in this case first check point will not reach previous =current method
                firstLambdaCompleted = false;

                winnerImportantFeaturesCount = winnerTree.importantFeaturesForThisSpectrum.Count();
            }
            else
            {
                isJustRefined = true;
                winnerTree.refinedWithinThisConcept = true;
            }


            #endregion check whether the refinement is meaningful                         

        }
    }
}
