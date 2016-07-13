using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChaseState : FSMState
{
    private Vector3 _dest;

    public ChaseState(ShipInputReceiver m)
    {
        _messege = m;
        _stateID = FSMStateID.Chasing;
        Keys = new List<KeyCode>();
    }

    public override void Reason(List<Transform> enemies, Transform npc)
    {
        // Test for emergency eject
        if (ShipStatus.EvacNeeded(npc))
        {
            npc.SendMessage("SetTransition", Transition.Eject);
            return;
        }

        // Check that we have enemies to fight
        if (enemies.Count <= 0) {
            npc.SendMessage("SetTransition", Transition.LostEnemy);
            return;
        }

        Transform target = DestUtil.FindClosestEnemy(enemies, npc.position);
        // No enemies around
        if (target == null)
        {
            npc.SendMessage("SetTransition", Transition.LostEnemy);
            return;
        }
        _dest = target.position;
        float dist = Vector3.Distance(npc.position, _dest);

        if (DestUtil.FrontIsClear(npc, 3f, 90f) != null)
        {
            npc.SendMessage("SetTransition", Transition.GoAround);
            return;
        }

        if (dist < 50.0f)
            npc.SendMessage("SetTransition", Transition.ReachEnemy);
        else if(dist > 200.0f)
            npc.SendMessage("SetTransition", Transition.LostEnemy);
    }

    public override void Act(List<Transform> enemies, Transform npc)
    {
        Keys.Clear();

        Keys.Add(Control_Config.GetKey("moveUp", "ship"));
        
        float angleDiff = DestUtil.FindAngleDifference(npc, _dest);
        
        if (angleDiff >= 10f)
        {
            _messege.ReleaseKey(Control_Config.GetKey("turnRight", "ship"));
            Keys.Add(Control_Config.GetKey("turnLeft", "ship"));
        } else if (angleDiff <= -10f)
        {
            _messege.ReleaseKey(Control_Config.GetKey("turnLeft", "ship"));
            Keys.Add(Control_Config.GetKey("turnRight", "ship"));
        } else
        {
            _messege.ReleaseKey(Control_Config.GetKey("turnRight", "ship"));
            _messege.ReleaseKey(Control_Config.GetKey("turnLeft", "ship"));
        }
    }
}

