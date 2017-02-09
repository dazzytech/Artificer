using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// Artificer
using UI;
using Space.Segment;
using Space.Ship;

namespace Space.UI.Ship
{
    /// <summary>
    /// When ship has a target
    /// Target viewer will display attributes
    /// </summary>
    public class TargetViewer : MonoBehaviour
    {
        #region ATTRIBUTES

        [Header("Tracker Elements")]

        // viewer icon
        [SerializeField]
        private ComponentBuilderUtility m_viewerPanel;

        // integrity tracker
        [SerializeField]
        private IntegrityTracker m_integrityTracker;

        // Name
        [SerializeField]
        private Text m_Label;

        // Icon if not ship
        [SerializeField]
        private Image m_Icon;

        [Header("HUD Prefabs")]

        [SerializeField]
        private GameObject m_piecePrefab;

        #endregion

        #region MONOBEHAVIOUR

        void OnEnable()
        {
            // Hide elements and display no target 
            m_Label.text = "No Target Found";

            m_integrityTracker.gameObject.SetActive(false);

            m_viewerPanel.gameObject.SetActive(false);
        }

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Takes objects and determines type as well
        /// as displaying attributes
        /// </summary>
        /// <param name="trackObj"></param>
        public void BuildTrackingObject(Transform trackObj)
        {
            if (trackObj == null)
                return;

            // Enable HUD elements
            m_integrityTracker.gameObject.SetActive(true);

            m_viewerPanel.gameObject.SetActive(true);

            // Start with testing for station 
            StationController station = trackObj.
                GetComponent<StationController>();

            if (station != null)
            {
                // target is a station
                m_Label.text = station.name;

                m_Icon.enabled = true;

                m_Icon.sprite = station.Icon;

                m_viewerPanel.ClearShip();

                StartCoroutine("Step", station);

                // no need to go further
                return;
            }

            // Next detect Ship
            ShipAttributes ship = trackObj.
                GetComponent<ShipAttributes>();

            if(ship != null)
            {
                m_Icon.enabled = false;

                m_Label.text = ship.Ship.Name;

                m_viewerPanel.BuildShip(ship, m_piecePrefab);

                m_integrityTracker.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Clears current tracking object attributes
        /// </summary>
        public void ClearObject()
        {
            m_Label.text = "No Target Found";

            m_Icon.sprite = null;

            m_integrityTracker.gameObject.SetActive(false);

            m_viewerPanel.ClearShip();

            m_viewerPanel.gameObject.SetActive(false);
        }

        #endregion

        #region COROUTINES

        private IEnumerator Step(StationController station)
        {
            while(m_integrityTracker.gameObject.activeSelf)
            {
                m_integrityTracker.Step(station.NormalizedHealth);

                yield return null;
            }

            m_Icon.enabled = false;

            yield break;
        }

        #endregion
    }
}
