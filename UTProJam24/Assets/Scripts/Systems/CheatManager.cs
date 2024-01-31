using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CheatManager : MonoBehaviour
{
    public static event Action<bool> ToggleUI;
    private PlayerControls playerControls;
    // Start is called before the first frame update
    void Awake()
    {
        if (!(BuildConstants.isDebug || BuildConstants.isExpo)) return;
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        if (!(BuildConstants.isDebug || BuildConstants.isExpo)) return;
    }

    private void OnDisable()
    {
        if (!(BuildConstants.isDebug || BuildConstants.isExpo)) return;
    }
}
