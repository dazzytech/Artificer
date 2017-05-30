using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CircleFollowState : FSMState
{
    private Vector3 _dest;
    private Transform _follow;
    private Vector2 _local;
    
    public CircleFollowState(ShipMessegeController m, Transform followTrans)
    {
        _messege = m;
        _stateID = FSMStateID.Following;
        _follow = followTrans;
        Keys = new List<KeyCode>();

        // Create a random position within 
        _local = FollUtil.FindLocalPositionCircle(20, 5);
    }
    
    public override void Reason(List<Transform> enemies, Transform npc)
    {
        // Test for emergency eject
        if (ShipStatus.EvacNeeded(npc))
        {
            npc.SendMessage("SetTransition", Transition.Eject);
            return;
        }

        // Test if follow object exists
        if (_follow == null)
        {
            npc.SendMessage("SetTransition", Transition.ContinueSearch);
            return;
        }

        // Find the position in the circle corresponding to the follow object
        _dest = FollUtil.FindWorldPositionCircle(_local, _follow);

        Transform target = DestUtil.FindClosestEnemy(enemies, npc.position);
        // No enemies around
        if (target == null)
            return;
        
        float dist = Vector3.Distance(npc.position, target.position);

        Transform blockingTrans = DestUtil.FrontIsClear(npc, 3f, 90f);
        
        if (blockingTrans != null && !blockingTrans.Equals(_follow))
        {
            _local = FollUtil.FindLocalPositionCircle(20, 5);
            npc.SendMessage("SetTransition", Transition.GoAround);
            return;
        } else if (DestUtil.ShipWithinProximity(npc, 2f) != null)
        {
            npc.SendMessage("SetTransition", Transition.GoAround);
            return;
        }
        if (dist <= 200.0f)
        {
            npc.SendMessage("SetTransition", Transition.SawEnemy);
            _messege.ReleaseKey(Control_Config.GetKey("moveUp", "ship"));
            return;
        }


    }
    
    public override void Act(List<Transform> enemies, Transform npc)
    {
        Keys.Clear();

        _messege.ReleaseKey(Control_Config.GetKey("moveDown", "ship"));
        _messege.ReleaseKey(Control_Config.GetKey("fire", "ship"));
        
        if (Vector3.Distance(npc.position, _dest) < 0.1f)
        {
            _messege.ReleaseKey(Control_Config.GetKey("moveUp", "ship"));
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

    public void UpdateFollow(Transform follow)
    {
        _follow = follow;
        // Create a random position within 
        _local = FollUtil.FindLocalPositionCircle(20, 5);
    }
}

