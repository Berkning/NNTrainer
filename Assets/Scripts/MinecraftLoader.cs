using System.IO;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class MinecraftLoader : MonoBehaviour
{
    [SerializeField] private string soPath;
    [SerializeField] private string texturesPath;
    [SerializeField, TextArea(10, 100)] private string content;


    void Start()
    {
        string[] textureFiles = Directory.GetFiles(texturesPath);


        for (int i = 0; i < textureFiles.Length; i++)
        {
            if (!textureFiles[i].Contains(".meta")) continue;

            string name = textureFiles[i].Replace(texturesPath, "").Replace(".png.meta", "");

            string guid = File.ReadAllLines(textureFiles[i])[1].Remove(0, 6);

            //Debug.Log(name);
            //Debug.Log(guid);

            string creationPath = soPath + "/" + name + ".asset";

            File.WriteAllText(creationPath, content.Replace("PLACEHOLDERNAME", name).Replace("PLACEHOLDERGUID", guid));
        }
    }
}