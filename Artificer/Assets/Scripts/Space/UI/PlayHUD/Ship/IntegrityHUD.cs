using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
// Artificer Defined
using Space.Ship;
using Space.Ship.Components.Listener;
using Space.Ship.Components.Attributes;
using UI;

namespace Space.UI.Ship
{
    public class IntegrityHUD : HUDPanel
    {
        #region ATTRIBUTES

        /// <summary>
        /// Accessor for ship we are tracking
        /// </summary>
        private ShipAccessor m_ship;

        /// <summary>
        /// Prefab used for our component viewer
        /// </summary>
        [SerializeField]
        private GameObject PiecePrefab;

        #region HUD ELEMENTS

        [Header("HUD Elements")]

        /// <summary>
        /// Displays the 
        /// </summary>
        [SerializeField]
        private ComponentBuilderUtility ViewerPanel;

        /// <summary>
        /// Displays what state we are currently in
        /// </summary>
        [SerializeField]
        private Text m_status;

        #endregion

        #region COLORS

        [Header("Colours")]

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

        #region MONO BEHAVIOUR

        void OnDestroy()
        {
            if (m_ship != null)
                m_ship.OnShipCompleted -= OnShipCreated;
        }

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Initialise object and 
        /// begin listening for ship update
        /// </summary>
        /// <param name="ship"></param>
        public void SetShip(ShipAccessor ship)
        {
            m_ship = ship;

            // assign listener for when item is created or updated
            m_ship.OnShipCompleted += OnShipCreated;

            // If ship has been built then create ship viewer
            if (m_ship.Components.Length > 0)
            {
                OnShipCreated();
            }
        }

        #endregion

        #region PRIVATE UTILITIES

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

                    break;
                }

                // Update station status
                switch (m_ship.Status)
                {
                    // change and recolor text based on state
                    case 0:
                        m_status.text = "Safe";
                        m_status.color = m_safeText;
                        break;

                    case 1:
                        m_status.text = "In Combat";
                        m_status.color = m_attackText;
                        break;

                    case 2:
                        m_status.text = "Docked";
                        m_status.color = m_destroyedText;
                        break;
                }

                yield return null;
            }

            yield break;
        }

        #endregion
    }
}
