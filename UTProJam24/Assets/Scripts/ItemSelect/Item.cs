using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public static Action<ItemData> TryAddItemToInventory;
    public ItemData itemData;
    private Image image;
    private TextMeshProUGUI itemText;
    public void SetupItem(ItemData data, bool isInteractable = true)
    {
        image = transform.GetChild(0).GetComponent<Image>();
        itemText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        itemData = data;
        image.sprite = data.sprite;
        itemText.text = data.displayName;
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
        TryAddItemToInventory?.Invoke(itemData);
    }

    private void OnItemAddedToInventory(ItemData data)
    {
        if (itemData == data) Destroy(gameObject); // FIXME - does not allow duplicates
    }
}
