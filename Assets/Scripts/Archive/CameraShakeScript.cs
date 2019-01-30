using UnityEngine;

//Cause a camera shake upon being enabled
public class CameraShakeScript : MonoBehaviour
{
    //Initializing variables that will store influences on the camera shake
    [SerializeField] [Tooltip("How long the camera shakes for")] private float shakeTime;
    [SerializeField] [Tooltip("How powerful the shake is")] private float shakeForce;
    [SerializeField] [Tooltip("How much faster/slower the shake gets over time")] private float timeAcceleration;

    private float dynamicShakeTime;
    private Vector3 originalPosition;
    private static bool shakeBool;

    //Validating the inputted inspector values
    void OnValidate()
    {
        if (shakeTime < 0)
        {
            shakeTime *= -1;
        }
        if (shakeForce < 0)
        {
            shakeForce *= -1;
        }
        if (timeAcceleration < 0)
        {
            timeAcceleration *= -1;
        }
    }

    void OnEnable()
    {
        dynamicShakeTime = shakeTime;
        originalPosition = gameObject.transform.position;
        shakeBool = false;
    }

    void Update()
    {
        if (shakeBool)
        {
            if (dynamicShakeTime > 0)
            {
                //Moving the camera to a random position within a radius, influenced by shakeForce
                gameObject.transform.position = originalPosition + Random.insideUnitSphere * shakeForce;
                //Reducing the time left before the shake ends, influenced by timeAcceleration
                dynamicShakeTime -= Time.deltaTime * timeAcceleration;
            }
            else
            {
                dynamicShakeTime = shakeTime;
                //Returning the camera to its original position
                gameObject.transform.position = originalPosition;
                //Disabling the script so that it doesn't shake the camera again
                gameObject.GetComponent<CameraShakeScript>().enabled = false;
            }
        }
    }
}