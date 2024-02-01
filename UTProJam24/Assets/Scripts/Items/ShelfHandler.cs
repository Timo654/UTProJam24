using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShelfHandler : MonoBehaviour
{
    public static Action<List<ItemData>> OnOpenShelf;
    public static Action OnCloseShelf;
    private List<ShelfItem> items = new();
    private List<ItemData> shelfItems = new();
    TextMeshPro hintText;
    private bool interactable = false;
    private bool shelfOpen = false;
    private void Awake()
    {
        hintText = transform.GetChild(0).GetComponent<TextMeshPro>(); // TODO - unhardcode prompt guide
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
        InputHandler.interact += ShelfOpen;
    }


    private void CloseUI()
    {
        shelfOpen = false;
        OnCloseShelf?.Invoke();
    }
    private void OnDisable()
    {
        ItemSelectUI.OnCloseItemSelect += CloseUI;
        InventorySystem.ItemAdded -= RemoveItem;
        InputHandler.interact -= ShelfOpen;
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

    public void ShelfOpen(CurrentPlayer player)
    {
        if (player != CurrentPlayer.Past || !interactable || shelfOpen) return;
        shelfOpen = true;
        OnOpenShelf?.Invoke(shelfItems);
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
