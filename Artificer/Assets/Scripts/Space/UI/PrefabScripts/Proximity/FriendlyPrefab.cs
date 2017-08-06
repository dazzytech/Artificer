using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//Artificer
using Space.Ship;
using UnityEngine.EventSystems;
using UI;
using Game;
using Space.UI.Proxmity;

namespace Space.UI
{
    /// <summary>
    /// Keeps a referenceto an assigned friendly ship
    /// including ship state and component integrity
    /// </summary>
    public class FriendlyPrefab : SelectableHUDItem
    { 
        #region ATTRIBUTES

        // reference to ship
        private ShipAccessor m_ship;

        // If item was active before enable
        private bool m_activated;

        private uint m_ID;

        [Header("HUD Elements")]

        public static FriendlyHUD Base;

        // viewer window
        [SerializeField]
        private ComponentBuilderUtility ViewerPanel;

        [Header("HUD Prefabs")]

        // Prefab for component viewer
        [SerializeField]
        private GameObject PiecePrefab;

        #region HUD ELEMENTS

        [Header("HUD Elements")]

        // For now displays the ship name
        [SerializeField]
        private Text m_label;

        // displays if ship is safe, attacked or docked
        [SerializeField]
        private Text m_status;

        // displays distance from player
        [SerializeField]
        private Text m_distance;

        #endregion

        #region COLORS

        [Header("Colours")]

        [Header("Background")]

        [SerializeField]
        private Color m_safeColour;

        [SerializeField]
        private Color m_attackColour;

        [SerializeField]
        private Color m_destroyedColour;

        [SerializeField]
        private Color m_dockedColour;

        [Header("Text")]

        [SerializeField]
        private Color m_safeText;

        [SerializeField]
        private Color m_attackText;

        [SerializeField]
        private Color m_destroyedText;

        [SerializeField]
        private Color m_dockedText;

        #endregion

        #endregion

        #region MONOBEHAVIOUR

        void OnEnable()
        {
            if (m_activated)
                StartCoroutine("Step");
        }

        void OnDisable()
        {
            StopAllCoroutines();
        }

        void OnDestroy()
        {
            if(m_ship != null)
                m_ship.OnShipCompleted -= OnShipCreated;
        }

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Pass the friendly attributes to the Prefab to begintracking
        /// </summary>
        /// <param name="newShip"></param>
        public void DefineFriendly(ShipAccessor newShip, uint newID)
        {
            // keep reference
            m_ship = newShip;

            // non highlighted colour
            SetColour(m_safeColour);

            m_interactive = true;

            m_activated = true;

            // assign ID
            m_ID = newID;
            
            // assign listener for when item is created or updated
            m_ship.OnShipCompleted += OnShipCreated;

            // If ship has been built then create ship viewer
            if (newShip.Components.Length > 0)
            {
                OnShipCreated();
            }
        }

        #endregion

        #region EVENT LISTENER

        /// <summary>
        /// Rebuilds or builds ship
        /// visual 
        /// </summary>
        private void OnShipCreated()
        {
            ViewerPanel.BuildShip(m_ship, PiecePrefab);

            // Begin tracking process if active
            if (isActiveAndEnabled)
                StartCoroutine("Step");
        }

        #endregion

        #region COROUTINE

        private IEnumerator Step()
        {
            while (true)
            {
                if (m_ship == null)
                {
                    m_status.text = "Destroyed";
                    m_status.color = m_destroyedText;
                    SetColour(m_destroyedColour);

                    m_interactive = false;

                    m_distance.text = "-";

                    Invoke("DelayDestroy", 10f);
                    break;
                }

                // update name
                m_label.text = m_ship.Data.Name;

                Color newColour = Color.black;

                // Update station status
                switch (m_ship.Status)
                {
                    // change and recolor text based on station state
                    case 0:
                        m_status.text = "Safe";
                        newColour = m_safeColour;
                        m_status.color = m_safeText;
                        break;

                    case 1:
                        m_status.text = "In Combat";
                        newColour = m_attackColour;
                        m_status.color = m_attackText;
                        break;

                    case 2:
                        m_status.text = "Docked";
                        newColour = m_dockedColour;
                        m_status.color = m_destroyedText;
                        break;
                }

                // display colour

                // Find distance from station to player

                // Retrieve player object and check if 
                // Player object currently exists
                float distance = m_ship.Distance;

                if (distance == -1)
                {
                    m_distance.text = "-";
                    yield return null;
                }

                // display distance
                m_distance.text = ((int)distance * 0.01).ToString("F2") + "km";

                if (m_standardColour != newColour)
                {
                    SetColour(newColour);
                }

                yield return null;
            }

            // This HUD is due for removal
            Base.RemoveID(m_ID);

            yield break;
        }

        /// <summary>
        /// Allow the player to see that the 
        /// ship has been destroyed before removing
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private void DelayDestroy()
        {
            Destroy(this.gameObject);
            StopAllCoroutines();
        }

        #endregion
    }
}