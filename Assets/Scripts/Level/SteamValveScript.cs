using System.Collections;
using UnityEngine;

/*Manages an individual steam valve
Responsible for turning the steam on and off*/
public class SteamValveScript : MonoBehaviour
{
    //Initializing variables that will store the on/off time values for the steam valve
    [SerializeField] [Tooltip("How long the steam valve stays off for")] private float timeOff;
    [SerializeField] [Tooltip("How long the steam valve stays on for")] private float timeOn;

    //Validating the inputted inspector values
    void OnValidate()
    {
        if (timeOff < 0)
        {
            timeOff *= -1;
        }
        if (timeOn < 0)
        {
            timeOn *= -1;
        }
    }

    void Start()
    {
        StartCoroutine(ValveTimer());
    }

    /*Waits for 'timeOff',
    enables the emissions of all particle systems attached to the valve,
    waits for 'timeOn',
    disables the emissions of all particle systems attached to the valve,
    repeats*/
    IEnumerator ValveTimer()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeOff);

            foreach (Transform child in transform)
            {
                ParticleSystem.EmissionModule emission = child.GetComponent<ParticleSystem>().emission;
                emission.enabled = true;
            }
            yield return new WaitForSeconds(timeOn);

            foreach (Transform child in transform)
            {
                ParticleSystem.EmissionModule emission = child.GetComponent<ParticleSystem>().emission;
                emission.enabled = false;
            }
        }
    }
}