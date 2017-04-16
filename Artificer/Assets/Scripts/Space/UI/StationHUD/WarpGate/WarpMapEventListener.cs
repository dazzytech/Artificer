using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Space.UI.Station.Map
{
    public class WarpMapEventListener : MonoBehaviour
    {
        #region ATTRIBUTES

        [SerializeField]
        private WarpMapController m_con;

        #endregion

        #region MONO BEHAVIOUR

        private void OnEnable()
        {
            WarpGatePrefab.OnWarpSelected += WarpSelected;
        }

        private void OnDisable()
        {
            WarpGatePrefab.OnWarpSelected -= WarpSelected;
        }

        #endregion

        #region EVENTS

        public void ExitStation()
        {
            SystemManager.Space.LeaveStation();
        }

        private void WarpSelected(WarpGatePrefab warp)
        {
            m_con.SelectGate(warp);
        }

        #endregion
    }
}