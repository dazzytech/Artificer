using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DefenseState : FSMState
{
    private Path _nodePath;
    Vector3 _dest;
    Vector3 _target;

    public DefenseState(Path path, ShipInputReceiver m)
    {
        _nodePath = path;
        _messege = m;
        _stateID = FSMStateID.Defensive;
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
        if (enemies.Count <= 0 || enemies[0] == null) {
            npc.SendMessage("SetTransition", Transition.LostEnemy);
            return;
        }
        
        Transform target = DestUtil.FindClosestEnemy(enemies, npc.position);
        if (target == null)
        {
            npc.SendMessage("SetTransition", Transition.LostEnemy);
            return;
        }
        _target = target.position;
        float dist = Vector3.Distance(npc.position, _target);

        if (dist < 100.0f)
            npc.SendMessage("SetTransition", Transition.SawEnemy);
        else if (dist > 200.0f)
            npc.SendMessage("SetTransition", Transition.LostEnemy);
        else
            _dest = _nodePath.FindClosestTo(_target);
        
    }

    public override void Act(List<Transform> enemies, Transform npc)
    {
        Keys.Clear();
        _messege.ReleaseKey(Control_Config.GetKey("fire", "ship"));

        float distance = Vector3.Distance(npc.position, _dest);
        float angleDiff;

        if (distance > 20.0f)
        {
            Keys.Add(Control_Config.GetKey("moveUp", "ship"));
            angleDiff = DestUtil.FindAngleDifference(npc, _dest);
        } else
        {
            _messege.ReleaseKey(Control_Config.GetKey("moveUp", "ship"));
            angleDiff = DestUtil.FindAngleDifference(npc, _target);
        }

        if (angleDiff >= 15f)
        {
            _messege.ReleaseKey(Control_Config.GetKey("turnRight", "ship"));
            Keys.Add(Control_Config.GetKey("turnLeft", "ship"));
        } else if (angleDiff <= -15f)
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

