using System;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public static Action OnCameraSwitched;
    public static Action OnStartCameraSwitch;
    [SerializeField] Camera pastCamera;
    [SerializeField] Camera futureCamera;
    private Animator cameraAnim;
    // Start is called before the first frame update
    void Awake()
    {
        cameraAnim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        InputHandler.switchTimeline += OnCameraButton;
    }

    private void OnDisable()
    {
        InputHandler.switchTimeline -= OnCameraButton;
    }

    public void OnCameraButton()
    {
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
