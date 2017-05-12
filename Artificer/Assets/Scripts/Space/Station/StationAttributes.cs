using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Space.Teams;

namespace Stations
{
    // only one type current
    public enum STATIONTYPE {HOME, FOB, WARP};

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

        [SyncVar]
        public bool UnderAttack;

        [SyncVar]
        public bool Interactive;

        #endregion

        #region IDENTFIER 

        [SyncVar]
        public int SpawnID;
        
        public STATIONTYPE Type;

        #endregion

        #region CONSTRUCTION

        [SyncVar]
        public float BuildCounter;

        public Color BuildColour;

        // diameter of circle that
        // other ships can build in
        public int BuildDistance;

        #endregion

        #region REFERENCES

        [SyncVar]
        public NetworkInstanceId TeamID;

        #endregion

        public int MinDistance;

        /// <summary>
        /// Quick access to station team object
        /// </summary>
        public TeamController Team
        {
            get
            {
                return ClientScene.FindLocalObject(TeamID).GetComponent<TeamController>();
            }
        }

        public Sprite Icon;
    }
}
