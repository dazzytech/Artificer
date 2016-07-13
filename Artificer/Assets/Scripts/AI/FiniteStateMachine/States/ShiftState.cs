using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShiftState : FSMState
{
    Transform _block;
    public ShiftState(ShipInputReceiver m)
    {
        _messege = m;
        _stateID = FSMStateID.Shifting;
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

        _block = DestUtil.ShipWithinProximity(npc, 2f);
        if(_block == null)
            npc.SendMessage("SetTransition", Transition.Wait);
    }
    
    public override void Act(List<Transform> enemies, Transform npc)
    {
        Keys.Clear();
        
        if (_block != null)
        {
            float angleDiff = DestUtil.FindAngleDifference(npc, _block.position);

            if (Mathf.Abs(angleDiff) <= 90)
            {
                _messege.ReleaseKey(Control_Config.GetKey("moveUp", "ship"));
                Keys.Add(Control_Config.GetKey("moveDown", "ship"));
            }
            else
            {
                _messege.ReleaseKey(Control_Config.GetKey("moveDown", "ship"));
                Keys.Add(Control_Config.GetKey("moveUp", "ship"));
            }

            if (angleDiff <= 0f)
            {
                _messege.ReleaseKey(Control_Config.GetKey("turnRight", "ship"));
                Keys.Add(Control_Config.GetKey("turnLeft", "ship"));
            } else if (angleDiff > 0f)
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
}

