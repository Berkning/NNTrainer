using UnityEngine;

public interface ActivationFunction
{
    public double GetActivation(double x);
    public double GetGradient(double x);
}