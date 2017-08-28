using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
// Artificer
using Data.Space;
using Data.Space.DataImporter;
using Data.Space.Library;
using UnityEngine.Networking;

[RequireComponent(typeof(SystemAttributes))]
public class AssetPreloader : MonoBehaviour
{
    [SerializeField]
    private SystemAttributes _att;

    public void PreloadAssets()
    {
        PreloadItemAssets();
        PreloadShips();
        PreloadFactions();
        AddShipComponentsToNetwork();
        PopulateShipSpawns();
    }
    
    private void PreloadShips()
    {
        _att.PrebuiltShips = new ShipLibrary();
        
        ShipDataImporter.
            LoadShipDataAll
                (_att.PrebuiltShips);
    }
    
    private void PreloadItemAssets()
    {
        // Does not have a directory as is always loaded within 
        // preload
        _att.ItemLibrary = new ItemLibrary();
        
        ItemDataImporter
            .BuildItemLibrary
                (_att.ItemLibrary);
    }

    private void PreloadFactions()
    {
        _att.FactionLibrary = new FactionLibrary();

        FactionDataImporter.LoadFactions(_att.FactionLibrary);
    }

    /// <summary>
    /// Adds all our ship components in their 
    /// directories to the network spawn list
    /// automates the process
    /// </summary>
    private void AddShipComponentsToNetwork()
    {
        // Populate the top tab bar with component types
        // Retreive all the directories
        TextAsset ShipKey = Resources.Load("Space/Keys/ship_key",
                                 typeof(TextAsset)) as TextAsset;

        // Base Directory of ship components
        string dir = "Space/Ships/{0}/";

        // Loop through each component and add to the 
        // Server spawn prefabs
        foreach (string category in ShipKey.text.Split(","[0]))
        {
            // Load up all prefabs of that type
            GameObject[] components = Resources.LoadAll
                (string.Format(dir, category), typeof(GameObject))
                .Cast<GameObject>().ToArray(); ;

            // Add them to the network spawn prefab list
            foreach (GameObject component in components)
                ClientScene.RegisterPrefab(component);
        }
    }

    /// <summary>
    /// Assigns the ship data to ship
    /// spawn objects using the provided names
    /// </summary>
    private void PopulateShipSpawns()
    {
        for(int i = 0; i < _att.StarterShips.Length; i++)
        {
            _att.StarterShips[i].Ship = 
                ShipLibrary.GetShip(_att.StarterShips[i].ShipName);
        }
    }
}

