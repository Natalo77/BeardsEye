using System.Collections;
using UnityEngine;

public class DrainScript : MonoBehaviour
{
    [SerializeField] [Tooltip("Size: 3\nElement 0: The idle sprite for the pump\nElement 1: The active sprite for the pump\nElement 2: The cooldown sprite for the pump")] private Sprite[] sprites;
    [SerializeField] [Tooltip("The decrease in the vertical position of the water each second while the pump is activated")] private float waterDecrease;
    [SerializeField] [Tooltip("How many seconds the vertical position of the water will decrease for while the pump is activated")] private float waterDecreaseDuration;
    [SerializeField] [Tooltip("The keyboard key used to activate the pump")] private string activateKey;
    [SerializeField] [Tooltip("How many seconds after the vertical position of the water has finished decreasing until the pump can be activated again")] private float activateDelay;
    private SpriteRenderer spriteRenderer;      //Stores the SpriteRenderer of the object
    private bool canActivate;                   //Boolean stating whether or not the player can activate the pump
    private bool activated;                     //Boolean stating whether or not the pump is currently activated
    private AudioSource audioSource;            //Initializing the audio source

    void OnValidate()
    {
        if (waterDecrease < 0)
        {
            waterDecrease *= -1;
        }
        if (waterDecreaseDuration < 0)
        {
            waterDecreaseDuration *= -1;
        }
        if (activateKey.Length > 1)
        {
            activateKey = activateKey[0].ToString();
        }
        else if (activateKey.Length < 1)
        {
            activateKey = "e";
        }
        if (activateDelay < 0)
        {
            activateDelay *= -1;
        }
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        canActivate = true;
        activated = false;
        audioSource = GetComponent<AudioSource>();
    }

    public void Drain()
    {
        if (canActivate)
        {
            bool breakAll = false;
            foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
            {
                foreach (Collider2D playerCollider in player.GetComponents<Collider2D>())
                {
                    if (GetComponent<Collider2D>().Distance(playerCollider).isOverlapped)
                    {
                        StartCoroutine(activatedTimer());
                        audioSource.Play();
                        breakAll = true;
                        break;
                    }
                }
                if (breakAll == true)
                {
                    breakAll = false;
                    break;
                }
            }
        }
    }

    void Update()
    {
        /*If the 'activeKey' is pressed and 'canActivate' is true,
        the player's colliders are checked against the pump's colliders to see if there's an overlap.
        If there is, 'activatedTimer' is called.*/
        if (Input.GetKey(activateKey))
        {
            Drain();
        }
        /*If 'activated' is true, and the vertical position of the current 'Water' tagged object is greater than -10,
        the vertical position of the current 'Water' tagged object is decreased.*/
        if (activated)
        {
            foreach (GameObject water in GameObject.FindGameObjectsWithTag("Water"))
            {
                if (GetComponent<Collider2D>().Distance(water.GetComponent<BoxCollider2D>()).isOverlapped)    //-36.17 is currently the base Y position for the water
                {
                    water.transform.position = new Vector3(water.transform.position.x,
                                                           water.transform.position.y - (Time.deltaTime * waterDecrease),
                                                           water.transform.position.z);
                }
            }
        }
    }

    IEnumerator activatedTimer()
    {
        GameOverUIManager.timesPumped++;
        /*Setting the pump's sprite to active, disabling further activation,
        declaring that the pump is activated, and waiting for 'waterDecreaseDuration' seconds.*/
        spriteRenderer.sprite = sprites[1];
        canActivate = false;
        activated = true;
        yield return new WaitForSeconds(waterDecreaseDuration);
        /*Setting the pump's sprite to cooldown,
        declaring that the pump isn't activated, and waiting for 'activateDelay' seconds.*/
        spriteRenderer.sprite = sprites[2];
        activated = false;
        yield return new WaitForSeconds(activateDelay);
        //Setting the pump's sprite to idle and enabling further activation.
        spriteRenderer.sprite = sprites[0];
        canActivate = true;
    }
}