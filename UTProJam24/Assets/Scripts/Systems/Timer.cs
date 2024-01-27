using System;
using UnityEngine;

public class Timer : MonoBehaviour
{
    private float time;
    private bool counting;
    public static Action OnZero;
    public static Action<string> UpdateGUI;

    public void StartTimer(float start)
    {
        Debug.Log("started timer");
        time = start;
        counting = true;
    }

    public void EndTimer(bool withAction)
    {
        Debug.Log("end timer");
        time = 0f;
        counting = false;
        if (withAction) OnZero?.Invoke();
    }

    private string GetTime()
    {
        return Mathf.RoundToInt(time).ToString();
    }

    private void Update()
    {
        if (!counting) return;
        time -= Time.unscaledDeltaTime;
        if (time <= 0f) EndTimer(true);
        UpdateGUI.Invoke(GetTime());
    }
}
