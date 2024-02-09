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

        PlayerHandler.OnTimelineSwitch += HandleTimeline;
    }
    private void OnDisable()
    {
        CameraSwitcher.OnCameraSwitched -= SwitchSound;
        ShelfHandler.OnCloseShelf -= CloseToolboxSound;
        ShelfHandler.OnOpenShelf -= OpenToolboxSound;

        PlayerHandler.OnTimelineSwitch -= HandleTimeline;
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

    void HandleTimeline(CurrentPlayer player) {
    switch (player) {
    case CurrentPlayer.Past:
        AudioManager.Instance.SetMusicParameter("Switching", 1); //Value A in FMOD
        break;
    case CurrentPlayer.Present:
        AudioManager.Instance.SetMusicParameter("Switching", 0); //Value B in FMOD
        break;
    }
    }
}
