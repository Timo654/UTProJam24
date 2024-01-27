using TMPro;
using UnityEngine;

public class GameplayUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI facilityHPText;
    private float facilityHealth;
    private float visualFacilityHealth;
    public float hpCounterSpeed = 20f;
    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnEnable()
    {
        Timer.UpdateGUI += UpdateTimerText;
        FacilityHandler.OnFacilityHPDown += UpdateFacilityHP;
        FacilityHandler.SetInitialFacilityHP += UpdateInitialFacilityHP;
    }

    private void OnDisable()
    {
        Timer.UpdateGUI -= UpdateTimerText;
        FacilityHandler.OnFacilityHPDown -= UpdateFacilityHP;
        FacilityHandler.SetInitialFacilityHP -= UpdateInitialFacilityHP;
    }

    private void UpdateInitialFacilityHP(float hp)
    {
        facilityHealth = hp;
        visualFacilityHealth = hp;
        facilityHPText.text = $"Facility HP: {Mathf.RoundToInt(visualFacilityHealth)}/{facilityHealth}";
    }

    private void Update()
    {
        if (facilityHealth != visualFacilityHealth)
        {
            visualFacilityHealth = Mathf.MoveTowards(visualFacilityHealth, facilityHealth, hpCounterSpeed * Time.deltaTime);
            facilityHPText.text = $"Facility HP: {Mathf.RoundToInt(visualFacilityHealth)}/{facilityHealth}";
        }
    }
    void UpdateTimerText(string time)
    {
        timerText.SetText($"Time left: {time}");
    }

    void UpdateFacilityHP(float newHP)
    {
        facilityHealth = newHP;
    }
}
