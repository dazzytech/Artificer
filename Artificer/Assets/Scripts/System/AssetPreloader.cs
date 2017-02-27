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
        PreloadMaterials();
        PreloadFactions();
    }
    
    private void PreloadShips()
    {
        _att.PrebuiltShips = new ShipLibrary();
        
        ShipDataImporter.
            LoadShipDataAll
                (_att.PrebuiltShips);
    }
    
    private void PreloadMaterials()
    {
        // Does not have a directory as is always loaded within 
        // preload
        _att.ElementLibrary = new ElementLibrary();
        
        ElementDataImporter
            .BuildElementLibrary
                (_att.ElementLibrary);
    }

    private void PreloadFactions()
    {
        _att.FactionLibrary = new FactionLibrary();

        FactionDataImporter.LoadFactions(_att.FactionLibrary);
    }
}

