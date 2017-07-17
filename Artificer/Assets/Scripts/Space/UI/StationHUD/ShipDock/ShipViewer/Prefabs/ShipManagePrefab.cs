using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Space.UI.Station.Viewer.Prefabs
{
    /// <summary>
    /// Extends ship UI prefab
    /// enables the user to edit, unlock
    /// or revert the ship to it's 
    /// original state
    /// </summary>
    public class ShipManagePrefab : ShipUIPrefab
    {
        #region ATTRIBUTES

        #region HUD ELEMENTS

        [Header("Ship Manage Prefabs")]

        [Header("HUD Elements")]

        /// <summary> 
        /// Displays the number of credits
        /// needed to unlock the ship
        /// </summary>
        [SerializeField]
        private Text m_costText;

        /// <summary>
        /// a text item that displays if
        /// owned or created
        /// </summary>
        [SerializeField]
        private Text m_status;

        /// <summary>
        /// Switches to the edit viewer
        /// with this particular ship
        /// </summary>
        [SerializeField]
        private Button m_edit;

        /// <summary>
        /// enabled if the ship is not owned
        /// clicked to buy ship
        /// </summary>
        [SerializeField]
        private Button m_purchase;

        /// <summary>
        /// On player made ships this 
        /// will delete the ship from
        /// the list, otherwise reverts to
        /// old design
        /// </summary>
        [SerializeField]
        private Button m_delete;

        /// <summary>
        /// Can be changed to say
        /// delete or revert
        /// </summary>
        [SerializeField]
        private Text m_deleteText;

        #endregion

        #region COLOURS

        [Header("Colours")]
        
        /// <summary>
        /// Colour that the HUD
        /// element appears when we own it
        /// </summary>
        [SerializeField]
        private Color m_ownedColour;

        #endregion

        #endregion

        #region ACCESSORS

        public bool Owned
        {
            get
            {
                return SystemManager.PlayerShips
                  [m_shipReference].Owned;
            }
            set
            {
                SystemManager.PlayerShips
                  [m_shipReference].Owned = value;
            }
        }

        public bool PlayerMade
        {
            get
            {
                return SystemManager.PlayerShips
                  [m_shipReference].PlayerMade;
            }
        }

        public int Cost
        {
            get
            {
                return SystemManager.PlayerShips
                  [m_shipReference].UnlockCost;
            }
        }

        #endregion
        
        #region PUBLIC INTERACTION

        /// <summary>
        /// For now just accepts the 
        /// purchase and continues
        /// </summary>
        public void Purchase()
        {
            Owned = true;
            DisplayOwned();
        }

        /// <summary>
        /// TODO : deletes the ship from
        /// player list if playermade else
        /// overwrite player data with team data
        /// </summary>
        public void Delete()
        {

        }

        /// <summary>
        /// TODO : Consider calling delegate for 
        /// ship editor to set edit mode with 
        /// ship data or index
        /// </summary>
        public void Edit()
        {

        }

        #region OVERRIDE

        /// <summary>
        /// Invokes functions for displaying 
        /// ship state and UI Buttons
        /// </summary>
        /// <param name="shipIndex"></param>
        public override void AssignShip(int shipIndex)
        {
            base.AssignShip(shipIndex);

            if (Owned)
                DisplayOwned();
            else
                DisplayUnowned();
        }

        /// <summary>
        /// Overriden for now to do nothing
        /// </summary>
        public override void Deselect()
        {
        }

        /// <summary>
        /// Overriden for now to do nothing
        /// </summary>
        public override void Select()
        {
        }

        /// <summary>
        /// Overriden for now to do nothing
        /// </summary>
        public override void Highlight(bool highlighted)
        {
            
        }

        #endregion

        #endregion

        #region PRIVATE UTILITIES

        /// <summary>
        /// recolour the ship 
        /// element and display owned UI
        /// </summary>
        private void DisplayOwned()
        {
            m_background.color = m_ownedColour;
            m_status.gameObject.SetActive(true);
            m_costText.gameObject.SetActive(false);
            m_purchase.gameObject.SetActive(false);
            m_delete.gameObject.SetActive(true);
            m_edit.gameObject.SetActive(true);

            // Player made ships means made by self
            // and not on teamlist
            if (PlayerMade)
            {
                m_status.text = "Privately Owned";
                m_deleteText.text = "Delete";
            }
            else
            {
                m_status.text = "Owned";
                m_deleteText.text = "Revert";
            }       
        }

        /// <summary>
        /// Add ability to purchase the ship
        /// </summary>
        private void DisplayUnowned()
        {
            m_status.gameObject.SetActive(false);
            m_costText.gameObject.SetActive(true);
            m_purchase.gameObject.SetActive(true);
            m_delete.gameObject.SetActive(false);
            m_edit.gameObject.SetActive(false);

            // Display the price to unlock
            m_costText.text = string.Format("Unlock for: ¤{0}", Cost);
        }

        #endregion
    }
}