using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
// Artificer
using Data.Shared;
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
        PreloadShips();
        PreloadItemAssets();
        PreloadFactions();
        AddShipComponentsToNetwork();
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

    private void PreloadAITemplates()
    {
        _att.AILibrary = new AITemplateLibrary();

        AgentDataImporter.BuildTemplates(_att.AILibrary);
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
}

