using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
using UnityEngine.Networking;
using Data.Space;

namespace Space.AI
{
    #region SHIPSTATETRACKER

    /// <summary>
    /// Logs information about ship
    /// that is being tracked
    /// </summary>
    public class ShipState
    {
        /// <summary>
        /// The ship we are tracking
        /// 
        /// </summary>
        private Transform m_ship;

        private List<NetworkInstanceId> m_persuingShips;

        /// <summary>
        /// Initializes the object with 
        /// reference to the ship
        /// </summary>
        /// <param name="ship"></param>
        public ShipState(Transform ship) 
        {
            m_ship = ship;
            m_persuingShips = new List<NetworkInstanceId>();

            GameServerEvents.EventShipDestroyed 
                += ShipDestroyedEvent;
        }

        #region PUBLIC INTERACTION

        /// <summary>
        /// Adds a reference to a ship
        /// pursuing this one
        /// </summary>
        /// <param name="ship"></param>
        public void AddPersuit(Transform ship)
        {
            if (ship != null)
                m_persuingShips.Add
                    (ship.GetComponent<NetworkIdentity>().
                        netId);
        }

        #endregion

        #region PRIVATE UTILITIES

        /// <summary>
        /// Returns a value based on 
        /// distance from station and 
        /// number of friendlies
        /// </summary>
        private int DetectThreat(int startThreat = 10)
        {
            // decrement depending on distance to
            // statons
            if (!WithinRangeStation(400))
            {
                startThreat -= 4;

                if(!WithinRangeStation(1000))
                {
                    startThreat -= 4;
                    if (!WithinRangeStation(2000))
                        startThreat -= 2;
                }
            }

            // now increment value depending on friendly ships
            // maxing at 3
            startThreat += ShipsWithinRange();

            // 10 is max we can have
            return Mathf.Min(startThreat, 10);
        }

        /// <summary>
        /// Returns true is any stations are within
        /// the defined circle radiusofthe ship
        /// </summary>
        /// <param name="distance"></param>
        /// <returns></returns>
        private bool WithinRangeStation(float distance)
        {
            RaycastHit2D[] withinRange;

            withinRange = Physics2D.CircleCastAll
                (Self.position, distance, Vector2.zero);

            foreach (RaycastHit2D hit in withinRange)
            {
                if (hit.transform.tag == "Station")
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Increment return by one for every 
        /// ship within 5 km of the ship
        /// max return = 10
        /// </summary>
        /// <returns></returns>
        private int ShipsWithinRange()
        {
            int ships = 0;

            RaycastHit2D[] withinRange;

            withinRange = Physics2D.CircleCastAll
                (Self.position, 50, Vector2.zero);

            foreach(RaycastHit2D hit in withinRange)
            {
                if (hit.transform.tag == "Friendly"
                    || hit.transform.tag == "Enemy")
                    ships += ships == 3 ? 0 : 1;
            }

            return ships;
        }

        #endregion

        #region EVENTS

        /// <summary>
        /// Deletes reference to the persuit
        /// ship if it is destroyed
        /// </summary>
        /// <param name="DD"></param>
        private void ShipDestroyedEvent(DestroyDespatch DD)
        {
            for(int i = 0; i < m_persuingShips.Count; i++)
            {
                if (m_persuingShips[i] == DD.Self)
                {
                    m_persuingShips.RemoveAt(i);
                    i--;
                }
            }
            
        }

        #endregion

        #region ACCESSORS

        /// <summary>
        /// calls func to detect threat level
        /// </summary>
        public int ThreatLevel
        {
            get
            {
                return DetectThreat();
            }
        }

        /// <summary>
        /// Returns the amount of raiders targeting
        /// </summary>
        public int PersuitCount
        {
            get
            {
                return m_persuingShips.Count;
            }
        }

        /// <summary>
        /// Stores an accessible reference 
        /// to self
        /// </summary>
        public Transform Self
        {
            get { return m_ship; }
        }

        /// <summary>
        /// network instance if of the tracked
        /// object
        /// </summary>
        public NetworkInstanceId SelfNetID
        {
            get { return Self.GetComponent<NetworkIdentity>().netId; }
        }

        #endregion
    }

    #endregion

    public class AIAttributes : MonoBehaviour
    {
        public AgentData[] AgentTemplates;

        public List<ShipState> TrackedShips;

        
    }
}
