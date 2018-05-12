using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// Artificer Defined
using Space.Ship;
using Space.Ship.Components.Listener;
using Space.Ship.Components.Attributes;
using UI;
using Space.UI.Warp;
using System.Collections.Generic;
using Space.Map;
using UnityEngine.Networking;
using Stations;

namespace Space.UI.Ship
{
    public class WarpHUD : HUDPanel
    {
        #region ATTRIBUTES

        /// <summary>
        /// Store the accessor for the warp component
        /// </summary>
        private WarpListener m_warp;

        [Header("WarpHUD")]

        /// <summary>
        /// Link to the map so that warp prefabs can be placed
        /// </summary>
        [SerializeField]
        private MapViewer m_viewer;

        #region WARP SELECTION

        /// <summary>
        /// When the player selects a point, store the prefab of that map object
        /// </summary>
        private WarpSelectHUDItem m_selectedPoint;

        /// <summary>
        /// All the possible warp points that can be selected
        /// </summary>
        private List<WarpSelectHUDItem> m_warpList;

        /// <summary>
        /// Object that represents a warp item on map
        /// </summary>
        [SerializeField]
        private GameObject m_warpPrefab;

        #endregion

        #region UI ELEMENTS

        [Header("UI Elements")]

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

        #region COLOUR

        [Header("Colour")]

        /// <summary>
        /// Change the bar to this colour to show we are warping
        /// </summary>
        [SerializeField]
        private Color m_warpingColour;

        /// <summary>
        /// change bar to this colour to show a cooldown
        /// </summary>
        [SerializeField]
        private Color m_delayColour;

        /// <summary>
        /// The default colour of a warp icon
        /// </summary>
        [SerializeField]
        private Color m_warpIconColour;

        #endregion  

        #endregion

        #region MONOBEHAVIOUR 

        protected override void OnEnable()
        {
            base.OnEnable();

            Initialize();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            // stop listening to warp
            if(m_warp != null)
                m_warp.OnStateUpdate -= UpdateWarpState;
        }

        private void Start()
        {
            m_viewer.InitializeMap();

            Initialize();
        }

        private void Update()
        {
            if (m_warp != null)
            {
                // update the slider with the delay timer or disable if not in use
                if (m_rechargeBar.gameObject.activeSelf)
                    if(m_warp.WarpEngaged)
                        m_rechargeBar.Value = m_warp.WarpWarmUp;
                    else
                        m_rechargeBar.Value = m_warp.WarpDelay;

                // Update the distance between the warp object
                m_targetDistance.text = ((int)m_warp.TargetDistance * 0.01).ToString("F2") + "km"; ;
            }
        }

        #endregion

        #region PUBLIC INTERACTION

        public void WarpToPoint()
        {
            m_warp.Activate();

            m_selectedPoint.Deselect();
            m_selectedPoint = null;
        }

        /// <summary>
        /// Initializes Warp HUD Elements with the warp listener
        /// </summary>
        public void Initialize()
        {
            GameObject GO = GameObject.FindGameObjectWithTag
                    ("PlayerShip");

            if (GO == null)
                return;

            // Retreive data of warp ship
            m_warp = GO.GetComponent<ShipAccessor>().Warp;

            m_warpDistance.text = ((int)m_warp.WarpDistance * 0.01).ToString("F2") + "km";

            // Assign warp change event and run the warp update initially
            m_warp.OnStateUpdate += UpdateWarpState;
            UpdateWarpState();

            // assign warp points
            BuildWarpGates();
        }

        #endregion

        #region PRIVATE UTILITIES

        /// <summary>
        /// Populate our warp points with the warp gates
        /// </summary>
        private void BuildWarpGates()
        {
            // create a list to store these in future
            foreach (StationAccessor gate in m_warp.AccessibleWarps) // use warp accessor in future
            {
                // Station is warp, create prefab instance
                GameObject warpMapObj = Instantiate(m_warpPrefab);

                // Initialize the selectable item
                WarpSelectHUDItem spawnItem =
                    warpMapObj.GetComponent<WarpSelectHUDItem>();
                spawnItem.Initialize(SelectWarp);
                spawnItem.Reference = gate.transform;

                // Add new prefab to list
                if (m_warpList == null)
                    m_warpList = new List<WarpSelectHUDItem>();

                m_warpList.Add(spawnItem);

                // Update map
                MapObject mObj = SystemManager.Space.GetMapObject
                    (gate.transform);

                if (mObj != null)
                    m_viewer.DeployPrefab(mObj, warpMapObj);
            }
        }

        /// <summary>
        /// populate warp points with waypoints
        /// </summary>
        private void BuildWayPoints()
        {
            // to be added
        }

        /// <summary>
        /// Triggered when the warp component is changed to edit
        /// the HUD
        /// </summary>
        private void UpdateWarpState()
        {
            m_rechargeBar.gameObject.SetActive(false);
            m_warpButton.interactable = false;

            if (m_selectedPoint != null)
                m_targetLabel.text = m_selectedPoint.Reference.name;

            if (m_warp.ReadyToWarp)
            {
                m_statusText.text = "Ready to Warp";
                m_warpButton.interactable = true;
            }
            else
            {
                if (m_warp.WarpEngaged)
                {
                    m_statusText.text = "Warp Engaged";
                    m_rechargeBar.SetColour(m_warpingColour);
                    m_rechargeBar.gameObject.SetActive(true);
                }
                else if (m_warp.WarpDelay > 0.1f)
                {
                    m_statusText.text = "Warp in Cooldown";
                    m_rechargeBar.SetColour(m_delayColour);
                    m_rechargeBar.gameObject.SetActive(true);                 
                }
                else if (m_selectedPoint == null)
                    m_statusText.text = "Select a Warp Point";
                else if (m_warp.TargetDistance > m_warp.WarpDistance)
                    m_statusText.text = "Out of Range";
                else if (m_warp.TargetDistance < m_warp.WarpMinDistance)
                    m_statusText.text = "Too Close to Engage Warp";
                else if (m_warp.CombatState)
                    m_statusText.text = "Cannot Warp While Ship in Combat";
            }
        }

        /// <summary>
        /// Selects that point for the ship to jump to
        /// </summary>
        /// <param name="selected"></param>
        private void SelectWarp(SelectableHUDItem selected)
        {
            // don't change anything while ship is warping
            if (!m_warp.WarpAvailable)
                return;

            // add to attributes
            m_selectedPoint = (WarpSelectHUDItem)selected;

            // update the visual appearance of all selections
            foreach (WarpSelectHUDItem warpGate in m_warpList)
                if (!m_selectedPoint.Equals(warpGate))
                    warpGate.Deselect();
                else
                    warpGate.Select();

            // update the warp and HUD
            m_warp.SetPoint(m_selectedPoint.Reference.position);

            UpdateWarpState();
        }

        #endregion

    }
}
