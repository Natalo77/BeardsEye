using System.Collections;
using UnityEngine;

//Makes the camera follow the player, but forces it to stay within the level
public class CameraPositioningScript : MonoBehaviour
{
    //Initializing the array which will store the objects that mark the positions of the four sides of the level
    [SerializeField] [Tooltip("Size: 4\nElement 0: Top\nElement 1: Bottom\nElement 2: Right\nElement 3: Left")] private GameObject[] levelPositions;
    [Space(5)]
    [SerializeField] [Tooltip("How long the camera shakes for")] private float shakeTime;
    [SerializeField] [Tooltip("How powerful the shake is")] private float shakeForce;
    [Space(5)]
    [SerializeField] [Tooltip("HUD-lower from HUD in Canvas goes here")] private GameObject handheldHUD;

    private Camera camera;
    private GameObject player;
    private bool shakeBool;

	void Start()
    {
        camera = GetComponent<Camera>();
        player = GameObject.FindWithTag("Player");
        shakeBool = false;

        if (SystemInfo.deviceType.ToString() == "Handheld")
        {
            handheldHUD.SetActive(true);
            camera.orthographicSize = 10;
        }
    }

    void Update()
    {
        var playerPosition = player.transform.position;

        /*cameraPosition initially stores the player's x & y values, but keeps the camera's z value.
        The camera's x & y positions will therefore mirror the player unless it's modified due to one of it's sides leaving the level.*/
        var cameraPosition = new Vector3(playerPosition.x, playerPosition.y, camera.transform.position.z);
        var cameraHalfHeight = camera.orthographicSize;
        var cameraHalfWidth = cameraHalfHeight * camera.aspect;

        var levelTop = levelPositions[0].transform.position.y;
        var levelBottom = levelPositions[1].transform.position.y;
        var levelRight = levelPositions[2].transform.position.x;
        var levelLeft = levelPositions[3].transform.position.x;

        /*Checking the positions of the sides of the camera against the positions of the sides of the level.
        The position of the camera is moved back into the level if a side goes over the corresponding side of the level.*/
        if (playerPosition.y + cameraHalfHeight > levelTop)
        {
            cameraPosition = new Vector3(cameraPosition.x, levelTop - cameraHalfHeight, cameraPosition.z);
        }
        if (playerPosition.y - cameraHalfHeight < levelBottom)
        {
            cameraPosition = new Vector3(cameraPosition.x, levelBottom + cameraHalfHeight, cameraPosition.z);
        }
        if (playerPosition.x + cameraHalfWidth > levelRight)
        {
            cameraPosition = new Vector3(levelRight - cameraHalfWidth, cameraPosition.y, cameraPosition.z);
        }
        if (playerPosition.x - cameraHalfWidth < levelLeft)
        {
            cameraPosition = new Vector3(levelLeft + cameraHalfWidth, cameraPosition.y, cameraPosition.z);
        }

        //Setting the camera's position to the new cameraPosition
        if (shakeBool)
        {
            camera.transform.position = cameraPosition + Random.insideUnitSphere * shakeForce;
        }
        else
        {
            camera.transform.position = cameraPosition;
        }
    }

    public IEnumerator shakeTimer()
    {
        shakeBool = true;
        yield return new WaitForSeconds(shakeTime);
        shakeBool = false;
    }
}