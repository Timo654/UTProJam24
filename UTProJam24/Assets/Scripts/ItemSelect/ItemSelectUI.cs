using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemSelectUI : MonoBehaviour
{
    public static Action OnOpenItemSelect;
    public static Action OnCloseItemSelect;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private Transform itemList;
    private void OnEnable()
    {
        ShelfHandler.OnOpenShelf += OpenBox;
    }

    private void OnDisable()
    {
        ShelfHandler.OnOpenShelf -= OpenBox;
    }
    private void UpdateItems(List<ItemData> items)
    {
        foreach (var item in items)
        {
            var button = Instantiate(itemPrefab, itemList.position, itemList.rotation, itemList);
            var buttonScript = button.GetComponent<Item>();
            buttonScript.SetupItem(item);
        }
    }
    public void OpenBox(List<ItemData> items)
    {
        if (itemList.parent.gameObject.activeSelf) return;
        OnOpenItemSelect?.Invoke();
        UpdateItems(items);
        itemList.parent.gameObject.SetActive(true);
    }

    public void CloseBox()
    {
        if (!itemList.parent.gameObject.activeSelf) return;
        OnCloseItemSelect?.Invoke();
        itemList.parent.gameObject.SetActive(false);
        // kill the children
        for (int i = 0; i < itemList.childCount; i++)
        {
            Destroy(itemList.GetChild(i).gameObject);
        }
    }
}
