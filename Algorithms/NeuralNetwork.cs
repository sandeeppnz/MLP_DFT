﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Algorithms
{
    /*  
 * https://visualstudiomagazine.com/Articles/2015/04/01/Back-Propagation-Using-C.aspx?Page=3 
 */


    public interface INeuralNetwork
    {

        int GetTotalWeights();
        void SetWeights(double[] weights);


        void SetAllWeights(double[] weights);
        double[] GetAllWeights();


        int GetNumOutputNodes();
        int GetNumInputNodes();
        int GetNumHiddenNodes();


        double[] ComputeOutputs(double[] xValues);
        double[] Train(double[][] trainData, int maxEpochs, double learnRate, double momentum);
        double[] NewTrain(float[][] trainData, int maxEpochs, double learnRate, double momentum);
        double Accuracy(double[][] data, double[] rankArray = null);
        double NewAccuracy(float[][] data, double[] rankArray = null);
        void Dispose(bool disposing);


    }


    public partial class NeuralNetwork : INeuralNetwork, IDisposable
    {


        public double[] AllWeights { get; set; }

        public int NumInputNodes { get; set; }
        public int NumOutputNodes { get; set; }
        public int NumHiddenNodes { get; set; }


        private double[] inputs;
        private double[][] ihWeights; // input-hidden
        private double[] hBiases;
        private double[] hOutputs;

        private double[][] hoWeights; // hidden-output
        private double[] oBiases;
        private double[] outputs;

        private Random rnd;
        private bool isFeatureSelection;

        public int _totalWeights;


        public NeuralNetwork(int numInput, int numHidden, int numOutput, bool isFeatureSelection)
        {
            this.NumInputNodes = numInput;
            this.NumHiddenNodes = numHidden;
            this.NumOutputNodes = numOutput;

            this.inputs = new double[numInput];

            this.ihWeights = MakeMatrix(numInput, numHidden, 0.0);
            this.hBiases = new double[numHidden];
            this.hOutputs = new double[numHidden];

            this.hoWeights = MakeMatrix(numHidden, numOutput, 0.0);
            this.oBiases = new double[numOutput];
            this.outputs = new double[numOutput];

            this.rnd = new Random(0);

            this.isFeatureSelection = isFeatureSelection;

            this.InitializeWeights(); // all weights and biases

            int numWeights = (numInput * numHidden) + numHidden +
                  (numHidden * numOutput) + numOutput;
            this._totalWeights = numWeights;

        } // ctor



        public int GetNumOutputNodes()
        {
            return NumOutputNodes;
        }

        public int GetNumHiddenNodes()
        {
            return NumHiddenNodes;
        }

        public int GetNumInputNodes()
        {
            return NumInputNodes;
        }


        private static double[][] MakeMatrix(int rows,
          int cols, double v) // helper for ctor, Train
        {
            double[][] result = new double[rows][];
            for (int r = 0; r < result.Length; ++r)
                result[r] = new double[cols];
            for (int i = 0; i < rows; ++i)
                for (int j = 0; j < cols; ++j)
                    result[i][j] = v;
            return result;
        }


        //private static double[][] MakeMatrixRandom(int rows,
        //  int cols, int seed) // helper for ctor, Train
        //{
        //  Random rnd = new Random(seed);
        //  double hi = 0.01;
        //  double lo = -0.01;
        //  double[][] result = new double[rows][];
        //  for (int r = 0; r < result.Length; ++r)
        //    result[r] = new double[cols];
        //  for (int i = 0; i < rows; ++i)
        //    for (int j = 0; j < cols; ++j)
        //      result[i][j] = (hi - lo) * rnd.NextDouble() + lo;
        //  return result;
        //}

        private void InitializeWeights() // helper for ctor
        {
            // initialize weights and biases to small random values
            int numWeights = (NumInputNodes * NumHiddenNodes) +
              (NumHiddenNodes * NumOutputNodes) + NumHiddenNodes + NumOutputNodes;
            double[] initialWeights = new double[numWeights];
            for (int i = 0; i < initialWeights.Length; ++i)
                initialWeights[i] = (0.001 - 0.0001) * rnd.NextDouble() + 0.0001;
            this.SetWeights(initialWeights);
        }

        public void SetAllWeights(double[] weights)
        {
            AllWeights = weights;
        }

        public double[] GetAllWeights()
        {
            return AllWeights;
        }



        public void SetWeights(double[] weights)
        {
            // copy serialized weights and biases in weights[] array
            // to i-h weights, i-h biases, h-o weights, h-o biases
            int numWeights = (NumInputNodes * NumHiddenNodes) +
              (NumHiddenNodes * NumOutputNodes) + NumHiddenNodes + NumOutputNodes;

            if (weights.Length != numWeights)
                throw new Exception("Bad weights array in SetWeights");

            int k = 0; // points into weights param

            for (int i = 0; i < NumInputNodes; ++i)
                for (int j = 0; j < NumHiddenNodes; ++j)
                    ihWeights[i][j] = weights[k++];
            for (int i = 0; i < NumHiddenNodes; ++i)
                hBiases[i] = weights[k++];
            for (int i = 0; i < NumHiddenNodes; ++i)
                for (int j = 0; j < NumOutputNodes; ++j)
                    hoWeights[i][j] = weights[k++];
            for (int i = 0; i < NumOutputNodes; ++i)
                oBiases[i] = weights[k++];
        }

        public double[] GetWeights()
        {
            int numWeights = (NumInputNodes * NumHiddenNodes) +
              (NumHiddenNodes * NumOutputNodes) + NumHiddenNodes + NumOutputNodes;
            double[] result = new double[numWeights];
            int k = 0;
            for (int i = 0; i < ihWeights.Length; ++i)
                for (int j = 0; j < ihWeights[0].Length; ++j)
                    result[k++] = ihWeights[i][j];
            for (int i = 0; i < hBiases.Length; ++i)
                result[k++] = hBiases[i];
            for (int i = 0; i < hoWeights.Length; ++i)
                for (int j = 0; j < hoWeights[0].Length; ++j)
                    result[k++] = hoWeights[i][j];
            for (int i = 0; i < oBiases.Length; ++i)
                result[k++] = oBiases[i];
            return result;
        }

        public double[] InitialOutput(double[] yValues)
        {
            for (int i = 0; i < yValues.Length; ++i) // copy x-values to inputs
                this.outputs[i] = yValues[i];

            double[] retResult = new double[NumOutputNodes]; // could define a GetOutputs 
            Array.Copy(this.outputs, retResult, retResult.Length);
            return retResult;
        }

        public double[] ComputeOutputs(double[] xValues)
        {
            double[] hSums = new double[NumHiddenNodes]; // hidden nodes sums scratch array
            double[] oSums = new double[NumOutputNodes]; // output nodes sums

            for (int i = 0; i < xValues.Length; ++i) // copy x-values to inputs
                this.inputs[i] = xValues[i];
            // note: no need to copy x-values unless you implement a ToString.
            // more efficient is to simply use the xValues[] directly.

            for (int j = 0; j < NumHiddenNodes; ++j)  // compute i-h sum of weights * inputs
                for (int i = 0; i < NumInputNodes; ++i)
                    hSums[j] += this.inputs[i] * this.ihWeights[i][j]; // note +=

            for (int i = 0; i < NumHiddenNodes; ++i)  // add biases to hidden sums
                hSums[i] += this.hBiases[i];

            for (int i = 0; i < NumHiddenNodes; ++i)   // apply activation
                this.hOutputs[i] = HyperTan(hSums[i]); // 

            for (int j = 0; j < NumOutputNodes; ++j)   // compute h-o sum of weights * hOutputs
                for (int i = 0; i < NumHiddenNodes; ++i)
                    oSums[j] += hOutputs[i] * hoWeights[i][j];

            for (int i = 0; i < NumOutputNodes; ++i)  // add biases to output sums
                oSums[i] += oBiases[i];

            double[] softOut = Softmax(oSums); 
            
            
            // all outputs at once for efficiency
            
            Array.Copy(softOut, outputs, softOut.Length);

            double[] retResult = new double[NumOutputNodes]; // could define a GetOutputs 
            Array.Copy(this.outputs, retResult, retResult.Length);
            return retResult;
        }

        private static double HyperTan(double x)
        {
            if (x < -20.0) return -1.0; // approximation is correct to 30 decimals
            else if (x > 20.0) return 1.0;
            else return Math.Tanh(x);
        }

        private static double[] Softmax(double[] oSums)
        {
            // does all output nodes at once so scale
            // doesn't have to be re-computed each time

            double sum = 0.0;
            for (int i = 0; i < oSums.Length; ++i)
                sum += Math.Exp(oSums[i]);

            double[] result = new double[oSums.Length];
            for (int i = 0; i < oSums.Length; ++i)
                result[i] = Math.Exp(oSums[i]) / sum;

            return result; // now scaled so that xi sum to 1.0
        }


        //private static double[] SigmoidOutput(double[] oSums)
        //{
        //    // does all output nodes at once so scale
        //    // doesn't have to be re-computed each time

        //    double sum = 0.0;
        //    for (int i = 0; i < oSums.Length; ++i)
        //        sum += Math.Exp(oSums[i]);

        //    double[] result = new double[oSums.Length];
        //    for (int i = 0; i < oSums.Length; ++i)
        //        result[i] = Math.Exp(oSums[i]) / sum;

        //    return result; // now scaled so that xi sum to 1.0
        //}





        public double[] Train(double[][] trainData, int maxEpochs,
          double learnRate, double momentum)
        {
            // train using back-prop
            // back-prop specific arrays
            double[][] hoGrads = MakeMatrix(NumHiddenNodes, NumOutputNodes, 0.0); // hidden-to-output weight gradients
            double[] obGrads = new double[NumOutputNodes];                   // output bias gradients

            double[][] ihGrads = MakeMatrix(NumInputNodes, NumHiddenNodes, 0.0);  // input-to-hidden weight gradients
            double[] hbGrads = new double[NumHiddenNodes];                   // hidden bias gradients

            double[] oSignals = new double[NumOutputNodes];                  // local gradient output signals - gradients w/o associated input terms
            double[] hSignals = new double[NumHiddenNodes];                  // local gradient hidden node signals

            // back-prop momentum specific arrays 
            double[][] ihPrevWeightsDelta = MakeMatrix(NumInputNodes, NumHiddenNodes, 0.0);
            double[] hPrevBiasesDelta = new double[NumHiddenNodes];
            double[][] hoPrevWeightsDelta = MakeMatrix(NumHiddenNodes, NumOutputNodes, 0.0);
            double[] oPrevBiasesDelta = new double[NumOutputNodes];

            int epoch = 0;
            double[] xValues = new double[NumInputNodes]; // inputs
            //double[] tValues = new double[NumOutputNodes]; // target values

            double[] tValuesFile = new double[NumOutputNodes]; // TODO: target values


            double derivative = 0.0;
            double errorSignal = 0.0;

            int[] sequence = new int[trainData.Length];
            for (int i = 0; i < sequence.Length; ++i)
                sequence[i] = i;

            int errInterval = maxEpochs / 10; // interval to check error
            while (epoch < maxEpochs)
            {
                ++epoch;

                if (epoch % errInterval == 0 && epoch < maxEpochs)
                {
                    double trainErr = Error(trainData);
                    Console.WriteLine("epoch = " + epoch + "  error = " +
                      trainErr.ToString("F4"));
                    //Console.ReadLine();
                }

                Shuffle(sequence); // visit each training data in random order
                for (int ii = 0; ii < trainData.Length; ++ii)
                {
                    int idx = sequence[ii];

                    Array.Copy(trainData[idx], xValues, NumInputNodes);
                    //Array.Copy(trainData[idx], numInput, tValues, 0, numOutput);

                    //i.e. Last two colummns are Actual Values
                    Array.Copy(trainData[idx], NumInputNodes + 2, tValuesFile, 0, NumOutputNodes);


                    ComputeOutputs(xValues); // copy xValues in, compute outputs 

                    // indices: i = inputs, j = hiddens, k = outputs

                    // 1. compute output node signals (assumes softmax)
                    for (int k = 0; k < NumOutputNodes; ++k)
                    {
                        //errorSignal = tValues[k] - outputs[k];  // Wikipedia uses (o-t)
                        errorSignal = tValuesFile[k] - outputs[k];  // Wikipedia uses (o-t)

                        derivative = (1 - outputs[k]) * outputs[k]; // for softmax
                        oSignals[k] = errorSignal * derivative;
                    }

                    // 2. compute hidden-to-output weight gradients using output signals
                    for (int j = 0; j < NumHiddenNodes; ++j)
                        for (int k = 0; k < NumOutputNodes; ++k)
                            hoGrads[j][k] = oSignals[k] * hOutputs[j];

                    // 2b. compute output bias gradients using output signals
                    for (int k = 0; k < NumOutputNodes; ++k)
                        obGrads[k] = oSignals[k] * 1.0; // dummy assoc. input value

                    // 3. compute hidden node signals
                    for (int j = 0; j < NumHiddenNodes; ++j)
                    {
                        derivative = (1 + hOutputs[j]) * (1 - hOutputs[j]); // for tanh
                        double sum = 0.0; // need sums of output signals times hidden-to-output weights
                        for (int k = 0; k < NumOutputNodes; ++k)
                        {
                            sum += oSignals[k] * hoWeights[j][k]; // represents error signal
                        }
                        hSignals[j] = derivative * sum;
                    }

                    // 4. compute input-hidden weight gradients
                    for (int i = 0; i < NumInputNodes; ++i)
                        for (int j = 0; j < NumHiddenNodes; ++j)
                            ihGrads[i][j] = hSignals[j] * inputs[i];

                    // 4b. compute hidden node bias gradients
                    for (int j = 0; j < NumHiddenNodes; ++j)
                        hbGrads[j] = hSignals[j] * 1.0; // dummy 1.0 input

                    // == update weights and biases

                    // update input-to-hidden weights
                    for (int i = 0; i < NumInputNodes; ++i)
                    {
                        for (int j = 0; j < NumHiddenNodes; ++j)
                        {
                            double delta = ihGrads[i][j] * learnRate;
                            ihWeights[i][j] += delta; // would be -= if (o-t)
                            ihWeights[i][j] += ihPrevWeightsDelta[i][j] * momentum;
                            ihPrevWeightsDelta[i][j] = delta; // save for next time
                        }
                    }

                    // update hidden biases
                    for (int j = 0; j < NumHiddenNodes; ++j)
                    {
                        double delta = hbGrads[j] * learnRate;
                        hBiases[j] += delta;
                        hBiases[j] += hPrevBiasesDelta[j] * momentum;
                        hPrevBiasesDelta[j] = delta;
                    }

                    // update hidden-to-output weights
                    for (int j = 0; j < NumHiddenNodes; ++j)
                    {
                        for (int k = 0; k < NumOutputNodes; ++k)
                        {
                            double delta = hoGrads[j][k] * learnRate;
                            hoWeights[j][k] += delta;
                            hoWeights[j][k] += hoPrevWeightsDelta[j][k] * momentum;
                            hoPrevWeightsDelta[j][k] = delta;
                        }
                    }

                    // update output node biases
                    for (int k = 0; k < NumOutputNodes; ++k)
                    {
                        double delta = obGrads[k] * learnRate;
                        oBiases[k] += delta;
                        oBiases[k] += oPrevBiasesDelta[k] * momentum;
                        oPrevBiasesDelta[k] = delta;
                    }

                } // each training item

            } // while
            double[] bestWts = GetWeights();
            return bestWts;
        } // Train



        public double[] NewTrain(float[][] trainData, int maxEpochs,
          double learnRate, double momentum)
        {
            Console.WriteLine("\nTraining started...");
            Stopwatch sw = new Stopwatch();
            sw.Start();


            // train using back-prop
            // back-prop specific arrays
            double[][] hoGrads = MakeMatrix(NumHiddenNodes, NumOutputNodes, 0.0); // hidden-to-output weight gradients
            double[] obGrads = new double[NumOutputNodes];                   // output bias gradients

            double[][] ihGrads = MakeMatrix(NumInputNodes, NumHiddenNodes, 0.0);  // input-to-hidden weight gradients
            double[] hbGrads = new double[NumHiddenNodes];                   // hidden bias gradients

            double[] oSignals = new double[NumOutputNodes];                  // local gradient output signals - gradients w/o associated input terms
            double[] hSignals = new double[NumHiddenNodes];                  // local gradient hidden node signals

            // back-prop momentum specific arrays 
            double[][] ihPrevWeightsDelta = MakeMatrix(NumInputNodes, NumHiddenNodes, 0.0);
            double[] hPrevBiasesDelta = new double[NumHiddenNodes];
            double[][] hoPrevWeightsDelta = MakeMatrix(NumHiddenNodes, NumOutputNodes, 0.0);
            double[] oPrevBiasesDelta = new double[NumOutputNodes];

            int epoch = 0;
            double[] xValues = new double[NumInputNodes]; // inputs

            double[] tValuesFile = new double[NumOutputNodes]; // TODO: target values


            double derivative = 0.0;
            double error = 0.0;

            int[] sequence = new int[trainData.Length];
            for (int i = 0; i < sequence.Length; ++i)
                sequence[i] = i;

            int errInterval = maxEpochs / 10; // interval to check error

            bool updatedCal = false;

            while (epoch < maxEpochs)
            {
                ++epoch;

                if (epoch % errInterval == 0 && epoch < maxEpochs)
                {
                    double trainErr = NewError(trainData);
                    Console.WriteLine("epoch = " + epoch + "  error = " +
                      trainErr.ToString("F4"));
                    //Console.ReadLine();
                }

                Shuffle(sequence); // visit each training data in random order

                for (int ii = 0; ii < trainData.Length; ++ii)
                {
                    int idx = sequence[ii];
                    //int idx = ii;

                    Array.Copy(trainData[idx], xValues, NumInputNodes);
                    //Array.Copy(trainData[idx], numInput, tValues, 0, numOutput);

                    Array.Copy(trainData[idx], NumInputNodes + 3, tValuesFile, 0, NumOutputNodes);

                    //ComputeOutputs(xValues); // copy xValues in, compute outputs 

                    ////TODO: added to set the calculated class to dataset, maybe give diff results
                    //Originally, it was set stored in GenerateArtificalDataUsingNN()
                    var calOutput = ComputeOutputs(xValues); // copy xValues in, compute outputs 

                    if (!updatedCal)
                    {
                        // translate outputs to 1-of-N
                        float[] oneOfN = new float[NumOutputNodes]; // all 0.0

                        int maxIndex = 0;
                        double maxValue = calOutput[0];
                        for (int i = 0; i < NumOutputNodes; ++i)
                        {
                            if (calOutput[i] > maxValue)
                            {
                                maxIndex = i;
                                maxValue = calOutput[i];
                            }
                        }
                        oneOfN[maxIndex] = 1;

                        int c = NumInputNodes + 1;
                        for (int i = 0; i < NumOutputNodes; ++i) // outputs
                            trainData[ii][c++] = oneOfN[i];

                        updatedCal = true;
                    }





                    // indices: i = inputs, j = hiddens, k = outputs

                    // 1. compute output node signals (assumes softmax)
                    for (int k = 0; k < NumOutputNodes; ++k)
                    {
                        error = tValuesFile[k] - outputs[k];  // Wikipedia uses (o-t)
                        derivative = (1 - outputs[k]) * outputs[k]; // for softmax
                        oSignals[k] = error * derivative;
                    }   

                    // 2. compute hidden-to-output weight gradients using output signals
                    for (int j = 0; j < NumHiddenNodes; ++j)
                        for (int k = 0; k < NumOutputNodes; ++k)
                            hoGrads[j][k] = oSignals[k] * hOutputs[j];

                    // 2b. compute output bias gradients using output signals
                    for (int k = 0; k < NumOutputNodes; ++k)
                        obGrads[k] = oSignals[k] * 1.0; // dummy assoc. input value

                    // 3. compute hidden node signals

                    for (int j = 0; j < NumHiddenNodes; ++j)
                    {
                        derivative = (1 + hOutputs[j]) * (1 - hOutputs[j]); // for tanh
                        double sum = 0.0; // need sums of output signals times hidden-to-output weights
                        for (int k = 0; k < NumOutputNodes; ++k)
                        {
                            sum += oSignals[k] * hoWeights[j][k]; // represents error signal
                        }
                        hSignals[j] = derivative * sum;
                    }

                    // 4. compute input-hidden weight gradients
                    for (int i = 0; i < NumInputNodes; ++i)
                        for (int j = 0; j < NumHiddenNodes; ++j)
                            ihGrads[i][j] = hSignals[j] * inputs[i];

                    // 4b. compute hidden node bias gradients
                    for (int j = 0; j < NumHiddenNodes; ++j)
                        hbGrads[j] = hSignals[j] * 1.0; // dummy 1.0 input



                    // == update weights and biases

                    // update input-to-hidden weights
                    for (int i = 0; i < NumInputNodes; ++i)
                    {
                        for (int j = 0; j < NumHiddenNodes; ++j)
                        {
                            double delta = ihGrads[i][j] * learnRate;
                            ihWeights[i][j] += delta; // would be -= if (o-t)
                            ihWeights[i][j] += ihPrevWeightsDelta[i][j] * momentum;
                            ihPrevWeightsDelta[i][j] = delta; // save for next time
                        }
                    }

                    // update hidden biases
                    for (int j = 0; j < NumHiddenNodes; ++j)
                    {
                        double delta = hbGrads[j] * learnRate;
                        hBiases[j] += delta;
                        hBiases[j] += hPrevBiasesDelta[j] * momentum;
                        hPrevBiasesDelta[j] = delta;
                    }

                    // update hidden-to-output weights
                    for (int j = 0; j < NumHiddenNodes; ++j)
                    {
                        for (int k = 0; k < NumOutputNodes; ++k)
                        {
                            double delta = hoGrads[j][k] * learnRate;
                            hoWeights[j][k] += delta;
                            hoWeights[j][k] += hoPrevWeightsDelta[j][k] * momentum;
                            hoPrevWeightsDelta[j][k] = delta;
                        }
                    }

                    // update output node biases
                    for (int k = 0; k < NumOutputNodes; ++k)
                    {
                        double delta = obGrads[k] * learnRate;
                        oBiases[k] += delta;
                        oBiases[k] += oPrevBiasesDelta[k] * momentum;
                        oPrevBiasesDelta[k] = delta;
                    }

                } // each training item

            } // while

            sw.Stop();
            Console.WriteLine("Time taken:{0}", sw.Elapsed);

            Console.WriteLine("Training ended...\n");

            double[] bestWts = GetWeights();
            return bestWts;
        } // Train



        private void Shuffle(int[] sequence) // instance method
        {
            for (int i = 0; i < sequence.Length; ++i)
            {
                int r = this.rnd.Next(i, sequence.Length);
                int tmp = sequence[r];
                sequence[r] = sequence[i];
                sequence[i] = tmp;
            }
        } // Shuffle

        private double Error(double[][] trainData)
        {
            // average squared error per training item
            double sumSquaredError = 0.0;
            double[] xValues = new double[NumInputNodes]; // first numInput values in trainData
            //double[] tValues = new double[numOutput]; // last numOutput values

            double[] tValuesFileLocal = new double[NumOutputNodes]; // last numOutput values



            // walk thru each training case. looks like (6.9 3.2 5.7 2.3) (0 0 1)
            for (int i = 0; i < trainData.Length; ++i)
            {
                Array.Copy(trainData[i], xValues, NumInputNodes);
                //Array.Copy(trainData[i], numInput, tValues, 0, numOutput); // get target values
                Array.Copy(trainData[i], NumInputNodes + 2, tValuesFileLocal, 0, NumOutputNodes); // get target values



                double[] yValues = this.ComputeOutputs(xValues); // outputs using current weights
                for (int j = 0; j < NumOutputNodes; ++j)
                {
                    //double err = tValues[j] - yValues[j];

                    double err = tValuesFileLocal[j] - yValues[j];
                    ///TODO: change the logic
                    //double err = tValueFile[0][j] - yValues[j];
                    sumSquaredError += err * err;
                }
            }
            return sumSquaredError / trainData.Length;
        } // MeanSquaredError

        private double NewError(float[][] trainData)
        {
            // average squared error per training item
            double sumSquaredError = 0.0;
            double[] xValues = new double[NumInputNodes]; // first numInput values in trainData
            //double[] tValues = new double[numOutput]; // last numOutput values

            double[] tValuesFileLocal = new double[NumOutputNodes]; // last numOutput values



            // walk thru each training case. looks like (6.9 3.2 5.7 2.3) (0 0 1)
            for (int i = 0; i < trainData.Length; ++i)
            {
                Array.Copy(trainData[i], xValues, NumInputNodes);
                //Array.Copy(trainData[i], numInput, tValues, 0, numOutput); // get target values
                Array.Copy(trainData[i], NumInputNodes + 3, tValuesFileLocal, 0, NumOutputNodes); // get target values



                double[] yValues = this.ComputeOutputs(xValues); // outputs using current weights
                for (int j = 0; j < NumOutputNodes; ++j)
                {
                    //double err = tValues[j] - yValues[j];

                    double err = tValuesFileLocal[j] - yValues[j];
                    ///TODO: change the logic
                    //double err = tValueFile[0][j] - yValues[j];
                    sumSquaredError += err * err;
                }
            }
            return sumSquaredError / trainData.Length;
        } // MeanSquaredError


        public double NewAccuracy(float[][] data, double[] rankArray = null)
        {
            // percentage correct using winner-takes all
            int numCorrect = 0;
            int numWrong = 0;
            double[] xValues = new double[NumInputNodes]; // inputs
            double[] tFileValues = new double[NumOutputNodes]; // targets
            double[] yValues; // computed Y

            for (int i = 0; i < data.Length; ++i)
            {
                Array.Copy(data[i], xValues, NumInputNodes); // get x-values
                //Array.Copy(data[i], numInput, tValues, 0, numOutput); // get t-values
                Array.Copy(data[i], NumInputNodes + 3, tFileValues, 0, NumOutputNodes); // get target values

                //Set X values to zero if ranking...
                //TODO: rank array comment
                if (isFeatureSelection)
                {
                    xValues = Helper.SetXValueToZeroByRankCheck(xValues, rankArray, NumInputNodes);
                }


                yValues = this.ComputeOutputs(xValues);

                int maxIndex = Helper.MaxIndex(yValues); // which cell in yValues has largest value?
                int tMaxIndex = Helper.MaxIndex(tFileValues);

                if (maxIndex == tMaxIndex)
                    ++numCorrect;
                else
                    ++numWrong;
            }

            return (numCorrect * 1.0) / (numCorrect + numWrong);
        }


        public double Accuracy(double[][] data, double[] rankArray = null)
        {
            // percentage correct using winner-takes all
            int numCorrect = 0;
            int numWrong = 0;
            double[] xValues = new double[NumInputNodes]; // inputs
            double[] tValues = new double[NumOutputNodes]; // targets
            double[] yValues; // computed Y

            for (int i = 0; i < data.Length; ++i)
            {
                Array.Copy(data[i], xValues, NumInputNodes); // get x-values
                //Array.Copy(data[i], numInput, tValues, 0, numOutput); // get t-values
                Array.Copy(data[i], NumInputNodes + 2, tValues, 0, NumOutputNodes); // get target values

                //Set X values to zero if ranking...
                //TODO: rank array comment
                if (isFeatureSelection)
                {
                    xValues = Helper.SetXValueToZeroByRankCheck(xValues, rankArray, NumInputNodes);
                }


                yValues = this.ComputeOutputs(xValues);

                int maxIndex = Helper.MaxIndex(yValues); // which cell in yValues has largest value?
                int tMaxIndex = Helper.MaxIndex(tValues);

                if (maxIndex == tMaxIndex)
                    ++numCorrect;
                else
                    ++numWrong;
            }
            return (numCorrect * 1.0) / (numCorrect + numWrong);
        }

        ///// <summary>
        ///// Calculates Accuracy and Sets the yOne and YZero
        ///// </summary>
        ///// <param name="data"></param>
        ///// <param name="rankArray"></param>
        ///// <param name="yZero"></param>
        ///// <param name="yOne"></param>
        //public void CalculateAccuracyAndAppendYValMy(double[][] data, double[] rankArray, out List<string> yZero, out List<string> yOne)
        //{
        //    // percentage correct using winner-takes all
        //    int numCorrect = 0;
        //    int numWrong = 0;
        //    double[] xValues = new double[numInput]; // inputs
        //    double[] tValues = new double[numOutput]; // targets
        //    double[] yValues; // computed Y

        //    //List<string> dataList = new List<string>();
        //    int binaryResult = 0;

        //    yZero = new List<string>();
        //    yOne = new List<string>();

        //    var yZeroTemp = new List<string>();
        //    var yOneTemp = new List<string>();


        //    for (int i = 0; i < data.Length; i++)
        //    {
        //        Array.Copy(data[i], xValues, numInput); // get x-values
        //        Array.Copy(data[i], numInput + 2, tValues, 0, numOutput); // get target values from array

        //        //Set X values to zero if ranking...

        //        //TODO: xValues comments
        //        if (isFeatureSelection)
        //        {
        //            xValues = DFTModel.SetXValueToZeroByRankCheck(xValues, rankArray, numInput);
        //        }

        //        yValues = this.ComputeOutputs(xValues);
        //        binaryResult = MaxIndex(yValues); // which cell in yValues has largest value?

        //        //string s = binaryResult.ToString();
        //        string s = string.Empty;
        //        for (int j = 0; j < numInput; j++)
        //        {
        //            s += xValues[j].ToString(); //TODO: for continuous variables this would raise errors
        //        }

        //        if (binaryResult == 0)
        //        {
        //            //dataList.Add(s);
        //            yZeroTemp.Add(s);
        //        }
        //        else
        //        {
        //            yOneTemp.Add(s);
        //        }
        //    }

        //    int k = 0;
        //    int[] outputZero = new int[yZeroTemp.Count];
        //    foreach (string s in yZeroTemp)
        //    {
        //        outputZero[k] = Convert.ToInt32(s, 2);
        //        k++;
        //    }
        //    Array.Sort(outputZero);
        //    var outputZeroDistinct = outputZero.Distinct().ToArray();

        //    k = 0;
        //    int[] outputOne = new int[yOneTemp.Count];
        //    foreach (string s in yOneTemp)
        //    {
        //        outputOne[k] = Convert.ToInt32(s, 2);
        //        k++;
        //    }
        //    Array.Sort(outputOne);
        //    var outputOneDistinct = outputOne.Distinct().ToArray();

        //    for (int i = 0; i < outputZeroDistinct.Count(); i++)
        //    {
        //        yZero.Add(Convert.ToString(outputZeroDistinct[i], 2).PadLeft(numInput, '0'));
        //    }

        //    for (int i = 0; i < outputOneDistinct.Count(); i++)
        //    {
        //        yOne.Add(Convert.ToString(outputOneDistinct[i], 2).PadLeft(numInput, '0'));
        //    }
        //}



        public int GetTotalWeights()
        {
            return this._totalWeights;
        }


        private bool disposed = false;
        protected void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    AllWeights = null;
                    inputs = null;
                    ihWeights = null;
                    hBiases = null;
                    hOutputs = null;
                    hoWeights = null;
                    oBiases = null;
                    outputs = null;
                    //Dispose(true);
                    //GC.SuppressFinalize(this);

                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            //GC.SuppressFinalize(this);
        }




        void INeuralNetwork.Dispose(bool disposing)
        {
            //Dispose(true);
            GC.SuppressFinalize(this);
            //throw new NotImplementedException();
        }
    } // NeuralNetwork


}
