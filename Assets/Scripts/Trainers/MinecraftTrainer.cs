using UnityEngine;

public class MinecraftTrainer : Trainer
{
    //TODO: Try binary cross entropy as loss function
    [SerializeField] private MinecraftItem[] minecraftItems;
    [SerializeField] private DataSet dataSet;
    [SerializeField] private bool useBlackAndWhite = true;


    void Start()
    {
        network.Randomize(1d, 1d);
        GetData();
    }

    private void GetData()
    {

        if (useBlackAndWhite)
        {
            double[] data = new double[minecraftItems.Length * 257]; //256 black/white values per texture + 1 output value per texture

            for (int i = 0; i < minecraftItems.Length; i++)
            {
                double output = minecraftItems[i].isArmor ? 1d : 0d;

                for (int x = 0; x < 16; x++)
                {
                    for (int y = 0; y < 16; y++)
                    {
                        Texture2D texture2D = (Texture2D)minecraftItems[i].texture;
                        Color color = texture2D.GetPixel(x, y);

                        Debug.Log(color);
                    }
                }
            }
        }
    }
}