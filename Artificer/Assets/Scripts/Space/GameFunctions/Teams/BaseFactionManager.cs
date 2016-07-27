using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
//Artificer
using Data.Space;

/// <summary>
/// Base faction manager.
/// contains all the faction 
/// lists and is responsible for
/// initalizing and updating them
/// </summary>
public class BaseFactionManager: MonoBehaviour
{
    /*public List<FactionTemplate> _factionList;

    // time period faction relation updates
    protected float heartBeatTimer;

    // Faction manager is initialized through the GM
    public void Initialize(GameBaseAttributes data)
    {
        _factionList = new List<FactionTemplate>();

        foreach (FactionData faction in data.Factions)
        {
            FactionTemplate newFaction = ScriptableObject.
                             CreateInstance(faction.FactionClass) 
                             as FactionTemplate;

            newFaction.Initialize(data, faction.Activity);

            _factionList.Add(newFaction);
        }
    }

    // Updates through monoB and updates each faction
    void Update()
    {
        foreach (FactionTemplate faction in _factionList)
        {
            faction.UpdateFactionStatistics();
            faction.UpdateShips();
        }
    }

    // Has a seperate tick time to update faction relations outside the current segment.
    public void HeartBeat()
    {
        // faction not implement for now
    }*/
}