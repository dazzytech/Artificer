using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

// Artificer
using Data.Space;
using Space.Teams;
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
        #region EVENTS

        public delegate void StationEvent(StationController controller);
        public static event StationEvent EnterStation;
        public static event StationEvent ExitStation;
        public static event StationEvent InBuildRange;
        public static event StationEvent OutOfBuildRange;

        #endregion

        #region ATTRIBUTES

        private StationAttributes m_att;

        // stop flickering effect caused by ships mulitple components
        private bool m_dockReady;

        // stop flickering effect caused by ships mulitple components
        private bool m_buildReady;

        #endregion

        #region MONO BEHAVIOUR 

        void Awake()
        {
            m_att = GetComponent<StationAttributes>();

            m_att.CurrentIntegrity = m_att.Integrity;

            m_dockReady = false;
        }

        void Start()
        {
            if(m_att.Type == STATIONTYPE.HOME)
                StartCoroutine("DistanceBasedDocking");

            if (m_att.Type == STATIONTYPE.HOME || m_att.Type == STATIONTYPE.FOB)
                StartCoroutine("DistanceBasedConstruction");

            if (!m_att.Interactive)
                StartCoroutine("CheckForActivity");
        }

        void OnDestroy()
        {
            StopCoroutine("DistanceBasedDocking");
        }

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// called from team spawner to 
        /// pass important information to the station object
        /// </summary>
        /// <param name="newID"></param>
        /// <param name="newType"></param>
        [Server]
        public void Initialize(int newID, NetworkInstanceId newTeam)
        {
            // Store our ID for when the station is destroyed
            m_att.ID = newID;

            // Only home stations are created immediately
            if (m_att.Type == STATIONTYPE.HOME)
                m_att.Interactive = true;
            else
            {
                // Here we would begin construction process
                m_att.Interactive = false;

                StartCoroutine("GenerateStation");
            }

            // reference to our team
            m_att.TeamID = newTeam;

            // place station under correct parent
            transform.SetParent(m_att.Team.transform);
        }

        /// <summary>
        /// Called by impact collider to test if 
        /// Station is still functional and 
        /// performs the actual destruction
        /// </summary>
        /// <param name="hitD"></param>
        [Server]
        public void ProcessDamage(HitData hitD)
        {
            // For now ignore friendly fire
            if (m_att.Team.PlayerOnTeam(hitD.originID))
                return;

            // First apply damage to station integrity
            m_att.CurrentIntegrity -= hitD.damage;


            // After successfully taking damage, set to under attack mode
            StopCoroutine("AttackTimer");

            m_att.UnderAttack = true;

            StartCoroutine("AttackTimer");

            IntegrityChangedMsg intmsg = new IntegrityChangedMsg();
            intmsg.Amount = -hitD.damage;
            intmsg.Location = transform.position;
            intmsg.PlayerID = -1;

            SystemManager.singleton.client.Send((short)MSGCHANNEL.INTEGRITYCHANGE, intmsg);

            // if station destroyed then being destroy process
            if (m_att.CurrentIntegrity <= 0)
            {
                StationDestroyMessage msg = new StationDestroyMessage();
                msg.SelfID = netId;
                msg.ID = m_att.ID;

                SystemManager.singleton.client.Send((short)MSGCHANNEL.STATIONDESTROYED, msg);

                NetworkServer.UnSpawn(this.gameObject);
                Destroy(this.gameObject);
            }
        }

        #endregion

        #region COROUTINE

        /// <summary>
        /// When Station is hit by an enemy
        /// Station is in under attack mode
        /// for 20 sec after last shot
        /// </summary>
        /// <returns></returns>
        private IEnumerator AttackTimer()
        {
            if (m_att.UnderAttack)
            {
                yield return new WaitForSeconds(20f);

                m_att.UnderAttack = false;
            }
            yield return null;
        }

        /// <summary>
        /// This function will detect if a ship
        /// has come into distance with the station
        /// we will need to check the ship is local player and friendly.
        /// Continue when working on next step
        /// </summary>
        private IEnumerator DistanceBasedDocking()
        {
            while (true)
            {
                if (!m_att.Interactive)
                {
                    yield return null;
                    continue;
                }

                GameObject playerObj =
                    GameObject.FindGameObjectWithTag("PlayerShip");

                // Ensure player is alive
                if (playerObj == null)
                {
                    yield return null;
                    continue;
                }

                // Retrieve NetworkInstance of player
                NetworkInstanceId localInstID = playerObj.GetComponent
                    <NetworkIdentity>().netId;

                // only proceed if local player 
                // is on the correct team
                if (!m_att.Team.PlayerOnTeam(localInstID))
                {
                    yield break;
                }

                // Find distance between station and player object
                float distance = Vector2.Distance
                    (transform.position, playerObj.transform.position);

                // determine range
                if(distance <= m_att.MinDistance)
                {
                    if (!m_dockReady)
                    {
                        // Call the event
                        EnterStation(this);

                        m_dockReady = true;
                    }
                }
                else
                {
                    if(m_dockReady)
                    {
                        // Call the event
                        ExitStation(this);

                        m_dockReady = false;
                    }
                }

                yield return null;
            }
        }

        private IEnumerator DistanceBasedConstruction()
        {
            while (true)
            {
                // Skip is not currently active
                if(!m_att.Interactive)
                {
                    yield return null;
                    continue;
                }

                GameObject playerObj =
                        GameObject.FindGameObjectWithTag("PlayerShip");

                // Ensure player is alive
                if (playerObj == null)
                {
                    yield return null;
                    continue;
                }

                // Retrieve NetworkInstance of player
                NetworkInstanceId localInstID = playerObj.GetComponent
                    <NetworkIdentity>().netId;

                // only proceed if local player 
                // is on the correct team
                if (!m_att.Team.PlayerOnTeam(localInstID))
                {
                    yield break;
                }

                // Find distance between station and player object
                float distance = Vector2.Distance
                    (transform.position, playerObj.transform.position);

                // determine range
                if (distance <= m_att.BuildDistance)
                {
                    if (!m_buildReady)
                    {
                        // Call the event
                        InBuildRange(this);

                        Debug.Log("Within building range.");

                        m_buildReady = true;
                    }
                }
                else
                {
                    if (m_buildReady)
                    {
                        // Call the event
                        OutOfBuildRange(this);

                        Debug.Log("Left building range.");

                        m_buildReady = false;
                    }
                }

                yield return null;
            }
        }

        /// <summary>
        /// Keep checking if ship has been set
        /// to active yet and change visual appearance
        /// </summary>
        /// <returns></returns>
        private IEnumerator CheckForActivity()
        {
            GetComponent<SpriteRenderer>().color = m_att.BuildColour;

            while(!m_att.Interactive)
            {
                yield return null;
            }

            GetComponent<SpriteRenderer>().color = Color.white;
            yield break;
        }

        /// <summary>
        /// runs on server
        /// slowly builds station and enables when 
        /// completed
        /// </summary>
        /// <returns></returns>
        private IEnumerator GenerateStation()
        {
            yield return new WaitForSeconds(m_att.BuildCounter);
            
            m_att.Interactive = true;

            yield break;
        }

        #endregion

        #region ACCESSORS

        public int ID
        {
            get { return m_att.ID; }
        }

        /// <summary>
        /// returns int labeling the state the station is currently in
        /// 0 - Safe
        /// 1 - Under Attack
        /// 2 - Destroyed
        /// </summary>
        public int Status
        {
            get
            {
                if (m_att.UnderAttack)
                    return 1;
                else
                    return 0;
            }
        }

        /// <summary>
        /// Return texture within sprite renderer of this object as sprite
        /// </summary>
        public Sprite Icon
        {
            get
            {
                if(m_att.Icon != null)
                    return m_att.Icon;
                else
                    return GetComponent<SpriteRenderer>().sprite;
            }
        }

        /// <summary>
        /// Return health in a value between 1.0 - 0.0
        /// </summary>
        public float NormalizedHealth
        {
            get
            {
                if (m_att.Integrity == 0)
                    return 1;
                else
                return m_att.CurrentIntegrity / m_att.Integrity;
            }
        }

        public float Distance
        {
            get
            {
                // Retrieve player object and check if 
                // Player object currently exists
                GameObject playerTransform =
                    GameObject.FindGameObjectWithTag("PlayerShip");

                if (playerTransform == null)
                {
                    return -1;
                }

                // return distance
                return Vector3.Distance(this.transform.position, 
                    playerTransform.transform.position);
            }
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