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
            Debug.Log("INVENTORY FULL!!");
            InventoryFull?.Invoke();
            return;
            // TODO - tell the player their inventory full
        }

        if (items.Contains(item))
        {
            Debug.Log("ITEM ALREADY EXISTS!");
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
        Debug.Log($"added {item}");
    }

    private void RemoveDisplayItem(ItemData item)
    {
        // FIXME - game seems to die here when using an item?
        foreach (var displayItem in displayItemList)
        {
            if (displayItem.itemData == item)
            {
                Debug.Log("item found");
                Destroy(displayItem.gameObject);
                break;
            }
        }
    }
}
