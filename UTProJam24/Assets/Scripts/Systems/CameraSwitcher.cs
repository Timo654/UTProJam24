using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraSwitcher : MonoBehaviour
{
    public static Action OnCameraSwitched;
    public static Action OnStartCameraSwitch;
    [SerializeField] Camera pastCamera;
    [SerializeField] Camera futureCamera;
    private Animator cameraAnim;
    private PlayerControls playerControls;
    private InputAction cameraAction;
    private bool canSwitch = false;
    private bool isInMenu = false;
    // Start is called before the first frame update
    void Awake()
    {
        playerControls = new PlayerControls();
        cameraAction = playerControls.Gameplay.SwitchTimeline;
        cameraAnim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        GameManager.SetTimelineSwitch += SetCanTimelineSwitch;
        ItemSelectUI.OnOpenItemSelect += OnOpenMenu;
        ItemSelectUI.OnCloseItemSelect += OnCloseMenu;
        ConfirmItemUsage.OpenUI += OnOpenMenu;
        ConfirmItemUsage.CloseUI += OnCloseMenu;
        cameraAction.Enable();
        cameraAction.performed += OnCameraButton;
    }

    private void OnDisable()
    {
        GameManager.SetTimelineSwitch -= SetCanTimelineSwitch;
        ItemSelectUI.OnOpenItemSelect -= OnOpenMenu;
        ItemSelectUI.OnCloseItemSelect -= OnCloseMenu;
        ConfirmItemUsage.OpenUI -= OnOpenMenu;
        ConfirmItemUsage.CloseUI -= OnCloseMenu;
        cameraAction.Disable();
        cameraAction.performed -= OnCameraButton;
    }

    private void OnOpenMenu()
    {
        isInMenu = true;
    }

    private void OnCloseMenu()
    {
        isInMenu = false;
    }

    private void SetCanTimelineSwitch(bool state)
    {
        canSwitch = state;
    }
    public void OnCameraButton(InputAction.CallbackContext context)
    {
        if (!(canSwitch && !isInMenu)) return;
        if (!cameraAnim.GetCurrentAnimatorStateInfo(0).IsName("CameraAnimation"))
        {
            OnStartCameraSwitch.Invoke();
            cameraAnim.SetTrigger("Fade");
        }
    }

    public void ToggleCamera()
    {
        OnCameraSwitched.Invoke();
        pastCamera.enabled = !pastCamera.enabled;
        futureCamera.enabled = !futureCamera.enabled;
    }
}
