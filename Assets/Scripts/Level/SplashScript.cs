using UnityEngine;

//Used to play the splash sound when the player enters the water
public class SplashScript : MonoBehaviour
{
    [SerializeField] [Tooltip("Player goes here")] private GameObject player;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //If the collider that's entered the water collider is the player's capsule collider, play the splash sound
        if (other == player.GetComponent<CapsuleCollider2D>())
        {
            audioSource.Play();
        }
    }
}