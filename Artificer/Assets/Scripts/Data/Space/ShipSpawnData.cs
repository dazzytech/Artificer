using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

namespace Data.Space
{
    #region SYNCLISTS 

    /// <summary>
    /// Create synced material lists for networking objects
    /// </summary>
    //public class MaterialSyncList : SyncListStruct<ElementItem> { }

    public class SyncListShip : SyncListStruct<ShipSpawnData> { }

    #endregion

    /// <summary>
    /// Spawn ship information 
    /// reference to the ship itself, 
    /// whether the ship is available and
    /// the cost to construct
    /// </summary>
    [System.Serializable]
    public struct ShipSpawnData 
    {
        /// <summary>
        /// The constructed ship data
        /// </summary>
        public ShipData Ship;

        /// <summary>
        /// Assigned to import data into Ship
        /// </summary>
        public string ShipName;

        /// <summary>
        /// If we are currently able to use the ship
        /// </summary>
        public bool Owned;

        // cost to build be added later

        /// <summary>
        /// Amount of credits needed to access the ship
        /// </summary>
        public int UnlockCost;
    }
}
