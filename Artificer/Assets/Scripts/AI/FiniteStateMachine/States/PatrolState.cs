using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// USED DOMINANTLY BY PATROL SHIPS
public class PatrolState : FSMState
{
    private Path _nodePath;
    private Vector3 _dest;

    public PatrolState(Path path, ShipInputReceiver m, Transform npc)
    {
        _nodePath = path;
        _messege = m;
        _stateID = FSMStateID.Patrolling;
        _dest = _nodePath.FindNextPoint(npc);
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

        Transform target = DestUtil.FindClosestEnemy(enemies, npc.position);
        // No enemies around
        if (target == null)
            return;

        float dist = Vector3.Distance(npc.position, target.position);

        if (DestUtil.FrontIsClear(npc, 3f, 90f) != null || 
            DestUtil.ShipWithinProximity(npc, 2f) != null)
        {
            npc.SendMessage("SetTransition", Transition.GoAround);
            return;
        }

        if (dist <= 200.0f)
        {
            npc.SendMessage("SetTransition", Transition.Guard);
            _messege.ReleaseKey(Control_Config.GetKey("moveUp", "ship"));
        }
    }

    public override void Act(List<Transform> enemies, Transform npc)
    {
        Keys.Clear();

        _messege.ReleaseKey(Control_Config.GetKey("moveDown", "ship"));
        _messege.ReleaseKey(Control_Config.GetKey("fire", "ship"));

        if (Vector3.Distance(npc.position, _dest) < 20.0f)
        {
            npc.SendMessage("SetTransition", Transition.Wait);
            _messege.ReleaseKey(Control_Config.GetKey("moveUp", "ship"));
            _dest = _nodePath.FindNextPoint(npc);
        }
        else
            if(!Keys.Contains(Control_Config.GetKey("moveUp", "ship")))
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

