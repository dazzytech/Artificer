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
        
        public GameObject StationPrefab;

        public bool StationDeployed;

        public Color PendingColour;

        #endregion
    }
}
