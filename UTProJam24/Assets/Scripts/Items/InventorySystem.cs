using System;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    // FIXME - duplicate items do not show up properly in UI
    public static Action<ItemData> ItemAdded;
    public static Action<ItemData> ItemConsumed;
    public static Action InventoryFull;
    public static Action ItemAlreadyInInventory;
    public int inventorySize = 2;
    public List<ItemData> items = new();
    [SerializeField] private GameObject itemPrefab;
    private List<Item> displayItemList = new();
    private void OnEnable()
    {
        Item.TryAddItemToInventory += AddItem;
        Item.ItemUseConfirm += RemoveItem;
        PickuppableObject.PickUpItem += AddItem;
    }

    private void OnDisable()
    {
        Item.TryAddItemToInventory -= AddItem;
        Item.ItemUseConfirm -= RemoveItem;
        PickuppableObject.PickUpItem -= AddItem;
    }
    private void AddItem(ItemData item)
    {
        if (inventorySize <= items.Count)
        {
            InventoryFull?.Invoke();
            return;
        }

        if (items.Contains(item))
        {
            ItemAlreadyInInventory?.Invoke();
            return;
        }
        items.Add(item);
        ItemAdded?.Invoke(item);
        AddDisplayItem(item);
    }

    private void RemoveItem(ItemData item)
    {
        RemoveDisplayItem(item);
        items.Remove(item);
        ItemConsumed?.Invoke(item);
    }

    private void AddDisplayItem(ItemData item)
    {
        var button = Instantiate(itemPrefab, transform.position, transform.rotation, transform);
        var buttonScript = button.GetComponent<Item>();
        displayItemList.Add(buttonScript);
        buttonScript.SetupItem(item, false);
    }

    private void RemoveDisplayItem(ItemData item)
    {
        // FIXME - game seems to die here when using an item?
        foreach (var displayItem in displayItemList)
        {
            if (displayItem == null) continue; // otherwise itll die when u have multiple of same item
            Debug.Log(displayItem);
            Debug.Log(item);
            if (displayItem.itemData == item)
            {
                Destroy(displayItem.gameObject);
                break;
            }
        }
    }
}
