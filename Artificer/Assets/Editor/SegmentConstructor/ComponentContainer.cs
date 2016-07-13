using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

namespace Editor
{
	/// <summary>
	/// Component container.
	/// Create serializable data for storage
	/// </summary>
	[XmlRoot("segment", IsNullable = false)]
	public class ComponentContainer
	{
		[XmlAttribute("X")]
		public string x = "0";
		[XmlAttribute("Y")]
		public string y = "0";
		[XmlAttribute("SpaceX")]
		public string pX = "0";
		[XmlAttribute("SpaceY")]
		public string pY = "0";
		[XmlAttribute("sizeX")]
		public string sizeX = "0";
		[XmlAttribute("sizeY")]
		public string sizeY = "0";
        [XmlAttribute("threat")]
        public string threat = "low";

		[XmlElementAttribute("base", typeof(BaseComponent))]
		[XmlElementAttribute("station", typeof(StationComponent))]
		[XmlElementAttribute("asteroid", typeof(AsteroidComponent))]
		[XmlElementAttribute("satellite", typeof(SatelliteComponent))]
		public List<BaseComponent> stations = new List<BaseComponent> ();


		public void Save(string name, List<BaseComponent> comps)
		{
			stations = comps;

			XmlSerializer serializer = new XmlSerializer(typeof(ComponentContainer));
			using(FileStream stream = new FileStream("Assets/Resources" +
				"/Space/Segments/"+name+".xml", FileMode.Create))
			{
				serializer.Serialize(stream, this);
	        }
	    }

		public ComponentContainer Load(string name)
		{
			var serializer = new XmlSerializer(typeof(ComponentContainer));
			using(var stream = new FileStream("Assets/Resources" +
			                                  "/Space/Segments/"+name+".xml", FileMode.Open))
			{
				return serializer.Deserialize(stream) as ComponentContainer;
            }
        }

		public void PopComps(out List<BaseComponent> comps)
		{
			comps = new List<BaseComponent> ();
			comps = stations;
		}
	}
}

