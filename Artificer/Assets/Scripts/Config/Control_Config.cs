using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

/// <summary>
/// Stores all the data about the assigned key
/// </summary>
[System.Serializable]
public class KeyData
{
    /// <summary>
    /// Short form name used to retrieve key
    /// </summary>
    public string ID;
    /// <summary>
    /// Label publically displayed in game
    /// </summary>
    public string Label;
    /// <summary>
    /// category that the key belongs to 
    /// e.g. ship or system
    /// </summary>
    public string Category;
    /// <summary>
    /// KeyCode assigned to key
    /// </summary>
    public KeyCode Key;
}

public class Control_Config: MonoBehaviour
{
    #region ATTRIBUTES

    private static KeyData[] m_data;

    /// <summary>
    /// Default keys to be added to game
    /// changable in editor
    /// </summary>
    [SerializeField]
    private KeyData[] m_defaults;

    private static KeyData[] m_defaultSerialized;

    #endregion

    #region MONO BEHAVIOUR

    void Awake()
    {
        // Move the default keys over to the 
        // our key list
        m_defaultSerialized = m_defaults;

        LoadKeySettings();
    }

    #endregion

    #region PUBLIC INTERACTION

    /// <summary>
    /// Replaces the provided key 
    /// with the new key and saves
    /// </summary>
    /// <param name="KeyToSet"></param>
    /// <param name="SetTo"></param>
    /// <param name="type"></param>
    public static void SetNewKey(string KeyToSet, KeyCode SetTo, string type)
	{
        // find the correct item
        for (int i = 0; i < m_data.Length; i++)
        {
            if (type == m_data[i].Category 
                && KeyToSet == m_data[i].ID)
            {
                // assign our key and return
                m_data[i].Key = SetTo;

                Save();
                return;
            }
        }

        Debug.Log("Error: Control Config - SetNewKey: Provided key was not successfully saved");
    }

    /// <summary>
    /// returns key using ID and type
    /// </summary>
    /// <param name="KeyToGet"></param>
    /// <param name="type"></param>
    /// <returns></returns>
	public static KeyCode GetKey(string KeyToGet, string type)
	{
        // find the correct item
        for (int i = 0; i < m_data.Length; i++)
        {
            if (type == m_data[i].Category && KeyToGet == m_data[i].ID)
            {
                // return the key 
                return m_data[i].Key;
            }
        }

        if(KeyToGet != "")
            Debug.Log(string.Format("Error: Control Config - GetKey: Provided key {0} is not stored in list",
                KeyToGet));

        return KeyCode.None;
	}

    /// <summary>
    /// Uses linq to get the key list
    /// of specified type for settings
    /// </summary>
    /// <returns>The key list.</returns>
    /// <param name="type">Type.</param>
    public static KeyData[] GetKeyList(string type)
    {
        KeyData[] KeyListCatergory =
            m_data.Where(x => x.Category == type).ToArray();

        // warn that key category may not exist
        if(KeyListCatergory.Length == 0)
        {
            Debug.Log("Error: Control Config - GetKeyList: Provided type did not yield keys");
        }

        return KeyListCatergory;
    }

    /// <summary>
    /// Reloads our key bindings with keys set in editor
    /// </summary>
    public static void ReturnToDefaults()
    {
        CopyKeys();

        Save();
    }

    #endregion

    #region PRIVATE UTILITIES

    /// <summary>
    /// Copies our assigned keys into memory then
    /// overwrites with saved keys
    /// </summary>
    private void LoadKeySettings()
    {
        CopyKeys();

        // if we have saved the settings configuration
        // then load the custom key setting
        if (File.Exists("config/Control_Data.txt"))
        {
            using (StreamReader sr = File.OpenText("config/Control_Data.txt"))
            {
                string s = "";
                while ((s = sr.ReadLine()) != null)
                {
                    ReadKey(s);
                }
            }
        }
        else
            Save();
    }

    /// <summary>
    /// Retrieve a key data
    /// then replaces the key inside 
    /// with new
    /// </summary>
    /// <param name="data"></param>
    private void ReadKey(string data)
    {
        int divide = data.IndexOf('/');
        int code = data.IndexOf('=');
        string cat = data.Substring(0, divide);
        string key = data.Substring(divide + 1, code - divide - 1);
        string keyCode = data.Substring(code + 1);

        // find the correct item 
        for (int i = 0; i < m_data.Length; i++)
        {
            if (cat == m_data[i].Category && key == m_data[i].ID)
            {
                // assign our key and return
                m_data[i].Key = (KeyCode)System.Enum.Parse
                    (typeof(KeyCode), keyCode);
                return;
            }
        }
    }

    /// <summary>
    /// writes key list in a text file
    /// format "category/keyid=Alpha1"
    /// </summary>
    private static void Save()
	{
        if(!File.Exists("config"))
            Directory.CreateDirectory("config");

        // Create a file to write to.
        using (StreamWriter sw = File.CreateText("config/Control_Data.txt"))
        {
            foreach(KeyData key in m_data)
            {
                sw.WriteLine(string.Format("{0}/{1}={2}", 
                    key.Category, key.ID, key.Key.ToString()));
            }
        }
    }

    private static void CopyKeys()
    {
        m_data = new KeyData[m_defaultSerialized.Length];
        for (int i = 0; i < m_defaultSerialized.Length; i++)
        {
            m_data[i] = new KeyData();
            m_data[i].Category = m_defaultSerialized[i].Category;
            m_data[i].ID = m_defaultSerialized[i].ID;
            m_data[i].Label = m_defaultSerialized[i].Label;
            m_data[i].Key = m_defaultSerialized[i].Key;
        }
    }

    #endregion
}
