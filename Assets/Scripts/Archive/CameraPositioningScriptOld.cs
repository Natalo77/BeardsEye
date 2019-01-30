using UnityEngine;

public class CameraPositioningScriptOld : MonoBehaviour
{
    private Camera camera;
    [SerializeField] [Tooltip("Size: 4\nElement 0: Top Left\nElement 1: Top Right\nElement 2: Bottom Left\nElement 3: Bottom Right")] private GameObject[] levelPositions;

    void Start()
    {
        camera = Camera.main;
    }

	void Update()
    {
        var cameraPosition = new Vector3(GameObject.FindWithTag("Player").transform.position.x, GameObject.FindWithTag("Player").transform.position.y, camera.transform.position.z);

        var cameraTopLeft = (Vector2)camera.ScreenToWorldPoint(new Vector3(0, camera.pixelHeight, camera.nearClipPlane));
        var cameraTopRight = (Vector2)camera.ScreenToWorldPoint(new Vector3(camera.pixelWidth, camera.pixelHeight, camera.nearClipPlane));
        var cameraBottomLeft = (Vector2)camera.ScreenToWorldPoint(new Vector3(0, 0, camera.nearClipPlane));
        var cameraBottomRight = (Vector2)camera.ScreenToWorldPoint(new Vector3(camera.pixelWidth, 0, camera.nearClipPlane));

        var levelTopLeft = (Vector2)levelPositions[0].transform.position;
        var levelTopRight = (Vector2)levelPositions[1].transform.position;
        var levelBottomLeft = (Vector2)levelPositions[2].transform.position;
        var levelBottomRight = (Vector2)levelPositions[3].transform.position;

        var points = new Vector2[][]{new Vector2[]{cameraTopLeft, levelTopLeft},
                                     new Vector2[]{cameraTopRight, levelTopRight},
                                     new Vector2[]{cameraBottomLeft, levelBottomLeft},
                                     new Vector2[]{cameraBottomRight, levelBottomRight}};

        for (int i = 0; i < points.Length; i++)
        {
            if (i == 0 || i == 1)
            {
                if (points[i][0].y > points[i][1].y)
                {
                    //camera.transform.position = new Vector2(camera.transform.position.x, camera.transform.position.y - (points[i][0].y - points[i][1].y));
                    cameraPosition = new Vector3(cameraPosition.x, cameraPosition.y - (points[i][0].y - points[i][1].y), cameraPosition.z);
                }
            }
            if (i == 2 || i == 3)
            {
                if (points[i][0].y < points[i][1].y)
                {
                    print(points[i][1].y + points[i][0].y);
                    //camera.transform.position = new Vector2(camera.transform.position.x, camera.transform.position.y + (points[i][1].y + points[i][0].y));
                    cameraPosition = new Vector3(cameraPosition.x, cameraPosition.y + (points[i][1].y + points[i][0].y), cameraPosition.z);
                }
            }
            if (i == 0 || i == 2)
            {
                if (points[i][0].x < points[i][1].x)
                {
                    //camera.transform.position = new Vector2(camera.transform.position.x, camera.transform.position.y + (points[i][1].x + points[i][0].x));
                    cameraPosition = new Vector3(cameraPosition.x + (points[i][1].x + points[i][0].x), cameraPosition.y, cameraPosition.z);
                }
            }
            if (i == 1 || i == 3)
            {
                if (points[i][0].x > points[i][1].x)
                {
                    //camera.transform.position = new Vector2(camera.transform.position.x, camera.transform.position.y - (points[i][0].x - points[i][1].x));
                    cameraPosition = new Vector3(cameraPosition.x - (points[i][0].x - points[i][1].x), cameraPosition.y, cameraPosition.z);
                }
            }
        }

        camera.transform.position = cameraPosition;
    }
}