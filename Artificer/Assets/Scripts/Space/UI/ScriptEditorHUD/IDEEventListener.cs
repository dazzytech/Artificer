using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Space.UI.IDE
{
    /// <summary>
    /// Events that manage the IDE state or sub objects
    /// </summary>
    public class IDEEventListener : MonoBehaviour
    {
        [SerializeField]
        private IDEController m_con;

        #region BUTTON EVENTS

        /// <summary>
        /// Close IDE and return to game, apply any changes
        /// and re-enable ship
        /// </summary>
        public void ExitIDE()
        {
            SystemManager.UIState.RevertState();

            SystemManager.Space.Ship.EnableShip();
        }

        /// <summary>
        /// 
        /// </summary>
        public void SaveShip()
        {

        }

        public void RevertChanges()
        {

        }

        #endregion
    }
}