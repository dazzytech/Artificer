using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Space.Ship;

public class CombatDivertState
    : FSMState
{
    private Vector3 _dest;
    private Transform _block;

    public CombatDivertState(ShipInputReceiver m)
    {
        _messege = m;
        _stateID = FSMStateID.CombatDivert;
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
        if (enemies.Count <= 0 || enemies [0] == null)
        {
            npc.SendMessage("SetTransition", Transition.LostEnemy);
            return;
        }
        
        Transform target = DestUtil.FindClosestEnemy(enemies, npc.position);
        _dest = target.position;
        float dist = Vector3.Distance(npc.position, _dest);

        _block = CombUtil.EnemyIsVisible(_dest, dist, npc, target);
        if(_block == null)
        {
            if (dist >= 50.0f && dist < 200.0f)
                npc.SendMessage("SetTransition", Transition.SawEnemy);
            else if (dist > 200.0f)
                npc.SendMessage("SetTransition", Transition.LostEnemy);
            else if (dist < 10f)
                npc.SendMessage("SetTransition", Transition.PullOff);
        }
    }

    public override void Act(List<Transform> enemies, Transform npc)
    {
        Keys.Clear();

        float angleDiff = DestUtil.FindAngleDifference(npc, _dest);

        if (_block == null)
            return;

        if(Vector3.Distance(npc.position, _block.position) < 3f)
        {
            _messege.ReleaseKey(Control_Config.GetKey("moveUp", "ship"));
            Keys.Add(Control_Config.GetKey("moveDown", "ship"));
        }
        else
        {
            _messege.ReleaseKey(Control_Config.GetKey("moveDown", "ship"));
            Keys.Add(Control_Config.GetKey("moveUp", "ship"));
        }

        if (angleDiff >= -90f && angleDiff <= 0)
        {
            _messege.ReleaseKey(Control_Config.GetKey("turnRight", "ship"));
            Keys.Add(Control_Config.GetKey("turnLeft", "ship"));
        }
        else if (angleDiff <= 90f && angleDiff > 0)
        {
            _messege.ReleaseKey(Control_Config.GetKey("turnLeft", "ship"));
            Keys.Add(Control_Config.GetKey("turnRight", "ship"));
        }
    }
}

