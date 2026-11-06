using System;
using UnityEngine;

public class LeakyReLU : ActivationFunction
{
    [SerializeField] private double alpha;

    public override double GetActivation(double x)
    {
        if (x > 0) return x;

        return alpha * x;
    }

    public override double GetGradient(double x)
    {
        if (x > 0d) return 1d;
        else return alpha;
    }
}