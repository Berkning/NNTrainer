using Unity.VisualScripting;
using UnityEngine;

public class MinecraftLoader : MonoBehaviour
{
    [SerializeField] private Minecraft[] objects;
    [SerializeField] private Texture[] textures;


    void Start()
    {
        for (int i = 0; i < objects.Length; i++)
        {
            objects[i].name = textures[i].name;
            objects[i].texture = textures[i];
        }
    }
}