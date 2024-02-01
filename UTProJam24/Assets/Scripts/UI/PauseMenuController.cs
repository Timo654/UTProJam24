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
    private bool pauseSubscribed = true;
    
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
        InputHandler.pause += OnPauseButton;
        LevelChanger.OnFadeInFinished += EnablePause;
        ItemSelectUI.OnOpenItemSelect += DisablePause;
        ItemSelectUI.OnCloseItemSelect += EnablePause;
    }

    public void OnDisable()
    {
        InputHandler.pause -= OnPauseButton;
        InputHandler.back -= OnPauseButton;
        LevelChanger.OnFadeInFinished -= EnablePause;
        ItemSelectUI.OnOpenItemSelect -= DisablePause;
        ItemSelectUI.OnCloseItemSelect -= EnablePause;
    }

    void DisablePause()
    {
        canPause = false;
    }

    void EnablePause()
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
        else if (!pauseSubscribed)
        {
            SubscribeToPause();
        }
    }

    public void OnPauseButton()
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

    private void SubscribeToBack()
    {
        if (!pauseSubscribed) return;
        InputHandler.back += OnPauseButton;
        InputHandler.pause -= OnPauseButton;
        pauseSubscribed = false;
    }

    private void SubscribeToPause()
    {
        if (pauseSubscribed) return;
        InputHandler.back -= OnPauseButton;
        InputHandler.pause += OnPauseButton;
        pauseSubscribed = true;
    }
    void PauseGame()
    {
        if (Time.timeScale < 1.0f) return;
        GamePaused?.Invoke(true);
        Time.timeScale = 0f;
        UIFader.FadeCanvasGroup(pauseMenuCG);
        EVRef.SetSelectedGameObject(firstSelectedUIElement);
        AudioManager.Pause();
        isPaused = true;
    }

    void UnpauseGame()
    {
        SubscribeToPause();
        Time.timeScale = 1f;
        GamePaused?.Invoke(false);
        pauseMenu.SetActive(false);
        AudioManager.Unpause();
        isPaused = false;
    }

    public void HandleOptions()
    {
        if (optionsMenu.activeSelf)
        {
            SubscribeToPause();
            UIFader.FadeObjects(pauseButtons, pauseButtonsCG, optionsMenu, optionsMenuCG);
        }
        else
        {
            SubscribeToBack();
            UIFader.FadeObjects(optionsMenu, optionsMenuCG, pauseButtons, pauseButtonsCG);
        }
    }

    public void HandleQuit()
    {
        LevelChanger.Instance.FadeToLevel("MainMenu");
    }
}
