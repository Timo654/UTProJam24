using System;
using UnityEngine;

public class FacilityHandler : MonoBehaviour
{
    // Start is called before the first frame update
    public static Action<EndingType> OnFacilityDestroyed;
    public static Action<EndingType> OnFacilitySaved;
    public static Action<float> OnFacilityHPDown;
    public static Action<float> SetInitialFacilityHP;
    private float facilityHealth = 1000f;
    [SerializeField] private Timer timer;

    void Start()
    {
        SetInitialFacilityHP?.Invoke(facilityHealth);
        timer.StartTimer(120f);
    }

    // Update is called once per frame
    void Update()
    {
        if (facilityHealth <= 0f)
        {
            OnFacilityDestroyed.Invoke(EndingType.BadEnding);
        }
    }

    private void OnEnable()
    {
        FixableObject.DamageFacility += DecreaseFacilityHealth;
        Timer.OnZero += FacilitySaved;
    }

    private void OnDisable()
    {
        FixableObject.DamageFacility -= DecreaseFacilityHealth;
        Timer.OnZero -= FacilitySaved;
    }

    private void FacilitySaved()
    {
        OnFacilitySaved?.Invoke(EndingType.GoodEnding);
    }
    private void DecreaseFacilityHealth(float decrAmount)
    {
        facilityHealth -= decrAmount;
        OnFacilityHPDown?.Invoke(facilityHealth);
    }

}
