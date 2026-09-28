using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatsBarManager : MonoBehaviour
{
    [Header("UI Fill Images")]
    [SerializeField] private Image healthAmount;
    [SerializeField] private Image staminaAmount;
    [SerializeField] private Image manaAmount;
    [SerializeField] private TextMeshProUGUI soulsAmountEarned;
    [SerializeField] private Button pauseButton;
    [Header("Current Player Reference")]
    [SerializeField] private Player player;
    
    private void Start()
    {
        RefreshPlayerReference();
        pauseButton.onClick.AddListener(PauseGame);
    }

    public void PauseGame()
    {
        GameManager.Instance.PauseGame();
    }

    private void OnEnable()
    {
        RefreshPlayerReference();
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
    }

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }

    private void LateUpdate()
    {
        if (Player.Instance != player)
        {
            RefreshPlayerReference();
        }

        UpdateVisual();
    }

    public void RefreshPlayerReference()
    {
        UnsubscribeEvents();

        player = Player.Instance;

        SubscribeEvents();
        UpdateVisual();
    }

    private void SubscribeEvents()
    {
        if (player != null)
        {
            player.OnStatsChanged += UpdateVisual;
        }
    }

    private void UnsubscribeEvents()
    {
        if (player != null)
        {
            player.OnStatsChanged -= UpdateVisual;
            player = null;
        }
    }

    public void UpdateVisual()
    {
        if (player == null) return;

        if (healthAmount != null && player.maxHealth > 0)
        {
            healthAmount.fillAmount = Mathf.Clamp01((float)player.GetHealth() / player.maxHealth);
        }
        if (staminaAmount != null && player.maxStamina > 0)
        {
            staminaAmount.fillAmount = Mathf.Clamp01((float)player.GetStamina() / player.maxStamina);
        }
        if (manaAmount != null && player.maxMana > 0)
        {
            manaAmount.fillAmount = Mathf.Clamp01((float)player.GetMana() / player.maxMana);
        }
        if (soulsAmountEarned != null && player != null) {
            soulsAmountEarned.text = player.GetSoulsEarned().ToString();
        }
    }
}