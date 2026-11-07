using System;
using System.Diagnostics;
using UnityEngine;

public class CrossEntropy : LossFunction
{
    public override double GetLoss(double pred, double target)
    {
        if (pred < 0d) UnityEngine.Debug.Log(pred);
        return -target * Math.Log(pred);
    }

    public override double GetGradient(double pred, double target)
    {
        return pred - target;
    }
}