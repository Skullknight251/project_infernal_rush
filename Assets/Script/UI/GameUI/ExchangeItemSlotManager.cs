using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExchangeItemSlotManager : MonoBehaviour
{
    public ItemSO itemSO;

    [SerializeField] private Image itemSprite;
    [SerializeField] private TextMeshProUGUI price;
    [SerializeField] private Button buyButton;
    [SerializeField] private TextMeshProUGUI itemName;

    public void Setup(ItemSO itemSO)
    {
        this.itemSO = itemSO;

        if (itemSO == null)
            return;

        itemSprite.sprite = itemSO.sprite;
        itemSprite.enabled = itemSO.sprite != null;

        price.text = itemSO.GetPrice().ToString();

        if (itemSO is WeaponSO weaponSO)
        {
            itemName.text = weaponSO.weaponObject != null ? weaponSO.weaponObject.name : itemSO.name;
        }
        else if (itemSO is SpellSO spellSO)
        {
            itemName.text = spellSO.spellPrefab != null ? spellSO.spellPrefab.name : itemSO.name;
        }
        else
        {
            itemName.text = itemSO.name; 
        }

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(OnBuyButtonClicked);
    }

    private void OnBuyButtonClicked()
    {
        if (itemSO == null)
            return;

        ShopManagement.Instance.ExchangeAction(itemSO);
    }
}