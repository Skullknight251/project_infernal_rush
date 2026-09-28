using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private GameObject mainMenuContainer;
    void Start()
    {
        resumeButton.onClick.AddListener(ReturnToGame);
        quitButton.onClick.AddListener(GameOver);
    }
    public void ReturnToGame()
    {
        PlayerMovement.Instance.SetCanRun(true);
        gameObject.SetActive(false);
    }
    void Update()
    {
        
    }
    public void GameOver()
    {
        GameManager.Instance.GameOver();
    }
}
