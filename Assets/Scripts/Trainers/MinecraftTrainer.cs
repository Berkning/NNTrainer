using System;
using TMPro;
using UnityEngine;

public class MinecraftTrainer : Trainer
{
    //TODO: Try binary cross entropy as loss function
    [SerializeField] private MinecraftItem[] minecraftItems;
    [Space(5), SerializeField] private TMP_Text toolText;
    [SerializeField] private TMP_Text edibleText;
    [SerializeField] private TMP_Text armorText;
    [SerializeField] private TMP_Text resourceText;
    [SerializeField] private TMP_Text potionText;
    [SerializeField] private TMP_Text musicDiscText;
    [SerializeField] private TMP_Text armorTrimText;
    [SerializeField] private TMP_Text spawnEggText;
    [SerializeField] private TMP_Text dyeText;
    [Space(10), SerializeField] private DataSet dataSet;
    [Space(5), SerializeField] private Texture2D inputTexture;
    [SerializeField] private bool evaluateInput;
    [SerializeField] private bool train;
    [SerializeField] private bool listWrongs;
    [SerializeField] private bool useBlackAndWhite = true;


    void Start()
    {
        network.Randomize(1d, 1d);
        GetData();
    }

    void Update()
    {
        if (train)
        {
            train = false;

            StartTraining(dataSet);
        }

        if (listWrongs)
        {
            listWrongs = false;
            ListWrongs();
        }

        if (evaluateInput)
        {
            evaluateInput = false;
            ShowNetworkOutput(inputTexture);
        }
    }

    private void ListWrongs()
    {
        double[] input = new double[256];

        for (int i = 0; i < minecraftItems.Length; i++)
        {
            for (int x = 0; x < 16; x++)
            {
                for (int y = 0; y < 16; y++)
                {
                    Texture2D texture2D = (Texture2D)minecraftItems[i].texture;
                    Color color = texture2D.GetPixel(x, y);

                    double grayScale = color.grayscale;

                    if (color.a == 0d) grayScale = -1d;

                    int pixelIndex = x * 16 + y;

                    input[pixelIndex] = grayScale;
                }
            }

            ReadOnlySpan<double> output = network.Evaluate(input);

            if (Math.Abs(output[0] - (minecraftItems[i].isDye ? 1d : 0d)) > 0.5d) Debug.Log(minecraftItems[i].name);
            if (Math.Abs(output[1] - (minecraftItems[i].isSpawnEgg ? 1d : 0d)) > 0.5d) Debug.Log(minecraftItems[i].name);
            if (Math.Abs(output[2] - (minecraftItems[i].isArmorTrim ? 1d : 0d)) > 0.5d) Debug.Log(minecraftItems[i].name);
            if (Math.Abs(output[3] - (minecraftItems[i].isMusicDisc ? 1d : 0d)) > 0.5d) Debug.Log(minecraftItems[i].name);
            if (Math.Abs(output[4] - (minecraftItems[i].isPotion ? 1d : 0d)) > 0.5d) Debug.Log(minecraftItems[i].name);
            if (Math.Abs(output[5] - (minecraftItems[i].isResource ? 1d : 0d)) > 0.5d) Debug.Log(minecraftItems[i].name);
            if (Math.Abs(output[6] - (minecraftItems[i].isArmor ? 1d : 0d)) > 0.5d) Debug.Log(minecraftItems[i].name);
            if (Math.Abs(output[7] - (minecraftItems[i].isEdible ? 1d : 0d)) > 0.5d) Debug.Log(minecraftItems[i].name);
            if (Math.Abs(output[8] - (minecraftItems[i].isTool ? 1d : 0d)) > 0.5d) Debug.Log(minecraftItems[i].name);
        }
    }

    private void ShowNetworkOutput(Texture2D texture)
    {
        double[] input = new double[256];

        for (int x = 0; x < 16; x++)
        {
            for (int y = 0; y < 16; y++)
            {
                Color color = texture.GetPixel(x, y);

                double grayScale = color.grayscale;

                if (color.a == 0d) grayScale = -1d;


                int pixelIndex = x * 16 + y;

                input[pixelIndex] = grayScale;
            }
        }

        ReadOnlySpan<double> output = network.Evaluate(input);

        dyeText.text = output[0].ToString("0.###");
        spawnEggText.text = output[1].ToString("0.###");
        armorTrimText.text = output[2].ToString("0.###");
        musicDiscText.text = output[3].ToString("0.###");
        potionText.text = output[4].ToString("0.###");
        resourceText.text = output[5].ToString("0.###");
        armorText.text = output[6].ToString("0.###");
        edibleText.text = output[7].ToString("0.###");
        toolText.text = output[8].ToString("0.###");
    }


    private void GetData()
    {
        if (useBlackAndWhite)
        {
            double[] data = new double[minecraftItems.Length * 265]; //256 black/white values per texture + 1 output value per texture

            for (int i = 0; i < minecraftItems.Length; i++)
            {
                int indexOffset = i * 265;


                for (int x = 0; x < 16; x++)
                {
                    for (int y = 0; y < 16; y++)
                    {
                        Texture2D texture2D = (Texture2D)minecraftItems[i].texture;
                        Color color = texture2D.GetPixel(x, y);

                        double grayScale = color.grayscale;

                        if (color.a == 0d) grayScale = -1d;

                        int pixelIndex = x * 16 + y;

                        data[indexOffset + pixelIndex] = grayScale;
                    }
                }


                data[indexOffset + 256] = minecraftItems[i].isDye ? 1d : 0d;
                data[indexOffset + 257] = minecraftItems[i].isSpawnEgg ? 1d : 0d;
                data[indexOffset + 258] = minecraftItems[i].isArmorTrim ? 1d : 0d;
                data[indexOffset + 259] = minecraftItems[i].isMusicDisc ? 1d : 0d;
                data[indexOffset + 260] = minecraftItems[i].isPotion ? 1d : 0d;
                data[indexOffset + 261] = minecraftItems[i].isResource ? 1d : 0d;
                data[indexOffset + 262] = minecraftItems[i].isArmor ? 1d : 0d;
                data[indexOffset + 263] = minecraftItems[i].isEdible ? 1d : 0d;
                data[indexOffset + 264] = minecraftItems[i].isTool ? 1d : 0d;
            }


            dataSet = new DataSet(data, 256, 9, validationPercent);
        }
    }
}