using FMOD.Studio;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShelfHandler : MonoBehaviour
{
    public static Action<List<ItemData>> OnOpenShelf;
    public static Action OnCloseShelf;
    [SerializeField] private List<ItemData> items;
    TextMeshPro hintText;
    PlayerControls playerControls;
    InputAction openAction;
    private bool interactable = false;
    private bool shelfOpen = false;
    private EventInstance boxSound;
    private void Awake()
    {
        hintText = transform.GetChild(0).GetComponent<TextMeshPro>(); // TODO - unhardcode prompt guide
        playerControls = new PlayerControls();
        openAction = playerControls.Gameplay.OpenShelf;
    }

    private void Start()
    {
        boxSound = AudioManager.Instance.CreateInstance(FMODEvents.Instance.ToolboxSound);
    }
    private void OnEnable()
    {
        InventorySystem.ItemAdded += RemoveItem;
        ItemSelectUI.OnCloseItemSelect += CloseUI;
        PlayerHandler.OnTimelineSwitch += HandleTimelineSwitch;
        openAction.Enable();
        openAction.performed += ShelfOpen;
    }

    void HandleTimelineSwitch(CurrentPlayer player)
    {
        switch (player)
        {
            case CurrentPlayer.Past:
                openAction.Enable();
                break;
            case CurrentPlayer.Present:
                openAction.Disable();
                break;
        }
    }

    private void CloseUI()
    {
        boxSound.start();
        shelfOpen = false;
        OnCloseShelf?.Invoke();
    }
    private void OnDisable()
    {
        ItemSelectUI.OnCloseItemSelect += CloseUI;
        PlayerHandler.OnTimelineSwitch += HandleTimelineSwitch;
        InventorySystem.ItemAdded -= RemoveItem;
        openAction.Disable();
        openAction.performed -= ShelfOpen;
    }

    private void RemoveItem(ItemData item)
    {
        if (!shelfOpen) return;
        items.Remove(item);
    }

    public void ShelfOpen(InputAction.CallbackContext context)
    {
        if (interactable && !shelfOpen)
        {
            boxSound.start();
            shelfOpen = true;
            Debug.Log("opening shelf");
            OnOpenShelf?.Invoke(items);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("Got near a shelf.");
        hintText.enabled = true;
        interactable = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        hintText.enabled = false;
        interactable = false;
    }
}
