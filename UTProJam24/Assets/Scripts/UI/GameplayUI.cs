using TMPro;
using UnityEngine;

public class GameplayUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timer;
    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnEnable()
    {
        Timer.UpdateGUI += UpdateTimerText;
    }

    private void OnDisable()
    {
        Timer.UpdateGUI -= UpdateTimerText;
    }

    void UpdateTimerText(string time)
    {
        timer.SetText(time);
    }
}
