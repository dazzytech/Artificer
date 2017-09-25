using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Space.Teams;
using Space.Ship;
using Data.UI;

namespace Stations
{
    // only one type current
    public enum STATIONTYPE {HOME, FOB, WARP, DEPOT, TRADER};

    /// <summary>
    /// Container for attributes for stations 
    /// these include:
    /// Station integrity - damage the station takes
    /// Type (enum) and ID - identify the station
    /// Construction Progress - How far along in construction 
    /// the station is, counts up till reaches integrity
    /// </summary>
    public class StationAttributes : NetworkBehaviour
    {
        #region INTEGRITY

        [SyncVar]
        [HideInInspector]
        public float CurrentIntegrity;
        public float Integrity;

        [HideInInspector]
        [SyncVar]
        public bool UnderAttack;

        [SyncVar]
        public bool Interactive;

        #endregion

        #region IDENTFIER 

        [SyncVar]
        public int SpawnID;

        [HideInInspector]
        public STATIONTYPE Type;

        /// <summary>
        /// Message that displays to the 
        /// player when in range
        /// </summary>
        [HideInInspector]
        public string ProximityMessage;

        #endregion

        #region CONSTRUCTION

        [SyncVar]
        public float BuildCounter;

        public Color BuildColour;

        // diameter of circle that
        // other ships can build in
        public int BuildDistance;

        /// <summary>
        /// Space that the station requrires to be clear
        /// </summary>
        public float ClearRadius;

        #endregion

        #region REFERENCES

        [SyncVar]
        public NetworkInstanceId TeamID;

        /// <summary>
        /// Passed in event to extenal controller
        /// </summary>
        public StationAccessor Accessor;

        /// <summary>
        /// Ship that has interact with the station
        /// </summary>
        public ShipAccessor Ship;

        /// <summary>
        /// Stores refence to prompt
        /// for when player is invited to dock
        /// </summary>
        [HideInInspector]
        public PromptData DockPrompt;

        /// <summary>
        /// Called when player is able to
        /// interact with station
        /// </summary>
        [HideInInspector]
        public PromptData InteractPrompt;

        #endregion

        public int MinDistance;

        [HideInInspector]
        public bool InRange;

        public Sprite Icon;
    }
}
