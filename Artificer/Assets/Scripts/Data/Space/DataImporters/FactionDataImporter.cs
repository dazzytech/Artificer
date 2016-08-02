using UnityEngine;
using System.Collections;
using System.Xml;

using Data.Space.Library;

namespace Data.Space.DataImporter
{
    public class FactionDataImporter
    {
        #region STATIC IMPORTER
        public static bool LoadFactions(FactionLibrary library)
        {
            // load all the ship schematics within 
            // the ship directories.

            // load to Object and then convert to text
            System.Object[] objAssets = Resources.LoadAll
                ("Space/Factions");

            TextAsset[] factionTexts =
                new TextAsset[objAssets.Length];

            for (var i = 0; i < objAssets.Length; i++)
                factionTexts[i] = objAssets[i] as TextAsset;

            // Create empty data objects for the ships
            // and its index
            FactionData[] factionData =
                new FactionData[factionTexts.Length];
            int factionIndex = 0;

            // Create 
            XmlDocument baseShipXml = new XmlDocument();
            // loop through each ship and create it
            foreach (TextAsset shipText in factionTexts)
            {
                baseShipXml.LoadXml(shipText.text);
                factionData[factionIndex] = LoadFaction(baseShipXml, shipText.name);
                factionIndex++;
            }

            library.AssignData(factionData);
            return true;
        }

        #endregion

        private static FactionData LoadFaction(XmlDocument baseShipXml, string shipName)
        {
            XmlNode factionInfo =
                baseShipXml.LastChild;

            FactionData newFaction = new FactionData();

            foreach (XmlNode componentList
                    in factionInfo.ChildNodes)
            {
                switch (componentList.Name)
                {
                    case "info":
                        newFaction.ID = int.Parse(componentList.Attributes["id"].Value);
                        newFaction.Name = componentList.Attributes["name"].Value;
                        newFaction.Icon = componentList.Attributes["icon"].Value;
                        break;

                    case "desc":
                        newFaction.Description = componentList.InnerText;
                        break;

                    case "styles":
                        newFaction.Styles = new string[componentList.ChildNodes.Count];
                        int i = 0;
                        foreach (XmlNode style
                            in componentList.ChildNodes)
                        {
                            newFaction.Styles[i] = style.InnerText;
                        }
                        break;

                    default:
                        Debug.Log("Faction Data Importer - Load Faction: Unknown List Type in XML");
                        break;
                }
            }

            return newFaction;
        }
    }
}
