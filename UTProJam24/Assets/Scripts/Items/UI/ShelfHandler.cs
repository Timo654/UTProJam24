using FMOD.Studio;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShelfHandler : MonoBehaviour
{
    public static Action<List<ItemData>> OnOpenShelf;
    public static Action OnCloseShelf;
    private List<ShelfItem> items = new();
    private List<ItemData> shelfItems = new();
    TextMeshPro hintText;
    PlayerControls playerControls;
    InputAction openAction;
    private bool interactable = false;
    private bool shelfOpen = false;
    private void Awake()
    {
        hintText = transform.GetChild(0).GetComponent<TextMeshPro>(); // TODO - unhardcode prompt guide
        playerControls = new PlayerControls();
        openAction = playerControls.Gameplay.Interact;
        items.AddRange(GetComponentsInChildren<ShelfItem>());
        foreach (var item in items)
        {
            shelfItems.Add(item.item);
        }
    }
    private void OnEnable()
    {
        InventorySystem.ItemAdded += RemoveItem;
        ItemSelectUI.OnCloseItemSelect += CloseUI;
        PlayerHandler.OnTimelineSwitch += HandleTimelineSwitch;
        openAction.Enable();
        openAction.performed += ShelfOpen;
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

    private void CloseUI()
    {
        shelfOpen = false;
        OnCloseShelf?.Invoke();
    }
    private void OnDisable()
    {
        ItemSelectUI.OnCloseItemSelect += CloseUI;
        PlayerHandler.OnTimelineSwitch += HandleTimelineSwitch;
        InventorySystem.ItemAdded -= RemoveItem;
        openAction.Disable();
        openAction.performed -= ShelfOpen;
    }

    private void RemoveItem(ItemData item)
    {
        if (!shelfOpen) return;
        shelfItems.Remove(item);
        var index = items.FindIndex(a => a.item.displayName == item.displayName);
        if (index == -1) return;
        Destroy(items[index].gameObject);
        items.RemoveAt(index);
    }

    public void ShelfOpen(InputAction.CallbackContext context)
    {
        if (interactable && !shelfOpen)
        {
            shelfOpen = true;
            OnOpenShelf?.Invoke(shelfItems);
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
