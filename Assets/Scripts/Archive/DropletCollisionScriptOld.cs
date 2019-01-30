using UnityEngine;

//Used to be used to increase the water level upon droplet collision
public class DropletCollisionScriptOld : MonoBehaviour
{
    [SerializeField] [Tooltip("How much the water level increases by per droplet")] private float waterIncrease;
    private GameObject water;

    void OnValidate()
    {
        if (waterIncrease < 0)
        {
            waterIncrease *= -1;
        }
    }

    void Start()
    {
        water = GameObject.Find("Water");
    }

    void OnParticleCollision(GameObject other)
    {
        var waterPosition = water.transform.position;
        if (waterPosition.y < 0)
        {
            waterPosition.y += waterIncrease;
            water.transform.position = waterPosition;
        }
    }
}