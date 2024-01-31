using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PickuppableObject : MonoBehaviour
{
    public static Action<ItemData> PickUpItem;
    [SerializeField] private ItemData itemData;
    TextMeshPro hintText;
    PlayerControls playerControls;
    InputAction openAction;
    private bool pickupStarted = false;
    private bool interactable = false;
    private void Awake()
    {
        hintText = transform.GetChild(0).GetComponent<TextMeshPro>(); // TODO - unhardcode prompt guide
        playerControls = new PlayerControls();
        openAction = playerControls.Gameplay.Interact;
    }

    private void OnEnable()
    {
        InventorySystem.ItemAdded += RemoveItem;
        PlayerHandler.OnTimelineSwitch += HandleTimelineSwitch;
        openAction.Enable();
        openAction.performed += ShelfOpen;
    }

    private void OnDisable()
    {
        InventorySystem.ItemAdded -= RemoveItem;
        PlayerHandler.OnTimelineSwitch -= HandleTimelineSwitch;
        openAction.Disable();
        openAction.performed -= ShelfOpen;
    }

    void HandleTimelineSwitch(CurrentPlayer player)
    {
        switch (player)
        {
            case CurrentPlayer.Past:
                openAction.Enable();
                break;
            case CurrentPlayer.Present:
                openAction.Disable();
                break;
        }
    }

    public void ShelfOpen(InputAction.CallbackContext context)
    {
        if (interactable)
        {
            pickupStarted = true;
            Debug.Log("picking up item");
            PickUpItem?.Invoke(itemData);
        }
    }

    private void RemoveItem(ItemData item)
    {
        if (pickupStarted)
        {
            pickupStarted = false;
            Destroy(gameObject);
        }
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
