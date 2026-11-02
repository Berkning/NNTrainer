using System;

public class ReLU : ActivationFunction
{
    public double GetActivation(double x)
    {
        return Math.Max(0d, x);
    }

    public double GetGradient(double x)
    {
        if (x > 0d) return 1d;
        else return 0d;
    }
}