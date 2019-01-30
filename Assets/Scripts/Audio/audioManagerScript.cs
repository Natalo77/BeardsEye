using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

//Allows the volume level of the different audio mixer groups to be modified (and saved using PlayerPrefs)
public class audioManagerScript : MonoBehaviour
{
    [SerializeField] [Tooltip("Audio Mixer goes here")] private AudioMixer audioMixer;

    //Setting the initial values of the sliders to their corresponding audio mixer values
    void Start()
    {
        foreach (string mixerName in new string[]{"masterVolume", "musicVolume", "sfxVolume"})
        {
            if (name.ToLower().Contains(mixerName.ToLower()))
            {
                float sliderValue;
                audioMixer.GetFloat(mixerName, out sliderValue);
                GetComponent<Slider>().value = sliderValue;
            }
        }
    }

    public void setMasterVolume(float masterVolume)
    {
        audioMixer.SetFloat("masterVolume", masterVolume);
        PlayerPrefs.SetFloat("masterVolume", masterVolume);
        PlayerPrefs.Save();
    }

    public void setMusicVolume(float musicVolume)
    {
        audioMixer.SetFloat("musicVolume", musicVolume);
        PlayerPrefs.SetFloat("musicVolume", musicVolume);
        PlayerPrefs.Save();
    }

    public void setSFXVolume(float sfxVolume)
    {
        audioMixer.SetFloat("sfxVolume", sfxVolume);
        PlayerPrefs.SetFloat("sfxVolume", sfxVolume);
        PlayerPrefs.Save();
    }
}