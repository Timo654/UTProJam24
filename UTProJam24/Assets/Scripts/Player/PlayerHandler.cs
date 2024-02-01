using System;
using UnityEngine;

public class PlayerHandler : MonoBehaviour
{
    //hardcoded two players yeah
    public static Action<CurrentPlayer> OnTimelineSwitch;
    [SerializeField] private Player_Walk[] playerWalk;
    [SerializeField] private Player_Jump[] playerJump;
    private int playerIndex = 1;

    private void Start()
    {
        for (int i=0; i<playerWalk.Length; i++)
        {
            playerWalk[i].enabled = false;
            playerJump[i].enabled = false;
        }
        OnTimelineSwitch?.Invoke((CurrentPlayer)playerIndex);
    }
    private void OnEnable()
    {
        CameraSwitcher.OnStartCameraSwitch += HandleStartSwitch;
        CameraSwitcher.OnCameraSwitched += HandleSwitch;
        GameManager.AllowMovement += SetMovement;
    }

    private void OnDisable()
    {
        CameraSwitcher.OnStartCameraSwitch -= HandleStartSwitch;
        CameraSwitcher.OnCameraSwitched -= HandleSwitch;
        GameManager.AllowMovement -= SetMovement;
    }

    private void SetMovement(bool enabled)
    {
        playerWalk[playerIndex].enabled = enabled;
        playerJump[playerIndex].enabled = enabled;
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
