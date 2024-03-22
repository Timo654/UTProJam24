using FMODUnity;
using UnityEngine;

public class FMODEvents : MonoBehaviour
{
    [field: Header("SFX")]
    [field: SerializeField] public EventReference TestSound { get; private set; }
    [field: SerializeField] public EventReference FireExting { get; private set; }
    [field: SerializeField] public EventReference FixWrench { get; private set; }
    [field: SerializeField] public EventReference GlassBreak { get; private set; }
    [field: SerializeField] public EventReference PageTurn { get; private set; }
    [field: SerializeField] public EventReference StepsFuture { get; private set; }
    [field: SerializeField] public EventReference StepsPast { get; private set; }
    [field: SerializeField] public EventReference SwitchTime { get; private set; }
    [field: SerializeField] public EventReference ToolboxSound { get; private set; }
    [field: SerializeField] public EventReference FireSound { get; private set; }
    [field: SerializeField] public EventReference LeakSound { get; private set; }
    [field: SerializeField] public EventReference OxygenPump { get; private set; }
    [field: SerializeField] public EventReference TapeSound { get; private set; }
    
    [field: Header("UI")]
    [field: SerializeField] public EventReference ButtonClick { get; private set; }
    [field: SerializeField] public EventReference HoverButton { get; private set; }
    [field: SerializeField] public EventReference BackSound { get; private set; }

    [field: Header("Music")]
    [field: SerializeField] public EventReference MenuTheme { get; private set; }
    [field: SerializeField] public EventReference MainTheme { get; private set; }
    [field: SerializeField] public EventReference CreditsTheme { get; private set; }
    [field: SerializeField] public EventReference GoodEndingTheme { get; private set; }
    public static FMODEvents Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            return;
        }
        Instance = this;
    }
}
