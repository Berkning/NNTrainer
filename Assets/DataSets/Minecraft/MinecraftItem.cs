using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/New Minecraft Item")]
public class MinecraftItem : Minecraft
{
    public bool isTool; //Defined very vaguely
    public bool isEdible; //Defined as anything that is consumable by the player alone (not things like trial keys, which are consumable by the environment)
    public bool isArmor; //All types of armor, even for a nautilus or horse
    public bool isResource; //Defined only as mineable resources
    public bool isPotion;
    public bool isMusicDisc;
    public bool isArmorTrim;
    public bool isSpawnEgg;
    public bool isDye; //Defined only as the actual dyes, not their predecessors
}