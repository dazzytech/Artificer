using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using System.Collections;
using System.Collections.Generic;
// Artificer
using Data.Shared;
using Data.Space;
using Space.Ship;
using Space.Teams.SpawnManagers;
using Networking;

/// <summary>
/// An enitity solely responsible for managing all of
/// a teams processes and attributes e.g currently surviving stations and
/// material inventory.
/// Based entirely on server object
/// </summary>
namespace Space.Teams
{
    #region SYNCLISTS 

    /// <summary>
    /// Create synced material lists for networking objects
    /// </summary>
    public class MaterialListSync : SyncListStruct<MaterialData> { }

    #endregion

    public class TeamController : NetworkBehaviour
    {
        #region EVENTS

        public delegate void SyncListDelegate();

        // client HUDs listen to this event to update friendly list
        [SyncEvent]
        public event SyncListDelegate EventPlayerListChanged;

        [SyncEvent]
        public event SyncListDelegate EventStationListChanged;

        #endregion

        #region ATTRIBUTES

        // Faction that the team belongs to
        [SyncVar]
        private FactionData m_faction;

        // Store a list of player connections for that team
        private SyncListUInt m_players = new SyncListUInt();

        // Store a list of Net IDs of stations that the stations owns
        private SyncListUInt m_stations = new SyncListUInt();

        /*private List<Transform> _attackPoints;
        //private List<Transform> _homePoints;

        // unlocked components
        //private SyncListString _unlockedComponents;

        // team-owned materials
        //private MaterialListSync _collectedMaterials;*/

        [SerializeField]
        private TeamSpawnManager _teamSpawn;

        #endregion

        #region MONO BEHAVIOUR

        /*void OnEnable()
        {
            // Assign Events
            ShipMessageController.OnShipDestroyed += ProcessShipDestroyed;
            StationController.OnShipDestroyed += ProcessStationDestroyed;
        }

        void OnDisable()
        {
            // De-assign events
            ShipMessageController.OnShipDestroyed -= ProcessShipDestroyed;
            StationController.OnShipDestroyed -= ProcessStationDestroyed;
        }*/

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// for now just add the faction data to 
        /// local memory
        /// </summary>
        [Server]
        public void Initialize(FactionData faction)
        {
            m_faction = faction;

            // Assign callbacks
            m_players.Callback = PlayerListChanged;
            m_stations.Callback = StationListChanged;
        }

        /// <summary>
        /// Adds player physical object to list when player spawns
        /// </summary>
        /// <param name="netID"></param>
        [Server]
        public void AddPlayerObject(NetworkInstanceId netID)
        {
            m_players.Add(netID.Value);
        }

        /// <summary>
        /// Called when player leaves team or leaves game or dies
        /// </summary>
        /// <param name="netID"></param>
        [Server]
        public void RemovePlayerObject(NetworkInstanceId netID)
        {
            m_players.Remove(netID.Value);
        }

        /// <summary>
        /// returns if the network ID is associated
        /// with player on this team (friendly fire)
        /// </summary>
        /// <param name="netID"></param>
        /// <returns></returns>
        public bool PlayerOnTeam(NetworkInstanceId netID)
        {
            return m_players.Contains(netID.Value);
        }

        [Server]
        public void AddStationObject(NetworkInstanceId netID)
        {
            m_stations.Add(netID.Value);
        }

        [Server]
        public void RemoveStationObject(NetworkInstanceId netID)
        {
            m_stations.Remove(netID.Value);
        }

        #endregion

        #region ACCESSORS

        /// <summary>
        /// Returns faction ID without access to faction
        /// </summary>
        public int ID
        {
            get { return m_faction.ID; }
        }

        /// <summary>
        /// Returns spawner for use by game object
        /// </summary>
        public TeamSpawnManager Spawner
        {
            get { return _teamSpawn; }
        }

        public SyncListUInt Players
        {
            get { return m_players; }
        }

        public SyncListUInt Stations
        {
            get { return m_stations; }
        }

        #endregion

