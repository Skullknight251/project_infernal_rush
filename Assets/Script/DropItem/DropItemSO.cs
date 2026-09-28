using UnityEngine;


[CreateAssetMenu(menuName = "Item/Drop Item")]
public class DropItemSO : ScriptableObject
{
    [SerializeField] public string name;
    [SerializeField] public int quantity;
    [SerializeField] public GameObject prefab;
}
