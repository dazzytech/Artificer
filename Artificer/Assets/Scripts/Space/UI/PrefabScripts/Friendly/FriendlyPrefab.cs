﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//Artificer
using Space.Ship;
using UnityEngine.EventSystems;
using UI;
using Game;

namespace Space.UI.Ship
{
    /// <summary>
    /// Keeps a referenceto an assigned friendly ship
    /// including ship state and component integrity
    /// </summary>
    public class FriendlyPrefab : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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

        // For now displays the ship name
        [SerializeField]
        [Header("Label")]
        private Text m_label;

        // displays if ship is safe, attacked or docked
        [SerializeField]
        [Header("Status")]
        private Text m_status;

        // displays distance from player
        [SerializeField]
        [Header("Distance")]
        private Text m_distance;

        [SerializeField]
        [Header("Self Panel")]
        public Image m_selfPanel;

        #endregion

        #region COLORS

        [Header("Colours")]

        [SerializeField]
        
        private Color m_safeColour;

        [SerializeField]
        private Color m_attackColour;

        [SerializeField]
        private Color m_destroyedColour;

        [SerializeField]
        private Color m_dockedColour;

        [SerializeField]
        private Color m_highlightColour;

        private Color m_standardColour;

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
                SystemManager.Events.EventShipCreated -= OnShipCreated;
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
            m_standardColour = m_selfPanel.color;

            m_activated = true;

            // assign ID
            m_ID = newID;

            // If ship has been built then create ship viewer
            if (newShip.Components.Length > 0)
            {
                ViewerPanel.BuildShip(newShip, PiecePrefab);

                // Begin tracking process if active
                if (isActiveAndEnabled)
                    StartCoroutine("Step");
            }
            else
            {
                // If ship is still pending creation then 
                // assign listener for when item is created
                SystemManager.Events.EventShipCreated += OnShipCreated;
            }
        }

        #endregion

        #region EVENT LISTENER

        private void OnShipCreated(CreateDispatch CD)
        {
            if (m_ship.NetID.Equals(CD.Self))
            {
                ViewerPanel.BuildShip(m_ship, PiecePrefab);

                // Begin tracking process if active
                if (isActiveAndEnabled)
                    StartCoroutine("Step");
            }
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
                    m_status.color = m_destroyedColour;

                    m_distance.text = "-";

                    Invoke("DelayDestroy", 10f);
                    break;
                }

                // update name
                m_label.text = m_ship.Data.Name;

                // Update station status
                switch (m_ship.Status)
                {
                    // change and recolor text based on station state
                    case 0:
                        m_status.text = "Safe";
                        m_status.color = m_safeColour;
                        break;

                    case 1:
                        m_status.text = "In Combat";
                        m_status.color = m_attackColour;
                        break;

                    case 2:
                        m_status.text = "Docked";
                        m_status.color = m_destroyedColour;
                        break;
                }

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

                //finished this step

                yield return null;
            }

            yield return null;

            // This HUD is due for removal
            Base.RemoveID(m_ID);
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

        #region IPOINTEREVENTS

        public void OnPointerEnter(PointerEventData eventData)
        {
            m_selfPanel.color = m_highlightColour;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            m_selfPanel.color = m_standardColour;
        }

        #endregion
    }
}