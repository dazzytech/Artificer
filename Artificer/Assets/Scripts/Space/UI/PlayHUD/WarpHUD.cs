using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// Artificer Defined
using Space.Ship;
using Space.Ship.Components.Listener;
using Space.Ship.Components.Attributes;
using UI;

namespace Space.UI.Ship
{
    public class WarpHUD : HUDPanel
    {
        #region ATTRIBUTES

        /// <summary>
        /// Store the accessor for the warp component
        /// </summary>
        private WarpListener m_warp;

        /// <summary>
        /// When the player selects a point, store the prefab of that map object
        /// </summary>
        private GameObject m_selectedPoint;

        #region UI ELEMENTS

        [Header("WarpHUD")]

        /// <summary>
        /// Displays the distance of the warp point
        /// selected
        /// </summary>
        [SerializeField]
        private Text m_targetDistance;

        /// <summary>
        /// Displays the warp distance potiential
        /// </summary>
        [SerializeField]
        private Text m_warpDistance;

        /// <summary>
        /// Displays if the warp is recharging, out of range, or able to warp
        /// </summary>
        [SerializeField]
        private Text m_statusText;

        /// <summary>
        /// Used to enable and disable Warp
        /// </summary>
        [SerializeField]
        private Button m_warpButton;

        /// <summary>
        /// Slider value that shows the delay (And warm up?)
        /// </summary>
        [SerializeField]
        private HUDBar m_rechargeBar;

        /// <summary>
        /// Displays the name of the object that is selected
        /// </summary>
        [SerializeField]
        private Text m_targetLabel;

        #endregion

        #endregion

        #region MONOBEHAVIOUR 

        protected override void OnEnable()
        {
            base.OnEnable();

            BuildShipData();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            // stop listening to warp
            if(m_warp != null)
                m_warp.OnStateUpdate -= UpdateWarpState;
        }

        private void Update()
        {
            if (m_warp != null)
            {
                // update the slider with the delay timer or disable if not in use
                if (m_rechargeBar.gameObject.activeSelf)
                    m_rechargeBar.Value = m_warp.WarpDelay;

                // Update the distance between the warp object
                m_targetDistance.text = m_warp.TargetDistance.ToString() + "km";
            }
        }

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Initializes Warp HUD Elements with the warp listener
        /// </summary>
        public void BuildShipData()
        {
            GameObject GO = GameObject.FindGameObjectWithTag
                    ("PlayerShip");

            if (GO == null)
                return;

            // Retreive data of warp ship
            m_warp = GO.GetComponent<ShipAccessor>().Warp;

            m_warpDistance.text = m_warp.WarpDistance.ToString() + "km";

            // Assign warp change event and run the warp update initially
            m_warp.OnStateUpdate += UpdateWarpState;
            UpdateWarpState();

            // assign warp points
        }

        #endregion

        #region PRIVATE UTILITIES

        /// <summary>
        /// Triggered when the warp component is changed to edit
        /// the HUD
        /// </summary>
        private void UpdateWarpState()
        {
            m_rechargeBar.gameObject.SetActive(false);
            m_warpButton.enabled = false;

            if (m_warp.ReadyToWarp)
            {
                m_statusText.text = "Ready to Warp";
                m_warpButton.enabled = true;
            }
            else
            {
                if (m_selectedPoint == null)
                    m_statusText.text = "Select a Warp Point";
                else if (m_warp.TargetDistance > m_warp.WarpDistance)
                    m_statusText.text = "Out of Range";
                else if (m_warp.CombatState)
                    m_statusText.text = "Cannot Warp While Ship in Combat";
                else
                {
                    m_statusText.text = "Warp in Cooldown";
                    m_rechargeBar.gameObject.SetActive(true);
                }
                
            }
        }

        #endregion

    }
}
