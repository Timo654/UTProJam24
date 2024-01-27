using System;
using UnityEngine;

public class FixableObject : MonoBehaviour
{
    public static Action<float> DamageFacility;
    public float hitDamage = 1f;
    public float itemHealth = 200f;
    public ObstacleType obstacleType;
    private bool currentlyActive = false;
    private void Start()
    {
        switch (obstacleType)
        {
            case ObstacleType.Fire:
                // will start burning at some point. burning slowly fills the facility "broken" meter
                break;
            case ObstacleType.WaterLeak:
                // will start leaking at some point. leaking slowly fills the facility "broken" meter
                break;
            case ObstacleType.AirPressure:
                // slowly decreases all the time. if it gets too low, the facility "broken" meter will fill fast
                break;
            default:
                Debug.LogWarning($"Unknown obstacle {obstacleType}");
                break;
        }
    }
    private void Update()
    {
        switch (obstacleType)
        {
            case ObstacleType.Fire:
                if (currentlyActive)
                {
                    DamageFacility.Invoke(hitDamage * Time.deltaTime);
                }
                // does nothing unless burning currently. has to be manually triggered in another script.
                break;
            case ObstacleType.WaterLeak:
                if (currentlyActive)
                {
                    DamageFacility.Invoke(hitDamage * Time.deltaTime);
                }
                // does nothing unless leaking currently. has to be manually triggered in another script
                break;
            case ObstacleType.AirPressure:
                if (currentlyActive)
                {
                    // go down instead of damaging right away
                }
                else
                {
                    // decide when to activate randomly
                }
                // slowly starts decreasing at some point. starts by itself eventually
                break;
            default:
                break;
        }
    }
    public void ActivateObstacle()
    {
        currentlyActive = true;
    }

    public void DeactivateObstacle()
    {
        currentlyActive = false;
    }
}
