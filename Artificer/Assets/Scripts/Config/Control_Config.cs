using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class Control_Data
{
    public Dictionary<string, KeyCode> ShipControls;
    public Dictionary<string, KeyCode> CombatControls;
    public Dictionary<string, KeyCode> SystemControls;
}

public class Control_Config: MonoBehaviour
{
    private static Control_Data m_data;

    void Awake()
    {
        // attempt to load data if exists
        Control_Data tempData = null;
        
        if(File.Exists("config/Control_Data"))
            tempData = Serializer.Serializer.Load<Control_Data>("config/Control_Data");

        // assign data if not null
        if (tempData != null)
            m_data = tempData;
        else
        {
            m_data = new Control_Data();
            m_data.ShipControls = new Dictionary<string, KeyCode> ();
            m_data.CombatControls = new Dictionary<string, KeyCode> ();
            m_data.SystemControls = new Dictionary<string, KeyCode> ();
            SetDefaults();
        }
    }
	
	public static void SetDefaults()
	{
		m_data.ShipControls.Clear();
        m_data.CombatControls.Clear();
        m_data.SystemControls.Clear();

		SetShipDefaults();
		SetCombatDefaults();
        SetSystemDefaults();
	}

	private static void SetShipDefaults()
	{
		// Assign ship controls
        m_data.ShipControls.Add ("moveUp", KeyCode.W);
        m_data.ShipControls.Add ("turnLeft", KeyCode.A);
        m_data.ShipControls.Add ("moveDown", KeyCode.S);
        m_data.ShipControls.Add ("turnRight", KeyCode.D);
        m_data.ShipControls.Add ("strafeLeft", KeyCode.Q);
        m_data.ShipControls.Add ("strafeRight", KeyCode.E);
        m_data.ShipControls.Add ("changeState", KeyCode.F);
        m_data.ShipControls.Add ("use", KeyCode.Tab);
        m_data.ShipControls.Add ("fire", KeyCode.Space);
        m_data.ShipControls.Add("deploy", KeyCode.U);
        m_data.ShipControls.Add ("secondary", KeyCode.LeftShift);
        m_data.ShipControls.Add ("tertiary", KeyCode.Tab);
        m_data.ShipControls.Add ("well", KeyCode.Z);
        m_data.ShipControls.Add("jump", KeyCode.X);
        m_data.ShipControls.Add ("eject", KeyCode.LeftControl);
        m_data.ShipControls.Add ("switchtocombat", KeyCode.F);
        m_data.ShipControls.Add ("Activate Shield", KeyCode.C);
        Save();
	}

	private static void SetCombatDefaults()
	{
		//Assign walking controls
        m_data.CombatControls.Add ("moveUp", KeyCode.W);
        m_data.CombatControls.Add ("moveDown", KeyCode.S);
        m_data.CombatControls.Add ("strafeLeft", KeyCode.A);
        m_data.CombatControls.Add ("strafeRight", KeyCode.D);
        m_data.CombatControls.Add ("use", KeyCode.E);
        m_data.CombatControls.Add ("fire", KeyCode.Mouse0);
        m_data.CombatControls.Add ("secondary", KeyCode.Mouse1);
        m_data.CombatControls.Add ("tertiary", KeyCode.Space);
        m_data.CombatControls.Add ("Activate Shield", KeyCode.Space);
		Save();
	}

    private static void SetSystemDefaults()
    {
        m_data.SystemControls.Add("pause", KeyCode.Escape);
        m_data.SystemControls.Add("zoomIn", KeyCode.Plus);
        m_data.SystemControls.Add("zoomOut", KeyCode.Minus);
        m_data.SystemControls.Add("dock", KeyCode.Return);
        m_data.SystemControls.Add("toggle objectives", KeyCode.O);
        m_data.SystemControls.Add("toggle hud", KeyCode.H);
        Save();
    }

	public static void SetNewKey(string KeyToSet, KeyCode SetTo, string type)
	{
        switch (type)
        {
            case "ship":
                if (m_data.ShipControls.ContainsKey (KeyToSet))
                    m_data.ShipControls [KeyToSet] = SetTo;
                break;
            case "combat":
                if (m_data.CombatControls.ContainsKey (KeyToSet))
                    m_data.CombatControls [KeyToSet] = SetTo;
                break;
            case "sys":
                if (m_data.SystemControls.ContainsKey (KeyToSet))
                    m_data.SystemControls [KeyToSet] = SetTo;
                break;
        }
        Save();
	}

	public static KeyCode GetKey(string KeyToGet, string type)
	{
        switch (type)
        {
            case "ship":
                if (m_data.ShipControls.ContainsKey (KeyToGet))
                    return m_data.ShipControls [KeyToGet];
                break;
            case "combat":
                if (m_data.CombatControls.ContainsKey (KeyToGet)) 
                    return m_data.CombatControls [KeyToGet];
                break;
            case "sys":
                if (m_data.SystemControls.ContainsKey (KeyToGet)) 
                    return m_data.SystemControls [KeyToGet];
                break;
            default:
                break;
        }

		return KeyCode.None;
	}

    /// <summary>
    /// Gets the key list.
    /// of specified type for 
    /// settings
    /// </summary>
    /// <returns>The key list.</returns>
    /// <param name="type">Type.</param>
    public static Dictionary<string, KeyCode> GetKeyList(string type)
    {
        switch (type)
        {
            case "ship":
                return m_data.ShipControls;
            case "combat":
                return m_data.CombatControls;
            case "sys":
                return m_data.SystemControls;
        }
        return null;
    }

	private static void Save()
	{
        if(!File.Exists("config"))
            Directory.CreateDirectory("config");

        Serializer.Serializer.Save<Control_Data> ("config/Control_Data", m_data);
	}
}
