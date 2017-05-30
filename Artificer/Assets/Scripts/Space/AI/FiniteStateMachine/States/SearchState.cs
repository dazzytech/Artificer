using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// USED DOMINANTLY BY RAIDERS AND ATTACK SHIPS
public class SearchState : FSMState
{
    private Vector3 _dest;
    private Transform _follow;
    
    public SearchState(ShipMessegeController m, Transform npc, Transform follow)
    {
        _messege = m;
        _stateID = FSMStateID.Searching;
        _dest = FindNewPoint(npc);
        _follow = follow;

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

        if (DestUtil.FrontIsClear(npc, 3f, 90f) != null
            ||DestUtil.ShipWithinProximity(npc, 2f) != null)
        {
            npc.SendMessage("SetTransition", Transition.GoAround);
            return;
        }

        Transform target = DestUtil.FindClosestEnemy(enemies, npc.position);
        // No enemies around
        if (target == null)
            return;

        float dist = Vector3.Distance(npc.position, target.position);

        if (dist <= 200.0f)
        {
            npc.SendMessage("SetTransition", Transition.SawEnemy);
            _messege.ReleaseKey(Control_Config.GetKey("moveUp", "ship"));
        }
    }
    
    public override void Act(List<Transform> enemies, Transform npc)
    {
        Keys.Clear();

        _messege.ReleaseKey(Control_Config.GetKey("moveDown", "ship"));
        
        if (Vector3.Distance(npc.position, _dest) < 20.0f)
        {
            _messege.ReleaseKey(Control_Config.GetKey("moveUp", "ship"));
            _dest = FindNewPoint(npc);
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

    public void SelectTarget(Transform targetObj)
    {
        _dest = targetObj.position;
    }

    public void DeselectTarget(Transform npc)
    {
        _dest =  FindNewPoint(npc);
    }

    public Vector3 FindNewPoint(Transform npc)
    {
        return new Vector3(Random.Range(npc.position.x - 100f, npc.position.x + 100f),
                           Random.Range(npc.position.y - 100f, npc.position.y + 100f), 0f);
    }
}

