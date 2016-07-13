using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
// Artificer
using Data.Shared;
using Data.Space.Library;

namespace Data.Space.DataImporter
{
    public class ElementDataImporter
    {
        public static void BuildElementLibrary(ElementLibrary library)
        {
            TextAsset txtAsset = (TextAsset)Resources.Load 
                ("Space/Keys/element_key") as TextAsset;

            XmlDocument baseElement = new XmlDocument();
            baseElement.LoadXml(txtAsset.text);

            XmlNode elementContainer = baseElement.LastChild;

            List<MaterialData> mats = new List<MaterialData>();

            foreach (XmlNode xmlElement
                    in elementContainer.ChildNodes)
            {
                MaterialData material = new MaterialData();
                material.Name = xmlElement.Attributes["name"].Value;
                material.Element = xmlElement.Attributes["PTE"].Value;
                material.Description = xmlElement.Attributes["desc"].Value;
                material.Density = float.Parse(xmlElement.Attributes["dens"].Value);

                mats.Add(material);
            }

            library.AssignData(mats.ToArray());
        }
    }
}
