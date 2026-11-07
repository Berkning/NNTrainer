using UnityEngine;

public class SGD : Optimizer
{
    [SerializeField] private double learningRate;

    public override void ApplyGradients(Network network, int batchSize)
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
}