        /*
        // store faction ship generator // change generator to not extend monobehaviour, maybe have a base generator
        // different types?
        // patrol
        // attacker
        //protected FactionSpawnerTemplate _spawn;

        // store ship configurations

        // store list of station owned on this area
        /*protected List<PatrolGroup> _factionPatrols;
        protected List<GuardGroup> _factionGuards;
        protected List<AttackGroup> _factionAttackers;

        // store percentage of faction activity in this segment
        // 0.0f to 1.0f
        protected float _factionActivity;
        // Threat activity is affected by raider threat and enemy activity
        protected float _threatActivity;

        // SHARED
        // Spawn density
        // also dependant on activity
        protected int _maxStationGuard;
        protected int _curStationGuard;

        protected int _maxPatrolCount;
        protected int _curPatrolCount;

        protected int _maxAttackCount;
        protected int _curAttackCount;

        // ship spawner timers
        // station ship respawn will also depend on activity
        protected float _stationShipRespawn;
        protected float _patrolGroupSpawn;
        protected float _attackGroupSpawn;

        // maximum time reinforcements could possibly take
        protected static float _maxWaitTimeStation = 180f;                 // 1 - 3 min seconds respawn
        // minimum time possible
        protected static float _minWaitTimeStation = 60f;

        // maximum time reinforcements could possibly take
        protected static float _maxWaitTimePatrol = 360f;                 // 3 - 6 min seconds respawn
        // minimum time possible
        protected static float _minWaitTimePatrol = 180f;

        // maximum time reinforcements could possibly take
        protected static float _maxWaitTimeAttack = 900f;                 // 10 - 15 min seconds respawn
        // minimum time possible
        protected static float _minWaitTimeAttack = 600f;

        // currentTime
        protected float _waitTimeGuard;
        protected float _waitTimePatrol;
        protected float _waitTimeAttack;

        /// <summary>
        /// 
        /// </summary>
        public void Initialize(GameBaseAttributes data)
        {
            /*_factionGuards = new List<GuardGroup>();
            // Station ship respawn is instant on init
            _stationShipRespawn = 0;
            _waitTimeGuard = 0;
            _waitTimePatrol = 0;
            _waitTimeAttack = 0;

            Threat = 0.1f; // at least one guard
            _AttackPoints = new List<Transform>();
            _HomePoints = new List<Transform>();

            Activity = 0.1f;

            // add faction owned stations to the list
            // this method may be used for other seg 
            // objects in future
            foreach (StationData station in 
                    data.Segment.GetObjsOfType("station"))
            {
                // this will allow the faction to protect 
                // it's stations
                if(station.FactionName == _faction)
                {
                    GuardGroup patGroup = new GuardGroup();
                    patGroup.Station = station;
                    patGroup.Ships = new List<FSM>();
                    _factionGuards.Add(patGroup);
                    _HomePoints.Add(GameObject.Find(station.Name).transform);
                    Activity += 0.2f;
                }
                else
                {
                    // determine if faction is enemy
                    if(GetRelation(station.FactionName) == "Enemy")
                    {
                        // if so, add to enemy station list
                        _AttackPoints.Add(GameObject.Find(station.Name).transform);
                        // increment threat level
                        Threat += 0.2f;
                    }
                }
            }

            // Initialize 
            _factionPatrols = new List<PatrolGroup>();

            _factionAttackers = new List<AttackGroup>();

            // Initialize Spawner
            _spawn.InitializeSpawner(GameObject.Find("space")
                                     .GetComponent<ShipGenerator>(), this);

            _spawn.Boundaries = data.Segment.Size;

            // Run the spawner first time
            SpawnInitialGuard();
        }

         public virtual void SpawnInitialGuard()
         {
             UpdateFactionStatistics();

             foreach (GuardGroup group in _factionGuards)
             {
                 while (group.Ships.Count < _curStationGuard)
                 {
                     // for now spawn all the patrols together
                     group.Ships.Add(
                         _spawn.SpawnGuardShip(group.Station));
                 }
             }
         }

        /// <summary>
        /// Updates the faction statistics.
        /// dependant upon changes in the faction 
        /// activity
        /// </summary>
        public virtual void UpdateFactionStatistics()
        {
            
            // STATION
            // set respawn count
            _curStationGuard = Mathf.CeilToInt(_maxStationGuard * _threatActivity);
            // Set respawn time
            _stationShipRespawn = _maxWaitTimeStation - // 30 - (30 - 10 = 20) * 1f = 10f
                ((_maxWaitTimeStation - _minWaitTimeStation)
                * _factionActivity);

            _curPatrolCount = Mathf.CeilToInt(_maxPatrolCount * _factionActivity);
            // patrol spawn time
            _patrolGroupSpawn = _maxWaitTimePatrol - // 30 - (30 - 10 = 20) * 1f = 10f
                ((_maxWaitTimePatrol - _minWaitTimePatrol)
                 * _factionActivity);

            // update attack stats 
            _curAttackCount = Mathf.CeilToInt(_maxAttackCount * _factionActivity); 
            _attackGroupSpawn = _maxWaitTimeAttack - // 30 - (30 - 10 = 20) * 1f = 10f
                ((_maxWaitTimeAttack - _minWaitTimeAttack)
                 * _factionActivity);
                 * 
        }

        /// <summary>
        /// Updates removing any destroyed AI
        /// If amount is under a 
        /// </summary>
        public void UpdateShips()
        {
            /* if (_factionGuards != null)
             {
                 // update spawn timer then spawn patros
                 _waitTimeGuard += Time.deltaTime;

                 foreach (GuardGroup group in _factionGuards)
                 {
                     List<FSM> deadFSMs = new List<FSM>();
                     foreach (FSM fsm in group.Ships)
                     {
                         if (fsm == null) // destroyed
                             deadFSMs.Add(fsm);
                     }

                     foreach (FSM fsm in deadFSMs)
                     {
                         group.Ships.Remove(fsm);
                     }

                     if (_waitTimeGuard >= _stationShipRespawn)
                     {
                         _waitTimeGuard = 0;
                         // Spawn patrols
                         while (group.Ships.Count < _curStationGuard)
                         {
                             // for now spawn all the patrols together
                             group.Ships.Add(
                             _spawn.SpawnGuardShip(group.Station));
                         }
                     }
                 }
             }

             if (_factionPatrols != null)
             {
                 foreach (PatrolGroup group in _factionPatrols)
                 {
                     List<FSM> deadFSMs = new List<FSM>();
                     foreach (FSM fsm in group.Ships)
                     {
                         if (fsm == null)
                             // killed
                             deadFSMs.Add(fsm);
                     }

                     foreach (FSM fsm in deadFSMs)
                     {
                         group.Ships.Remove(fsm);
                     }
                 }

                 // update spawn timer then spawn patros
                 _waitTimePatrol += Time.deltaTime;
                 if (_waitTimePatrol >= _patrolGroupSpawn)
                 {
                     _waitTimePatrol = 0;
                         // Spawn patrols
                         // Leader is null until first ship is made
                     PatrolGroup group = new PatrolGroup();
                     group.Ships = new List<FSM>();
                     group.Station = _HomePoints[Random.Range(0, _HomePoints.Count)];

                     while (group.Ships.Count < _curPatrolCount)
                     {
                             // for now spawn all the patrols together
                         group.Ships.Add(_spawn.SpawnPatrolShip(group));
                     }

                         // Set the Group vars to ships
                     foreach(FSM fsm in group.Ships)
                     {
                         ((SimplePatrolController)fsm).SetGroup(group);
                     }
                     _factionPatrols.Add(group);
                 }
             }

             if (_factionAttackers != null)
             {
                 foreach (AttackGroup group in _factionAttackers)
                 {
                     List<FSM> deadFSMs = new List<FSM>();
                     foreach (FSM fsm in group.Ships)
                     {
                         if (fsm == null)
                             // killed
                             deadFSMs.Add(fsm);
                     }

                     foreach (FSM fsm in deadFSMs)
                     {
                         group.Ships.Remove(fsm);
                     }
                 }

                 // update spawn timer then spawn patros
                 _waitTimeAttack += Time.deltaTime;
                 if (_waitTimeAttack >= _attackGroupSpawn)
                 {
                     _waitTimeAttack = 0;
                     // Spawn patrols
                     // Leader is null until first ship is made
                     AttackGroup group = new AttackGroup();
                     group.Ships = new List<FSM>();
                     group.Target = _AttackPoints[Random.Range(0, _AttackPoints.Count)];

                     while (group.Ships.Count < _curAttackCount)
                     {
                         // for now spawn all the patrols together
                         group.Ships.Add(_spawn.SpawnAttackShip(group));
                     }

                     // Set the Group vars to ships
                     foreach(FSM fsm in group.Ships)
                     {
                         ((SimpleAttackController)fsm).SetGroup(group);
                     }
                     _factionAttackers.Add(group);
                 }
             }
        }

        
        // GET & SET
        // External objects only need to change the activity
        public float Activity
        {
            set { if(value <= 1.0f && value >= 0.0f) _factionActivity = value; }
            get {return _factionActivity; }
        } 

        public float Threat
        {
            set { if(value <= 1.0f && value >= 0.0f) _threatActivity = value; }
            get {return _threatActivity;}
        }

        public string Name
        {
            get { return _faction;}
        }*/

