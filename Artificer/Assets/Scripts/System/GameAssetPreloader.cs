using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
// Artificer
using Data.Shared;
using Data.Space;
using Data.Space.DataImporter;
using Data.Space.Library;

[RequireComponent(typeof(GameBaseAttributes))]
public class GameAssetPreloader : MonoBehaviour
{
    private GameBaseAttributes _att;
    // Use this for initialization
    void Awake()
    {
        _att = GetComponent<GameBaseAttributes>();
    }
    
    public void PreloadAssets()
    {
        PreloadShips();
        PreloadMaterials();
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
}

