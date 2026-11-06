using System;

public class Linear : ActivationFunction
{
    public override double GetActivation(double x)
    {
        return x;
    }

    public override double GetGradient(double x)
    {
        return 1d;
    }
}
