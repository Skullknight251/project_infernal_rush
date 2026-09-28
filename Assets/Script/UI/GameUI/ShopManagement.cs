using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManagement : MonoBehaviour
{
    public static ShopManagement Instance { get; private set; }

    [SerializeField] private Transform weaponSlotContainer;
    [SerializeField] private Transform spellSlotContainer;

    [SerializeField] private GameObject itemWeaponSlotTemplate;
    [SerializeField] private GameObject itemSpellSlotTemplate;
    [SerializeField] private TextMeshProUGUI soulsAmountText;
    [SerializeField] private Button QuitButton;

    [SerializeField] private GameObject exchangeConfirmWindow;
    [SerializeField] private GameObject exchangeFailPopup;
    [SerializeField] private GameObject mainMenuContainer;
    private List<WeaponSO> weaponSOList => GameManager.Instance.weaponSOList;
    private List<SpellSO> spellSOList => GameManager.Instance.spellSOList;
    private int soulsAmount;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        QuitButton.onClick.AddListener(ReturnToMainMenu);
    }
    private void OnEnable()
    {
        UpdateSoulsAmount();
        RefreshShop();
    }
    public void RefreshShop()
    {
        ClearContainer(weaponSlotContainer);
        ClearContainer(spellSlotContainer);

        List<string> purchasedItemList = GameManager.Instance.LoadPurchasedItems();

        if (purchasedItemList == null)
            purchasedItemList = new List<string>();

        foreach (WeaponSO weaponSO in weaponSOList)
        {
            if (weaponSO == null)
                continue;

            if (!purchasedItemList.Contains(weaponSO.itemID))
            {
                CreateWeaponItemSlot(
                    weaponSO,
                    weaponSlotContainer
                );
            }
        }

        foreach (SpellSO spellSO in spellSOList)
        {
            if (spellSO == null)
                continue;

            if (!purchasedItemList.Contains(spellSO.itemID))
            {
                CreateSpellItemSlot(
                    spellSO,
                    spellSlotContainer
                );
            }
        }
    }

    private void CreateWeaponItemSlot(ItemSO itemSO, Transform parent)
    {
        GameObject itemSlot =
            Instantiate(itemWeaponSlotTemplate, parent);

        itemSlot.SetActive(true);

        ExchangeItemSlotManager slot =
            itemSlot.GetComponent<ExchangeItemSlotManager>();

        if (slot == null)
            return;

        slot.Setup(itemSO);
    }
    private void CreateSpellItemSlot(ItemSO itemSO, Transform parent)
    {
        GameObject itemSlot =
            Instantiate(itemSpellSlotTemplate, parent);

        itemSlot.SetActive(true);

        ExchangeItemSlotManager slot =
            itemSlot.GetComponent<ExchangeItemSlotManager>();

        if (slot == null)
            return;

        slot.Setup(itemSO);
    }
    private void ClearContainer(Transform container)
    {
        if (container == null)
            return;

        for (int i = container.childCount - 1; i >= 0; i--)
        {
            Destroy(container.GetChild(i).gameObject);
        }
    }

    public void ExchangeAction(ItemSO itemSO)
    {
        if (itemSO == null)
            return;

        int exchangeAmount = itemSO.GetPrice();
        Debug.Log(exchangeAmount + " " + soulsAmount);
        
        if (soulsAmount < exchangeAmount)
        {
            exchangeFailPopup.SetActive(true);
            Debug.Log(soulsAmount - exchangeAmount);
            return;
        }
        

        ConfirmExchangeManager confirmExchange =
            exchangeConfirmWindow.GetComponent<ConfirmExchangeManager>();

        if (confirmExchange == null)
            return;
        
        confirmExchange.Setup(itemSO);
    }

    public void UpdateSoulsAmount()
    {
        soulsAmount =
            GameManager.Instance.LoadCurrentSouls();

        soulsAmountText.text =
            soulsAmount.ToString();
    }

    public int GetSoulsAmount()
    {
        return soulsAmount;
    }

    public void DecreaseSouls(int amount)
    {
        GameManager.Instance.UpdateSouls(-amount);
        UpdateSoulsAmount();
    }

    public void ReturnToMainMenu()
    {
        mainMenuContainer.GetComponent<MainMenuManager>().ShowMenu();
        gameObject.SetActive(false);
    }
}