using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Space.Ship.Components.Listener;
using UI;
using Data.Shared;

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

        #endregion

        #region EDITOR 

        
        public void SelectShip(ShipData ship)
        {
            m_att.Editor.LoadExistingShip(ship);
        }

        public void CreateNewShip()
        {
            //m_att.Editor.CreateNewShip();
        }

        public void SaveShip()
        {
            //m_att.Editor.SaveShipData();
            //SendMessage("UpdateShipList");
        }

        public void DeleteShip()
        {
            //m_att.Editor.ClearShip();
            //SendMessage("UpdateShipList");
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
