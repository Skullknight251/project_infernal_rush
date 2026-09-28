using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialTextBox : MonoBehaviour
{
    public TutorialCheck.TutorialStep step;
    [SerializeField] private TextMeshProUGUI textMeshPro;
    [SerializeField] private Button quitButton;
    void Start()
    {
        quitButton.onClick.AddListener(QuitTutorial);
    }
    public void QuitTutorial()
    {
        TutorialCheck.Instance.CompleteTutorial(step);
        Time.timeScale = 1.0f;
        Destroy(gameObject);
    }
}
