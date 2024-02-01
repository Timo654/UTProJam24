using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public static Action<ItemData> TryAddItemToInventory;
    public static Action<ItemData> ItemUseConfirm;
    public ItemData itemData;
    private Image image;
    private TextMeshProUGUI itemText;
    private bool confirmUI = false;
    public void SetupItem(ItemData data, bool isInteractable = true, bool isConfirmUI = false)
    {
        image = transform.GetChild(0).GetComponent<Image>();
        itemText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        itemData = data;
        image.sprite = data.sprite;
        itemText.text = data.displayName;
        confirmUI = isConfirmUI;
        GetComponent<Button>().interactable = isInteractable;
    }

    private void OnEnable()
    {
        InventorySystem.ItemAdded += OnItemAddedToInventory;
    }

    private void OnDisable()
    {
        InventorySystem.ItemAdded -= OnItemAddedToInventory;
    }
    public void OnItemSelected()
    {
        if (confirmUI)
        {
            ItemUseConfirm?.Invoke(itemData);
        }
        else
        {
            TryAddItemToInventory?.Invoke(itemData);
        }
        
    }

    private void OnItemAddedToInventory(ItemData data)
    {
        if (itemData == data) Destroy(gameObject); // FIXME - does not allow duplicates. Investigate if this breaks UI??
    }
}
