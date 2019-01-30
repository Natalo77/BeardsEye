using System;
using UnityEngine;
using UnityEngine.UI;

//Keeps the time survived text updated
public class TimeSurvivedTextScript : MonoBehaviour
{
    private Text timeSurvivedText;

    void Start()
    {
        timeSurvivedText = GetComponent<Text>();
    }

    void Update()
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(Time.timeSinceLevelLoad);
        timeSurvivedText.text = string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
    }
}