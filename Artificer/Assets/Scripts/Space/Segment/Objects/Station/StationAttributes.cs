using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Space.Teams;

namespace Space.Segment
{
    // only one type current
    public enum STATIONTYPE {DEFAULT};

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

        #endregion

        #region IDENTFIER 

        [SyncVar]
        public int ID;

        [SyncVar]
        public STATIONTYPE Type;

        #endregion

        #region CONSTRUCTION

        [SyncVar]
        public float BuildCounter;

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
    }
}
