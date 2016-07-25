using UnityEngine;
using System.Collections;
using System.Collections.Generic;
// Artificer
using Data.Shared;
using Data.Space.Faction;

/// <summary>
/// Faction template.
/// </summary>
public abstract class FactionTemplate:ScriptableObject
{
    // Faction name
    protected string _faction;

    // misc faction info will go here

    // note: Raiders are not a faction
    // Faction relations to other factions
    // e.g. Friendly, Neutral, Enemy
    protected Dictionary<string, string> _factionRelations; // <name, relation>

    protected List<Transform> _AttackPoints;
    protected List<Transform> _HomePoints;
    
    // store faction ship generator // change generator to not extend monobehaviour, maybe have a base generator
    // different types?
    // patrol
    // attacker
    protected FactionSpawnerTemplate _spawn;

    // store ship configurations
    
    // store list of station owned on this area
    protected List<PatrolGroup> _factionPatrols;
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
    /// Initialize this instance.
    /// </summary>
    public virtual void Initialize(GameBaseAttributes data, float activity)
    {
        _factionGuards = new List<GuardGroup>();
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
    }

    /// <summary>
    /// Updates removing any destroyed AI
    /// If amount is under a 
    /// </summary>
    public virtual void UpdateShips()
    {
        if (_factionGuards != null)
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

    /// <summary>
    /// Gets the relation between this faction and the other faction.
    /// if faction is not found then is added as neutral
    /// if raider that is enemy
    /// </summary>
    /// <returns>The relation.</returns>
    /// <param name="otherFaction">Other faction.</param>
    public string GetRelation(string otherFaction)
    {
        if (otherFaction == "None")
            return "Neutral";
        else if (otherFaction == "Raider")
            return "Enemy";
        else if (!_factionRelations.ContainsKey(otherFaction))
            _factionRelations.Add(otherFaction, "Neutral");

        return _factionRelations [otherFaction];
    }

    public void AddRelation(string tag, string relation)
    {
        if (!_factionRelations.ContainsKey(tag))
            _factionRelations.Add(tag, relation);
        else
            _factionRelations [tag] = relation;
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
    }
}

