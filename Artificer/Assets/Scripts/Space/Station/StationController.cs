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

        public static event StationEvent EnterRange;
        public static event StationEvent ExitRange;
        public static event StationEvent EnterBuildRange;
        public static event StationEvent ExitBuildRange;

        public static event StationEvent StationCreated;

        #endregion

        #region ATTRIBUTES

        private IEnumerator m_docking;
        private IEnumerator m_building;

        #endregion

        #region MONO BEHAVIOUR 

        void Awake()
        {
            Att.CurrentIntegrity = Att.Integrity;
        }

        void Start()
        {
            if (Att.Type == STATIONTYPE.HOME || Att.Type == STATIONTYPE.WARP)
            {
                m_docking = CheckRange
                    (EnterRange, ExitRange, Att.MinDistance);

                StartCoroutine(m_docking);
            }

            if (Att.Type == STATIONTYPE.HOME || Att.Type == STATIONTYPE.FOB)
            {
                m_building = CheckRange
                    (EnterBuildRange, ExitBuildRange,
                    Att.BuildDistance);

                StartCoroutine(m_building);
            }

            if (!Att.Interactive)
                StartCoroutine("CheckForActivity");

            if (StationCreated != null)
                StationCreated(this);
        }

        void OnDestroy()
        {
            if(m_docking != null)
                StopCoroutine(m_docking);

            if (m_building != null)
                StopCoroutine(m_building);
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
        public virtual void Initialize
            (int newID, NetworkInstanceId newTeam, bool delayBuild = false)
        {
            // Store our ID for when the station is destroyed
            Att.ID = newID;

            // Only home stations are created immediately
            if (!delayBuild)
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

        /// <summary>
        /// Catch all function that triggered provided events
        /// when in or out of range of ship
        /// </summary>
        /// <param name="InRange"></param>
        /// <param name="OutRange"></param>
        protected IEnumerator CheckRange
            (StationEvent InRange, StationEvent OutRange, float Distance)
        {
            // Create bools so only single
            // events are triggered
            bool triggered = false;

            // Endless loop
            while (true)
            {
                // Always want to skip if we 
                // arent currently active
                if (!Att.Interactive)
                {
                    yield return null;
                    continue;
                }

                // Only proceed if the player is 
                // currently deployed
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
                if (distance <= Distance)
                {
                    if (!triggered)
                    {
                        // Call the event
                        InRange(this);

                        triggered = true;
                    }
                }
                else
                {
                    if (triggered)
                    {
                        // Call the event
                        OutRange(this);

                        triggered = false;
                    }
                }

                yield return null;
            }
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