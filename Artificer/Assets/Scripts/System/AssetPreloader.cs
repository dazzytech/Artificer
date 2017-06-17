using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
// Artificer
using Data.Shared;
using Data.Space;
using Data.Space.DataImporter;
using Data.Space.Library;

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
}

