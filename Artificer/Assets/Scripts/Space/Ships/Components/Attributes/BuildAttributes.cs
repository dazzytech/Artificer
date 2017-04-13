using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Space.Ship.Components.Attributes
{
    public class BuildAttributes :
        ComponentAttributes
    {
        #region CONSTRUCTION 

        [Header("Builder Attributes")]
        
        public List<string> SpawnableStations;

        public float StationDelay;

        public bool ReadyToDeploy;

        #endregion
    }
}
