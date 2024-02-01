using System;
using TMPro;
using UnityEngine;

public class PickuppableObject : MonoBehaviour
{
    public static Action<ItemData> PickUpItem;
    [SerializeField] private ItemData itemData;
    [SerializeField] bool hideText = false;
    TextMeshPro hintText;
    private bool pickupStarted = false;
    private bool interactable = false;
    private void Awake()
    {
        hintText = transform.GetChild(0).GetComponent<TextMeshPro>(); // TODO - unhardcode prompt guide 
        if (hideText)
        {
            hintText.text = "";
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
        
    }

    private void OnEnable()
    {
        InventorySystem.ItemAdded += RemoveItem;
        InputHandler.interact += ShelfOpen;
    }

    private void OnDisable()
    {
        InventorySystem.ItemAdded -= RemoveItem;
        InputHandler.interact -= ShelfOpen;
    }

    public void ShelfOpen(CurrentPlayer player)
    {
        if (player != CurrentPlayer.Past || !interactable) return;
        pickupStarted = true;
        PickUpItem?.Invoke(itemData);
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
        hintText.enabled = true;
        interactable = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        hintText.enabled = false;
        interactable = false;
    }
}
