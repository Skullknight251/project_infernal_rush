using UnityEngine;
using UnityEngine.UI;

public class ExchangeFailManager : MonoBehaviour
{
    [SerializeField] private Button quitButton;
    void Start()
    {
        quitButton.onClick.AddListener(QuitApplication);
    }

    public void QuitApplication()
    {
        gameObject.SetActive(false);
    }
    void Update()
    {
        
    }
}
