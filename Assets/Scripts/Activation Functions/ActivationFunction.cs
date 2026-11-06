using UnityEngine;

[System.Serializable]
public abstract class ActivationFunction : MonoBehaviour
{
    public abstract double GetActivation(double x);
    public abstract double GetGradient(double x);
}