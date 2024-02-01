using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    // INPUTS
    public static Action<CurrentPlayer> interact;
    public static Action switchTimeline;
    public static Action pause;
    public static Action back;

    // BOOLS TO CHECK STUFF
    private bool uiOpen = false;
    private bool tutorialOpen = false;
    private bool timelineSwitchEnabled = true;
    private bool isMidTransition = false;
    private bool isPaused = false;
    private bool canInteract = false;
    private InputAction interactInput;
    private InputAction cameraAction;
    private InputAction pauseAction;
    private InputAction backAction;
    private CurrentPlayer currentPlayer;
    // Start is called before the first frame update
    void Awake()
    {
        PlayerControls playerControls = new();
        interactInput = playerControls.Gameplay.Interact;
        cameraAction = playerControls.Gameplay.SwitchTimeline;
        pauseAction = playerControls.UI.Pause;
        backAction = playerControls.UI.Back;
    }

    private void OnEnable()
    {
        interactInput.Enable();
        cameraAction.Enable();
        cameraAction.performed += TimelineSwitchPerformed;
        interactInput.performed += InteractPerformed;
        pauseAction.Enable();
        backAction.Enable();
        pauseAction.performed += PausePerformed;
        backAction.performed += BackPerformed;
        GameManager.SetTimelineSwitch += SetCanTimelineSwitch;
        ItemSelectUI.OnOpenItemSelect += OnOpenMenu;
        ItemSelectUI.OnCloseItemSelect += OnCloseMenu;
        ConfirmItemUsage.OpenUI += OnOpenMenu;
        ConfirmItemUsage.CloseUI += OnCloseMenu;
        DialogueManager.OnBottomDialogueStateChanged += HandleBottomDialogueToggle;
        CameraSwitcher.OnStartCameraSwitch += StartTransition;
        CameraSwitcher.OnCameraSwitched += EndTransition;
        PlayerHandler.OnTimelineSwitch += TogglePlayer;
        PauseMenuController.GamePaused += TogglePause;
        GameManager.AllowInteract += SetCanInteract;
    }
    private void OnDisable()
    {
        interactInput.Disable();
        interactInput.performed -= InteractPerformed;
        cameraAction.Disable();
        cameraAction.performed -= TimelineSwitchPerformed;
        pauseAction.Disable();
        backAction.Disable();
        pauseAction.performed -= PausePerformed;
        backAction.performed -= BackPerformed;
        GameManager.SetTimelineSwitch -= SetCanTimelineSwitch;
        ItemSelectUI.OnOpenItemSelect -= OnOpenMenu;
        ItemSelectUI.OnCloseItemSelect -= OnCloseMenu;
        ConfirmItemUsage.OpenUI -= OnOpenMenu;
        ConfirmItemUsage.CloseUI -= OnCloseMenu;
        DialogueManager.OnBottomDialogueStateChanged -= HandleBottomDialogueToggle;
        CameraSwitcher.OnStartCameraSwitch -= StartTransition;
        CameraSwitcher.OnCameraSwitched -= EndTransition;
        PlayerHandler.OnTimelineSwitch -= TogglePlayer;
        PauseMenuController.GamePaused -= TogglePause;
        GameManager.AllowInteract -= SetCanInteract;
    }

    private void TogglePause(bool obj)
    {
        isPaused = obj;
    }

    private void TogglePlayer(CurrentPlayer curPlayer)
    {
        currentPlayer = curPlayer;
    }

    private void StartTransition()
    {
        isMidTransition = true;
    }

    private void EndTransition()
    {
        isMidTransition = false;
    }
    private void HandleBottomDialogueToggle(bool enabled)
    {
        tutorialOpen = enabled;
    }

    private void SetCanTimelineSwitch(bool state)
    {
        timelineSwitchEnabled = state;
    }

    private void SetCanInteract(bool state)
    {
        canInteract = state;
        Debug.Log($"interact {state}");
    }

    private void OnCloseMenu()
    {
        uiOpen = false;
    }

    private void OnOpenMenu()
    {
        uiOpen = true;
    }

    private void InteractPerformed(InputAction.CallbackContext context)
    {
        if (uiOpen || tutorialOpen || isMidTransition || isPaused || !canInteract) return;
        interact?.Invoke(currentPlayer);
    }

    private void TimelineSwitchPerformed(InputAction.CallbackContext context)
    {
        if (uiOpen || tutorialOpen || !timelineSwitchEnabled || isMidTransition || isPaused) return;
        switchTimeline?.Invoke();
    }

    private void PausePerformed(InputAction.CallbackContext context)
    {
        pause?.Invoke();
    }

    private void BackPerformed(InputAction.CallbackContext context)
    {
        back?.Invoke();
    }
}
