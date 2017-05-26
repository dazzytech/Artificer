using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
// Artificer
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
                ("Space/Keys/item_key") as TextAsset;

            XmlDocument baseElement = new XmlDocument();
            baseElement.LoadXml(txtAsset.text);

            XmlNode elementContainer = baseElement.LastChild;

            foreach (XmlNode xmlElement
                    in elementContainer.ChildNodes)
            {
                BuildElements(library, xmlElement, xmlElement.Name);
            }
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
            (ItemLibrary library, XmlNode elementContainer, string type)
        {
            foreach (XmlNode xmlElement
                    in elementContainer.ChildNodes)
            {
                if (xmlElement.Name == "#comment")
                    continue;

                ItemData item = null;

                if (type == "elements")
                    item = new ElementItem();
                if (type == "materials")
                    item = new MaterialItem();

                if (item == null)
                    return;

                item.Name = xmlElement.Attributes["name"].Value;
                item.Description = xmlElement.Attributes["desc"].Value;
                item.Density = float.Parse(xmlElement.Attributes["dens"].Value);

                if (type == "materials")
                {
                    // Cast the material as child node and init yield keys
                    MaterialItem material = item as MaterialItem;
                    material.Composition = new string[xmlElement.ChildNodes.Count-1];
                    int i = 0;

                    foreach (XmlNode elemKey
                        in xmlElement.ChildNodes)
                    {
                        if (elemKey.Name == "#comment")
                            continue;

                        material.Composition[i++] = 
                            elemKey.Attributes["key"].Value;
                    }
                }
                library.Add(item);
            }
        }

        #endregion

    }
}
