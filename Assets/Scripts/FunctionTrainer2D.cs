using System;
using UnityEngine;
using UnityEngine.UI;

public class FunctionTrainer2D : Trainer
{
    [SerializeField] private RawImage image;
    [SerializeField] private RawImage netImage;
    [SerializeField] private int resolution;

    void Start()
    {
        network.Randomize(1d, 1d);
        DrawImage();
        DrawNetwork();
    }

    private void DrawImage()
    {
        Texture2D texture = new Texture2D(resolution, resolution);

        double halfRes = resolution / 2d;
        double increment = halfRes / resolution;

        for (int x = 0; x < resolution; x++)
        {
            for (int y = 0; y < resolution; y++)
            {
                double xCoord = (x - halfRes + x * increment) / halfRes; //Between -1 -> 1
                double yCoord = (y - halfRes + y * increment) / halfRes; //Between -1 -> 1

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

        double halfRes = resolution / 2d;
        double increment = halfRes / resolution;

        for (int x = 0; x < resolution; x++)
        {
            for (int y = 0; y < resolution; y++)
            {
                double xCoord = (x - halfRes + x * increment) / halfRes; //Between -1 -> 1
                double yCoord = (y - halfRes + y * increment) / halfRes; //Between -1 -> 1

                double z = network.Evaluate(new double[] { xCoord, yCoord })[0]; //Between -1 -> 1
                double t = (z + 1d) / 2d; //Between 0 -> 1

                if (t > 1d) Debug.Log(t);


                texture.SetPixel(x, y, Color.Lerp(Color.black, Color.white, (float)t));
            }
        }

        texture.Apply();
        netImage.texture = texture;
    }



    //Returns a value between -1 and 1
    private double SampleFunction(double x, double y)
    {
        return (Math.Sin(x * 2d) + Math.Cos(y * 2d)) / 2d;
    }
}