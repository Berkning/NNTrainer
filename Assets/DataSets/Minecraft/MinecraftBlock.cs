using UnityEngine;

[CreateAssetMenu(fileName = "Block", menuName = "ScriptableObjects/New Minecraft Block")]
public class MinecraftBlock : Minecraft
{
    public bool isFromOverworld;
    public bool isFromNether;
    public bool isFromEnd;
}