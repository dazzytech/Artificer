using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Space.Ship;
using Space.Ship.Components.Attributes;
using Space.Ship.Components.Listener;

namespace Space.UI.Station
{
    /// <summary>
    /// Performs tasks for the station HUD
    /// such as initializing views
    /// </summary>
    [RequireComponent(typeof(ShipDockAttributes))]
    public class ShipDockController : MonoBehaviour
    {
        #region ATTRIBUTES

        [SerializeField]
        private ShipDockAttributes m_att;

        #endregion

        #region PUBLIC INTERACTION

        #region INITIALIZE

        /// <summary>
        /// Initializes the station HUD.
        /// for now only pass ship atts
        /// Entry Point
        /// </summary>
        /// <param name="param">Parameter.</param>
        public void InitializeHUD(ShipAttributes ship)
        {
            m_att.Ship = ship;

            m_att.State = DockState.MANAGE;
            InitializeManage();
        }

        /// <summary>
        /// Builds the manage specific HUD
        /// and initializes it
        /// </summary>
        public void InitializeManage()
        {
            foreach (GameObject GO in m_att.EditGOs)
                GO.SetActive(false);

            foreach (GameObject GO in m_att.ManageGOs)
                GO.SetActive(true);

            m_att.Viewer.BuildShip(m_att.Ship);
        }

        public void InitializeEdit()
        {
            foreach (GameObject GO in m_att.ManageGOs)
                GO.SetActive(false);

            foreach (GameObject GO in m_att.EditGOs)
                GO.SetActive(true);

            // Initialize Edit Elements
            m_att.Editor.LoadExistingShip
                (m_att.Ship.Ship);
        }

        #endregion

        #endregion
    }
}
