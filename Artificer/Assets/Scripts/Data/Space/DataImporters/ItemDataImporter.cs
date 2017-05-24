using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
// Artificer
using Data.Shared;
using Data.Space.Library;
using Data.Space.Collectable;

namespace Data.Space.DataImporter
{
    public class ItemDataImporter
    {
        #region PUBLIC INTERACTION

        public static void BuildItemLibrary(ItemLibrary library)
        {
            TextAsset txtAsset = (TextAsset)Resources.Load 
                ("Space/Keys/element_key") as TextAsset;

            XmlDocument baseElement = new XmlDocument();
            baseElement.LoadXml(txtAsset.text);

            XmlNode elementContainer = baseElement.LastChild;

            BuildElements(library, baseElement, elementContainer);
        }

        #endregion

        #region PRIVATE UTILITIES

        /// <summary>
        /// Populates the element item region
        /// of the itemlist
        /// </summary>
        /// <param name="library"></param>
        /// <param name="baseElement"></param>
        /// <param name="elementContainer"></param>
        private static void BuildElements
            (ItemLibrary library, XmlDocument baseElement, XmlNode elementContainer)
        {
            foreach (XmlNode xmlElement
                    in elementContainer.ChildNodes)
            {
                ElementItem material = new ElementItem();
                material.Name = xmlElement.Attributes["name"].Value;
                material.Element = xmlElement.Attributes["PTE"].Value;
                material.Description = xmlElement.Attributes["desc"].Value;
                material.Density = float.Parse(xmlElement.Attributes["dens"].Value);

                library.Add(material);
            }
        }

        #endregion

    }
}
