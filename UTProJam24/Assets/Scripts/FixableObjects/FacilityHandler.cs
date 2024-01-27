using System;
using UnityEngine;

public class FacilityHandler : MonoBehaviour
{
    // Start is called before the first frame updatep
    public static Action<EndingType> OnFacilityDestroyed;
    public static Action<EndingType> OnFacilitySaved;
    public static Action<float> OnFacilityHPDown;
    public static Action<float> SetInitialFacilityHP;
    private float facilityHealth = 1000f;
    [SerializeField] private Timer timer;
    [SerializeField] private float accidentDelay = 8f; // how often accidents happen, approximately
    [SerializeField] FixableObject[] fixableObjects;
    private float nextCheckTime;
    void Start()
    {
        SetInitialFacilityHP?.Invoke(facilityHealth);
        timer.StartTimer(120f);
        nextCheckTime = Time.time + 10f; // add 5 seconds of safe time MIGHT NEED TO REWORK WE KINDA NEED A TUTORIAL OR STH
    }

    // Update is called once per frame
    void Update()
    {
        if (facilityHealth <= 0f)
        {
            OnFacilityDestroyed.Invoke(EndingType.BadEnding);
        }
        if (Time.time > nextCheckTime)
        {
            nextCheckTime += Time.time + accidentDelay;
            var obj = fixableObjects[UnityEngine.Random.Range(0, fixableObjects.Length)];
            if (!obj.IsCurrentlyActive())
            {
                obj.ActivateObstacle();
            }   
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
