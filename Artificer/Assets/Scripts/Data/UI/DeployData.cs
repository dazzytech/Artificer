using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data.UI
{
    /// <summary>
    /// Container for information regarding 
    /// deploying stations
    /// </summary>
    [System.Serializable]
    public class DeployData
    {
        /// <summary>
        /// Name
        /// </summary>
        public string Name;
        /// <summary>
        /// This relates to station type 
        /// .e.g FOB, Warp, Depot
        /// </summary>
        public string Type;
        /// <summary>
        /// each station has a short description
        /// detailing uses
        /// </summary>
        public string Description;
        /// <summary>
        /// How much it costs to build this station.
        /// for now it will be currency
        /// </summary>
        public int Cost;
        /// <summary>
        /// Contains the prefab path for the station
        /// icon to display in the build menu.
        /// </summary>
        public string IconPath;
    }
}