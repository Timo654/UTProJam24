using UnityEngine;

public class ConfirmItemUsage : MonoBehaviour
{
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
        foreach (var item in inventory.items)
        {
            var button = Instantiate(itemPrefab, itemList.position, itemList.rotation, itemList);
            var buttonScript = button.GetComponent<Item>();
            buttonScript.SetupItem(item, true, true);
        }
        itemList.parent.gameObject.SetActive(true);
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
    }
}
