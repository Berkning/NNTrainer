using System;
using UnityEngine;

[System.Serializable]
public class Layer
{
    public int size;
    private int sizeOfNextLayer;

    public double[] biases;
    public double[,] weights;

    public double[] results;
    public double[] activations;

    public void Initialize(int _sizeOfNextLayer)
    {
        sizeOfNextLayer = _sizeOfNextLayer;

        biases = new double[sizeOfNextLayer];

        //Each neuron in this layer, has as many outgoing
        //connections as there are neurons in the next layer
        weights = new double[size, sizeOfNextLayer];

        results = new double[sizeOfNextLayer];
        activations = new double[sizeOfNextLayer];
    }

    public void Randomize(double weightRange, double biasRange)
    {
        for (int i = 0; i < sizeOfNextLayer; i++)
        {
            biases[i] = UnityEngine.Random.Range((float)-biasRange / 2f, (float)biasRange / 2f);
        }

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < sizeOfNextLayer; j++)
            {
                weights[i, j] = UnityEngine.Random.Range((float)-weightRange / 2f, (float)weightRange / 2f);
            }
        }
    }

    //Input is of length == size
    public ReadOnlySpan<double> Propogate(ReadOnlySpan<double> input, ActivationFunction activation)
    {
        //Clear results array
        for (int i = 0; i < sizeOfNextLayer; i++) results[i] = 0;


        for (int i = 0; i < size; i++)
        {
            for (int w = 0; w < sizeOfNextLayer; w++)
            {
                results[w] += input[i] * weights[i, w];
            }
        }

        for (int i = 0; i < sizeOfNextLayer; i++)
        {
            results[i] += biases[i];

            activations[i] = activation.GetActivation(results[i]);
        }

        return results;
    }
}