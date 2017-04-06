using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

// Artificer
using Data.Space;
using Space.Teams;
using Networking;
using Space.Segment;

namespace Stations
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

        // stop flickering effect caused by ships mulitple components
        private bool m_dockReady;

        // stop flickering effect caused by ships mulitple components
        private bool m_buildReady;

        #endregion

        #region MONO BEHAVIOUR 

        void Awake()
        {
            Att.CurrentIntegrity = Att.Integrity;

            m_dockReady = false;
        }

        void Start()
        {
            if(Att.Type == STATIONTYPE.HOME)
                StartCoroutine("DistanceBasedDocking");

            if (Att.Type == STATIONTYPE.HOME || Att.Type == STATIONTYPE.FOB)
                StartCoroutine("DistanceBasedConstruction");

            if (!Att.Interactive)
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
        public virtual void Initialize(int newID, NetworkInstanceId newTeam)
        {
            // Store our ID for when the station is destroyed
            Att.ID = newID;

            // Only home stations are created immediately
            if (Att.Type == STATIONTYPE.HOME)
                Att.Interactive = true;
            else
            {
                // Here we would begin construction process
                Att.Interactive = false;

                StartCoroutine("GenerateStation");
            }

            // reference to our team
            Att.TeamID = newTeam;

            // place station under correct parent
            transform.SetParent(Att.Team.transform);
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
            if (Att.Team.PlayerOnTeam(hitD.originID))
                return;

            // First apply damage to station integrity
            Att.CurrentIntegrity -= hitD.damage;


            // After successfully taking damage, set to under attack mode
            StopCoroutine("AttackTimer");

            Att.UnderAttack = true;

            StartCoroutine("AttackTimer");

            IntegrityChangedMsg intmsg = new IntegrityChangedMsg();
            intmsg.Amount = -hitD.damage;
            intmsg.Location = transform.position;
            intmsg.PlayerID = -1;

            SystemManager.singleton.client.Send((short)MSGCHANNEL.INTEGRITYCHANGE, intmsg);

            // if station destroyed then being destroy process
            if (Att.CurrentIntegrity <= 0)
            {
                StationDestroyMessage msg = new StationDestroyMessage();
                msg.SelfID = netId;
                msg.ID = Att.ID;

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
            if (Att.UnderAttack)
            {
                yield return new WaitForSeconds(20f);

                Att.UnderAttack = false;
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
                if (!Att.Interactive)
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
                if (!Att.Team.PlayerOnTeam(localInstID))
                {
                    yield break;
                }

                // Find distance between station and player object
                float distance = Vector2.Distance
                    (transform.position, playerObj.transform.position);

                // determine range
                if(distance <= Att.MinDistance)
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
                if(!Att.Interactive)
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
                if (!Att.Team.PlayerOnTeam(localInstID))
                {
                    yield break;
                }

                // Find distance between station and player object
                float distance = Vector2.Distance
                    (transform.position, playerObj.transform.position);

                // determine range
                if (distance <= Att.BuildDistance)
                {
                    if (!m_buildReady)
                    {
                        // Call the event
                        if(InBuildRange != null)
                            InBuildRange(this);

                        m_buildReady = true;
                    }
                }
                else
                {
                    if (m_buildReady)
                    {
                        // Call the event
                        if (OutOfBuildRange != null)
                            OutOfBuildRange(this);

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
            GetComponent<SpriteRenderer>().color = Att.BuildColour;

            while(!Att.Interactive)
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
            yield return new WaitForSeconds(Att.BuildCounter);
            
            Att.Interactive = true;

            yield break;
        }

        #endregion

        #region ACCESSORS

        /// <summary>
        /// Return unique ID
        /// </summary>
        public int ID
        {
            get { return Att.ID; }
        }

        /// <summary>
        /// returns int labeling the state the station is currently in
        /// 0 - Safe
        /// 1 - Under Attack
        /// 2 - Destroyed
        /// 3 - Building
        /// </summary>
        public int Status
        {
            get
            {
                if (Att.UnderAttack)
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
                if(Att.Icon != null)
                    return Att.Icon;
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
                if (Att.Integrity == 0)
                    return 1;
                else
                return Att.CurrentIntegrity / Att.Integrity;
            }
        }

        /// <summary>
        /// Distance between the player and this station
        /// </summary>
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

        /// <summary>
        /// External tools react differently to types
        /// </summary>
        public STATIONTYPE Type
        {
            get { return Att.Type; }
        }

        public StationAttributes Att
        {
            get
            {
                if (transform == null)
                    return null;
                else if (transform.GetComponent<StationAttributes>() != null)
                    return transform.GetComponent<StationAttributes>();
                else
                    return null;
            }
        }

        #endregion 
    }
}