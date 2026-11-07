using System;
using UnityEngine;
using UnityEngine.UI;

public class FunctionTrainer2D : Trainer
{
    [SerializeField] private RawImage image;
    [SerializeField] private RawImage netImage;
    [SerializeField] private int resolution;
    [SerializeField] private int dataPointCount;
    [SerializeField] private bool train;
    [SerializeField] private int trainingIterations;
    [Space(10), SerializeField] private float gizmoSpread;

    private (double x, double y, double z)[] dataPoints;

    void Start()
    {
        network.Randomize(1d, 1d);
        DrawImage();
        DrawNetwork();
        GetData();
    }

    void Update()
    {
        if (train)
        {
            train = false;

            for (int m = 0; m < trainingIterations; m++)
            {
                for (int i = 0; i < dataPointCount; i++)
                {
                    network.Evaluate(new double[] { dataPoints[i].x, dataPoints[i].y });
                    CalculateGradients(new double[] { dataPoints[i].z });
                }

                ApplyGradients(dataPointCount);

                DrawNetwork();
            }
        }
    }



    private void DrawImage()
    {
        Texture2D texture = new Texture2D(resolution, resolution);

        double increment = 2d / resolution;

        for (int x = 0; x < resolution; x++)
        {
            for (int y = 0; y < resolution; y++)
            {
                double xCoord = x * increment - 1d; //Between -1 -> 1
                double yCoord = y * increment - 1d; //Between -1 -> 1

                double z = SampleFunction(xCoord, yCoord); //Between -1 -> 1
                double t = (z + 1d) / 2d; //Between 0 -> 1

                if (t > 1d) Debug.Log(t);


                texture.SetPixel(x, y, Color.Lerp(Color.black, Color.white, (float)t));
            }
        }

        texture.Apply();
        image.texture = texture;
    }

    private void DrawNetwork()
    {
        Texture2D texture = new Texture2D(resolution, resolution);

        double increment = 2d / resolution;

        for (int x = 0; x < resolution; x++)
        {
            for (int y = 0; y < resolution; y++)
            {
                double xCoord = x * increment - 1d; //Between -1 -> 1
                double yCoord = y * increment - 1d; //Between -1 -> 1

                double z = network.Evaluate(new double[] { xCoord, yCoord })[0]; //Between -1 -> 1
                double t = (z + 1d) / 2d; //Between 0 -> 1

                if (t > 1d) Debug.Log(t);


                texture.SetPixel(x, y, Color.Lerp(Color.black, Color.white, (float)t));
            }
        }

        texture.Apply();
        netImage.texture = texture;
    }

    private void GetData()
    {
        dataPoints = new (double, double, double)[dataPointCount];

        for (int i = 0; i < dataPointCount; i++)
        {
            double x = UnityEngine.Random.Range(-1f, 1f);
            double y = UnityEngine.Random.Range(-1f, 1f);
            dataPoints[i] = (x, y, SampleFunction(x, y));
        }
    }



    //Returns a value between -1 and 1
    private double SampleFunction(double x, double y)
    {
        if (x > 0) return x;
        return -1d;
        //return (Math.Sin(x * 10d) + Math.Cos(y * 10d)) / 2d;
    }


    void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            for (int i = 0; i < dataPointCount; i++)
            {
                double t = (dataPoints[i].z + 1d) / 2d; //Between 0 -> 1
                Gizmos.color = Color.Lerp(Color.black, Color.white, (float)t);

                Vector3 position = new Vector3((float)dataPoints[i].x, (float)dataPoints[i].y, 0f) * gizmoSpread;
                position += image.transform.position;

                Gizmos.DrawSphere(position, 0.025f);
            }
        }
    }
}