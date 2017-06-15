using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Space.Ship;

namespace Space.AI
{
    /// <summary>
    /// Centralized system for managing ai states
    /// </summary>
    public class AIManager : MonoBehaviour
    {
        #region ATTRIBUTES

        [SerializeField]
        private AIAttributes m_att;

        #endregion

        #region MONO BEHAVIOUR

        /// <summary>
        /// Begin heatbeats and 
        /// object seeking
        /// </summary>
        void Start()
        {
            StartCoroutine("SeekShips");
        }

        #endregion

        #region PRIVATE UTILITIES

        #endregion

        #region COROUTINE

        /// <summary>
        /// Searches through each ship 
        /// that we have authority over 
        /// (runs on each client and not the server)
        /// and add that ship information to the 
        /// our tracker list
        /// /// </summary>
        /// <returns></returns>
        private IEnumerator SeekShips()
        {
            // Retrieve the container
            Transform shipContainer =
                GameObject.Find("_ships").transform;

            while (true)
            {
                // Iterate through each ship to check if
                // its appropriate for us to monitor
                foreach (Transform ship in shipContainer.transform)
                {
                    if (ship.GetComponent<NetworkIdentity>
                        ().hasAuthority)
                    {
                        if (ship.GetComponent<ShipAttributes>().TeamID != -1)
                        {
                            // If this is a ship that this client
                            // manages then we want to track it
                            ShipState state = m_att.TrackedShips.
                                FirstOrDefault(x => x.Self == ship);

                            // Check if we are already tracking this ship
                            if (state == null)
                            {
                                // We can create a tracker for this ship
                                m_att.TrackedShips.Add(new ShipState(ship));
                            }
                        }
                    }

                    yield return null;
                }

                // Now iterate through each tracker
                // and if the ship is gone then delete the tracker
                int i = 0;
                while(i < m_att.TrackedShips.Count)
                {
                    if (m_att.TrackedShips[i].Self == null)
                        // Ship is destroyed stop tracking
                        m_att.TrackedShips.RemoveAt(i);

                    i++;
                }

                yield return null;
            }
        }

        #endregion
    }
}
