using FMOD.Studio;
using System.Collections.Generic;
using UnityEngine;

public class GameplayAudio : MonoBehaviour
{
    // can add sounds here that just listen to events
    private EventInstance switchSound;
    private EventInstance boxSound;
    void Start()
    {
        switchSound = AudioManager.Instance.CreateInstance(FMODEvents.Instance.SwitchTime);
        boxSound = AudioManager.Instance.CreateInstance(FMODEvents.Instance.ToolboxSound);
    }

    private void OnEnable()
    {
        CameraSwitcher.OnCameraSwitched += SwitchSound; // if it should play earlier, use OnStartCameraSwitch instead
        ShelfHandler.OnCloseShelf += CloseToolboxSound;
        ShelfHandler.OnOpenShelf += OpenToolboxSound;
    }
    private void OnDisable()
    {
        CameraSwitcher.OnCameraSwitched -= SwitchSound;
        ShelfHandler.OnCloseShelf -= CloseToolboxSound;
        ShelfHandler.OnOpenShelf -= OpenToolboxSound;
    }
    void SwitchSound()
    {
        switchSound.start();
    }

    void CloseToolboxSound()
    {
        boxSound.start();
    }

    void OpenToolboxSound(List<ItemData> _)
    {
        boxSound.start();
    }
}
