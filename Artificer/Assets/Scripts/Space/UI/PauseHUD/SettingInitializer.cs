using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Space.UI.Pause
{
    public class SettingInitializer : MonoBehaviour
    {
        // HUD elements
        public Slider MasterVolume;
        public Slider AmbVolume;
        public Slider SFXVolume;
        public Slider MusicVolume;

        // Use this for initialization
        void Awake()
        {
            MasterVolume.value = Audio_Config.Audio.Master;
            AmbVolume.value = Audio_Config.Audio.Ambience;
            SFXVolume.value = Audio_Config.Audio.SoundFX;
            MusicVolume.value = Audio_Config.Audio.Music;
        }
    }
}

