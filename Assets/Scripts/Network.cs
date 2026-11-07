using System;
using UnityEngine;

public class Network : MonoBehaviour //TODO: Give each layer their own activation function
{
    public Layer[] layers;
    public int sizeOfOutputLayer;

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

    public ReadOnlySpan<double> Evaluate(double[] input)
    {
        layers[0].input = input;
        layers[0].Propogate();

        for (int i = 1; i < layers.Length; i++)
        {
            layers[i].input = layers[i - 1].activations; //TODO: Make this much faster through spans - or layers could reference each other sequentally
            layers[i].Propogate();
        }

        return layers[layers.Length - 1].activations;
    }
}