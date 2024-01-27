using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraSwitcher : MonoBehaviour
{
    [SerializeField] Camera pastCamera;
    [SerializeField] Camera futureCamera;
    private Animator cameraAnim;
    private PlayerControls playerControls;
    private InputAction cameraAction;
    // Start is called before the first frame update
    void Awake()
    {
        playerControls = new PlayerControls();
        cameraAction = playerControls.Gameplay.SwitchCamera;
        pastCamera.enabled = true;
        futureCamera.enabled = false;
        cameraAnim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        cameraAction.Enable();
        cameraAction.performed += OnCameraButton;
    }

    private void OnDisable()
    {
        cameraAction.Disable();
        cameraAction.performed -= OnCameraButton;
    }
    public void OnCameraButton(InputAction.CallbackContext context)
    {
        if (!cameraAnim.GetCurrentAnimatorStateInfo(0).IsName("CameraAnimation"))
        {
            cameraAnim.SetTrigger("Fade");
        }
    }

    public void ToggleCamera()
    {
        pastCamera.enabled = !pastCamera.enabled;
        futureCamera.enabled = !futureCamera.enabled;
    }
}
