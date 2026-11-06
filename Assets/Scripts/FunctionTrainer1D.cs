using System;
using UnityEngine;

public class FunctionTrainer1D : Trainer
{
    [SerializeField] private int resolution;
    [SerializeField] private int dataPointCount;
    [SerializeField] private double width = 10;

    [SerializeField] private LineRenderer graphRenderer;
    [SerializeField] private LineRenderer networkRenderer;

    private (double x, double y)[] dataPoints;

    [SerializeField] private bool train;

    void Start()
    {
        network.Randomize(0.5d, 0.5d);
        DrawGraph();
        DrawNetwork();
        GetData();
    }

    void Update()
    {
        if (train)
        {
            train = false;
            for (int i = 0; i < dataPointCount; i++)
            {
                network.Evaluate(new double[] { dataPoints[i].x });
                CalculateGradients(new double[] { dataPoints[i].y });
            }

            ApplyGradients(dataPointCount);

            DrawNetwork();
        }
    }

    private void GetData()
    {
        dataPoints = new (double, double)[dataPointCount];

        for (int i = 0; i < dataPointCount; i++)
        {
            double x = UnityEngine.Random.Range((float)-width / 2f, (float)width / 2f);
            dataPoints[i] = (x, SampleFunction(x));
        }
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
        return x;
        //return Math.Sin(x);
    }

    private double SampleNetwork(double x)
    {
        return network.Evaluate(new double[] { x })[0];
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        if (Application.isPlaying) for (int i = 0; i < dataPointCount; i++) Gizmos.DrawSphere(new Vector3((float)dataPoints[i].x, (float)dataPoints[i].y, 0f), 0.025f);
    }
}