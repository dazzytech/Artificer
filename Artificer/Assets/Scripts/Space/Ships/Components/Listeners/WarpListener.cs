using UnityEngine;
using System.Collections;

using Space.Ship.Components.Attributes;
using System.Collections.Generic;
using UnityEngine.Networking;
using Stations;

namespace Space.Ship.Components.Listener
{
    public class WarpListener : ComponentListener 
    {
        #region EVENTS

        /// <summary>
        /// Warp HUD listens for changes in the warp component
        /// </summary>
        public delegate void WarpStateUpdate();

        public event WarpStateUpdate OnStateUpdate;

        #endregion

        #region ATTRIBUTES

        private WarpAttributes m_att;
        private bool m_IsWarping;

        #endregion

        #region ACCESSORS

        /// <summary>
        /// Checks a number of criteria to determine if the warp is ready to fire 
        /// MAY BE NEEDED IN SHIP ACCESSOR
        /// </summary>
        public bool ReadyToWarp
        {
            get
            {
                if (!m_att.WarpReady)
                    return false;

                // Warp needs to have a selected warp point
                if (m_att.WarpPoint == new Vector2(-1, -1))
                    return false;

                // Warp point needs to be within warprange
                if (TargetDistance> m_att.MaxDistance)
                    return false;

                // can't warp if too close
                if (TargetDistance < m_att.MinDistance)
                    return false;

                if (CombatState)
                    return false;

                // passes all criteria
                return true;
            }
        }

        public bool WarpAvailable
        {
            get
            {
                return m_att.WarpReady;
            }
        }

        /// <summary>
        /// Returns a list of 
        /// other warp gates that do not belong to the enemy
        /// </summary>
        public List<StationAccessor> AccessibleWarps
        {
            get
            {
                // Create container list for nearby warps
                List<StationAccessor> nearbyWarps =
                    new List<StationAccessor>();

                // find each within distance
                foreach (StationAccessor station in
                    SystemManager.Accessor.GlobalStations)
                {
                    // only add warps
                    if (station.Type != STATIONTYPE.WARP)
                        continue;

                    // cant warp to enemies
                    if (station.Team.EnemyTeam.Contains(m_att.Ship.TeamID))
                        continue;

                    // Within distance, keep reference
                    nearbyWarps.Add(station);
                }

                // return our retreived list
                return nearbyWarps;
            }
        }

        /// <summary>
        /// Returns max distance for comparison
        /// </summary>
        public float WarpDistance
        {
            get
            {
                return m_att.MaxDistance;
            }
        }

        /// <summary>
        /// Returns min distance for comparison
        /// </summary>
        public float WarpMinDistance
        {
            get
            {
                return m_att.MinDistance;
            }
        }

        /// <summary>
        /// Returns a value between 0 & 1
        /// that represents how much time the warp is in delay for
        /// </summary>
        public float WarpDelay
        {
            get
            {
                return m_att.TimeCount / m_att.WarpDelay;
            }
        }

        /// <summary>
        /// value between 0 & 1 that represents time spent warming up 
        /// </summary>
        public float WarpWarmUp
        {
            get
            {
                return m_att.TimeCount / m_att.WarpWarmUp;
            }
        }

        /// <summary>
        /// Relays the combat state to the HUD
        /// </summary>
        public bool CombatState
        {
            get
            {
                return m_att.Ship.InCombat;
            }
        }

        /// <summary>
        /// Distance between self and target.
        /// </summary>
        public float TargetDistance
        {
            get
            {
                return Vector2.Distance(transform.position, m_att.WarpPoint);
            }
        }

        /// <summary>
        /// Displays when activated
        /// </summary>
        public bool WarpEngaged
        {
            get
            {
                return m_IsWarping;
            }
        }

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Triggeres the warping process
        /// </summary>
        public override void Activate()
        {
            if (m_att.WarpReady && !m_IsWarping)
            {
                // Create warp affects and start the warp warm up proccess
                StartCoroutine("FireWarp");
                m_IsWarping = true;
                base.Activate();

                if (OnStateUpdate != null)
                    OnStateUpdate();

                // no target, no changes
                StopCoroutine("TrackChanges");
            }
        }

        /// <summary>
        /// Passed a waypoint to the warp to travel to on
        /// activation
        /// </summary>
        /// <param name="Point"></param>
        public void SetPoint(Vector2 Point)
        {
            m_att.WarpPoint = Math.WithinRange(Point, SystemManager.Size);

            if (OnStateUpdate != null)
                OnStateUpdate();

            // update changes in real time
            StopCoroutine("TrackChanges");
            StartCoroutine("TrackChanges");
        }

        /// <summary>
        /// Nullify the point
        /// </summary>
        public void ClearPoint()
        {
            m_att.WarpPoint = new Vector2(-1, -1);

            if (OnStateUpdate != null)
                OnStateUpdate();

            StopCoroutine("TrackChanges");
        }

        #endregion

        #region PRIVATE UTILITIES

        protected override void InitializeComponent()
        {
            base.InitializeComponent();

            ComponentType = "Warps";
            m_att = GetComponent<WarpAttributes>();

            if (hasAuthority)
            {
                m_att.WarpReady = true;
                m_IsWarping = false;
                ClearPoint();
            }
        }
        
        protected override void RunUpdate()
        {
            if(m_IsWarping)
                m_att.TimeCount += Time.deltaTime;
            else if (!m_att.WarpReady)
                m_att.TimeCount += Time.deltaTime;
            else
                m_att.TimeCount = 0;
        }

        protected override void ActivateFx()
        {
            base.ActivateFx();

            GameObject warpFX = (GameObject)Instantiate(m_att.WarpFXPrefab,
                                      transform.position, Quaternion.identity);
            warpFX.transform.parent = transform;
        }

        #endregion

        #region COROUTINES

        /// <summary>
        /// Reenable the warp drive after an alloted time
        /// </summary>
        /// <returns></returns>
        private IEnumerator EngageDelay()
        {
            // delay for the given time
            yield return new WaitForSeconds (m_att.WarpDelay);

            // re-enable warp
            m_att.WarpReady = true;

            // clear warp point
            ClearPoint();

            m_att.TimeCount = 0;

            if (OnStateUpdate != null)
                OnStateUpdate();

            yield return null;
        }

        /// <summary>
        /// Move the ship to the warp point
        /// </summary>
        /// <returns></returns>
        private IEnumerator FireWarp()
        {
            // Disable the warp from firing again
            m_att.WarpReady = false;

            // start the coroutine that reenables the warp
            StartCoroutine("EngageDelay");

            // Delay before firing warm up
            yield return new WaitForSeconds(m_att.WarpWarmUp);
            
            m_IsWarping = false;

            // Move the warp to the new location
            Vector2 warpLocation = Math.RandomWithinRange(m_att.WarpPoint, 1f, 5f);
            transform.parent.transform.position = m_att.WarpPoint;

            if (OnStateUpdate != null)
                OnStateUpdate();
        }

        /// <summary>
        /// detect changes that happen in real time and trigger update
        /// when needed
        /// </summary>
        /// <returns></returns>
        private IEnumerator TrackChanges()
        {
            // initial vars - if these change there is an update
            bool outOfRange = false;
            bool combatstate = false;

            while(true)
            {
                if(combatstate != CombatState)
                {
                    combatstate = CombatState;
                    OnStateUpdate();
                }

                bool newRange = TargetDistance < m_att.MaxDistance && TargetDistance > m_att.MinDistance;

                if (newRange != outOfRange)
                {
                    outOfRange = newRange;
                    OnStateUpdate();
                }

                yield return null;
            }
        }

        #endregion
    }
}
