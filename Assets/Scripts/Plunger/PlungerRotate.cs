using UnityEngine;

public class PlungerRotate : MonoBehaviour
{
    private Transform plunger;
    public float rotationSpeed;

	void Start()
    {
        plunger = GetComponent<Transform>();
	}

	void Update()
    {
        if (Time.timeScale > 0)
        {
            plunger.transform.eulerAngles = new Vector3(plunger.transform.eulerAngles.x, plunger.transform.eulerAngles.y, plunger.transform.eulerAngles.z + rotationSpeed);
        }
	}
}