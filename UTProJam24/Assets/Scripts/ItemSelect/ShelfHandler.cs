using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShelfHandler : MonoBehaviour
{
    public static Action<List<ItemData>> OnOpenShelf;
    [SerializeField] private List<ItemData> items;
    TextMeshPro hintText;
    PlayerControls playerControls;
    InputAction openAction;
    private bool interactable = false;
    private void Awake()
    {
        hintText = transform.GetChild(0).GetComponent<TextMeshPro>(); // TODO - unhardcode
        playerControls = new PlayerControls();
        openAction = playerControls.Gameplay.OpenShelf;
    }

    private void OnEnable()
    {
        Item.AddItemToInventory += RemoveItem;
        openAction.Enable();
        openAction.performed += ShelfOpen;
    }

    private void OnDisable()
    {
        Item.AddItemToInventory -= RemoveItem;
        openAction.Disable();
        openAction.performed -= ShelfOpen;
    }

    private void RemoveItem(ItemData item)
    {
        items.Remove(item);
    }

    public void ShelfOpen(InputAction.CallbackContext context)
    {
        Debug.Log("opening shelf");
        OnOpenShelf?.Invoke(items);
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
