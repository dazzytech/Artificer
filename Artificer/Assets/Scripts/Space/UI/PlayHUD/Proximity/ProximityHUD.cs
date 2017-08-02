using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;

namespace Space.UI.Proxmity
{
    /// <summary>
    /// Communicates selection interaction 
    /// between the Friendly, Station and Tracker HUDS
    /// </summary>
    public class ProximityHUD : MonoBehaviour
    {
        #region ATTRIBUTES

        [Header("HUD Elements")]

        [SerializeField]
        private SelectableHUDList[] m_proxLists;

        #endregion

        #region PRIVATE UTILITIES

        public void Select(SelectableHUDItem triggered)
        {
            foreach (SelectableHUDList list in m_proxLists)
                list.SelectItem(triggered.SharedIndex, false);
        }

        public void Hover(SelectableHUDItem triggered)
        {
            foreach (SelectableHUDList list in m_proxLists)
                list.HoverItem(triggered.SharedIndex, false);
        }

        public void Leave(SelectableHUDItem triggered)
        {
            foreach (SelectableHUDList list in m_proxLists)
                list.LeaveItem(triggered.SharedIndex);
        }

        #endregion
    }
}
