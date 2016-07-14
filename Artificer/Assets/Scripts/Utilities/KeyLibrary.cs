using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class KeyLibrary
{
	static public List<KeyCode> FindKeysPressed()
	{
		List<KeyCode> keys = new List<KeyCode> ();
		int e = System.Enum.GetNames(typeof(KeyCode)).Length;
		for(int i = 0; i < e; i++)
		{
			if(Input.GetKey((KeyCode)i))
			{
				keys.Add((KeyCode)i);
			}
		}
		return keys;
	}

	static public KeyCode FindKeyPressed()
	{
		int e = System.Enum.GetNames(typeof(KeyCode)).Length;
		for(int i = 0; i < e; i++)
		{
			if(Input.GetKey((KeyCode)i))
			{
				return (KeyCode)i;
			}
		}
		return KeyCode.None;
	}

	static public KeyCode FindKeyReleased()
	{
		int e = System.Enum.GetNames(typeof(KeyCode)).Length;
		for(int i = 0; i < e; i++)
		{
			if(Input.GetKeyUp((KeyCode)i))
			{
				return (KeyCode)i;
			}
		}
		return KeyCode.None;
	}
	
	public static string SetString(string SetTo)
	{
		switch(SetTo)
		{
		case "BackQuote":
			SetTo = "`";
			break;
		case "Alpha1":
			SetTo = "1";
			break;
		case "Alpha2":
			SetTo = "2";
			break;
		case "Alpha3":
			SetTo = "3";
			break;
		case "Alpha4":
			SetTo = "4";
			break;
		case "Alpha5":
			SetTo = "5";
			break;
		case "Alpha6":
			SetTo = "6";
			break;
		case "Alpha7":
			SetTo = "7";
			break;
		case "Alpha8":
			SetTo = "8";
			break;
		case "Alpha9":
			SetTo = "9";
			break;
		case "Alpha0":
			SetTo = "0";
			break;
		case "Minus":
			SetTo = "-";
			break;
		case "Equals":
			SetTo = "=";
			break;
		case "Pause":
			SetTo = "Brk";
			break;
		case "Insert":
			SetTo = "Ins";
			break;
		case "LeftShift":
			SetTo = "L ⇧";
			break;
		case "RightShift":
			SetTo = "R ⇧";
			break;
		case "LeftControl":
			SetTo = "Lctrl";
			break;
		case "RightControl":
			SetTo = "Rctrl";
			break;
		case "Delete":
			SetTo = "Del";
			break;
		case "Backspace":
			SetTo = "⌫";
			break;
		case "Return":
			SetTo = "↵";
			break;
		case "Escape":
			SetTo = "Esc";
			break;
		case "Home":
			SetTo = "Hme";
			break;
		case "PageUp":
			SetTo = "Up";
			break;
		case "PageDown":
			SetTo = "Down";
			break;
		case "Quote":
			SetTo = "#";
			break;
		case "SemiColon":
			SetTo = ";";
			break;
		case "Comma":
			SetTo = ",";
			break;
		case "Period":
			SetTo = ".";
			break;
		case "Slash":
			SetTo = "/";
			break;
		case "Backslash":
			SetTo = "\\";
			break;
		case "Tab":
			SetTo = "↹";
			break;
		case "Space":
			SetTo = "␣";
			break;
		case "None":
			SetTo = "_";
			break;
		}
		return SetTo;
	}
}

