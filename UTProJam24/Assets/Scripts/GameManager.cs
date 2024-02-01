using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static Action StartGame;
    public static Action<bool> AllowMovement;
    public static Action<bool> SetTimelineSwitch;
    private bool tutorialMode = true;
    [SerializeField] private EndingData[] endings;
    // TUTORIAL STUFF
    private bool bottomDialogueActive = false;
    private bool topDialogueActive = false;
    private TutorialPhase currentTutorialPhase = TutorialPhase.Start;
    private float phaseStart;
    private float tutorialTimer;
    private bool returningPlayer = false;
    private bool phase1BFlag = false;
    private bool phase1Flag = false;
    private bool phase2Flag = false;
    private bool phase3Flag = false;
    [SerializeField] private DialogueAsset[] tutorialTextBottom; // file for each phase
    [SerializeField] private DialogueAsset[] tutorialTextTop; // file for each phase
    [SerializeField] private DialogueManager dialogueManager;
    private InputAction jump;

    private void Awake()
    {
        returningPlayer = SaveManager.Instance.gameData.tutorialDone;
        if (returningPlayer)
        {
            var pc = new PlayerControls();
            jump = pc.Gameplay.Jump;
        }
    }
    void Start()
    {
        LevelChanger.Instance.FadeIn();
    }

    private void OnEnable()
    {
        // TODO - cleanup, unsubscribe from tutorial events once tutorial ends
        LevelChanger.OnFadeInFinished += HandleStart;
        FacilityHandler.OnFacilityDestroyed += HandleEnding;
        FacilityHandler.OnFacilitySaved += HandleEnding;
        ShelfHandler.OnCloseShelf += HandlePhase2SwitchFlag;
        DialogueManager.OnBottomDialogueStateChanged += HandleBottomDialogueToggle;
        DialogueManager.OnTopDialogueStateChanged += HandleTopDialogueToggle;
        PlayerHandler.OnTimelineSwitch += SetFlags;
        ItemSelectUI.OnOpenItemSelect += DisableMovement;
        ItemSelectUI.OnCloseItemSelect += EnableMovement;
        ConfirmItemUsage.OpenUI += DisableMovement;
        ConfirmItemUsage.CloseUI += EnableMovement;
        if (returningPlayer)
        {
            AllowMovement += EnableJumpCheck;
            jump.performed += HandlePhase1BSwitchFlag;
        }
    }

    private void OnDisable()
    {
        LevelChanger.OnFadeInFinished -= HandleStart;
        FacilityHandler.OnFacilityDestroyed -= HandleEnding;
        FacilityHandler.OnFacilitySaved -= HandleEnding;
        ShelfHandler.OnCloseShelf -= HandlePhase2SwitchFlag;
        DialogueManager.OnBottomDialogueStateChanged -= HandleBottomDialogueToggle;
        DialogueManager.OnTopDialogueStateChanged -= HandleTopDialogueToggle;
        PlayerHandler.OnTimelineSwitch -= SetFlags;
        ItemSelectUI.OnOpenItemSelect -= DisableMovement;
        ItemSelectUI.OnCloseItemSelect -= EnableMovement;
        ConfirmItemUsage.OpenUI -= DisableMovement;
        ConfirmItemUsage.CloseUI -= EnableMovement;
        if (returningPlayer)
        {
            AllowMovement -= EnableJumpCheck;
            jump.performed -= HandlePhase1BSwitchFlag;
        }
    }

    void EnableMovement()
    {
        AllowMovement?.Invoke(true);
    }

    void DisableMovement()
    {
        AllowMovement?.Invoke(false);
    }
    void EnableJumpCheck(bool enabled)
    {
        if (enabled) jump.Enable();
        else jump.Disable();
    }
    void SetFlags(CurrentPlayer _)
    {
        switch (currentTutorialPhase)
        {
            case TutorialPhase.PresentFirst:
                phase1Flag = true;
                phaseStart = tutorialTimer;
                break;
            case TutorialPhase.End:
                phaseStart = tutorialTimer;
                phase3Flag = true;
                break;
        }
    }
    private void HandleStart()
    {
        Time.timeScale = 1.0f;
        AudioManager.Instance.InitializeMusic(FMODEvents.Instance.MainTheme);
        AudioManager.Instance.StartMusic();
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

    private void HandlePhase1BSwitchFlag(InputAction.CallbackContext context)
    {
        switch (currentTutorialPhase)
        {
            case TutorialPhase.PresentFirst:
                phase1BFlag = true;
                phaseStart = tutorialTimer;
                jump.Disable();
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
                AllowMovement?.Invoke(false);
                if (returningPlayer)
                {
                    dialogueManager.StartDialogue(tutorialTextBottom[4], DialogueManager.DialogueType.Bottom);
                }
                else
                {
                    dialogueManager.StartDialogue(tutorialTextBottom[0], DialogueManager.DialogueType.Bottom);
                }

                currentTutorialPhase = TutorialPhase.PresentFirst;
                phaseStart = tutorialTimer;
                break;
            case TutorialPhase.PresentFirst:

                if (phase1BFlag && returningPlayer && (tutorialTimer > phaseStart + 0.5f))
                {
                    // if tutorial compelted previusly
                    currentTutorialPhase = TutorialPhase.End;
                    phaseStart = tutorialTimer;

                }
                else if (phase1Flag && (tutorialTimer > phaseStart + 1.5f)) // add a delay between trigger and stuff
                {
                    dialogueManager.ResetTopDialogue(); // end top dialogue
                    dialogueManager.StartDialogue(tutorialTextBottom[1], DialogueManager.DialogueType.Bottom);
                    if (!bottomDialogueActive)
                    {
                        currentTutorialPhase = TutorialPhase.PastFirst;
                        phaseStart = tutorialTimer;
                        if (!topDialogueActive)
                        {
                            dialogueManager.StartDialogue(tutorialTextTop[1], DialogueManager.DialogueType.Top);
                        }
                        SetTimelineSwitch?.Invoke(false);
                    }
                }
                else if (!bottomDialogueActive)
                {
                    if (!topDialogueActive)
                    {
                        AllowMovement?.Invoke(true);
                        if (returningPlayer)
                        {
                            dialogueManager.StartDialogue(tutorialTextTop[3], DialogueManager.DialogueType.Top);
                        }
                        else
                        {
                            dialogueManager.StartDialogue(tutorialTextTop[0], DialogueManager.DialogueType.Top);
                        }
                        SetTimelineSwitch?.Invoke(true);
                    }
                }
                break;
            case TutorialPhase.PastFirst:
                if (phase2Flag && (tutorialTimer > phaseStart + 1.5f))
                {
                    dialogueManager.ResetTopDialogue(); // end top dialogue

                    dialogueManager.StartDialogue(tutorialTextBottom[2], DialogueManager.DialogueType.Bottom);

                    if (!bottomDialogueActive)
                    {
                        phaseStart = tutorialTimer;
                        currentTutorialPhase = TutorialPhase.End;
                        if (!topDialogueActive)
                        {
                            dialogueManager.StartDialogue(tutorialTextTop[2], DialogueManager.DialogueType.Top);
                            SetTimelineSwitch?.Invoke(true);
                        }
                    }
                }
                break;
            case TutorialPhase.End:
                if (phase1BFlag && returningPlayer)
                {
                    dialogueManager.ResetTopDialogue(); // end top dialogue
                    dialogueManager.StartDialogue(tutorialTextBottom[5], DialogueManager.DialogueType.Bottom);
                    if (tutorialTimer > phaseStart + 0.5f)
                    {
                        SetTimelineSwitch?.Invoke(true);
                        tutorialMode = false;
                        StartGame?.Invoke();
                    }
                }
                else if (phase3Flag && (tutorialTimer > phaseStart + 1.5f))
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
