using Space.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Space.UI.Station.Map
{
    public class WarpMapAttributes : MonoBehaviour
    {
        public List<SelectGateItem> NearbyWarpGates;

        public SelectGateItem SelectedGate;

        #region HUD ELEMENT

        public Button WarpButton;
             
        public MapViewer Map;

        #endregion

        #region  PREFABS

        public GameObject WarpGatePrefab;

        #endregion
    }
}
