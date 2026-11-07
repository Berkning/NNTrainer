using UnityEngine;

public class MomentumSGD : Optimizer
{
    [SerializeField] private double learningRate;
    [SerializeField] private double momentumCoefficient = 0.9d;

    public override void ApplyGradients(Network network, int batchSize)
    {
        for (int l = 0; l < network.layers.Length; l++)
        {
            Layer layer = network.layers[l];

            for (int b = 0; b < layer.biases.Length; b++)
            {
                layer.biasGradients[b].velocity = momentumCoefficient * layer.biasGradients[b].velocity + layer.biasGradients[b].gradient;

                layer.biases[b] -= learningRate * (layer.biasGradients[b].velocity / batchSize);


                layer.biasGradients[b].gradient = 0d;
            }

            for (int i = 0; i < layer.size; i++)
            {
                for (int w = 0; w < layer.sizeOfNextLayer; w++)
                {
                    layer.weightGradients[i, w].velocity = momentumCoefficient * layer.weightGradients[i, w].velocity + layer.weightGradients[i, w].gradient;

                    layer.weights[i, w] -= learningRate * (layer.weightGradients[i, w].velocity / batchSize);


                    layer.weightGradients[i, w].gradient = 0d;
                }
            }
        }
    }
}
