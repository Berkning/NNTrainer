using System;

public class CrossEntropy : LossFunction
{
    public override double GetLoss(double pred, double target)
    {
        return -target * Math.Log(pred);
    }

    public override double GetGradient(double pred, double target)
    {
        return pred - target;
    }
}