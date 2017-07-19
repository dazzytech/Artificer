using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Space.Ship.Components.Listener;
using UI;
using Data.Space;
using Space.UI.Station.Viewer.Prefabs;

namespace Space.UI.Station
{
    /// <summary>
    /// Listening compoent for player interaction
    /// with 
    /// </summary>
    public class ShipDockEventListener : MonoBehaviour
    {
        #region ATTRIBUTES

        [SerializeField]
        private ShipDockAttributes m_att;
        [SerializeField]
        private ShipDockController m_con;

        #endregion

        #region EVENTS

        #region VIEWER STATE

        /// <summary>
        /// Called when state is changed
        /// Activates all GOs associated with that state
        /// </summary>
        public void OnStateChanged(int newState)
        {
            switch (newState)
            {
                case 0:
                    m_att.State = DockState.MANAGE;
                    m_con.InitializeManage();
                    break;
                case 1:
                    m_att.State = DockState.EDIT;
                    m_con.InitializeEdit();
                    break;
            }
            
        }

        /// <summary>
        /// Called when a player made
        /// ship is delete so we remove
        /// it from our list
        /// </summary>
        /// <param name="ship"></param>
        public void OnShipDeleted(ShipManagePrefab ship)
        {
            if (m_att.ShipList.Contains(ship))
                m_att.ShipList.Remove(ship);

            ship.OnDelete -= OnShipDeleted;
            ship.OnEdit -= OnShipEdit;
        }

        /// <summary>
        /// Switches state to edit mode
        /// </summary>
        /// <param name="ship"></param>
        public void OnShipEdit(ShipManagePrefab ship)
        {
            OnStateChanged(1);
            m_att.ShipIndex = ship.ID;
            m_att.Editor.LoadExistingShip
                (ship.Ship);
        }

        public void OnEditCurrent()
        {
            OnStateChanged(1);
            m_att.ShipIndex = -1;
            m_att.Editor.LoadExistingShip(m_att.Ship.Ship);
        }

        #endregion

        #region EDITOR 

        public void CreateNewShip()
        {
            //m_att.Editor.CreateNewShip();
        }

        public void SaveShip()
        {
            m_att.Editor.SaveShipData();

            // overwrite existing information
            if (m_att.ShipIndex != -1)
                SystemManager.PlayerShips[m_att.ShipIndex].Ship
                    = m_att.Editor.Ship.Ship;
            else
            {
                // Ship we are currently using has been
                // changed
                // set data and reset ship construction
                m_att.Ship.Ship = m_att.Editor.Ship.Ship;
                
                // Set ship to reset

            }

            m_att.Editor.ClearShip();

            OnStateChanged(0);
        }

        public void DeleteShip()
        {
            m_att.Editor.ClearShip();

            OnStateChanged(0);
        }

        public void GoBack()
        {

        }
            
        #endregion

        public void ExitStation()
        {
            if(!m_att.Viewer.Busy)
                SystemManager.Space.LeaveStation();
        }

        #endregion
    }
}
