using FMOD.Studio;
using System;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class FixableObject : MonoBehaviour
{
    public static Action ConfirmItemUse;
    public static Action<FixableObject> OnAirPressureLeak;
    public static Action<float> DamageFacility;
    public Color arrowColour = Color.white; // arrow to point to the thing
    public float hitDamage = 1f;
    public float itemHealth = 60f; // hp is used for pressure gauge max value
    public ObstacleType obstacleType;
    private bool currentlyActive = false;
    private Animator animator;
    private bool interactable = false;
    private TextMeshPro hintText;
    private bool inTriggerArea = false;
    private bool currentlySelecting = false;
    private float nextActivationAttempt;
    private Transform arrowTransform;
    private float initialHP;
    private bool started = false;
    private EventInstance burningSound;
    private EventInstance fixSound;
    private EventInstance ethanolBreak;
    private void Awake()
    {
        hintText = transform.GetChild(0).GetComponent<TextMeshPro>(); // TODO - unhardcode prompt guide
        if (obstacleType == ObstacleType.AirPressure)
        {
            switch (Helper.GetControllerType())
            {
                case ControlType.Keyboard:
                    hintText.text = "Mash E";
                    break;
                case ControlType.Mobile:
                    hintText.text = "Mash OPEN";
                    break;
                case ControlType.XBOX:
                    hintText.text = "Mash B";
                    break;
                case ControlType.DualShock:
                    hintText.text = "Mash Circle";
                    break;
            }
        }
        else
        {
            switch (Helper.GetControllerType())
            {
                case ControlType.Keyboard:
                    hintText.text = "Press E";
                    break;
                case ControlType.Mobile:
                    hintText.text = "Press OPEN";
                    break;
                case ControlType.XBOX:
                    hintText.text = "Press B";
                    break;
                case ControlType.DualShock:
                    hintText.text = "Press Circle";
                    break;
            }
        }
        
        animator = GetComponent<Animator>();
        initialHP = itemHealth;
    }
    private void OnEnable()
    {
        InputHandler.interact += FixObject;
        Item.ItemUseConfirm += HandleItemSelect;
        GameManager.StartGame += HandleStart;
        PauseMenuController.GamePaused += PauseAudio;
    }


    private void OnDisable()
    {
        InputHandler.interact -= FixObject;
        GameManager.StartGame -= HandleStart;
        Item.ItemUseConfirm -= HandleItemSelect;
        PauseMenuController.GamePaused -= PauseAudio;
    }



    private void PauseAudio(bool paused)
    {

        if (paused)
        {
            if (currentlyActive)
            {
                if (burningSound.isValid()) burningSound.stop(STOP_MODE.IMMEDIATE);
            }
        }
        else
        {
            if (currentlyActive && burningSound.isValid()) burningSound.start();
        }
    }

    public void FixObject(CurrentPlayer player)
    {
        if (player != CurrentPlayer.Present || !interactable) return;
        if (obstacleType == ObstacleType.AirPressure)
        {
            itemHealth += 10f;
            fixSound.start();
            if (itemHealth > initialHP)
            {
                interactable = false;
                currentlyActive = false;
                itemHealth = initialHP;
                nextActivationAttempt = Time.time + 8f;
                hintText.enabled = false;
            }
            MapArrowToValue(itemHealth);
        }
        else
        {
            Debug.Log("interacting with object");
            currentlySelecting = true;
            ConfirmItemUse?.Invoke();
        }
    }

    private void HandleItemSelect(ItemData item)
    {
        if (!currentlySelecting) return;
        // TODO - implement strengths, weaknesses and instakills
        switch (obstacleType)
        {
            case ObstacleType.Fire:
                // will start burning at some point. burning slowly fills the facility "broken" meter
                switch (item.type)
                {
                    case ItemType.Gasoline:
                    case ItemType.Alcohol:
                        // deal a lot of dmg
                        ethanolBreak.start();
                        DamageFacility.Invoke(hitDamage * 50f);
                        break;
                    case ItemType.Tiksu:
                    case ItemType.FireExtinguisher:
                    case ItemType.Hammer:
                        DeactivateObstacle(); // fix item 
                        break;
                    default:
                        // do nothing
                        break;
                }
                break;
            case ObstacleType.WaterLeak:
                // will start leaking at some point. leaking slowly fills the facility "broken" meter
                switch (item.type)
                {
                    case ItemType.Gasoline:
                    case ItemType.Alcohol:
                        // deal a lot of dmg
                        ethanolBreak.start();
                        DamageFacility.Invoke(hitDamage * 50f);
                        break;
                    case ItemType.Wrench:
                    case ItemType.Tape:
                    case ItemType.Tiksu:
                        DeactivateObstacle(); // fix item 
                        break;
                    case ItemType.Food:
                        bool activate = UnityEngine.Random.value > 0.5f;
                        if (activate) DeactivateObstacle();
                        break;
                    default:
                        // do nothing
                        break;
                }
                break;
            case ObstacleType.AirPressure:
                // TODO - unimplemented
                break;
            default:
                Debug.LogWarning($"Unknown obstacle {obstacleType}");
                break;
        }
    }

    void MapArrowToValue(float mapValue)
    {
        // range: 120 to -110
        // values: 0 to 60
        arrowTransform.rotation = Quaternion.AngleAxis(math.remap(0, initialHP, 120f, -110f, mapValue), Vector3.forward);
    }

    private void HandleStart()
    {
        ethanolBreak = AudioManager.Instance.CreateInstance(FMODEvents.Instance.GlassBreak);
        switch (obstacleType)
        {
            case ObstacleType.Fire:
                burningSound = AudioManager.Instance.CreateInstance(FMODEvents.Instance.FireSound);
                FMODUnity.RuntimeManager.AttachInstanceToGameObject(burningSound, transform);
                fixSound = AudioManager.Instance.CreateInstance(FMODEvents.Instance.FireExting);
                // will start burning at some point. burning slowly fills the facility "broken" meter
                break;
            case ObstacleType.WaterLeak:
                burningSound = AudioManager.Instance.CreateInstance(FMODEvents.Instance.LeakSound);
                FMODUnity.RuntimeManager.AttachInstanceToGameObject(burningSound, transform);
                fixSound = AudioManager.Instance.CreateInstance(FMODEvents.Instance.FixWrench);
                // will start leaking at some point. leaking slowly fills the facility "broken" meter
                break;
            case ObstacleType.AirPressure:
                fixSound = AudioManager.Instance.CreateInstance(FMODEvents.Instance.OxygenPump);
                // TODO - air pressure
                nextActivationAttempt = Time.time + 20f; // don't start it right away
                arrowTransform = transform.GetChild(1);
                MapArrowToValue(itemHealth);
                // slowly decreases all the time. if it gets too low, the facility "broken" meter will fill fast
                break;
            default:
                Debug.LogWarning($"Unknown obstacle {obstacleType}");
                break;
        }
        started = true;
    }
    private void Update()
    {
        if (!started) return;
        switch (obstacleType)
        {
            case ObstacleType.Fire:
                if (currentlyActive)
                {
                    DamageFacility.Invoke(hitDamage * Time.deltaTime);
                }
                // does nothing unless burning currently. has to be manually triggered in another script.
                break;
            case ObstacleType.WaterLeak:
                if (currentlyActive)
                {
                    DamageFacility.Invoke(hitDamage * Time.deltaTime);
                }
                // does nothing unless leaking currently. has to be manually triggered in another script
                break;
            case ObstacleType.AirPressure:
                if (currentlyActive)
                {
                    if (itemHealth > 0f)
                    {
                        // go down instead of damaging right away
                        itemHealth -= 2f * Time.deltaTime;
                    }
                    else
                    {
                        DamageFacility.Invoke(200f * Time.deltaTime);
                    }
                    MapArrowToValue(itemHealth);
                }
                else
                {
                    // decide when to activate randomly
                    if (Time.time > nextActivationAttempt)
                    {
                        bool activate = UnityEngine.Random.value > 0.5f;
                        if (activate)
                        {
                            currentlyActive = true;
                            OnAirPressureLeak?.Invoke(this);
                        }
                        else nextActivationAttempt = Time.time + 3f; // delay the inevitable doom
                    }
                }
                // slowly starts decreasing at some point. starts by itself eventually
                break;
            default:
                break;
        }
    }
    public void ActivateObstacle()
    {
        // start fmod sfx here
        if (currentlyActive) return;
        currentlyActive = true;
        if (inTriggerArea)
        {
            hintText.enabled = true;
            interactable = true;
        }

        if (obstacleType != ObstacleType.AirPressure)
        {
            burningSound.start();
            animator.SetBool("Active", true);
        }
    }

    public void DeactivateObstacle()
    {
        // end fmod sfx
        if (!currentlyActive) return;
        currentlyActive = false;
        hintText.enabled = false;
        interactable = false;
        if (obstacleType != ObstacleType.AirPressure)
        {
            burningSound.stop(STOP_MODE.IMMEDIATE);
            fixSound.start();
            animator.SetBool("Active", false);
        }
    }

    public bool IsCurrentlyActive()
    {
        return currentlyActive;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        inTriggerArea = true;
        if (!currentlyActive) return;
        hintText.enabled = true;
        interactable = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        inTriggerArea = false;
        if (!currentlyActive) return;
        hintText.enabled = false;
        interactable = false;
    }
}
