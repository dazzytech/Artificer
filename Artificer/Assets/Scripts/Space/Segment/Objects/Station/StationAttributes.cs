using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

namespace Space.Segment
{
    public class StationAttributes : NetworkBehaviour
    {
        #region INTEGRITY

        [SyncVar]
        [HideInInspector]
        public float currentDensity;
        public float Density;

        #endregion


    }
}
