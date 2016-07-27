using UnityEngine;
using System.Collections;
using System.Collections.Generic;
// Artificer
using Data.Shared;

/// <summary>
/// Faction spawner template.
/// Contains functions used by 
/// faction manager to spawn
/// ships of that faction
/// </summary>
public abstract class FactionSpawnerTemplate
{/*
    // store the generator tool
    protected ShipGenerator _shipGen;

    // store ship configurations
    protected List<ShipData> _ships;

    // store the currently created ship
    protected Transform _newShip;

    protected FactionTemplate _faction;

    public Vector2 Boundaries;

    // attack group boundaries
    public float BaseSpawnMaxDistance = 300f;
    public float BaseSpawnMinDistance = 200f;

    public void InitializeSpawner(ShipGenerator gen, FactionTemplate faction)
    {
        _shipGen = gen;
        _faction = faction;
    }

    public void InitializeShips(List<ShipData> ships)
    {
        _ships = ships;
    }

    public void AddShip(ShipData ship)
    {
        if (_ships == null)
            _ships = new List<ShipData>();

        _ships.Add(ship);
    }

    /// <summary>
    /// Spawns an attack ship
    /// for the corresponding faction
    /// </summary>
    public virtual FSM SpawnAttackShip(AttackGroup group)
    {
        _newShip = null;
        
        if (group.Lead != null)
        {
            Vector3 position = new Vector3
                (group.Lead.position.x + Random.Range(-10, 10),
                 group.Lead.position.y + Random.Range(-10, 10));
            
            _newShip = _shipGen.GenerateShip
                (_ships [2], position,
                 Vector3.up);
        } else
        {
            Vector3 position = new Vector3();
            while (true)
            {
                position.x = Random.Range(group.Target.position.x - BaseSpawnMaxDistance,
                                              group.Target.position.x + BaseSpawnMaxDistance);
                
                position.y = Random.Range(group.Target.position.y - BaseSpawnMaxDistance,
                                          group.Target.position.y + BaseSpawnMaxDistance);

                if(Vector3.Distance(position,
                                    group.Target.position) < BaseSpawnMinDistance)
                {
                    // too close to a station
                    continue;
                }
                break;
            }
            
            _newShip = _shipGen.GenerateShip
                (_ships [2], position,
                 Vector3.up);
            
            group.Lead = _newShip.transform;
        }
        
        return null;
    }

    /// <summary>
    /// Spawns the guard ship.
    /// These stay by stations
    /// </summary>
    /// <returns>The guard ship.</returns>
    /// <param name="station">Station.</param>
    public virtual FSM SpawnGuardShip(StationData station)
    {
        _newShip = null;
        
        Vector3 position = new Vector3
            (station.Position.x + Random.Range(-10, 10),
             station.Position.y + Random.Range(-10, 10));
        
        _newShip = _shipGen.GenerateShip
            (_ships[1], position,
             Vector3.up);
        
        return null;
    }

    /// <summary>
    /// Spawns the patrol ship
    /// for the corresponding faction
    /// </summary>
    public virtual FSM SpawnPatrolShip(PatrolGroup group)
    {
        _newShip = null;

        if (group.Lead != null)
        {
            Vector3 position = new Vector3
                (group.Lead.position.x + Random.Range(-10, 10),
                 group.Lead.position.y + Random.Range(-10, 10));

            _newShip = _shipGen.GenerateShip
                (_ships [0], position,
                 Vector3.up);
        } else
        {
            Vector3 position = new Vector3();
            while (true)
            {
                position.x = Random.Range(group.Station.position.x - BaseSpawnMaxDistance,
                                          group.Station.position.x + BaseSpawnMaxDistance);
                
                position.y = Random.Range(group.Station.position.y - BaseSpawnMaxDistance,
                                          group.Station.position.y + BaseSpawnMaxDistance);
                
                if(Vector3.Distance(position,
                                    group.Station.position) < BaseSpawnMinDistance)
                {
                    // too close to a station
                    continue;
                }
                break;
            }
            
            _newShip = _shipGen.GenerateShip
                (_ships [0], position,
                 Vector3.up);

            group.Lead = _newShip.transform;
        }

        return null;
    }

    // To add more types, may be faction specific
*/
}
