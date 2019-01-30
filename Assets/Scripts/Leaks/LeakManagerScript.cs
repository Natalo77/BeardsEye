using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Manages the activation of leaks
public class LeakManagerScript : MonoBehaviour
{
    //Initializing variables that will store influences on the leak delay
    [SerializeField] [Tooltip("The lowest possible number of seconds between leaks starting")] private float minimumLeakDelay;
    [SerializeField] [Tooltip("The highest possible number of seconds between leaks starting")] private float maximumLeakDelay;
    [SerializeField] [Tooltip("Is multiplied by the original wait time between leaks starting")] private float leakDelayMultiplier;

    //Validating the inputted inspector values
    void OnValidate()
    {
        if (minimumLeakDelay < 0)
        {
            minimumLeakDelay = 0;
        }
        if (maximumLeakDelay < 0)
        {
            maximumLeakDelay = minimumLeakDelay;
        }
        if (leakDelayMultiplier <= 0)
        {
            leakDelayMultiplier = 1;
        }
    }

    void Start()
    {
        StartCoroutine(LeakManager());
    }

    //Returns a list of all inactive tiles
    List<int> InactiveTiles(int totalTiles)
    {
        List<int> InactiveTilesList = new List<int>();
        for (int i = 0; i < totalTiles; i++)
        {
            if (gameObject.transform.GetChild(i).transform.GetChild(0).gameObject.activeSelf == false && gameObject.transform.GetChild(i).gameObject.layer == LayerMask.NameToLayer("Leak"))
            {
                InactiveTilesList.Add(i);
            }
        }
        return InactiveTilesList;
    }

    IEnumerator LeakManager()
    {
        int totalTiles;
        int totalInactiveTiles;
        while ((totalInactiveTiles = InactiveTiles(totalTiles = gameObject.transform.childCount).Count) > 0)
        {
            //Waits number of seconds determined using current number of inactive leaks (totalInactiveTiles) and max possible number of leaks (totalTiles)
            //Influenced by minimumLeakDelay, maximumLeakDelay and leakDelayMultiplier
            yield return new WaitForSeconds(Mathf.Min(maximumLeakDelay, Mathf.Max(minimumLeakDelay, totalInactiveTiles * totalTiles * leakDelayMultiplier)));
            //Enables each 'Leak' of a random inactive 'Wood' (using RandomNumberScript)
            var leak = transform.GetChild(InactiveTiles(totalTiles)[RandomNumberScript.GenerateNumber(0, totalInactiveTiles)]);
            foreach (Transform child in leak.transform)
            {
                child.gameObject.SetActive(true);
            }
            //Triggering a camera shake
            StartCoroutine(Camera.main.GetComponent<CameraPositioningScript>().shakeTimer());
        }
    }
}