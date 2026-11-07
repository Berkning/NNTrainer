using System;
using UnityEngine;

public class Adam : Optimizer
{
    [SerializeField] private double learningRate;
    [SerializeField] private double firstMomentCoefficient = 0.9d;
    [SerializeField] private double secondMomentCoefficient = 0.999d;
    [SerializeField] private double epsilon = 0.00000001d;
    private int stepCount;

    public override void ApplyGradients(Network network, int batchSize)
    {
        stepCount++;

        double firstCorrection = 1d - Math.Pow(firstMomentCoefficient, stepCount);
        double secondCorrection = 1d - Math.Pow(secondMomentCoefficient, stepCount);

        for (int l = 0; l < network.layers.Length; l++)
        {
            Layer layer = network.layers[l];

            for (int b = 0; b < layer.biases.Length; b++)
            {
                double gradient = layer.biasGradients[b].gradient / batchSize;

                layer.biasGradients[b].velocity = firstMomentCoefficient * layer.biasGradients[b].velocity + (1d - firstMomentCoefficient) * gradient;
                layer.biasGradients[b].secondMoment = secondMomentCoefficient * layer.biasGradients[b].secondMoment + (1d - secondMomentCoefficient) * gradient * gradient;


                double correctedFirstMoment = layer.biasGradients[b].velocity / firstCorrection;
                double correctedSecondMoment = layer.biasGradients[b].secondMoment / secondCorrection;

                layer.biases[b] -= learningRate * (correctedFirstMoment / (Math.Sqrt(correctedSecondMoment) + epsilon));


                layer.biasGradients[b].gradient = 0d;
            }

            for (int i = 0; i < layer.size; i++)
            {
                for (int w = 0; w < layer.sizeOfNextLayer; w++)
                {
                    double gradient = layer.weightGradients[i, w].gradient / batchSize;

                    layer.weightGradients[i, w].velocity = firstMomentCoefficient * layer.weightGradients[i, w].velocity + (1d - firstMomentCoefficient) * gradient;
                    layer.weightGradients[i, w].secondMoment = secondMomentCoefficient * layer.weightGradients[i, w].secondMoment + (1d - secondMomentCoefficient) * gradient * gradient;


                    double correctedFirstMoment = layer.weightGradients[i, w].velocity / firstCorrection;
                    double correctedSecondMoment = layer.weightGradients[i, w].secondMoment / secondCorrection;

                    layer.weights[i, w] -= learningRate * (correctedFirstMoment / (Math.Sqrt(correctedSecondMoment) + epsilon));


                    layer.weightGradients[i, w].gradient = 0d;
                }
            }
        }
    }
}