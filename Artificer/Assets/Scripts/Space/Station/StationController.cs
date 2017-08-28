using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

// Artificer
using Data.Space;
using Space.Teams;
using Space.Ship;
using Networking;
using Space.Segment;
using Space.UI;

namespace Stations
{
    /// <summary>
    /// Station controller is the functionality of a station as well as 
    /// the public interface with external elements
    /// </summary>
    [RequireComponent(typeof(StationAttributes))]
    public class StationController : SegmentObjectBehaviour
    {
        #region EVENTS

        public delegate void StationEvent(StationAccessor station);

        public static event StationEvent OnEnterRange;
        public static event StationEvent OnExitRange;
        public static event StationEvent OnEnterBuildRange;
        public static event StationEvent OnExitBuildRange;

        public static event StationEvent OnStationCreated;

        #endregion

        #region ATTRIBUTES

        private IEnumerator m_docking;
        private IEnumerator m_building;

        #endregion

        #region MONO BEHAVIOUR 

        public virtual void Awake()
        {
            m_att.CurrentIntegrity = m_att.Integrity;

            m_att.ProximityMessage = "Press Enter to dock at station";
        }

        protected override void Start()
        {
            switch (m_att.Type)
            {
                case STATIONTYPE.HOME:
                case STATIONTYPE.WARP:
                case STATIONTYPE.DEPOT:
                m_docking = CheckRange
                    (OnEnterRange, OnExitRange,
                    m_att.MinDistance);

                StartCoroutine(m_docking);
                    break;
            }

            if (m_att.Type == STATIONTYPE.HOME || m_att.Type == STATIONTYPE.FOB)
            {
                m_building = CheckRange
                    (OnEnterBuildRange, OnExitBuildRange,
                    m_att.BuildDistance);

                StartCoroutine(m_building);
            }

            if (!m_att.Interactive)
                StartCoroutine("CheckForActivity");

            // Only trigger creation event once team is defined
            if (!m_att.TeamID.IsEmpty())
            {
                transform.SetParent(ClientScene.FindLocalObject(m_att.TeamID).transform);

                if (OnStationCreated != null)
                    OnStationCreated(m_att.Accessor);
            }

            // perform segment object behaviour
            DisableObj();

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

        #region EXTERNAL FUNCTION

        /// <summary>
        /// called from team spawner to 
        /// pass important information to the station object
        /// </summary>
        /// <param name="newID"></param>
        /// <param name="newType"></param>
        [Server]
        public virtual void Initialize
            (NetworkInstanceId newTeam, bool delayBuild = false)
        {
            // Only home stations are created immediately
            if (!delayBuild)
                m_att.Interactive = true;
            else
            {
                // Here we would begin construction process
                m_att.Interactive = false;

                StartCoroutine("GenerateStation");
            }

            // reference to our team
            m_att.TeamID = newTeam;
        }

        /// <summary>
        /// Called by impact collider to test if 
        /// Station is still functional and 
        /// performs the actual destruction
        /// </summary>
        /// <param name="hitD"></param>
        [Server]
        public void ProcessDamage
            (HitData hitD)
        {
            // For now ignore friendly fire
            if (m_att.Accessor.Team.PlayerOnTeam(hitD.originID))
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
                msg.ID = m_att.SpawnID;

                SystemManager.singleton.client.Send((short)MSGCHANNEL.STATIONDESTROYED, msg);

                NetworkServer.UnSpawn(this.gameObject);
                Destroy(this.gameObject);
            }
        }

        #endregion

        #region PLAYER INTERACTION

        public virtual void Interact(ShipAccessor ship) {}

        public virtual void Idle(ShipAccessor ship) {}

        /// <summary>
        /// Hides the ship
        /// and then displays the station information
        /// </summary>
        /// <param name="ship"></param>
        public virtual void Dock(ShipAccessor ship)
        {
            if (ship != null)
            {
                ship.DisableShip();

                // Next is to update the HUD to display the
                // micro stationHUD
                SystemManager.UIState.SetState(UIState.Station);

                // Add message for sending ship attributes
                SystemManager.UI.InitializeStationHUD(ship);
            }
        }

        /// <summary>
        /// Reverts the ui state
        /// and makes the player ship visible
        /// </summary>
        /// <param name="ship"></param>
        public virtual void UnDock(ShipAccessor ship)
        {
            if (ship != null)
            { 
                // Fade in ship
                ship.EnableShip();

                // Next is to update the HUD to display the
                // micro stationHUD
                SystemManager.UIState.SetState(UIState.Play);
            }
        }

        /// <summary>
        /// Display a prompt to dock
        /// </summary>
        public virtual void EnterRange()
        {
            SystemManager.UIMsg.DisplayPrompt(string.Format
                ("Press {0} to dock at this station.", Control_Config.GetKey("dock", "sys")));
        }

        /// <summary>
        /// Clear messages
        /// </summary>
        public virtual void ExitRange()
        {
            SystemManager.UIMsg.ClearPrompt();
        }

        #endregion

        #endregion

        #region PRIVATE ACCESSORS

        protected StationAttributes m_att
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

        #region PRIVATE UTILITIES

        

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
                if (!m_att.Interactive)
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
                if (!m_att.Accessor.Team.PlayerOnTeam(localInstID))
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
                        InRange(m_att.Accessor);

                        triggered = true;
                    }
                }
                else
                {
                    if (triggered)
                    {
                        // Call the event
                        OutRange(m_att.Accessor);

                        triggered = false;
                    }
                }

                yield return null;
            }
        }

        #endregion
    }
}