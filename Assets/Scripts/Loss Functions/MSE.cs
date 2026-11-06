using System;

public class MSE : LossFunction
{
    public override double GetGradient(double pred, double target)
    {
        double diff = pred - target;
        return diff * diff / 2d;
    }

    public override double GetLoss(double pred, double target)
    {
        return pred - target;
    }
}