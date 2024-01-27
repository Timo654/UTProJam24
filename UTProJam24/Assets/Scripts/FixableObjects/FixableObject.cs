using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class FixableObject : MonoBehaviour
{
    public static Action<float> DamageFacility;
    public float hitDamage = 1f;
    public float itemHealth = 200f;
    public ObstacleType obstacleType;
    private bool currentlyActive = false;
    private Animator animator;
    private bool interactable = false;
    private TextMeshPro hintText;
    PlayerControls playerControls;
    InputAction openAction;
    private void Awake()
    {
        hintText = transform.GetChild(0).GetComponent<TextMeshPro>(); // TODO - unhardcode prompt guide
        playerControls = new PlayerControls();
        openAction = playerControls.Gameplay.OpenShelf;
        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        switch (obstacleType)
        {
            case ObstacleType.Fire:
                // will start burning at some point. burning slowly fills the facility "broken" meter
                break;
            case ObstacleType.WaterLeak:
                // will start leaking at some point. leaking slowly fills the facility "broken" meter
                break;
            case ObstacleType.AirPressure:
                // slowly decreases all the time. if it gets too low, the facility "broken" meter will fill fast
                break;
            default:
                Debug.LogWarning($"Unknown obstacle {obstacleType}");
                break;
        }
    }
    private void OnEnable()
    {
        openAction.Enable();
        openAction.performed += ShelfOpen;
    }

    private void OnDisable()
    {
        openAction.Disable();
        openAction.performed -= ShelfOpen;
    }

    public void ShelfOpen(InputAction.CallbackContext context)
    {
        if (interactable)
        {
            Debug.Log("interacting with object");
        }
    }

    private void Update()
    {
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
                    // go down instead of damaging right away
                }
                else
                {
                    // decide when to activate randomly
                }
                // slowly starts decreasing at some point. starts by itself eventually
                break;
            default:
                break;
        }
    }
    public void ActivateObstacle()
    {
        if (currentlyActive) return;
        currentlyActive = true;
        animator.SetBool("Active", true);
    }

    public void DeactivateObstacle()
    {
        if (!currentlyActive) return;
        currentlyActive = false;
        animator.SetBool("Active", false);
    }

    public bool IsCurrentlyActive()
    {
        return currentlyActive;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("Got near a shelf.");
        hintText.enabled = true;
        interactable = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        hintText.enabled = false;
        interactable = false;
    }
}
