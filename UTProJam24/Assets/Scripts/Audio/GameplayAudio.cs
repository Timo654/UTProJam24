using FMOD.Studio;
using UnityEngine;

public class GameplayAudio : MonoBehaviour
{
    // Start is called before the first frame update
    EventInstance switchSound;
    void Start()
    {
        switchSound = AudioManager.Instance.CreateInstance(FMODEvents.Instance.SwitchTime);
    }

    private void OnEnable()
    {
        PlayerHandler.funnyTest += SwitchSound;
    }
    private void OnDisable()
    {
        PlayerHandler.funnyTest -= SwitchSound;
    }
    void SwitchSound()
    {
        switchSound.start();
    }
}
