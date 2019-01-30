using UnityEngine;

public class menuMusicStartupScript : MonoBehaviour
{
    [HideInInspector] public static AudioSource audioSource;

    void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.Play();
            DontDestroyOnLoad(audioSource);
        }
    }
}