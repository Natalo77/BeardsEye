using UnityEngine;
using UnityEngine.Audio;

//Sets the audio mixer values to their saved (PlayerPrefs) values
public class audioStartupScript : MonoBehaviour
{
    [SerializeField] [Tooltip("Audio Mixer goes here")] private AudioMixer audioMixer;

    void Start()
    {
        foreach (string mixerName in new string[]{"masterVolume", "musicVolume", "sfxVolume"})
        {
            audioMixer.SetFloat(mixerName, PlayerPrefs.GetFloat(mixerName));
        }
    }
}