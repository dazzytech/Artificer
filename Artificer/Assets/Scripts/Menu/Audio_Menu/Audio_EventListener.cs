using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Audio_EventListener : MonoBehaviour
{
    // Audio HUD elements
    public Slider MasterVolume;
    public Slider AmbVolume;
    public Slider SFXVolume;
    public Slider MusicVolume;
    public Transform TG;

    void Start()
    {
        // assign default/start values

        switch (Audio.AudioType)
        {
            case 0:
                TG.Find("Mono").GetComponent<Toggle>().isOn = true;
                break;
            case 1:
                TG.Find("Stereo").GetComponent<Toggle>().isOn = true;
                break;
            case 2:
                TG.Find("Surround").GetComponent<Toggle>().isOn = true;
                break;
            case 3:
                TG.Find("Surround 5.1").GetComponent<Toggle>().isOn = true;
                break;
            case 4:
                TG.Find("Surround 7.1").GetComponent<Toggle>().isOn = true;
                break;
        }

        // Assign Initial Sliders
        MasterVolume.value = Audio.Master;
        AmbVolume.value = Audio.Ambience;
        SFXVolume.value = Audio.SoundFX;
        MusicVolume.value = Audio.Music;
    }

    /// <summary>
    /// Sets the volume, using the corresponding sliders
    /// </summary>
    /// <param name="volumeType">Volume type.</param>
    public void SetVolume(string volumeType)
    {
        switch (volumeType)
        {
            case "master":
                Audio.Master = MasterVolume.value;
                break;
            case "amb":
                Audio.Ambience = AmbVolume.value;
                break;
            case "sfx":
                Audio.SoundFX = SFXVolume.value;
                break;
            case "music":
                Audio.Music = MusicVolume.value;
                break;
        }
    }

    /// <summary>
    /// Sets the type of the audio.
    /// </summary>
    /// <param name="index">Index.</param>
    public void SetAudioType(int index)
    {
        // condition to fix bug when setting first button
        if(index != Audio.AudioType)
            Audio.AudioType = index;
    }

    /// <summary>
    /// Sets the defaults.
    /// </summary>
    public void SetDefaults()
    {
        Audio.SetDefaults();
    }

    private Audio_Config Audio
    {
        get{ return Audio_Config.Audio;}
    }
}

