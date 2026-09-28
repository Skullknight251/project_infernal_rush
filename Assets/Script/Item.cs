using UnityEngine;

public class Item : MonoBehaviour
{
    public enum ItemType
    {
        Weapon,
        Spell
    }
    public ItemSO itemSO;
    private ItemType itemType;
    private float price;
}
