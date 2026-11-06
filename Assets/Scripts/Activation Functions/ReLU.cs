using System;

public class ReLU : ActivationFunction
{
    public override double GetActivation(double x)
    {
        return Math.Max(0d, x);
    }

    public override double GetGradient(double x)
    {
        if (x > 0d) return 1d;
        else return 0d;
    }
}