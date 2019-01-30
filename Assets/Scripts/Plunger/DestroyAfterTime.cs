using System.Collections;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    [SerializeField] [Tooltip("Number of seconds before plunger is destroyed after un-sticking")] private float destroySeconds;

	void Start()
    {
        StartCoroutine(Destroy());
	}
	
    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(destroySeconds);
        Destroy(gameObject);
    }
}