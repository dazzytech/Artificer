using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;

namespace Space.UI.Station.Map
{
    public class WarpMapEventListener : MonoBehaviour
    {
        #region ATTRIBUTES

        [SerializeField]
        private WarpMapController m_con;

        #endregion

        #region EVENTS

        public void ExitStation()
        {
            SystemManager.Space.LeaveStation();
        }

        public void WarpSelected(SelectableHUDItem warp)
        {
            m_con.SelectGate((SelectGateItem)warp);
        }

        public void WarpToSelection()
        {
            m_con.WarpToGate();
        }

        #endregion
    }
}