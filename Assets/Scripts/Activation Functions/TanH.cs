using System;

public class TanH : ActivationFunction
{
    public override double GetActivation(double x)
    {
        return Math.Tanh(x);
    }

    public override double GetGradient(double x)
    {
        double tanh = Math.Tanh(x);
        return 1d - tanh * tanh;
    }
}
