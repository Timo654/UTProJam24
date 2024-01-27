using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public static Action<ItemData> AddItemToInventory;
    private ItemData itemData;
    private Image image;
    private TextMeshProUGUI itemText;
    public void SetupItem(ItemData data)
    {
        image = transform.GetChild(0).GetComponent<Image>();
        itemText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        itemData = data;
        image.sprite = data.sprite;
        itemText.text = data.displayName;
    }

    public void OnItemSelected()
    {
        AddItemToInventory?.Invoke(itemData);
        Destroy(gameObject);
    }
}
