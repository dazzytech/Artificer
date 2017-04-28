using Space.Ship.Components.Listener;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Space.UI.Ship
{
    /// <summary>
    /// Interface which enables player to pick 
    /// a station to build within the vicinity
    /// </summary>
    public class BuildHUD : MonoBehaviour
    {
        #region ATTRIBUTES

        private Deploy m_deploy;

        private List<string> m_stationList;

        #region HUD ELEMENTS

        [Header("HUD Elements")]
        [SerializeField]
        private Transform m_base;

        [SerializeField]
        private Transform m_stationListLeft;

        [SerializeField]
        private Transform m_stationListRight;

        [SerializeField]
        private GameObject m_stationItemPrefab;

        #endregion

        #endregion

        #region MONO BEHAVIOUR

        private void Awake()
        {
            m_base.gameObject.SetActive(false);
        }

        #endregion

        #region PUBLIC INTERACTION

        public void InitializeHUD(Deploy func, List<string> stations)
        {
            // assign functions and params
            m_deploy = func;
            m_stationList = stations;

            m_base.gameObject.SetActive(true);

            // refresh and build HUD
            ClearInterface();

            BuildInterface();
        }

        public void CancelBuild()
        {
            ClearInterface();

            m_base.gameObject.SetActive(false);
        }

        #endregion

        #region PRIVATE UTILITIES

        /// <summary>
        /// Triggers the station picker func 
        /// and disables
        /// </summary>
        /// <param name="station"></param>
        private void PickStation(int station)
        {
            m_deploy(station);

            ClearInterface();

            m_base.gameObject.SetActive(false);
        }

        /// <summary>
        /// Clear all station items
        /// </summary>
        private void ClearInterface()
        {
            foreach (Transform child in m_stationListLeft)
                Destroy(child.gameObject);

            foreach (Transform child in m_stationListRight)
                Destroy(child.gameObject);
        }

        /// <summary>
        /// Build up station list on HUD
        /// and assign behaviours
        /// </summary>
        private void BuildInterface()
        {
            int i = 0;
            foreach(string station in m_stationList)
            {
                // Init our new object
                GameObject newStation = Instantiate
                    (m_stationItemPrefab);
                newStation.GetComponentInChildren<Text>().
                    text = station;

                int index = i;
                // Add listener behaviour
                newStation.GetComponent<Button>()
                    .onClick.AddListener(delegate () { PickStation(index); });

                // alternate between sides
                if (i % 2 == 0)
                    newStation.transform.SetParent
                        (m_stationListLeft);
                else
                    newStation.transform.SetParent
                        (m_stationListRight);
                // inc for next
                i++;
            }
        }

        #endregion
    }

}
