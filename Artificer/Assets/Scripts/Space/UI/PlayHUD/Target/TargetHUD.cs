using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using Space.Ship;
using Space.Ship.Components.Listener;
using Space.Ship.Components.Attributes;
using System.Linq;

namespace Space.UI.Ship
{
    #region SHIP SELECT

    /// <summary>
    /// Container class for selecting
    /// other ships
    /// </summary>
    public class ShipSelect
    {
        public ShipAccessor Ship;
        public List<Transform> TargetedComponents;
    }

    #endregion

    /// <summary>
    /// Keeps track of targets that
    /// the player shit has selected
    /// </summary>
    public class TargetHUD : HUDPanel
    {
        #region ATTRIBUTES

        [Header("Target HUD")]

        /// <summary>
        /// Accessor to our current ship
        /// </summary>
        private ShipAccessor m_shipRef;

        #region VIEWER STYLE

        /// <summary>
        /// if we track ourselves being targeted
        /// </summary>
        private bool m_trackSelf;

        /// <summary>
        /// Track selected enemies
        /// </summary>
        private bool m_trackEnemies;

        /// <summary>
        /// Track selected friendlies
        /// </summary>
        private bool m_trackFriendly;

        #endregion

        /// <summary>
        /// Current target we are engaged in combat with
        /// </summary>
        private Transform m_combatant;

        /// <summary>
        /// prefab list for selected target
        /// </summary>
        private List<TargetShipItem> m_shipTargets;

        #region PREFABS

        [Header("HUD Prefab")]

        [SerializeField]
        private GameObject m_shipPrefab;

        #endregion

        #region COLORS

        [Header("Prefab Colour")]

        ///<summary>
        /// If selecting friendly
        /// display friendly colour 
        /// </summary>
        [SerializeField]
        private Color m_friendlyColour;

        /// <summary>
        /// When selecting self
        /// </summary>
        [SerializeField]
        private Color m_selfColour;

        /// <summary>
        /// When selecting enemies
        /// </summary>
        [SerializeField]
        private Color m_enemyColour;

        #endregion

        #region HUD ELEMENTS

        [Header("HUD Elements")]

        /// <summary>
        /// Used to display current combatant
        /// </summary>
        [SerializeField]
        private TargetViewer m_targetViewer;

        #endregion

        #endregion

        public Toggle _AutoFire;

        #region MONOBEHAVIOUR 

        // Quick utlity to clear target list upon death
        protected override void OnEnable()
        {
            base.OnEnable();

            SystemManager.Space.PlayerExitScene += PlayerDeath;

            m_combatant = null;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if(SystemManager.Space != null)
                SystemManager.Space.PlayerExitScene += PlayerDeath;

            m_combatant = null;
        }

        void Update()
        {
            // need ship reference to run
            if (m_shipRef == null)
                return;

            // First update the 
            // target player has engaged
            DisplayCurrentTarget();

            // If the HUD is tracking
            // selected targets, update
            // said targets
            if (m_trackEnemies)
            {
                // Update targeted ship list
                if(m_shipTargets != null)
                    UpdateShipTargets();

                SeekShipTargets();
            }
        }

        #endregion

        #region PUBLIC INTERACTION

        // Use this for initialization
        public void SetShip(ShipAccessor data)
        {
            m_shipRef = data;
            
            // always true atm
            m_trackEnemies = true;

            // Could we specify listeners here for targeted ship
            // list changed?

            /* _markers = new List<Marker>();
              _pending = new List<Transform>();

            if (m_shipRef.Targeter.Count > 0)
            {
                if (_targeterHUD.gameObject.activeSelf == false)
                    _targeterHUD.gameObject.SetActive(true);
                m_trackTargets = true;
            } else if (m_shipRef.Launchers.Count > 0)
            {
                    if (_targeterHUD.gameObject.activeSelf == true)
                        _targeterHUD.gameObject.SetActive(false);
                    m_trackTargets = true;
            }else
                m_trackTargets = false; */
        }

