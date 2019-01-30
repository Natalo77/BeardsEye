using UnityEngine;

//Infinitely spins an object
public class LeakRotate : MonoBehaviour
{
    [SerializeField] [Tooltip("What angle the object begins at")] private float startingAngle;
    [SerializeField] [Tooltip("How fast the object rotates")] private float rotateSpeed;

    //Validating the inputted inspector values
    void OnValidate()
    {
        if (startingAngle > 360)
        {
            startingAngle = 360;
        }
        if (startingAngle < -360)
        {
            startingAngle = -360;
        }
    }

    void Start()
    {
        //Applying initial rotation to the object it's attached to using startingAngle's value
        transform.Rotate((float)0, (float)0, -startingAngle);
    }

    void Update()
    {
        if (Time.timeScale > 0)
        {
            //Spins the object it's attatched to using rotateSpeed's value
            transform.Rotate((float)0, (float)0, -rotateSpeed);
        }
    }
}