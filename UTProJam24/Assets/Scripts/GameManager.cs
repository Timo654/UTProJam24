using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private EndingData[] endings;
    void Start()
    {
        LevelChanger.Instance.FadeIn();
    }

    private void OnEnable()
    {
        LevelChanger.OnFadeInFinished += HandleStart;
        FacilityHandler.OnFacilityDestroyed += HandleEnding;
        FacilityHandler.OnFacilitySaved += HandleEnding;
    }

    private void HandleStart()
    {
        Time.timeScale = 1.0f;
        Debug.Log("start!");
    }

    private void HandleEnding(EndingType endingType)
    {
        Debug.Log("ending!");
        Debug.Log(endingType);
        foreach(EndingData ending in endings)
        {
            if (ending.endingType == endingType) {
                SaveManager.Instance.runtimeData.currentEnding = ending;
                break;
            }
        }
        LevelChanger.Instance.FadeToLevel("Ending");
    }
}
