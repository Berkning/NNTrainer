using System;

public class BinaryCrossEntropy : LossFunction
{
    public override double GetLoss(double pred, double target)
    {
        return -target * Math.Log(pred) - (1d - target) * Math.Log(1 - pred);
    }

    public override double GetGradient(double pred, double target)
    {
        return (pred - target) / (pred - pred * pred);
    }
}