using UnityEngine;
using System.Collections;
using System.Collections.Generic;
// Artificer
using Data.Shared;

public class TC : FactionTemplate
{
    public override void Initialize(GameBaseAttributes data, float activity)
    {
        _faction = "TC";
        
        // Init spawning data
        _spawn = new TCSpawner();
        _maxStationGuard = 4;                      // 2 patrols per station on 100% activity
        _maxPatrolCount = 3;
        _maxAttackCount = 2;
        
        // Initalize Ship List
        List<ShipData> tcShips = new List<ShipData>();
        tcShips.Add(data.PrebuiltShips.GetShip("ship_tc_combat_small_02"));
        tcShips.Add(data.PrebuiltShips.GetShip("ship_tc_combat_small_01"));
        tcShips.Add(data.PrebuiltShips.GetShip("ship_tc_combat_small_03"));
        _spawn.InitializeShips(tcShips);

        _factionRelations = new Dictionary<string, string>();

        // Set relation with UAN
        _factionRelations.Add("UAN", "Enemy");

        // lastly perform base faction actions
        base.Initialize(data, activity);
    }
    public override void UpdateShips()
    {
        base.UpdateShips();
    }
}

