using UnityEngine;
using System.Collections;
using System;
using System.IO;

using Menu;



[System.Serializable]
public class Video_Data
{
    public int ScreenWidth;
    public int ScreenHeight;
    public int ScreenHz;
    public bool FS;
    public int vSync;
    public string QLevel;
    public string AA;
    public string TexQual;
}

public class Video_Config : MonoBehaviour
{
    // CONST VARS
    public const int MIN_WIDTH = 800;
    public const int MIN_HEIGHT = 600;

    Video_Data tempData = null;

	public static Video_Config Video
	{
		get
		{
			return m_instance;
		}
	}

	void Awake()
	{
		if (m_instance) {
			DestroyImmediate(gameObject);
			return;
		}
		
		m_instance = this;
		
		DontDestroyOnLoad (gameObject);

        // Assign data
        if(File.Exists("config/Video_Data"))
            tempData = Serializer.Serializer.Load<Video_Data>("config/Video_Data");

        if (tempData != null)
        {
            m_data = tempData;
            Load();
        } else
        {
            m_data = new Video_Data();
            Screen.SetResolution(MIN_WIDTH, MIN_HEIGHT, false);
            m_data.ScreenWidth = MIN_WIDTH;
            m_data.ScreenHeight = MIN_HEIGHT;
            m_data.vSync = VSync;
            m_data.QLevel = Settings;
            m_data.AA = GetAA;
            m_data.TexQual = GetTexQual;
        }
	}

    private static Video_Data m_data = null;
	
	private static Video_Config m_instance = null;

    private void Load()
    {
        Screen.SetResolution(m_data.ScreenWidth, m_data.ScreenHeight, 
                             m_data.FS, m_data.ScreenHz);

        QualitySettings.vSyncCount = m_data.vSync;
    }

	public void SetResolution(Resolution Res, bool fs)
	{
        Screen.SetResolution(Res.width, Res.height, fs, Res.refreshRate);

        tempData = new Video_Data();
        tempData.ScreenWidth = m_data.ScreenWidth;
        tempData.ScreenHeight = m_data.ScreenHeight;
        tempData.ScreenHz = m_data.ScreenHz;
        tempData.FS = m_data.FS;

        m_data.ScreenWidth = Res.width;
        m_data.ScreenHeight = Res.height;
        m_data.ScreenHz = Res.refreshRate;
        m_data.FS = fs;

        Menu.Popup_Dialog.ShowPopup("Confirm Resolution", "Reverting in 10 seconds.", DialogType.YESNOTIMER);
        Menu.Popup_Dialog.OnDialogEvent += ConfirmResolution;
	}

    public void ConfirmResolution(DialogResult result)
    {
        if (result == DialogResult.YES)
        {
            Save();
        } else
        {
            m_data.ScreenWidth = tempData.ScreenWidth;
            m_data.ScreenHeight = tempData.ScreenHeight;
            m_data.ScreenHz = tempData.ScreenHz;
            m_data.FS = tempData.FS;
            Screen.SetResolution(m_data.ScreenWidth, m_data.ScreenHeight,
                                 m_data.FS, m_data.ScreenHz);
        }

        Menu.Popup_Dialog.OnDialogEvent -= ConfirmResolution;
    }

    public bool IsCurrent(Resolution Res)
    {
        if (m_data.ScreenWidth == Res.width &&
            m_data.ScreenHeight == Res.height &&
            m_data.ScreenHz == Res.refreshRate)
            return true;
        return false;
    }
	
	public int VSync
	{
		get { return QualitySettings.vSyncCount;}
		set { 
            if(value >= 0 && value < 3)
            {
    			QualitySettings.vSyncCount = value;
                m_data.vSync = value;
                Save();
            }
		}
	}
	
	public string Settings
	{
		get{ 
			switch (QualitySettings.GetQualityLevel ()) {
			case 0:
				return "Fastest";
			case 1:
				return "Fast";
			case 2:
				return "Simple";
            case 3:
                return "Good";
            case 4:
                return "Beautiful";
            case 5:
                return "Fantastic";
			}
			return "";
		}
		set{
			int level;
			switch (value) {
			case "Fastest":
				level = 0;
				break;
			case "Fast":
				level = 1;
				break;
			case "Simple":
				level = 2;
				break;
            case "Good":
                level = 3;
                break;
            case "Beautiful":
                level = 4;
                break;
            case "Fantastic":
                level = 5;
                break;
			default:
				return;
			}
			QualitySettings.SetQualityLevel (level);

            m_data.QLevel = value;
            m_data.vSync = VSync;
            m_data.QLevel = Settings;
            m_data.AA = GetAA;
            m_data.TexQual = GetTexQual;
            Save();
		}
	}

    public string GetAA
    {
        get
        { 
            switch (QualitySettings.antiAliasing)
            {
                case 0:
                    return "Disabled";
                case 2:
                    return "2X Multi Sampling";
                case 4:
                    return "4X Multi Sampling";
                case 8:
                    return "8X Multi Sampling";
            }
            return "";
        }
    }
    public int SetAA
    {
        set{
            int level;
            switch (value) {
                case 0:
                    m_data.AA =  "Disabled";
                    break;
                case 2:
                    m_data.AA =  "2X Multi Sampling";
                    break;
                case 4:
                    m_data.AA =  "4X Multi Sampling";
                    break;
                case 8:
                    m_data.AA =  "8X Multi Sampling";
                    break;
                default:
                    return;
            }
            QualitySettings.antiAliasing = value;
            Save();
        }
    }

    public string GetTexQual
    {
        get
        { 
            switch (QualitySettings.masterTextureLimit)
            {
                case 0:
                    return "Full Resolution";
                case 1:
                    return "Half Resolution";
                case 2:
                    return "Quarter Resolution";
                case 3:
                    return "Eighth Resolution";
            }
            return "";
        }
    }
    public int SetTexQual
    {
        set{
            switch (value) {
                case 0:
                    m_data.TexQual = "Full Resolution";
                    break;
                case 1:
                    m_data.TexQual = "Half Resolution";
                    break;
                case 2:
                    m_data.TexQual = "Quarter Resolution";
                    break;
                case 3:
                    m_data.TexQual = "Eighth Resolution";
                    break;
                default:
                    return;
            }
            QualitySettings.masterTextureLimit = value;
            Save();
        }
    }

    private static void Save()
    {
        if(!File.Exists("config"))
            Directory.CreateDirectory("config");

        Serializer.Serializer.Save<Video_Data> ("config/Video_Data", m_data);
    }
}