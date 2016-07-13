using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Xml.Serialization;

namespace Editor
{
	public class StationComponent : BaseComponent
	{
		[XmlAttribute("name")]
		public string _name;

		[XmlAttribute("path")]
		public string _texName;

        [XmlAttribute("faction")]
        public string _faction;

        [XmlAttribute("val")]
        public string _value;

		public StationComponent()
		{
			windowTitle = "Station Component";
			_type = "station";
		}

		public override void DrawWindow()
		{
			base.DrawWindow ();

			_name = EditorGUILayout.TextField ("Station Name", _name);
			_texName = EditorGUILayout.TextField ("Texture Name", _texName);
            _faction = EditorGUILayout.TextField ("Owning Faction", _faction);
            _value = EditorGUILayout.TextField ("Mission Value", _value);
		}
	}
}

