using UnityEngine;
using System.Collections;

using Space.Ship.Components.Attributes;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace Space.Ship.Components.Listener
{
    public class WarpListener : ComponentListener 
    {
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
                if (m_att.WarpPoint != new Vector2(-1, -1))
                    return false;

                // Warp point needs to be within warprange
                if (Vector2.Distance(transform.position, m_att.WarpPoint) > m_att.MaxDistance)
                    return false;

                // passes all criteria
                return true;
            }
        }

        /// <summary>
        /// Returns a list of 
        /// other warp gates nearby
        /// </summary>
        public List<uint> NearbyWarps
        {
            get
            {
                // Create container list for nearby warps
                List<uint> nearbyWarps =
                    new List<uint>();

                // find each within distance
                foreach (uint warpID in
                    SystemManager.Space.Team.WarpSyncList)
                {
                    // ignore self
                    if (warpID == netId.Value)
                        continue;
                    GameObject warpObj = ClientScene.FindLocalObject
                        (new NetworkInstanceId(warpID));

                    if (Vector2.Distance(transform.position,
                        warpObj.transform.position) <= m_att.MaxDistance)
                    {
                        // Within distance, keep reference
                        nearbyWarps.Add(warpID);
                    }
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
        }

        /// <summary>
        /// Nullify the point
        /// </summary>
        public void ClearPoint()
        {
            m_att.WarpPoint = new Vector2(-1, -1);
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
            if (m_att.WarpReady)
                m_att.TimeCount = 0;
            else
                m_att.TimeCount += Time.deltaTime;
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

            yield return null;
        }

        /// <summary>
        /// Move the ship to the warp point
        /// </summary>
        /// <returns></returns>
        private IEnumerator FireWarp()
        {
            // Delay before firing warm up
            yield return new WaitForSeconds(m_att.WarpWarmUp);
            
            // Disable the warp from firing again
            m_att.WarpReady = false;
            m_IsWarping = false;

            // Move the warp to the new location
            Vector2 warpLocation = Math.RandomWithinRange(m_att.WarpPoint, 1f, 5f);
            transform.parent.transform.position = m_att.WarpPoint;
            
            // start the coroutine that reenables the warp
            StartCoroutine("EngageDelay");
        }

        #endregion
    }
}
