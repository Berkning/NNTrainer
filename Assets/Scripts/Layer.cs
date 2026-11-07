using System;
using UnityEngine;

[System.Serializable]
public class Layer
{
    public int size;
    public ActivationFunction activationFunction;
    [HideInInspector] public int sizeOfNextLayer;

    [HideInInspector] public double[] input;

    [HideInInspector] public double[] biases;
    [HideInInspector] public double[,] weights;

    [HideInInspector] public Gradient[,] weightGradients;
    [HideInInspector] public Gradient[] biasGradients;

    [HideInInspector] public double[] results;
    [HideInInspector] public double[] activations; //This holds the same values as the input for the next layer

    public void Initialize(int _sizeOfNextLayer)
    {
        sizeOfNextLayer = _sizeOfNextLayer;

        biases = new double[sizeOfNextLayer];
        biasGradients = new Gradient[sizeOfNextLayer];

        //Each neuron in this layer, has as many outgoing
        //connections as there are neurons in the next layer
        weights = new double[size, sizeOfNextLayer];
        weightGradients = new Gradient[size, sizeOfNextLayer];

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
    public void Propogate()
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

            activations[i] = activationFunction.GetActivation(results[i]);
        }
    }
}

public struct Gradient
{
    public double gradient;
    public double velocity; //Stores first moment for Adam
    public double secondMoment;
}