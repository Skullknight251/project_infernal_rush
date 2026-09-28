using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmExchangeManager : MonoBehaviour
{
    [SerializeField] private Button quitButton;
    [SerializeField] private Button confirmButton;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private TextMeshProUGUI newSoulAmountText;

    private ItemSO itemExchangeSO;
    private int price;

    private void Awake()
    {
        quitButton.onClick.AddListener(QuitApplication);
        confirmButton.onClick.AddListener(ConfirmExchangeAction);
    }

    public void Setup(ItemSO itemSO)
    {
        itemExchangeSO = itemSO;

        if (itemExchangeSO == null)
            return;

        price = itemExchangeSO.GetPrice();

        priceText.text = price.ToString();

        int currentSouls = ShopManagement.Instance.GetSoulsAmount();

        newSoulAmountText.text =
            (currentSouls - price).ToString();

        gameObject.SetActive(true);
    }

    public void ConfirmExchangeAction()
    {
        if (itemExchangeSO == null)
            return;

        int currentSouls = ShopManagement.Instance.GetSoulsAmount();

        if (currentSouls < price)
        {
            gameObject.SetActive(false);
            return;
        }

        GameManager.Instance.AddPurchasedItem(itemExchangeSO);

        ShopManagement.Instance.DecreaseSouls(price);

        ShopManagement.Instance.UpdateSoulsAmount();

        gameObject.SetActive(false);

        ShopManagement.Instance.RefreshShop();
    }

    public void QuitApplication()
    {
        gameObject.SetActive(false);
    }
}