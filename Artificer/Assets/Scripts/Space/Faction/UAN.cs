using UnityEngine;
using System.Collections;
using System.Collections.Generic;
// Artificer
using Data.Shared;

public class UAN : FactionTemplate
{
    public override void Initialize(GameBaseAttributes data, float activity)
    {
        _faction = "UAN";

        // Init spawning data
        _spawn = new UANSpawner();
        _maxStationGuard = 5;                      // 2 patrols per station on 100% activity
        _maxPatrolCount = 3;
        _maxAttackCount = 2;

        // Initalize Ship List
        List<ShipData> uanShips = new List<ShipData>();
        uanShips.Add(data.PrebuiltShips.GetShip("ship_uan_combat_small_01"));
        uanShips.Add(data.PrebuiltShips.GetShip("ship_uan_combat_small_02"));
        uanShips.Add(data.PrebuiltShips.GetShip("ship_uan_combat_small_03"));
        _spawn.InitializeShips(uanShips);

        _factionRelations = new Dictionary<string, string>();
        // Set relation with TC
        _factionRelations.Add("TC", "Enemy");

        // lastly perform base faction actions
        base.Initialize(data, activity);
    }
    public override void UpdateShips()
    {
        base.UpdateShips();
    }
}

