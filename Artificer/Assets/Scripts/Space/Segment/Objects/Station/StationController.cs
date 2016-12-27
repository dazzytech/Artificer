using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
// Artificer
using Data.Space;

namespace Space.Segment
{
    /// <summary>
    /// Station controller is the functionality of a station as well as 
    /// the public interface with external elements
    /// </summary>
    [RequireComponent(typeof(StationAttributes))]
    public class StationController : NetworkBehaviour
    {
        #region ATTRIBUTES

        private StationAttributes m_att;

        #endregion

        #region EVENTS

        public delegate void DestroyedEvent(DestroyDespatch DD);
        public static event DestroyedEvent OnStationDestroyed;

        #endregion

        #region MONO BEHAVIOUR 

        void Awake()
        {
            m_att = GetComponent<StationAttributes>();

            m_att.CurrentIntegrity = m_att.Integrity;
        }

        #endregion

        #region PUBLIC INTERACTION

        public void Initialize()
        {
            // no current reason to initialize yet
        }

        /// <summary>
        /// Called by impact collider to test if 
        /// Station is still functional and 
        /// performs the actual destruction
        /// </summary>
        /// <param name="hitD"></param>
        public void ProcessDamage(HitData hitD)
        {
            // First apply damage to station integrity
            m_att.CurrentIntegrity -= hitD.damage;
            
            // if station destroyed then being destroy process
            if(m_att.CurrentIntegrity <= 0)
            {
                DestroyDespatch DD = new DestroyDespatch();
                DD.Self = netId;
                OnStationDestroyed(DD);

                NetworkServer.UnSpawn(this.gameObject);
                Destroy(this.gameObject);
            }
        }

        #endregion
       
        #region COLLISION TRIGGERS
        
        /// <summary>
        /// This function will detect if a ship
        /// has come into distance with the station
        /// we will need to check the ship is local player and friendly.
        /// Continue when working on next step
        /// </summary>
        /// <param name="collider"></param>
        public void OnTriggerEnter2D(Collider2D collider)
        {
            /*if (Enter)
                GameObject.Find("space").SendMessage("StationReached",
                    collider.transform.parent, SendMessageOptions.DontRequireReceiver);*/
        }

        public void OnTriggerExit2D(Collider2D collider)
        {
        }

        #endregion

        #region ACCESSORS

        public bool Functional
        {
            get { return m_att.CurrentIntegrity > 0; }
        }

        public float NormalizedHealth
        {
            get { return m_att.CurrentIntegrity / m_att.Integrity; }
        }

        #endregion 


        /*
        //private SegmentObject _station;

        public SegmentObject Station
        {
            set { _station = value; }
        }*/
    }
}