using System;
using System.Collections.Generic;
using UnityEngine;

public class FacilityHandler : MonoBehaviour
{
    // Start is called before the first frame updatep
    public static Action<EndingType> OnFacilityDestroyed;
    public static Action<EndingType> OnFacilitySaved;
    public static Action<float> OnFacilityHPDown;
    public static Action<float> SetInitialFacilityHP;
    [SerializeField] private float facilityHealth = 400f;
    [SerializeField] private Timer timer;
    [SerializeField] private float gameLength = 180f;
    [SerializeField] private float accidentDelay = 8f; // how often accidents happen, approximately
    [SerializeField] FixableObject[] fixableObjects;
    [SerializeField] GameObject directionArrowPrefab;
    [SerializeField] Transform futurePlayer;
    private float nextCheckTime;
    private bool started = false;


    void HandleStart()
    {
        SetInitialFacilityHP?.Invoke(facilityHealth);
        timer.StartTimer(gameLength);
        nextCheckTime = Time.time + 10f; // add 5 seconds of safe time MIGHT NEED TO REWORK WE KINDA NEED A TUTORIAL OR STH
        started = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (!started) { return; }
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
                var arrow = Instantiate(directionArrowPrefab, futurePlayer.position, futurePlayer.rotation, futurePlayer);
                var arrowScript = arrow.GetComponent<TargetArrow>();
                arrowScript.Setup(obj);
            }
        }
    }

    private void InitializeArrow(FixableObject fixableObject)
    {
        fixableObject.ActivateObstacle();
        var arrow = Instantiate(directionArrowPrefab, futurePlayer.position, futurePlayer.rotation, futurePlayer);
        var arrowScript = arrow.GetComponent<TargetArrow>();
        arrowScript.Setup(fixableObject);
    }
    private void OnEnable()
    {
        FixableObject.DamageFacility += DecreaseFacilityHealth;
        FixableObject.OnAirPressureLeak += InitializeArrow;
        GameManager.StartGame += HandleStart;
        Timer.OnZero += FacilitySaved;
    }

    private void OnDisable()
    {
        FixableObject.DamageFacility -= DecreaseFacilityHealth;
        FixableObject.OnAirPressureLeak -= InitializeArrow;
        GameManager.StartGame += HandleStart;
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
