using UnityEngine;

public class MinecraftTrainer : Trainer
{
    //TODO: Try binary cross entropy as loss function
    [SerializeField] private MinecraftItem[] minecraftItems;
    [SerializeField] private DataSet dataSet;
    [SerializeField] private bool train;
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