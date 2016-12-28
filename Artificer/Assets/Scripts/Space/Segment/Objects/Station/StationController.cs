using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

// Artificer
using Data.Space;
using Networking;

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

        #region MONO BEHAVIOUR 

        void Awake()
        {
            m_att = GetComponent<StationAttributes>();

            m_att.CurrentIntegrity = m_att.Integrity;
        }

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// called from team spawner to 
        /// pass important information to the station object
        /// </summary>
        /// <param name="newID"></param>
        /// <param name="newType"></param>
        public void Initialize(int newID, STATIONTYPE newType = STATIONTYPE.DEFAULT)
        {
            // Store our ID for when the station is destroyed
            m_att.ID = newID;

            // What type of station is being constructed
            m_att.Type = newType;
        }

        /// <summary>
        /// Called by impact collider to test if 
        /// Station is still functional and 
        /// performs the actual destruction
        /// </summary>
        /// <param name="hitD"></param>
        public void ProcessDamage(HitData hitD)
        {
            // For now ignore friendly fire
            //if (m_att.Team.PlayerOnTeam(hitD.originID))
                //return;

            // First apply damage to station integrity
            m_att.CurrentIntegrity -= hitD.damage;
            
            // if station destroyed then being destroy process
            if(m_att.CurrentIntegrity <= 0)
            {
                StationDestroyMessage msg = new StationDestroyMessage();
                msg.SelfID = netId;
                msg.ID = m_att.ID;

                GameManager.singleton.client.Send((short)MSGCHANNEL.STATIONDESTROYED, msg);

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

        public int ID
        {
            get { return m_att.ID; }
        }

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