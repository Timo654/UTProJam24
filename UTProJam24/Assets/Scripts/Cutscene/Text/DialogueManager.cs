using System;
using System.Collections;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using FMOD.Studio;

public class DialogueManager : MonoBehaviour
{
    public static event Action<bool> OnBottomDialogueStateChanged; // start/stop - true/false
    public static event Action<bool> OnTopDialogueStateChanged; // start/stop - true/false
    private PlayerControls playerControls;
    private InputAction interactAction;
    private int currentTopLine;
    private int currentBottomLine;
    private DialogueAsset currentTopDialogue;
    private DialogueAsset currentBottomDialogue;
    public TextMeshProUGUI topTutorialText;
    public TextMeshProUGUI bottomTutorialText;
    public TextMeshProUGUI CTCText;
    public GameObject bottomTutorialBox;
    public GameObject topTutorialBox;
    public GameObject touchButton;
    private ControlType controller;
    private bool gamePaused;
    private readonly int[] variables = new int[10];

    public EventInstance dialogClicking;
    public void Awake()
    {
        playerControls = new PlayerControls();
        interactAction = playerControls.UI.Interact;
        controller = Helper.GetControllerType();
        switch (controller)
        {
            case ControlType.XBOX:
                CTCText.text = "A to continue";
                break;
            case ControlType.DualShock:
                CTCText.text = "Cross to continue";
                break;
            case ControlType.Mobile:
                CTCText.text = "Tap to continue";
                break;
            case ControlType.TiltControls:
                if (BuildConstants.isMobile) CTCText.text = "Tap to continue";
                else CTCText.text = "Cross to continue";
                break;
        }
    }

    private void OnEnable()
    {
        interactAction.performed += OnSkipBottomLine;
        PauseMenuController.GamePaused += TogglePause;
    }
    private void OnDisable()
    {
        interactAction.performed -= OnSkipBottomLine;
        PauseMenuController.GamePaused -= TogglePause;
    }

    public void SetVariable(int variableIndex, int value)
    {
        variables[variableIndex] = value;
    }

    private void TogglePause(bool enable)
    {
        gamePaused = enable;
    }
    void UpdateBottomText()
    {
        bottomTutorialText.text = TextParser(currentBottomDialogue, currentBottomLine);
        bottomTutorialBox.SetActive(true);
    }

    public void UpdateTopText()
    {
        topTutorialText.text = TextParser(currentTopDialogue, currentTopLine);
        topTutorialBox.SetActive(true);
    }
    public void ResetTopDialogue()
    {
        EndTopText();
        currentTopDialogue = null;
    }
    public string TextParser(DialogueAsset dialogue, int currentIndex)
    {
        string inputString;
        if (dialogue.isInputRelated)
        {
            switch (controller)
            {
                case ControlType.Mobile:
                    inputString = dialogue.dialogueMobile[currentIndex];
                    break;
                case ControlType.TiltControls:
                    inputString = dialogue.dialogueTilt[currentIndex];
                    break;
                case ControlType.XBOX:
                    inputString = dialogue.dialogueXBOX[currentIndex];
                    break;
                case ControlType.DualShock:
                    inputString = dialogue.dialoguePS[currentIndex];
                    break;
                default:
                    inputString = dialogue.dialogue[currentIndex];
                    break;
            }
        }
        else
        {
            inputString = dialogue.dialogue[currentIndex];
        }

        string outputString = inputString;
        string pattern = @"{(\d+)}";
        MatchCollection matches = Regex.Matches(inputString, pattern);
        foreach (Match match in matches)
        {
            int index = int.Parse(match.Groups[1].Value);
            if (index < variables.Length)
            {
                outputString = outputString.Replace(match.Value, variables[index].ToString());
            }
        }
        return outputString;
    }



    void EndTopText()
    {
        if (!topTutorialBox.activeSelf) return;
        topTutorialBox.SetActive(false);
        topTutorialText.text = null;
        OnTopDialogueStateChanged?.Invoke(false);
    }
    void EndBottomText()
    {
        if (!bottomTutorialBox.activeSelf) return;
        bottomTutorialBox.SetActive(false);
        interactAction.Disable();
        bottomTutorialText.text = null;
        OnBottomDialogueStateChanged?.Invoke(false);
        if (BuildConstants.isMobile) touchButton.SetActive(false);
        StartCoroutine(DelayTimescale());
    }

    public void HandleTouch()
    {
        SkipBottomLine();
    }
    // exists to prevent inputs from getting listened to twice in 1 frame
    IEnumerator DelayTimescale()
    {
        yield return 0;
        Time.timeScale = 1f;
    }
    DialogueAsset UpdateDialogueData(DialogueAsset dialogue)
    {
        if (dialogue.isInputRelated)
        {
            if (dialogue.dialogueXBOX == null || dialogue.dialogueXBOX.Length == 0) dialogue.dialogueXBOX = dialogue.dialogue;
            if (dialogue.dialoguePS == null || dialogue.dialoguePS.Length == 0) dialogue.dialoguePS = dialogue.dialogueXBOX;
            if (dialogue.dialogueMobile == null || dialogue.dialogueMobile.Length == 0) dialogue.dialogueMobile = dialogue.dialogue;
            if (dialogue.dialogueTilt == null || dialogue.dialogueTilt.Length == 0)
            {
                if (BuildConstants.isMobile)
                {
                    dialogue.dialogueTilt = dialogue.dialogueMobile;
                }
                else
                {
                    dialogue.dialogueTilt = dialogue.dialogue;
                }

            }
        }
        return dialogue;
    }
    public void StartDialogue(DialogueAsset dialogue, DialogueType type)
    {
        switch (type)
        {
            case DialogueType.Bottom:
                if (currentBottomDialogue == dialogue) return;
                OnBottomDialogueStateChanged?.Invoke(true);
                interactAction.Enable();
                currentBottomDialogue = UpdateDialogueData(dialogue);
                currentBottomLine = 0;
                Time.timeScale = 0f; // pause game when dialogue starts
                UpdateBottomText();
                if (BuildConstants.isMobile) touchButton.SetActive(true);
                break;
            case DialogueType.Top:
                if (currentTopDialogue == dialogue) return;
                OnTopDialogueStateChanged?.Invoke(true);
                currentTopDialogue = UpdateDialogueData(dialogue);
                currentTopLine = 0;
                UpdateTopText();
                break;
        }
    }
    public void ContinueTopDialogue()
    {
        if (currentTopLine + 1 >= currentTopDialogue.dialogue.Length)
        {
            EndTopText();
            return;
        }
        currentTopLine++;
        UpdateTopText();
    }
    void OnSkipBottomLine(InputAction.CallbackContext context)
    {
        SkipBottomLine();
    }

    void SkipBottomLine()
    {
        if (gamePaused) return;
        if (currentBottomLine + 1 >= currentBottomDialogue.dialogue.Length)
        {
            EndBottomText();
            return;
        }
        
        dialogClicking = AudioManager.Instance.CreateInstance(FMODEvents.Instance.DialogClick);

        currentBottomLine++;
        UpdateBottomText();
    }
    public enum DialogueType
    {
        Bottom,
        Top
    }
}
