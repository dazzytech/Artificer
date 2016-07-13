using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Xml.Serialization;

namespace Editor
{
	public class SatelliteComponent : BaseComponent
	{
		[XmlAttribute("name")]
		public string _name;
		
		[XmlAttribute("path")]
		public string _texName;
		
		public SatelliteComponent()
		{
			windowTitle = "Satellite Component";
			_type = "satellite";
		}
		
		public override void DrawWindow()
		{
			base.DrawWindow ();
			
			_name = EditorGUILayout.TextField ("Satellite Name", _name);
			_texName = EditorGUILayout.TextField ("Texture Name", _texName);
		}
	}
}

