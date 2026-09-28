using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI totalSoul;
    [SerializeField] private Button returnButton;
    [SerializeField] private GameObject mainMenuContainer;
    void Start()
    {
        returnButton.onClick.AddListener(ReturnToMainMenu);
    }

    public void SetSoulsAmount(float amount)
    {
        totalSoul.text = amount.ToString();
    }
    
    void Update()
    {
        
    }
    public void ReturnToMainMenu()
    {
        gameObject.SetActive(false);
        mainMenuContainer.GetComponent<MainMenuManager>().ShowMenu();
        GameManager.Instance.StartGameSetUp();
    }
}
