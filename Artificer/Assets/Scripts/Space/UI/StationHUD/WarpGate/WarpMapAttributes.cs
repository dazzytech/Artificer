using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Space.UI.Station.Map
{
    public class WarpMapAttributes : MonoBehaviour
    {
        public List<WarpGatePrefab> NearbyWarpGates;

        public WarpGatePrefab SelectedGate;

        #region HUD ELEMENTS

        public Transform WarpGateList;

        #endregion

        #region  PREFABS

        public GameObject WarpGatePrefab;

        #endregion
    }
}
