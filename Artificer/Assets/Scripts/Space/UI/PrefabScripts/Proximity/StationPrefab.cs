using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

//Artificer
using Stations;
using Space.UI.Proxmity;
using Space.UI.Ship;
using UI;

namespace Space.UI
{
    /// <summary>
    /// Local HUD element that tracks state of station
    /// </summary>
    public class StationPrefab : SelectableHUDItem
    {
        #region ATTRIBUTES

        // Reference to tracked station
        private StationAccessor m_station;


        [SerializeField]
        [Header("Integrity Tracker")]
        private IntegrityTracker m_integrity;

        // If item was active before enable
        private bool m_activated;

        private uint m_ID;

        [Header("StationHUD")]
        public static StationHUD Base;

        #region HUD ELEMENTS

        [Header("HUD Elements")]

        [SerializeField]
        public Text m_label;

        [SerializeField]
        public Text m_distance;

        [SerializeField]
        public Text m_status;

        [SerializeField]
        public Image m_icon;

        #endregion

        #region COLORS

        [Header("Colours")]

        [SerializeField]
        private Color m_safeColor;

        [SerializeField]
        private Color m_attackColor;

        [SerializeField]
        private Color m_destroyedColor;

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

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Pass the station to HUD element for
        /// tracking
        /// </summary>
        /// <param name="Station"></param>
        public void DefineStation(StationAccessor Station, uint newID)
        {
            m_station = Station;

            m_label.text = Station.name;

            m_icon.overrideSprite = m_station.Icon;

            // assign ID
            m_ID = newID;

            m_activated = true;

            // Begin tracking process if active
            if (isActiveAndEnabled)
                StartCoroutine("Step");
        }

        #endregion

        #region COROUTINE

        private IEnumerator Step()
        {
            while(true)
            {
                if (m_station == null)
                {
                    m_status.text = "Destroyed";
                    m_status.color = m_destroyedColor;

                    m_distance.text = "-";

                    Invoke("DelayDestroy", 20f);
                    break;
                }

                // Pass info to integrity tracker 
                m_integrity.Step(m_station.NormalizedHealth);

                // Update station status
                switch(m_station.Status)
                {
                    // change and recolor text based on station state
                    case 0:
                        m_status.text = "Safe";
                        m_status.color = m_safeColor;
                        break;

                    case 1:
                        m_status.text = "Attacked";
                        m_status.color = m_attackColor;
                        break;
                }

                // Find distance from station to player

                // Retrieve player object and check if 
                // Player object currently exists
                float distance = m_station.Distance;

                if(distance == -1)
                {
                    m_distance.text = "-";
                    yield return null;
                }

                // display distance
                m_distance.text = ((int)distance  *0.01).ToString("F2") + "km";

                //finished this step
                yield return null;
            }

            Base.RemoveID(m_ID);

            yield return null;
        }

        /// <summary>
        /// Delays the removal of the 
        /// HUD element for players to know station was destroyed
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

