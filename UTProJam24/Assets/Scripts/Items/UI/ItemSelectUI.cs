using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSelectUI : MonoBehaviour
{
    public static Action OnOpenItemSelect;
    public static Action OnCloseItemSelect;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private Transform itemList;
    EventSystem evSys;

    private void Awake()
    {
        evSys = EventSystem.current;
    }
    private void OnEnable()
    {
        ShelfHandler.OnOpenShelf += OpenBox;
        Item.ItemDestroyed += UpdateCurrentSelection;
    }

    private void OnDisable()
    {
        ShelfHandler.OnOpenShelf -= OpenBox;
        Item.ItemDestroyed -= UpdateCurrentSelection;
    }
    IEnumerator SubscribeToNaviEvents()
    {
        yield return null; // waits a frame so we dont instantly close the thing
        InputHandler.back += CloseBox;
    }

    void UnsubscribeFromNaviEvents()
    {
        InputHandler.back -= CloseBox;
    }

    void UpdateCurrentSelection()
    {
        StartCoroutine(UpdateSelection());
    }

    IEnumerator UpdateSelection()
    {
        yield return null; // waits a frame before setting it so the object could get destroyed.
        evSys.SetSelectedGameObject(itemList.GetChild(0).gameObject);
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
        StartCoroutine(SubscribeToNaviEvents());
        OnOpenItemSelect?.Invoke();
        UpdateItems(items);
        UpdateCurrentSelection();
        itemList.parent.gameObject.SetActive(true);
    }

    public void CloseBox()
    {
        if (!itemList.parent.gameObject.activeSelf) return;
        UnsubscribeFromNaviEvents();
        OnCloseItemSelect?.Invoke();
        itemList.parent.gameObject.SetActive(false);
        // kill the children
        for (int i = 0; i < itemList.childCount; i++)
        {
            Destroy(itemList.GetChild(i).gameObject);
        }
    }
}
