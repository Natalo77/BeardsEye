using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DrowningScript : MonoBehaviour
{
    [SerializeField] [Tooltip("Number of seconds between heart beats")] private float heartbeatDelay;
    private float dynamicHeartbeatDelay;
    [SerializeField] [Tooltip("Smallest number of seconds between heart beats when drowning")] private float heartbeatDelayDrowning;
    [SerializeField] [Tooltip("Heart UI object goes here")] private GameObject heart;
    [SerializeField] [Tooltip("Size: 2\nElement 0: Alive heart\nElement 1: Dead heart")] private Sprite[] heartSprites;
    private RectTransform heartRectTransform;
    private Vector2 heartSize;
    private Vector2 dynamicHeartSize;
    [SerializeField] [Tooltip("How much the heart shrinks with each beat")] private float heartSizeMin;

    [Space(5)]
    [SerializeField] [Tooltip("How many seconds the player can stay underwater before they 'drown'")] private float drownTime;
    private float dynamicDrownTime;

    [Space(5)]
    [SerializeField] [Tooltip("Game Over from Canvas goes here")] private GameObject GameOver;

    [Space(5)]
    [SerializeField] [Tooltip("'Game Over' Audio Source goes here")] private AudioSource audioSource;

    void OnValidate()
    {
        if (heartbeatDelay < 0)
        {
            heartbeatDelay *= -1;
        }
        if (heartbeatDelayDrowning > heartbeatDelay)
        {
            heartbeatDelayDrowning = heartbeatDelay;
        }
        if (heartSizeMin > 1)
        {
            heartSizeMin = 1;
        }
        if (drownTime < 0)
        {
            drownTime *= -1;
        }
    }

    void Start()
    {
        heartRectTransform = heart.GetComponent<RectTransform>();
        heartSize = heartRectTransform.sizeDelta;
        dynamicHeartSize = heartSize;
        dynamicHeartbeatDelay = heartbeatDelay;
        dynamicDrownTime = drownTime;
    }

    void Update()
    {
        //Saving all of the player collider's vertices to a list
        List<Vector2> allPlayerColliderPoints = gameObject.GetComponent<PolygonCollider2D>().points.ToList();
        //Converts the player collider's vertices from local space to world space
        for (int i = 0; i < allPlayerColliderPoints.Count; i++)
        {
            allPlayerColliderPoints[i] = transform.TransformPoint(allPlayerColliderPoints[i]);
        }
        List<Vector2> dryPlayerColliderPoints = new List<Vector2>(allPlayerColliderPoints);
        /*Iterating through all tagged 'Water' objects, along with all of their colliders,
        and removing any vertices in dryPlayerColliderPoints that they overlap*/
        foreach (GameObject water in GameObject.FindGameObjectsWithTag("Water"))
        {
            foreach (Collider2D waterCollider in water.GetComponents<Collider2D>())
            {
                foreach (Vector2 playerColliderPoint in allPlayerColliderPoints)
                {
                    if (waterCollider.OverlapPoint(playerColliderPoint))
                    {
                        dryPlayerColliderPoints.Remove(playerColliderPoint);
                    }
                }
            }
        }
        /*If there are no remaining 'dry' vertices (dryPlayerColliderPoints is empty),
        and if the 'drown timer' has run out (dynamicDrownTime <= 0), the game ends.
        If there's still time left before the player drowns,
        the 'drown timer' is decreased (dynamicDrownTime -= Time.deltaTime).
        If there are remaining 'dry' vertices (dryPlayerColliderPoints isn't empty),
        then the player can 'breathe' and the 'drown timer' is reset (dynamicDrownTime = drownTime).*/
        if (!dryPlayerColliderPoints.Any())
        {
            if (dynamicDrownTime - Time.deltaTime <= 0)
            {
                audioSource.Play();
                GameOver.GetComponent<GameOverUIManager>().populateGameOver();
                UIManager.changeGameState(2);
                heart.GetComponent<Image>().overrideSprite = heartSprites[1];
            }
            else
            {
                dynamicDrownTime -= Time.deltaTime;
            }
            GetComponent<CustomPlayerController>().canMove = 1;
            GetComponent<CustomPlayerController>().slipCooldown = false;
        }
        else
        {
            dynamicDrownTime = drownTime;
        }
        /*Modifying the size of the heart each frame to simulate heartbeats
        Heartbeats get faster the longer the player is drowning for*/
        if (dynamicHeartbeatDelay <= 0)
        {
            dynamicHeartSize = heartSize;
            dynamicHeartbeatDelay = heartbeatDelay;
        }
        else
        {
            //'heartbeatDelayDrowning + (heartbeatDelay - heartbeatDelayDrowning)' is the time difference between a heartbeat when not drowning and a heartbeat when fully drowning
            //'dynamicDrownTime / drownTime' indicates how long the player has been drowning for (1 being not drowning at all and 0 being fully drowned)
            dynamicHeartbeatDelay -= Time.deltaTime / (heartbeatDelayDrowning + (heartbeatDelay - heartbeatDelayDrowning) * (dynamicDrownTime / drownTime));
            //'(heartSize - (heartSize * heartSizeMin))' is the size difference between a heartbeat when not drowning and a heartbeat when fully drowning
            dynamicHeartSize -= (heartSize - (heartSize * heartSizeMin)) * (Time.deltaTime / (heartbeatDelayDrowning + (heartbeatDelay - heartbeatDelayDrowning) * (dynamicDrownTime / drownTime)));
        }
        heartRectTransform.sizeDelta = dynamicHeartSize;
    }
}