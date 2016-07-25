using UnityEngine;
using System.Collections;
// Artificer
using Data.Shared;
using Data.Space.Faction;

public class TCSpawner : FactionSpawnerTemplate
{
    public override FSM SpawnAttackShip(AttackGroup group)
    {
        // Base will build the actual transform
        base.SpawnAttackShip(group);
        
        // Create a patrol ai and assign it to this faction
        _newShip.name = "TC";
        
        SimpleAttackController attack = 
            _newShip.gameObject.AddComponent<SimpleAttackController>();
        
        attack.SetController
            (_newShip.gameObject.GetComponent<ShipMessegeController>());
        if (_newShip == group.Lead)
            attack.isLead = true;
        
        attack.SetFaction(_faction as TC, new string[1]{"RaiderNode"});
        
        GameObject.Find("_gui").
            SendMessage("AddUIPiece", _newShip, 
                        SendMessageOptions.DontRequireReceiver);
        
        return attack;
    }

    public override FSM SpawnGuardShip(StationData station)
    {
        // Base will build the actual transform
        base.SpawnGuardShip(station);
        
        // Create a patrol ai and assign it to this faction
        _newShip.name = "TC";
        
        SimpleGuardController patrol = 
            _newShip.gameObject.AddComponent<SimpleGuardController>();
        
        patrol.SetController
            (_newShip.gameObject.GetComponent<ShipMessegeController>());
        
        patrol.SetFaction(_faction as TC, new string[1]{"TCNode"});
        
        GameObject.Find("_gui").
            SendMessage("AddUIPiece", _newShip, 
                        SendMessageOptions.DontRequireReceiver);
        
        return patrol;
    }
    
    public override FSM SpawnPatrolShip(PatrolGroup group)
    {
        // Base will build the actual transform
        base.SpawnPatrolShip(group);
        
        // Create a patrol ai and assign it to this faction
        _newShip.name = "TC";
        
        SimplePatrolController patrol = 
            _newShip.gameObject.AddComponent<SimplePatrolController>();
        
        patrol.SetController
            (_newShip.gameObject.GetComponent<ShipMessegeController>());
        if (_newShip == group.Lead)
            patrol.isLead = true;
        
        patrol.SetFaction(_faction as TC, new string[1]{"RaiderNode"});
        
        GameObject.Find("_gui").
            SendMessage("AddUIPiece", _newShip, 
                        SendMessageOptions.DontRequireReceiver);
        
        return patrol;
    }
}
