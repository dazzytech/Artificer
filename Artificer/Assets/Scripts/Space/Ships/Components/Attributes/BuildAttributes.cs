using Data.UI;
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
        
        public List<DeployData> SpawnableStations;

        public float StationDelay;

        public bool ReadyToDeploy;

        #endregion
    }
}