        #endregion

        #region PRIVATE UTILITIES

        #region SINGLE TARGET

        /// <summary>
        /// Updates the current target within
        /// the header bar if in combat
        /// </summary>
        private void DisplayCurrentTarget()
        {
            // Update tracker if we are in or
            // left combat
            if (m_shipRef.Target != null && m_combatant != m_shipRef.Target)
                m_targetViewer.BuildTrackingObject(m_shipRef.Target);
            else if (m_shipRef.Target == null && m_combatant != null)
                m_targetViewer.ClearObject();

            // keep track of combat state
            m_combatant = m_shipRef.Target;
        }

        #endregion

        #region TARGET LISTS

        /// <summary>
        /// Iterates through each targeted
        /// ship and remove any that are null
        /// or no longer targeted
        /// </summary>
        private void UpdateShipTargets()
        {
            // Use a forloop so we are able
            // to remove an item if we need to
            for (int i = 0; i < m_shipTargets.Count; i++)
            {
                TargetShipItem currentTarget =
                    m_shipTargets[i];

                // test if ship is destroyed
                if(currentTarget.Selected == null)
                {
                    // remove and skip
                    RemoveShipTarget(i);
                    i--;
                    continue;
                }

                // Next test to see if ship is
                // deselected
                if(m_shipRef.Targets.FirstOrDefault
                    (o => o.Ship.Equals(currentTarget.Selected.Ship)) == null)
                {
                    // remove and skip
                    RemoveShipTarget(i);
                    i--;
                    continue;
                }
            }
        }

        /// <summary>
        /// Add any ships that are targeted
        /// but not displayed
        /// </summary>
        private void SeekShipTargets()
        {
            // Loop through each ship target
            foreach (ShipSelect ship in m_shipRef.Targets)
            {
                // if first ship just build
                if (m_shipTargets == null)
                    BuildShipTarget(ship);
                else
                {
                    // Use LINQ to discover if our list 
                    // already contains this ship
                    TargetShipItem item = m_shipTargets.
                        FirstOrDefault(o => o.Selected == ship);

                    // if null then this ship need to be added
                    if (item == null)
                        BuildShipTarget(ship);
                }
            }
        }

        #endregion

        #region CREATE/DESTROY

        /// <summary>
        /// Clears and deletes a targeted ship
        /// </summary>
        /// <param name="index"></param>
        private void RemoveShipTarget(int index)
        {
            m_shipTargets[index].ClearShip();
            Destroy(m_shipTargets[index].gameObject);
            m_shipTargets.RemoveAt(index);
        }

        /// <summary>
        /// Builds HUD element that overlays 
        /// targeted ship within the HUD
        /// </summary>
        /// <param name="ship"></param>
        private void BuildShipTarget(ShipSelect ship)
        {
            // Create HUD element
            GameObject shipObj = Instantiate(m_shipPrefab);

            // Set parent to HUD 
            shipObj.transform.SetParent
                (m_body, false);

            // retreive ship target util
            TargetShipItem target = 
                shipObj.GetComponent<TargetShipItem>();

            // Initialize target item and store
            // in list
            target.BuildShip(ship, m_enemyColour);

            if (m_shipTargets == null)
                m_shipTargets = new List<TargetShipItem>();

            m_shipTargets.Add(target);
        }

        #endregion

        #endregion

        #region EVENT LISTENER

        // Targeter HUD buttons value
        public void ChangeValue()
        {
            foreach (TargeterListener targ in m_shipRef.Targeter)
            {
                TargeterAttributes att = (TargeterAttributes)targ.GetAttributes();
                att.EngageFire = _AutoFire.isOn;
            }
        }

        /// <summary>
        /// Clear the target selection list
        /// upon death
        /// </summary>
        private void PlayerDeath()
        {
            if(m_shipTargets != null)
            {
                for (int i = 0; i < m_shipTargets.Count; i++)
                    // clear each ship
                    RemoveShipTarget(i);
            }
        }

        #endregion
    }
}
