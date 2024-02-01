using System;
using UnityEngine;

public class ConfirmItemUsage : MonoBehaviour
{
    public static Action OpenUI;
    public static Action CloseUI;
    [SerializeField] private InventorySystem inventory;
    [SerializeField] private Transform itemList;
    [SerializeField] GameObject itemPrefab;
    // Start is called before the first frame update
    void Start()
    {

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
    public void OnShowConfirmation()
    {
        if (itemList.parent.gameObject.activeSelf) return;
        foreach (var item in inventory.items)
        {
            var button = Instantiate(itemPrefab, itemList.position, itemList.rotation, itemList);
            var buttonScript = button.GetComponent<Item>();
            buttonScript.SetupItem(item, true, true);
        }
        itemList.parent.gameObject.SetActive(true);
        OpenUI?.Invoke();
    }

    public void CloseBox(ItemData _)
    {
        if (!itemList.parent.gameObject.activeSelf) return;
        itemList.parent.gameObject.SetActive(false);
        // kill the children
        for (int i = 0; i < itemList.childCount; i++)
        {
            Destroy(itemList.GetChild(i).gameObject);
        }
        CloseUI?.Invoke();
    }
}