        #region EVENT LISTENER

        /// <summary>
        /// When ship is destroyed decide something to do (lol)
        /// </summary>
        /// <param name="destroyed"></param>
        public void ProcessShipDestroyed(DestroyDespatch destroyed)
        {

        }

        public void ProcessStationDestroyed(DestroyDespatch destroyed)
        {

        }

        // Here will be an event listening for station destroyed

        #endregion

        #region CALLBACKS

        void PlayerListChanged(SyncListUInt.
            Operation op, int itemIndex)
        {
            if(EventPlayerListChanged != null)
                EventPlayerListChanged();
        }

        void StationListChanged(SyncListUInt.
            Operation op, int itemIndex)
        {
            if (EventStationListChanged != null)
                EventStationListChanged();
        }

        #endregion

        /*

        #region TEAM RESOURCE INTERACTION

        /// <summary>
        /// Called to the server when components are unlocked for the faction.
        /// when added here these components can be added to ships by players
        /// </summary>
        /// <param name="comps"></param>
        [Command]
        public void CmdUnlockComponents(List<string> comps)
        {
            if (_unlockedComponents == null)
                _unlockedComponents = new SyncListString();

            foreach (string comp in comps)
            {
                if (!_unlockedComponents.Contains(comp))
                    _unlockedComponents.Add(comp);
            }
        }

        /// <summary>
        /// Called between objects within the same client
        /// tests that may actually use the ccomponent if in team's arsenal
        /// </summary>
        /// <param name="inComp"></param>
        /// <returns></returns>
        public bool IsUnlocked(string inComp)
        {
            foreach (string comp in _unlockedComponents)
            {
                if (_unlockedComponents.Contains(inComp))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Called from player objects to transfer
        /// materials to the team pool
        /// </summary>
        /// <param name="inMats"></param>
        [Command]
        public void CmdAddMaterialToPool(List<MaterialData> inMats)
        {
            // null if not assigned
            if (_collectedMaterials == null)
                _collectedMaterials = new MaterialListSync();

            foreach (MaterialData mat in inMats)
            {
                // See if station already has material
                int matPos = _collectedMaterials.IndexOf(mat);
                if (matPos == -1) // -1 means we don't already have it
                    _collectedMaterials.Add(mat);
                else
                {
                    // if we already own it, to add the amount to the existing amount.
                    MaterialData homeMat = _collectedMaterials[matPos];
                    homeMat.Amount += mat.Amount;
                    _collectedMaterials[matPos] = homeMat;
                }
            }
        }

        /// <summary>
        /// Returns true if the amount of a material in 
        /// team storage is more or equal to the requirement 
        /// material given
        /// </summary>
        /// <param name="inMat"></param>
        /// <returns></returns>
        public bool CheckTeamStorage(MaterialData inMat)
        {
            // null if not assigned
            if (_collectedMaterials == null)
                return false;

            // See if station already has material
            int matPos = _collectedMaterials.IndexOf(inMat);
            if (matPos == -1) // -1 means we don't already have it
                return false;
            else
            {
                // if we already own it, to add the amount to the existing amount.
                if (_collectedMaterials[matPos].Amount >= inMat.Amount)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Decreases the shared storage
        /// </summary>
        /// <param name="inMats"></param>
        /// <returns></returns>
        public bool ExpendTeamStorage(List<MaterialData> inMats)
        {
            // null if not assigned
            if (_collectedMaterials == null)
                return false;

            // Do our own check to make sure we have all the materials
            foreach (MaterialData inMat in inMats)
            {
                if (!CheckTeamStorage(inMat))
                    return false;           // inadequate storage
            }

            // now we can expend the material
            foreach (MaterialData inMat in inMats)
            {
                // Reduce storage on server
                CmdReduceInventory(inMat);
            }

            // Return successful expendature
            return true;
        }

        /// <summary>
        /// Made when making purchase and decreases stock
        /// </summary>
        /// <param name="inMat"></param>
        [Command]
        private void CmdReduceInventory(MaterialData inMat)
        {
            // See if station has the material
            int matPos = _collectedMaterials.IndexOf(inMat);
            // decrease the amount from the existing amount.
            MaterialData homeMat = _collectedMaterials[matPos];
            homeMat.Amount -= inMat.Amount;

            // Delete material if we don't have any of that material left
            if (homeMat.Amount <= 0)
                _collectedMaterials.RemoveAt(matPos);
            else
                _collectedMaterials[matPos] = homeMat;
        }

        /// <summary>
        /// When a station is built, add it to our list
        /// </summary>
        /// <param name="inNetID"></param>
        [Command]
        public void CmdAddStation(NetworkInstanceId inNetID)
        {
            // init if null
            if (_stations == null)
                _stations = new NetIDListSync();

            // Don't create duplicates
            if (!_stations.Contains(inNetID))
                _stations.Add(inNetID);
        }

        /// <summary>
        /// Retreives the stations local to the player
        /// </summary>
        /// <returns></returns>
        public List<Transform> RetrieveStations()
        {
            // new list
            List<Transform> retList = new List<Transform>();

            // find the local object
            foreach(NetworkInstanceId nID in _stations)
            {
                GameObject station = ClientScene.FindLocalObject(nID);
                if (station != null)
                    retList.Add(station.transform);
            }

            return retList;
        }

        // todo: create singular station retreival

        #endregion*/
    }
}