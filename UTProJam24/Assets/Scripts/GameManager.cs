using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static Action StartGame;
    public static Action<bool> AllowMovement;
    public static Action<bool> AllowInteract;
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
    private bool phase1BFlag = false; // did a sick flip to skip the tutorial
    private bool phase1Flag = false; // switched timelines
    private bool phase2Flag = false; // closed shelf
    private bool phase2bFlag = false; // picked item from shelf
    private bool phase2cFlag = false; // inventory full
    private bool phase3Flag = false; // went back to present
    private int pickupCounter;
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
        ItemSelectUI.OnOpenItemSelect += DisableMovement; // TODO - move to inputhandler?
        ItemSelectUI.OnCloseItemSelect += EnableMovement;
        ConfirmItemUsage.OpenUI += DisableMovement;
        ConfirmItemUsage.CloseUI += EnableMovement;
        InventorySystem.ItemAdded += HandlePhase2bFlag;
        PickuppableObject.PickUpItem += HandlePhase2cFlag;
        if (returningPlayer)
        {
            AllowMovement += EnableJumpCheck;
            jump.performed += HandlePhase1BSwitchFlag;
        }
    }

    private void HandlePhase2bFlag(ItemData data)
    {
        if (phase2bFlag) return;
        phase2bFlag = true;
        phaseStart = tutorialTimer;
    }

    private void HandlePhase2cFlag(ItemData _)
    {
        if (phase2cFlag) return;
        pickupCounter += 1;
        if (pickupCounter >= 2)
        {
            phase2cFlag = true;
            phaseStart = tutorialTimer;
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
        InventorySystem.ItemAdded -= HandlePhase2bFlag;
        PickuppableObject.PickUpItem -= HandlePhase2cFlag;
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
                if (phase1Flag) return;
                phase1Flag = true;
                phaseStart = tutorialTimer;
                break;
            case TutorialPhase.End:
                if (phase3Flag) return;
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
        if (!phase2bFlag) return;
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
                AllowInteract?.Invoke(false);
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

                if (!phase1Flag && phase1BFlag && returningPlayer && (tutorialTimer > phaseStart + 0.5f))
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
                        AllowInteract?.Invoke(true);
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
                if (phase2cFlag && (tutorialTimer > phaseStart + 1.5f))
                {
                    phaseStart = tutorialTimer;
                    dialogueManager.ResetTopDialogue(); // end top dialogue

                    dialogueManager.StartDialogue(tutorialTextBottom[6], DialogueManager.DialogueType.Bottom);

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
                else if (phase2Flag && phase2bFlag && (tutorialTimer > phaseStart + 1.5f))
                {
                    dialogueManager.ResetTopDialogue(); // end top dialogue

                    dialogueManager.StartDialogue(tutorialTextBottom[2], DialogueManager.DialogueType.Bottom);

                    if (!bottomDialogueActive)
                    {
                        phaseStart = tutorialTimer;
                        AllowInteract?.Invoke(false);
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
                        AllowInteract?.Invoke(true);
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
                        AllowInteract?.Invoke(true);
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
