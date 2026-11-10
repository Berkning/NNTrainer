using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/New Minecraft Item")]
public class MinecraftItem : Minecraft
{
    public bool isTool;
    public bool isEdible;
    public bool isArmor;
    public bool isResource;
    public bool isPotion;
    public bool isMusicDisc;
    public bool isArmorTrim;
    public bool isSpawnEgg;
    public bool isDye;
}