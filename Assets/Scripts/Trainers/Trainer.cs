using System;
using UnityEngine;

public class Trainer : MonoBehaviour
{
    [SerializeField] protected Network network;
    [SerializeField] private double learningRate;
    [SerializeField] private LossFunction lossFunction;

    public void ApplyGradients(int batchSize)
    {
        for (int l = 0; l < network.layers.Length; l++)
        {
            Layer layer = network.layers[l];

            for (int b = 0; b < layer.biases.Length; b++)
            {
                layer.biases[b] -= learningRate * (layer.biasGradients[b] / batchSize);

                layer.biasGradients[b] = 0d;
            }

            for (int i = 0; i < layer.size; i++)
            {
                for (int w = 0; w < layer.sizeOfNextLayer; w++)
                {
                    layer.weights[i, w] -= learningRate * (layer.weightGradients[i, w] / batchSize);

                    layer.weightGradients[i, w] = 0d;
                }
            }
        }
    }

    public void CalculateGradients(double[] targets)
    {
        double loss = 0d;
        Span<double> errorDelta = stackalloc double[targets.Length]; //TODO: Don't allocate at all, just store errorDeltas in layer
        errorDelta.Clear();

        int lastLayerIndex = network.layers.Length - 1;
        Layer lastLayer = network.layers[lastLayerIndex];

        for (int i = 0; i < lastLayer.sizeOfNextLayer; i++)
        {
            loss += lossFunction.GetLoss(lastLayer.activations[i], targets[i]);

            //How much does the loss change with respect to the pre-activation values of each neuron in the layer
            errorDelta[i] = lossFunction.GetGradient(lastLayer.activations[i], targets[i]) * lastLayer.activationFunction.GetGradient(lastLayer.results[i]);
        }

        //TODO: Display loss
        //Debug.Log(loss);

        PropogateError(errorDelta, lastLayerIndex);
    }

    //The layer at layerIndex-1 holds the activations for this layer, but the layer at layerIndex hold the weights and biases connecting this layer to the next (The output at first call)
    private void PropogateError(Span<double> lastErrorDelta, int layerIndex)
    {
        Layer layer = network.layers[layerIndex];

        Span<double> errorDelta = stackalloc double[layer.size]; //TODO: Don't allocate at all, just store errorDeltas in layer
        errorDelta.Clear();

        for (int i = 0; i < layer.size; i++)
        {
            for (int w = 0; w < layer.sizeOfNextLayer; w++)
            {
                layer.weightGradients[i, w] += lastErrorDelta[w] * layer.input[i];

                errorDelta[i] += layer.weights[i, w] * lastErrorDelta[w];
            }

            if (layerIndex > 0) errorDelta[i] *= network.layers[layerIndex - 1].activationFunction.GetGradient(network.layers[layerIndex - 1].results[i]);
        }

        for (int b = 0; b < layer.sizeOfNextLayer; b++)
        {
            layer.biasGradients[b] += lastErrorDelta[b];
        }


        if (layerIndex == 0) return;

        PropogateError(errorDelta, layerIndex - 1);
    }
}