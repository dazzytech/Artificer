using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Xml.Serialization;

namespace Editor
{
	public class AsteroidComponent : BaseComponent
	{
		[XmlAttribute("asteroidCount")]
		public string _count;

		[XmlAttribute("width")]
		public string _width;

		[XmlAttribute("height")]
		public string _height;

        [XmlAttribute("val")]
        public string _value;

		public AsteroidComponent()
		{
			windowTitle = "Asteroid Field Component";
			_type = "asteroid";
            _value = "low";
		}

		public override void DrawWindow ()
		{
			base.DrawWindow ();
			
			_count = EditorGUILayout.TextField ("Asteroid Count", _count);
			_width = EditorGUILayout.TextField ("Field Width", _width);
			_height = EditorGUILayout.TextField ("Field Height", _height);
            _value = EditorGUILayout.TextField ("Rock Value", _value);
		}
	}
}

