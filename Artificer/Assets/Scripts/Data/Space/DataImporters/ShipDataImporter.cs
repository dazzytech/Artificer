using UnityEngine;
using System;
using System.Collections.Generic;
using System.Xml;
// Artificer
using Data.Shared;
using Data.Space.Library;

namespace Data.Space.DataImporter
{
    public class ShipDataImporter
    {
        #region STATIC IMPORTER

        public static ShipData[] LoadShipDataWreckage()
        {
            // load all the ship schematics within 
            // the ship directories.

            // load to Object and then convert to text
            System.Object[] objAssets = Resources.LoadAll
                ("Space/Wreckage/templates");

            TextAsset[] shipTexts =
                new TextAsset[objAssets.Length];

            for (var i = 0; i < objAssets.Length; i++)
                shipTexts[i] = objAssets[i] as TextAsset;

            // Create empty data objects for the ships
            // and its index
            ShipData[] shipsData =
                new ShipData[shipTexts.Length];
            int shipIndex = 0;

            // Create 
            XmlDocument baseShipXml = new XmlDocument();
            // loop through each ship and create it
            foreach (TextAsset shipText in shipTexts)
            {
                baseShipXml.LoadXml(shipText.text);
                shipsData[shipIndex] = LoadShip(baseShipXml, shipText.name);
                shipIndex++;
            }
            return shipsData;
        }

        /// <summary>
        /// Loops through each stored ship information
        /// and retrives all the information stored in the xml
        /// and places it in SHipData objs
        /// </summary>
        /// <param name="library"></param>
        /// <returns></returns>
        public static bool LoadShipDataAll(ShipLibrary library)
    	{
    		// load all the ship schematics within 
    		// the ship directories.

    		// load to Object and then convert to text
    		System.Object[] objAssets = Resources.LoadAll 
    			("Space/Pre-built_Ships");

    		TextAsset[] shipTexts = 
    			new TextAsset[objAssets.Length]; 

    		for(var i = 0; i < objAssets.Length; i++)
    			 shipTexts[i] = objAssets[i] as TextAsset;

    		// Create empty data objects for the ships
    		// and its index
    		ShipData[] shipsData = 
    			new ShipData[shipTexts.Length];
    		int shipIndex = 0;

    		// Create 
    		XmlDocument baseShipXml = new XmlDocument ();
    		// loop through each ship and create it
    		foreach (TextAsset shipText in shipTexts) {
    			baseShipXml.LoadXml(shipText.text);
                shipsData[shipIndex] = LoadShip(baseShipXml, shipText.name);
                shipIndex++;
    		}

    		library.AssignData (shipsData);
    		return true;
    	}

        /// <summary>
        /// Takes an xml document and enters it within a ship data obj
        /// </summary>
        /// <param name="baseShipXml"></param>
        /// <param name="shipName"></param>
        /// <returns></returns>
        private static ShipData LoadShip(XmlDocument baseShipXml, string shipName)
        {
            XmlNode shipInfo = 
                baseShipXml.LastChild;

            ShipData newShip = new ShipData ();

            newShip.CombatResponsive = true;
            newShip.Initialized = true;

            newShip.Name = shipName;
            if(shipInfo.Attributes ["player"] != null)
                newShip.PlayerMade = shipInfo.Attributes ["player"].Value == "true"? true: false;
            if(shipInfo.Attributes ["rotorFollow"] != null)
                newShip.CombatResponsive = shipInfo.Attributes ["rotorFollow"].Value == "true"? true: false;
            
            foreach(XmlNode componentList
                    in shipInfo.ChildNodes)
            {
                switch(componentList.Name)
                {
                    case "pieces":
                        BuildPieces(componentList, ref newShip);
                        break;
                        
                    case "links":
                        LinkPieces(componentList, ref newShip);
                        break;

                    case "info":
                        WriteData(componentList, ref newShip);
                        break;
                        
                    default:
                        Debug.Log("Ship Data Importer - Load Ship: Unknown List Type in XML");
                        break;
                }
            }

            return newShip;
        }

        #endregion

        #region IMPORTER UTILITIES

        /// <summary>
        /// Builds up the individual components and 
        /// adds them to the reference shipdata obj
        /// </summary>
        /// <param name="componentList"></param>
        /// <param name="shipData"></param>
        public static void BuildPieces
    		(XmlNode componentList, ref ShipData shipData)
    	{
    		foreach(XmlNode componentInfo
    		        in componentList.ChildNodes)
    		{
    			// Create component empty
                Data.Shared.Component newComponent
                    = new Data.Shared.Component();

    			// Populate values
                // unique instance ID
    			newComponent.InstanceID
    				= Convert.ToInt32(componentInfo.
    					Attributes["instanceID"].Value);

    			// The folder the piece is stored in
                newComponent.Folder = componentInfo.
                    Attributes["folder"].Value;

                // Name of the prefab for the piece
                newComponent.Name
                    = componentInfo.
                        Attributes["name"].Value;

                // direction facing
                newComponent.Direction
                    = componentInfo.Attributes["direction"].Value;

                // trigger key
                newComponent.Trigger
                    = componentInfo.Attributes["trigger"].Value;

                newComponent.Style
                    = componentInfo.Attributes["style"].Value;

                if(componentInfo.Attributes["combat"] != null)
                {
                    // trigger key
                    newComponent.CTrigger
                        = componentInfo.Attributes["combat"].Value;
                }

                newComponent.AutoLock = true;
                newComponent.behaviour = 0;
                newComponent.AutoFire = true;

    			// Add to ship
    			switch(componentInfo.Name)
    			{
    			case "head":
    				shipData.AddComponent(newComponent, true);
    				break;
    			case "body":
    				shipData.AddComponent(newComponent, false);
    				break;
    			}
    		}
    	}

        /// <summary>
        /// Adds links between components as sockets
        /// and placed them within the owning components
        /// </summary>
        /// <param name="componentList"></param>
        /// <param name="shipData"></param>
    	public static void LinkPieces
    		(XmlNode componentList,
    		 ref ShipData shipData)
    	{
    		foreach (XmlNode componentInfo
    		        in componentList.ChildNodes) 
    		{
    			int FromID = Convert.ToInt32(componentInfo.
    			         Attributes["IDfrom"].Value);

    			int ToID = Convert.ToInt32(componentInfo.
    			          Attributes["IDto"].Value);

    			int LinkID = Convert.ToInt32(componentInfo.
    			         Attributes["linkID"].Value);

    			int LinkToID = Convert.ToInt32(componentInfo.
    			         Attributes["linkToID"].Value);

                shipData.AddSocket(FromID, LinkID, LinkToID, ToID);
    	    }
    	}

        /// <summary>
        /// Writes descriptive information to 
        /// the ship obj
        /// </summary>
        /// <param name="componentList"></param>
        /// <param name="shipData"></param>
        public static void WriteData(XmlNode componentList,
                                     ref ShipData shipData)
        {
            foreach (XmlNode componentInfo
                     in componentList.ChildNodes)
            {
                // Add to ship
                switch(componentInfo.Name)
                {
                    case "name":
                        if(componentInfo.InnerText != "")
                            shipData.Name = componentInfo.InnerText;
                        break;
                    case "cat":
                        if(componentInfo.InnerText != "")
                            shipData.Category = componentInfo.InnerText;
                        break;
                    case "desc":
                        if(componentInfo.InnerText != "")
                            shipData.Description = componentInfo.InnerText;
                        break;
                }
            }
        }

        #endregion
    }
}
