using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager Instance { get; private set; }
    [SerializeField] private Button playButton;
    [SerializeField] private Button equipmentButton;
    [SerializeField] private Button shopButton;
    [SerializeField] private Button settingButton;
    [SerializeField] private Button quitButton;

    [Header("Menu")]
    [SerializeField] private CanvasGroup menuCanvasGroup;
    [SerializeField] private GameObject equipmentContainer;
    [SerializeField] private GameObject shopContainer;
    [Header("Camera")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float menuCameraSize = 2.5f;
    [SerializeField] private float gameplayCameraSize = 3.5f;
    [SerializeField] private float cameraTransitionDuration = 1.5f;
    [SerializeField] private AnimationCurve cameraTransitionCurve;

    [Header("Transition")]
    [SerializeField] private float menuFadeDuration = 0.5f;

    private bool isStartingGame;

    private void Awake()
    {
        Instance = this;
        equipmentContainer.SetActive(false);
        shopContainer.SetActive(false);
        
        playButton.onClick.AddListener(StartGame);
        quitButton.onClick.AddListener(QuitGame);
        equipmentButton.onClick.AddListener(EquipWindowActive);
        shopButton.onClick.AddListener(ShopWindowActive);
        Time.timeScale = 1f;

        mainCamera.orthographicSize = menuCameraSize;

        playButton.Select();
    }

    private void StartGame()
    {
        if (isStartingGame)
            return;

        isStartingGame = true;

        StartCoroutine(StartGameRoutine());
    }
    private void EquipWindowActive()
    {
        equipmentContainer.SetActive(true);
        GameManager.Instance.ResetPlayer();
        HideMenu();
    }
    public void ShopWindowActive()
    {
        shopContainer.SetActive(true);
        HideMenu();
    }
    private IEnumerator StartGameRoutine()
    {
        playButton.interactable = false;
        equipmentButton.interactable = false;
        shopButton.interactable = false;
        settingButton.interactable = false;
        quitButton.interactable = false;

        yield return FadeMenu(0f);

        yield return ZoomCamera();

        BeginGameplay();
    }

    private IEnumerator FadeMenu(float targetAlpha)
    {
        float startAlpha = menuCanvasGroup.alpha;
        float elapsed = 0f;

        while (elapsed < menuFadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsed / menuFadeDuration);
            menuCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);

            yield return null;
        }

        menuCanvasGroup.alpha = targetAlpha;
    }

    private IEnumerator ZoomCamera()
    {
        float startSize = mainCamera.orthographicSize;
        float elapsed = 0f;

        while (elapsed < cameraTransitionDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsed / cameraTransitionDuration);

            if (cameraTransitionCurve != null)
                t = cameraTransitionCurve.Evaluate(t);

            mainCamera.orthographicSize = Mathf.Lerp(
                startSize,
                gameplayCameraSize,
                t
            );

            yield return null;
        }

        mainCamera.orthographicSize = gameplayCameraSize;
    }

    private void BeginGameplay()
    {
        GameManager.Instance.StartIntro();
        GameManager.Instance.isPlayingGame = true;
        HideMenu();
    }
    public void HideMenu()
    {
        menuCanvasGroup.alpha = 0f;
        menuCanvasGroup.interactable = false;
        menuCanvasGroup.blocksRaycasts = false;
    }
    public void ShowMenu()
    {
        isStartingGame = false;

        menuCanvasGroup.alpha = 1f;
        menuCanvasGroup.interactable = true;
        menuCanvasGroup.blocksRaycasts = true;

        playButton.interactable = true;
        equipmentButton.interactable = true;
        shopButton.interactable = true;
        settingButton.interactable = true;
        quitButton.interactable = true;

        playButton.Select();
    }
    private void QuitGame()
    {
        Application.Quit();
    }
}