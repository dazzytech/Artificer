using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Xml.Serialization;

namespace Editor
{
	public abstract class BaseComponent {

		public Rect windowRect;
		public string windowTitle = "";

		[XmlAttribute("X")]
		public string _posX = "";
		[XmlAttribute("Y")]
		public string _posY = "";

		[XmlIgnore]
		public string _type;

		public BaseComponent()
		{

		}

		public virtual void DrawWindow()
		{
			_posX =  EditorGUILayout.TextField ("X", _posX);
			_posY =  EditorGUILayout.TextField ("Y", _posY);
		}
	}
}
