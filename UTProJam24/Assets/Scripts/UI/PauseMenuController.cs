using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    public static Action<bool> GamePaused;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject pauseButtons;
    [SerializeField] private Button settingsBackButton;
    [SerializeField] private GameObject firstSelectedUIElement;
    [SerializeField] private GameObject touchUI;
    private PlayerControls playerControls;
    private InputAction pauseAction;
    private InputAction backAction;
    private CanvasGroup optionsMenuCG;
    private CanvasGroup pauseMenuCG;
    private CanvasGroup pauseButtonsCG;
    private EventSystem EVRef;
    private GameObject lastSelect;
    private bool canPause;
    private bool isPaused;

    private void Awake()
    {
        UIFader.InitializeFader();
        EVRef = EventSystem.current;
        optionsMenuCG = optionsMenu.AddComponent<CanvasGroup>();
        pauseMenuCG = pauseMenu.AddComponent<CanvasGroup>();
        pauseButtonsCG = pauseButtons.AddComponent<CanvasGroup>();
        playerControls = new PlayerControls();
        pauseAction = playerControls.UI.Pause;
        backAction = playerControls.UI.Back;
        lastSelect = firstSelectedUIElement;
        if (touchUI != null)
        {
            if (BuildConstants.isMobile)
            {
                touchUI.SetActive(true);
            }
            else
            {
                touchUI.SetActive(false);
            }
        }
    }

    public void OnEnable()
    {
        pauseAction.Enable();
        pauseAction.performed += OnPauseButton;
        backAction.performed += OnPauseButton;
        LevelChanger.OnFadeInFinished += HandleFadeIn;
    }

    public void OnDisable()
    {
        pauseAction.performed -= OnPauseButton;
        pauseAction.Disable();
        backAction.performed -= OnPauseButton;
        backAction.Disable();
        LevelChanger.OnFadeInFinished -= HandleFadeIn;
    }

    void HandleFadeIn()
    {
        canPause = true;
    }
    void Start()
    {

    }

    private void Update()
    {
        if (isPaused)
        {
            if (EVRef.currentSelectedGameObject == null)
            {
                EVRef.SetSelectedGameObject(lastSelect);
            }
            else
            {
                lastSelect = EVRef.currentSelectedGameObject;
            }
        }
    }

    public void OnPauseButton(InputAction.CallbackContext context)
    {
        if (settingsBackButton != null && settingsBackButton.IsActive()) settingsBackButton.onClick.Invoke();
        else HandlePause();
    }

    public void HandlePause()
    {
        if (!canPause) return;
        if (optionsMenu != null && optionsMenu.activeSelf)
        {
            HandleOptions();
            return;
        }
        if (isPaused) UnpauseGame();
        else PauseGame();
    }

    void PauseGame()
    {
        if (Time.timeScale < 1.0f) return;
        GamePaused?.Invoke(true);
        Time.timeScale = 0f;
        UIFader.FadeCanvasGroup(pauseMenuCG);
        EVRef.SetSelectedGameObject(firstSelectedUIElement);
        AudioManager.Pause();
        backAction.Enable();
        isPaused = true;
    }

    void UnpauseGame()
    {
        Time.timeScale = 1f;
        GamePaused?.Invoke(false);
        pauseMenu.SetActive(false);
        AudioManager.Unpause();
        backAction.Disable();
        isPaused = false;
    }

    public void HandleOptions()
    {
        if (optionsMenu.activeSelf)
        {
            UIFader.FadeObjects(pauseButtons, pauseButtonsCG, optionsMenu, optionsMenuCG);
        }
        else
        {
            UIFader.FadeObjects(optionsMenu, optionsMenuCG, pauseButtons, pauseButtonsCG);
        }
    }

    public void HandleQuit()
    {
        LevelChanger.Instance.FadeToLevel("MainMenu");
    }
}
