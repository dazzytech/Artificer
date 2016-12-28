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

        #endregion

        #region IDENTFIER 

        public int ID;

        public STATIONTYPE Type;

        #endregion

        #region CONSTRUCTION

        public float BuildCounter;

        #endregion

        #region REFERENCES

        public TeamController Team;

        #endregion
    }
}
