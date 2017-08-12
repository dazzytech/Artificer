using Data.Space;
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
        #region EVENTS

        public delegate void ShipManageEvent(ShipManagePrefab ship);

        /// <summary>
        /// Invoked when ship is deleted or 
        /// reverted so that container may update 
        /// their lists
        /// </summary>
        /// <param name="ship"></param>
        public event ShipManageEvent OnDelete;

        /// <summary>
        /// Called when the player opts to
        /// edit this ship
        /// </summary>
        public event ShipManageEvent OnEdit;

        #endregion

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

        private bool Owned
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

        public int UnlockCost
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
            WalletData temp = SystemManager.Wallet;
            if (temp.Withdraw(UnlockCost))
            {
                SystemManager.Wallet = temp;

                Owned = true;
                DisplayOwned();

                SystemManager.Space.StartCoroutine
                    ("UpdatePlayerSpawn", ID);
            }
        }

        /// <summary>
        /// TODO : deletes the ship from
        /// player list if playermade else
        /// overwrite player data with team data
        /// </summary>
        public void Delete()
        {
            if(PlayerMade)
            {
                // This ship is player made
                // so we can just delete it from 
                // existance

                // Create array with one less space
                ShipSpawnData[] ships = new ShipSpawnData
                [SystemManager.PlayerShips.Length - 1];

                // Copy over each ship that isn't ours
                for (int i = 0, a = 0; i < SystemManager.PlayerShips.Length; i++)
                    if (i != m_shipReference)
                        ships[a++] = SystemManager.PlayerShips[i];

                // Replace list with our new one
                SystemManager.PlayerShips = ships;

                // Alert any containers
                if (OnDelete != null)
                    OnDelete(this);

                Destroy(gameObject);
            }
            else
            {
                // We only need to overwrite our local
                // version with the team version
                // ONLY OVERWRITE SHIP DATA
                SystemManager.PlayerShips[m_shipReference].Ship
                    = SystemManager.Space.Team.Ships[m_shipReference].Ship;

                SystemManager.Space.StartCoroutine
                    ("UpdatePlayerSpawn", m_shipReference);
            }
        }

        /// <summary>
        /// ship editor to set edit mode with 
        /// ship data or index
        /// </summary>
        public void Edit()
        {
            OnEdit(this);
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

            if (SystemManager.Wallet.Currency >= UnlockCost)
            {
                // Display the price to unlock
                m_costText.text = string.Format("Unlock for: ¤{0}", UnlockCost);

                m_purchase.interactable = true;
            }
            else
            {
                // Display the price to unlock
                m_costText.text = string.Format("Can't Afford: ¤{0}", UnlockCost);

                m_purchase.interactable = false;
            }
        }

        #endregion
    }
}