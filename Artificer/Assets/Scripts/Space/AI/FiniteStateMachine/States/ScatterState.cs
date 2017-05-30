using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScatterState : FSMState
{
    private Vector3 _dest;
    private Transform _target;

    public ScatterState(ShipMessegeController m)
    {
        _messege = m;
        _stateID = FSMStateID.Scatter;
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
        if (enemies.Count <= 0)
        {
            npc.SendMessage("SetTransition", Transition.LostEnemy);
            return;
        }
        
        _target = DestUtil.FindClosestEnemy(enemies, npc.position);
        _dest = _target.position;
        float dist = Vector3.Distance(npc.position, _dest);

        // Detect enemy direction, if in front and facing away
        // Attack
        if (CombUtil.BehindEnemy(npc, _target))
        {
            npc.SendMessage("SetTransition", Transition.ReachEnemy);
            return;
        }

        if (dist > 15.0f)
            npc.SendMessage("SetTransition", Transition.ReachEnemy);
    }

    public override void Act(List<Transform> enemies, Transform npc)
    {
        Keys.Clear();

        _messege.ReleaseKey(Control_Config.GetKey("fire0", "ship"));

        float angleDiff = DestUtil.FindAngleDifference(npc, _dest);

        if (Vector3.Distance(npc.position, _dest) < 3)// || (angleDiff < 90f && angleDiff > -90f))
        {
            _messege.ReleaseKey(Control_Config.GetKey("moveUp", "ship"));
            Keys.Add(Control_Config.GetKey("moveDown", "ship"));
        } else
        {
            _messege.ReleaseKey(Control_Config.GetKey("moveDown", "ship"));
            Keys.Add(Control_Config.GetKey("moveUp", "ship"));
        }

        if (angleDiff >= 0.0f && angleDiff < 40.0f)
        {
            _messege.ReleaseKey(Control_Config.GetKey("turnLeft", "ship"));
            Keys.Add(Control_Config.GetKey("turnRight", "ship"));
        } else if (angleDiff < 0f && angleDiff > -40.0f)
        {
            _messege.ReleaseKey(Control_Config.GetKey("turnRight", "ship"));
            Keys.Add(Control_Config.GetKey("turnLeft", "ship"));
        }

        if (_target != null)
        {
            float enemyAngleDiff = DestUtil.FindAngleDifference(_target, npc.position);
            if (enemyAngleDiff < 20.0f)
            {
                _messege.ReleaseKey(Control_Config.GetKey("turnLeft", "ship"));
                Keys.Add(Control_Config.GetKey("turnLeft", "ship"));
            }
            else if (enemyAngleDiff > -20.0f)
            {
                _messege.ReleaseKey(Control_Config.GetKey("turnRight", "ship"));
                Keys.Add(Control_Config.GetKey("turnRight", "ship"));
            }
        }
    }
}

