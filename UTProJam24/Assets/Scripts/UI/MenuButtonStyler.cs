using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButtonStyler : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public Color selectedColor = Color.white;
    public Color unselectedColor = Color.white;
    private TextMeshProUGUI text;
    void Awake()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    void ISelectHandler.OnSelect(BaseEventData eventData)
    {
        text.color = selectedColor;
    }

    void IDeselectHandler.OnDeselect(BaseEventData eventData)
    {
        text.color = unselectedColor;
    }
}
