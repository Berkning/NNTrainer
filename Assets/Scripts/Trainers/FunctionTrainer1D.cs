using System;
using System.Diagnostics;
using UnityEngine;

public class FunctionTrainer1D : Trainer
{
    [Space(10), SerializeField] private int resolution;
    [SerializeField] private int dataPointCount;
    [SerializeField] private double width = 10;

    [SerializeField] private LineRenderer graphRenderer;
    [SerializeField] private LineRenderer networkRenderer;

    private (double x, double y)[] dataPoints;

    [SerializeField] private bool train;
    [SerializeField] private int trainingIterations;
    [SerializeField] private bool bench;
    [SerializeField] private int benchIterations;

    void Start()
    {
        network.Randomize(0.5d, 0.5d);
        DrawGraph();
        VisualizeNetwork();
        GetData();
    }

    void Update()
    {
        if (train)
        {
            train = false;

            double[] data = new double[dataPointCount * 2];

            for (int i = 0; i < dataPointCount; i++)
            {
                data[i * 2] = dataPoints[i].x;
                data[i * 2 + 1] = dataPoints[i].y;
            }

            DataSet dataSet = new DataSet(data, 1, 1);

            StartTraining(dataSet);
        }

        if (bench)
        {
            bench = false;

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            for (int m = 0; m < benchIterations; m++)
            {
                for (int i = 0; i < dataPointCount; i++)
                {
                    network.Evaluate(new double[] { dataPoints[i].x });
                }
            }

            stopwatch.Stop();
            UnityEngine.Debug.Log("Inference took " + stopwatch.ElapsedMilliseconds + "ms");


            stopwatch.Restart();

            for (int m = 0; m < benchIterations; m++)
            {
                for (int i = 0; i < dataPointCount; i++)
                {
                    CalculateGradients(new double[] { dataPoints[i].y });
                }
            }

            stopwatch.Stop();
            UnityEngine.Debug.Log("Gradients took " + stopwatch.ElapsedMilliseconds + "ms");
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

    protected override void VisualizeNetwork()
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
        return Math.Sin(x) * x * x / 20d;
        return 1d / (1d + Math.Pow(Math.E, -x)) * Math.Sin(x * 5d);
        return Math.Sqrt(1d / (1d + Math.Pow(Math.E, -x)) * Math.Sin(x * 5d) + 1d) * Math.Sqrt(2d) - 1d;
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