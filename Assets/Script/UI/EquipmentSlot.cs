using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlot : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private GameObject checkSign;
    public TextMeshProUGUI itemName;
    public Image itemSprite;
    private string itemIndex;
    public bool isEquipped;
    public void Setup(string index)
    {
        checkSign.SetActive(false);
        itemIndex = index;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClick);
        checkEquip();
    }

    public void checkEquip()
    {
        
        isEquipped = false;
        checkSign.SetActive(false);
        
        foreach (ItemSO itemSO in EquipmentManagement.Instance.equipItemList)
        {
            if (itemIndex == itemSO.itemID)
            {
                isEquipped = true;
                checkSign.SetActive(true);
                return;
            }
            
                
        }
        
    }

    private void OnClick() { 
        if (isEquipped) 
        { 
            EquipmentManagement.Instance.UnselectItem(itemIndex); 
        } else 
        { 
            EquipmentManagement.Instance.SelectItem(itemIndex); 
        } 
        checkEquip(); 
    }

    public string GetItemID()
    {
        return itemIndex;
    }
}
