using System;
using UnityEngine;

public class FunctionTrainer1D : MonoBehaviour
{
    [SerializeField] private Network network;
    [SerializeField] private int resolution;
    [SerializeField] private double width = 10;

    [SerializeField] private LineRenderer graphRenderer;
    [SerializeField] private LineRenderer networkRenderer;

    void Start()
    {
        DrawGraph();
        DrawNetwork();
    }

    private void DrawGraph()
    {
        Vector3[] points = new Vector3[resolution];

        for (int i = 0; i < resolution; i++)
        {
            double x = width / resolution * i - width / 2d;

            points[i] = new Vector3((float)x, (float)SampleFunction(x), 0f);
        }

        graphRenderer.positionCount = resolution;
        graphRenderer.SetPositions(points);
    }

    private void DrawNetwork()
    {
        Vector3[] points = new Vector3[resolution];

        for (int i = 0; i < resolution; i++)
        {
            double x = width / resolution * i - width / 2d;

            points[i] = new Vector3((float)x, (float)SampleNetwork(x), 0f);
        }

        networkRenderer.positionCount = resolution;
        networkRenderer.SetPositions(points);
    }

    private double SampleFunction(double x)
    {
        return Math.Sin(x);
    }

    private double SampleNetwork(double x)
    {
        return network.Evaluate(new double[] { x })[0];
    }
}