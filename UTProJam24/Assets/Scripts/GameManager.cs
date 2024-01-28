using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static Action StartGame;
    private bool tutorialMode = true;
    [SerializeField] private EndingData[] endings;
    // TUTORIAL STUFF
    private bool bottomDialogueActive = false;
    private bool topDialogueActive = false;
    private TutorialPhase currentTutorialPhase = TutorialPhase.Start;
    private bool triggeredFlag = false;
    private float phaseStart;
    private float tutorialTimer;
    private bool phase1Flag = false;
    private bool phase2Flag = false;
    private bool phase3Flag = false;

    [SerializeField] private DialogueAsset[] tutorialTextBottom; // file for each phase
    [SerializeField] private DialogueAsset[] tutorialTextTop; // file for each phase
    [SerializeField] private DialogueManager dialogueManager;
    void Start()
    {
        LevelChanger.Instance.FadeIn();
    }

    private void OnEnable()
    {
        Debug.Log("enagle!");
        LevelChanger.OnFadeInFinished += HandleStart;
        FacilityHandler.OnFacilityDestroyed += HandleEnding;
        FacilityHandler.OnFacilitySaved += HandleEnding;
        ShelfHandler.OnCloseShelf += HandlePhase2SwitchFlag;
        DialogueManager.OnBottomDialogueStateChanged += HandleBottomDialogueToggle;
        DialogueManager.OnTopDialogueStateChanged += HandleTopDialogueToggle;
        PlayerHandler.funnyTest += InvokeTest;
    }

    void InvokeTest()
    {
        Debug.Log(currentTutorialPhase);
        switch (currentTutorialPhase)
        {
            case TutorialPhase.PresentFirst:
                phase1Flag = true;
                phaseStart = tutorialTimer;
                Debug.Log("ph1 flag set");
                break;
            case TutorialPhase.End:
                phaseStart = tutorialTimer;
                phase3Flag = true;
                break;
        }
    }
    private void OnDisable()
    {
        DialogueManager.OnBottomDialogueStateChanged -= HandleBottomDialogueToggle;
        DialogueManager.OnTopDialogueStateChanged -= HandleTopDialogueToggle;
        LevelChanger.OnFadeInFinished -= HandleStart;
        FacilityHandler.OnFacilityDestroyed -= HandleEnding;
        FacilityHandler.OnFacilitySaved -= HandleEnding;
        ShelfHandler.OnCloseShelf -= HandlePhase2SwitchFlag;
    }
    private void HandleStart()
    {
        Time.timeScale = 1.0f;
        Debug.Log("start!");
        if (!tutorialMode)
        {
            StartGame?.Invoke();
        }
    }

    private void Update()
    {
        if (!tutorialMode) return;
        tutorialTimer += Time.deltaTime;
        HandleTutorial();

    }

    private void HandlePhase2SwitchFlag()
    {
        switch (currentTutorialPhase)
        {
            case TutorialPhase.PastFirst:
                phaseStart = tutorialTimer;
                phase2Flag = true;
                break;
        }
    }

    void HandleBottomDialogueToggle(bool enabled)
    {
        bottomDialogueActive = enabled;
    }

    void HandleTopDialogueToggle(bool enabled)
    {
        topDialogueActive = enabled;
    }

    void HandleTutorial()
    {
        switch (currentTutorialPhase)
        {
            case TutorialPhase.Start:
                dialogueManager.StartDialogue(tutorialTextBottom[0], DialogueManager.DialogueType.Bottom);
                currentTutorialPhase = TutorialPhase.PresentFirst;
                phaseStart = tutorialTimer;
                Debug.Log("SWITCH TO TUTORIAL PRESENT 1ST");
                break;
            case TutorialPhase.PresentFirst:
                if (!bottomDialogueActive)
                {
                    if (!topDialogueActive)
                    {
                        Debug.Log("start top0");
                        dialogueManager.StartDialogue(tutorialTextTop[0], DialogueManager.DialogueType.Top);
                    }
                }
                if (phase1Flag && (tutorialTimer > phaseStart + 1.5f)) // add a delay between trigger and stuff
                {
                    dialogueManager.ResetTopDialogue(); // end top dialogue
                    Debug.Log("ph1 flag set and read");
                    phaseStart = tutorialTimer;
                    dialogueManager.StartDialogue(tutorialTextBottom[1], DialogueManager.DialogueType.Bottom);
                    currentTutorialPhase = TutorialPhase.PastFirst;
                    if (!bottomDialogueActive)
                    {
                        if (!topDialogueActive)
                        {
                            Debug.Log("start top1");
                            dialogueManager.StartDialogue(tutorialTextTop[1], DialogueManager.DialogueType.Top);
                        }
                    }
                    Debug.Log("SWITCH TO TUTORIAL PAST 1ST");
                }
                break;
            case TutorialPhase.PastFirst:
                if (phase2Flag && (tutorialTimer > phaseStart + 1.5f))
                {
                    dialogueManager.ResetTopDialogue(); // end top dialogue
                    phaseStart = tutorialTimer;
                    dialogueManager.StartDialogue(tutorialTextBottom[2], DialogueManager.DialogueType.Bottom);
                    currentTutorialPhase = TutorialPhase.End;
                    if (!bottomDialogueActive)
                    {
                        if (!topDialogueActive)
                        {
                            Debug.Log("start top2");
                            dialogueManager.StartDialogue(tutorialTextTop[2], DialogueManager.DialogueType.Top);
                        }
                    }
                    Debug.Log("SWITCH TO TUTORIAL END");
                }
                break;
            case TutorialPhase.End:
                if (phase3Flag && (tutorialTimer > phaseStart + 1.5f))
                {
                    dialogueManager.ResetTopDialogue(); // end top dialogue
                    dialogueManager.StartDialogue(tutorialTextBottom[3], DialogueManager.DialogueType.Bottom);
                    if (tutorialTimer > phaseStart + 3f)
                    {
                        tutorialMode = false;
                        SaveManager.Instance.gameData.tutorialDone = true;
                        StartGame?.Invoke();
                    }
                }
                break;
        }
    }

    private void HandleEnding(EndingType endingType)
    {
        Debug.Log("ending!");
        Debug.Log(endingType);
        foreach (EndingData ending in endings)
        {
            if (ending.endingType == endingType)
            {
                SaveManager.Instance.runtimeData.currentEnding = ending;
                break;
            }
        }
        LevelChanger.Instance.FadeToLevel("Ending");
    }
}
