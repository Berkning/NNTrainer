using UnityEngine;

[System.Serializable]
public abstract class LossFunction : MonoBehaviour
{
    public abstract double GetLoss(double pred, double target);
    public abstract double GetGradient(double pred, double target);
}