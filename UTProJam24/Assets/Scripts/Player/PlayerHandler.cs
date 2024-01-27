using System;
using UnityEngine;

public class PlayerHandler : MonoBehaviour
{
    //hardcoded two players yeah
    public static Action<CurrentPlayer> OnTimelineSwitch; // TODO - finish implementing this to fix shit
    [SerializeField] private Player_Walk[] playerWalk;
    [SerializeField] private Player_Jump[] playerJump;
    private int playerIndex = 1; // TODO THIS IS DEBUGGGG REMEMBER

    private void Start()
    {
        for (int i=0; i<playerWalk.Length; i++)
        {
            if (i == playerIndex) continue;
            playerWalk[i].enabled = false;
            playerJump[i].enabled = false;
        }
        OnTimelineSwitch?.Invoke((CurrentPlayer)playerIndex);
    }
    private void OnEnable()
    {
        CameraSwitcher.OnStartCameraSwitch += HandleStartSwitch;
        CameraSwitcher.OnCameraSwitched += HandleSwitch;
    }

    private void OnDisable()
    {
        CameraSwitcher.OnStartCameraSwitch -= HandleStartSwitch;
        CameraSwitcher.OnCameraSwitched -= HandleSwitch;
    }

    private void HandleStartSwitch()
    {
        playerWalk[playerIndex].enabled = false;
        playerJump[playerIndex].enabled = false;
        playerIndex++;
        if (playerIndex >= playerWalk.Length)
        {
            playerIndex = 0;
        }
        OnTimelineSwitch?.Invoke((CurrentPlayer)playerIndex);
    }

    private void HandleSwitch()
    {
        playerWalk[playerIndex].enabled = true;
        playerJump[playerIndex].enabled = true;
    }
}
