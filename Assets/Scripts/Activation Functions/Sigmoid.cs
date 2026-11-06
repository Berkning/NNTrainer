using System;

public class Sigmoid : ActivationFunction
{
    public override double GetActivation(double x)
    {
        return 1d / (1d + Math.Pow(Math.E, -x));
    }

    public override double GetGradient(double x)
    {
        double s = GetActivation(x);
        return s * (1d - s);
    }
}
