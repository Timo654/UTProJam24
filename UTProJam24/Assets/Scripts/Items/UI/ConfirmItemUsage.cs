using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ConfirmItemUsage : MonoBehaviour
{
    public static Action OpenUI;
    public static Action CloseUI;
    [SerializeField] private InventorySystem inventory;
    [SerializeField] private Transform itemList;
    [SerializeField] GameObject itemPrefab;
    private EventSystem evSys;
    // Start is called before the first frame update
    void Awake()
    {
        evSys = EventSystem.current;
    }
    private void OnEnable()
    {
        FixableObject.ConfirmItemUse += OnShowConfirmation;
        Item.ItemUseConfirm += CloseBox;
    }

    private void OnDisable()
    {
        FixableObject.ConfirmItemUse -= OnShowConfirmation;
        Item.ItemUseConfirm -= CloseBox;
    }

    void SubscribeToNaviEvents()
    {
        InputHandler.back += CloseBox;
    }

    void UnsubscribeFromNaviEvents()
    {
        InputHandler.back -= CloseBox;
    }
    public void OnShowConfirmation()
    {
        if (itemList.parent.gameObject.activeSelf) return;
        SubscribeToNaviEvents();
        foreach (var item in inventory.items)
        {
            var button = Instantiate(itemPrefab, itemList.position, itemList.rotation, itemList);
            var buttonScript = button.GetComponent<Item>();
            buttonScript.SetupItem(item, true, true);
        }
        evSys.SetSelectedGameObject(itemList.GetChild(0).gameObject);
        itemList.parent.gameObject.SetActive(true);
        OpenUI?.Invoke();
    }

    public void CloseBox(ItemData _)
    {
        CloseBox();
    }
    public void CloseBox()
    {
        if (!itemList.parent.gameObject.activeSelf) return;
        UnsubscribeFromNaviEvents();
        itemList.parent.gameObject.SetActive(false);
        // kill the children
        for (int i = 0; i < itemList.childCount; i++)
        {
            Destroy(itemList.GetChild(i).gameObject);
        }
        CloseUI?.Invoke();
    }
}
