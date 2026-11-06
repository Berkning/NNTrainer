using System;
using UnityEngine;

public class Network : MonoBehaviour //TODO: Give each layer their own activation function
{
    public Layer[] layers;
    public int sizeOfOutputLayer;
    public ActivationFunction activationFunction;

    [Header("Testing")]
    [SerializeField] private double[] testInput;
    [SerializeField] private bool evaluate;

    void Awake()
    {
        for (int i = 0; i < layers.Length - 1; i++)
        {
            layers[i].Initialize(layers[i + 1].size);
        }

        layers[layers.Length - 1].Initialize(sizeOfOutputLayer);
    }

    void Update()
    {
        if (evaluate)
        {
            evaluate = false;
            Debug.Log(Evaluate(testInput)[0]);
        }
    }

    public void Randomize(double weightRange, double biasRange)
    {
        for (int i = 0; i < layers.Length; i++)
        {
            layers[i].Randomize(weightRange, biasRange);
        }
    }

    public ReadOnlySpan<double> Evaluate(ReadOnlySpan<double> input)
    {
        for (int i = 0; i < layers.Length; i++)
        {
            input = layers[i].Propogate(input, activationFunction);
        }

        return input;
    }
}