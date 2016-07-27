using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Space.Ship;

public class IdleState : FSMState
{
    float waitTime;
    float curTime;

    public IdleState(float waitFor, ShipInputReceiver m)
    {
        _messege = m;
        waitTime = waitFor;
        _stateID = FSMStateID.Static;
        curTime = 0.0f;
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

        foreach (Transform e in enemies) {
            // check the distance with player tank
            if (Vector3.Distance (npc.position, e.position)
                <= 200.0f) {
                npc.SendMessage("SetTransition", Transition.SawEnemy);
                return;
            }
        }

        if (DestUtil.ShipWithinProximity(npc, 2f) != null)
        {
            npc.SendMessage("SetTransition", Transition.Shift);
            return;
        }

        curTime += Time.deltaTime;
        if (curTime >= waitTime)
        {
            npc.SendMessage("SetTransition", Transition.ContinuePatrol);
            curTime = 0.0f;
        }
    }
    
    public override void Act(List<Transform> enemies, Transform npc)
    {
        Keys.Clear();

        _messege.ReleaseKey(Control_Config.GetKey("moveUp", "ship"));
        _messege.ReleaseKey(Control_Config.GetKey("moveDown", "ship"));
        _messege.ReleaseKey(Control_Config.GetKey("turnRight", "ship"));
        _messege.ReleaseKey(Control_Config.GetKey("turnLeft", "ship"));
    }
}

