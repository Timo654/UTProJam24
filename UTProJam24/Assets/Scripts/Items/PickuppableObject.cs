using System;
using TMPro;
using UnityEngine;

public class PickuppableObject : MonoBehaviour
{
    public static Action<ItemData> PickUpItem;
    [SerializeField] private ItemData itemData;
    TextMeshPro hintText;
    private bool pickupStarted = false;
    private bool interactable = false;
    private void Awake()
    {
        hintText = transform.GetChild(0).GetComponent<TextMeshPro>(); // TODO - unhardcode prompt guide 
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
