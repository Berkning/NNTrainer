using System;
using UnityEngine;

public class Network : MonoBehaviour
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

    public ReadOnlySpan<double> Evaluate(ReadOnlySpan<double> input)
    {
        for (int i = 0; i < layers.Length; i++)
        {
            input = layers[i].Propogate(input, activationFunction);
        }

        return input;
    }
}