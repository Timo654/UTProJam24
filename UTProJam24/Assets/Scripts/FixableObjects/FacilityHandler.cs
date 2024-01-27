using System;
using UnityEngine;

public class FacilityHandler : MonoBehaviour
{
    // Start is called before the first frame update
    public static Action OnFacilityDestroyed;
    public static Action OnFacilitySaved;
    private float facilityHealth = 1000f;

    void Start()
    {
        // TODO - add the timer here
    }

    // Update is called once per frame
    void Update()
    {
        if (facilityHealth <= 0f)
        {
            OnFacilityDestroyed.Invoke();
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
        OnFacilitySaved?.Invoke();
    }
    private void DecreaseFacilityHealth(float decrAmount)
    {
        facilityHealth -= decrAmount;
    }

}
