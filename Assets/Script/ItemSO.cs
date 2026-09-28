using UnityEngine;
[CreateAssetMenu()]
public class ItemSO : ScriptableObject
{
    public string itemID;
    public GameObject item;
    public Sprite sprite;
    [SerializeField]private int price;

    public int GetPrice() { return price; }
}
