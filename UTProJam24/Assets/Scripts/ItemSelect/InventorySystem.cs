using System;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    public static Action<ItemData> ItemAdded;
    public static Action<ItemData> ItemConsumed;
    public int inventorySize = 2;
    public List<ItemData> items;
    [SerializeField] private GameObject itemPrefab;
    private List<Item> displayItemList;
    private void OnEnable()
    {
        Item.TryAddItemToInventory += AddItem;
    }
    private void AddItem(ItemData item)
    {
        if (inventorySize <= items.Count)
        {
            Debug.Log("INVENTORY FULL!!");
            return;
            // TODO - tell the player their inventory full
        }
        items.Add(item);
        ItemAdded?.Invoke(item);
        AddDisplayItem(item);
    }

    private void RemoveItem(ItemData item)
    {
        items.Remove(item);
        ItemConsumed?.Invoke(item);
        RemoveDisplayItem(item);
    }

    private void AddDisplayItem(ItemData item)
    {
        var button = Instantiate(itemPrefab, transform.position, transform.rotation, transform);
        var buttonScript = button.GetComponent<Item>();
        buttonScript.SetupItem(item, false);
    }

    private void RemoveDisplayItem(ItemData item)
    {
        foreach (var displayItem in displayItemList)
        {
            if (displayItem.itemData == item)
            {
                Destroy(displayItem.gameObject);
                break;
            }
        }
    }
}
