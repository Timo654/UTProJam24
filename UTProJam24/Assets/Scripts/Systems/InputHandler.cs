using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public static Action<CurrentPlayer> interact;
    public static Action switchTimeline;
    private bool uiOpen = false;
    private bool tutorialOpen = false;
    private bool timelineSwitchEnabled = true;
    private bool isMidTransition = false;
    private InputAction interactInput;
    private InputAction cameraAction;
    private CurrentPlayer currentPlayer;
    // Start is called before the first frame update
    void Awake()
    {
        PlayerControls playerControls = new();
        interactInput = playerControls.Gameplay.Interact;
        cameraAction = playerControls.Gameplay.SwitchTimeline;
    }

    private void OnEnable()
    {
        interactInput.Enable();
        cameraAction.Enable();
        cameraAction.performed += TimelineSwitchPerformed;
        interactInput.performed += InteractPerformed;
        GameManager.SetTimelineSwitch += SetCanTimelineSwitch;
        ItemSelectUI.OnOpenItemSelect += OnOpenMenu;
        ItemSelectUI.OnCloseItemSelect += OnCloseMenu;
        ConfirmItemUsage.OpenUI += OnOpenMenu;
        ConfirmItemUsage.CloseUI += OnCloseMenu;
        DialogueManager.OnBottomDialogueStateChanged += HandleBottomDialogueToggle;
        CameraSwitcher.OnStartCameraSwitch += StartTransition;
        CameraSwitcher.OnCameraSwitched += EndTransition;
        PlayerHandler.OnTimelineSwitch += TogglePlayer;
    }
    private void OnDisable()
    {
        interactInput.Disable();
        interactInput.performed -= InteractPerformed;
        cameraAction.Disable();
        cameraAction.performed -= TimelineSwitchPerformed;
        GameManager.SetTimelineSwitch -= SetCanTimelineSwitch;
        ItemSelectUI.OnOpenItemSelect -= OnOpenMenu;
        ItemSelectUI.OnCloseItemSelect -= OnCloseMenu;
        ConfirmItemUsage.OpenUI -= OnOpenMenu;
        ConfirmItemUsage.CloseUI -= OnCloseMenu;
        DialogueManager.OnBottomDialogueStateChanged -= HandleBottomDialogueToggle;
        CameraSwitcher.OnStartCameraSwitch -= StartTransition;
        CameraSwitcher.OnCameraSwitched -= EndTransition;
        PlayerHandler.OnTimelineSwitch -= TogglePlayer;
    }

    void TogglePlayer(CurrentPlayer curPlayer)
    {
        currentPlayer = curPlayer;
    }

    void StartTransition()
    {
        isMidTransition = true;
    }

    void EndTransition()
    {
        isMidTransition = false;
    }
    void HandleBottomDialogueToggle(bool enabled)
    {
        tutorialOpen = enabled;
    }

    private void SetCanTimelineSwitch(bool state)
    {
        timelineSwitchEnabled = state;
    }

    private void OnCloseMenu()
    {
        uiOpen = false;
    }

    private void OnOpenMenu()
    {
        uiOpen = true;
    }

    public void InteractPerformed(InputAction.CallbackContext context)
    {
        if (uiOpen || tutorialOpen || isMidTransition) return;
        interact?.Invoke(currentPlayer);
    }

    public void TimelineSwitchPerformed(InputAction.CallbackContext context)
    {
        if (uiOpen || tutorialOpen || !timelineSwitchEnabled || isMidTransition) return;
        switchTimeline?.Invoke();
    }
}
