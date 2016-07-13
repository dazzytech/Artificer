using UnityEngine;
using System.Collections;
using System.IO;
using Serializer;

/// <summary>
/// Audio_ data
/// Container class for audio configuration
/// </summary>
[System.Serializable]
public class Audio_Data
{
    public float MasterVolume;
    public float AmbienceVolume;
    public float SoundEffectsVolume;
    public float MusicVolume;
    public int Type;
}

public class Audio_Config : MonoBehaviour
{
	public static Audio_Config Audio
	{
		get
		{
			if (m_instance == null) m_instance = new GameObject().AddComponent<Audio_Config>();
			return m_instance;
		}
	}
	
	private static Audio_Config m_instance = null;

	void Awake()
	{
		if (m_instance) {
			DestroyImmediate(gameObject);
			return;
		}
		
		m_instance = this;
		
		DontDestroyOnLoad (gameObject);

        // Assign data
        Audio_Data tempData = null;
        if(File.Exists("config/Audio_Data"))
            tempData = Serializer.Serializer.Load<Audio_Data>("config/Audio_Data");

        if (tempData != null)
            m_data = tempData;
        else
        {
            m_data = new Audio_Data();
            SetDefaults();
        }

        // Apply master volume
        AudioListener.volume = Audio.Master;
	}

    private static Audio_Data m_data;

	public void SetDefaults()
	{
		Master = 1f;
		Ambience = 0.80f;
		SoundFX = 0.80f;
		Music = 0.80f;
		AudioType = 1;
	}
	
	public float Master
	{
		get
		{
            return m_data.MasterVolume;
		}
		set
		{
			AudioListener.volume = value;
            m_data.MasterVolume = value;
            Save();
		}
	}

	public float Ambience
	{
		get
		{
            return m_data.AmbienceVolume;
		}
		set
		{
            m_data.AmbienceVolume = value;
            Save();
		}
	}
	
	public float SoundFX
	{
		get
		{
            return m_data.SoundEffectsVolume;
		}
		set
		{
            m_data.SoundEffectsVolume = value;
            Save();
		}
	}
	
	public float Music
	{
		get
		{
            return m_data.MusicVolume;
		}
		set
		{
            m_data.MusicVolume = value;
            Save();
		}
	}

	public int AudioType
	{
		get
		{
			return m_data.Type;
		}
		set
		{
            // if a sound is playing in the sound controller
            // save position of music and replay clip from that point onwards
            SoundController.PauseMusic();
            AudioConfiguration aConfig = AudioSettings.GetConfiguration();
			switch (value) {
			case 0:
                aConfig.speakerMode = AudioSpeakerMode.Mono;
				break;
			case 1:
                aConfig.speakerMode = AudioSpeakerMode.Stereo;
				break;
			case 2:
                aConfig.speakerMode = AudioSpeakerMode.Surround;
				break;
			case 3:
                aConfig.speakerMode = AudioSpeakerMode.Mode5point1;
				break;
			case 4:
                aConfig.speakerMode = AudioSpeakerMode.Mode7point1;
				break;
			default:
				return;
			}
            AudioSettings.Reset(aConfig);
            SoundController.ResumeMusic();
            m_data.Type = value;
            Save();
		}
	}

    private static void Save()
    {  
        if(!File.Exists("config"))
            Directory.CreateDirectory("config");

        Serializer.Serializer.Save<Audio_Data> ("config/Audio_Data", m_data);
    }
}