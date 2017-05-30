using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DivertState
    : FSMState
{
    private Vector3 _dest;
    private Transform _block;

    public DivertState(ShipMessegeController m)
    {
        _messege = m;
        _stateID = FSMStateID.Divert;
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
        /*if (enemies.Count <= 0 || enemies [0] == null)
        {
            npc.SendMessage("SetTransition", Transition.LostEnemy);
            return;
        }*/
        
        Transform target = DestUtil.FindClosestEnemy(enemies, npc.position);
        // no enemies
        if (target == null)
            return;
        _dest = target.position;
        float dist = Vector3.Distance(npc.position, _dest);

        _block = DestUtil.FrontIsClear(npc, 5f, 90f);
        
        if(_block == null)
        {
            _block = DestUtil.ShipWithinProximity(npc, 2f);

            if(_block == null)
            {
                if (dist < 200.0f)
                    npc.SendMessage("SetTransition", Transition.SawEnemy);
                else if (dist > 200.0f)
                    npc.SendMessage("SetTransition", Transition.LostEnemy);
            }
        }
    }
    
    public override void Act(List<Transform> enemies, Transform npc)
    {
        Keys.Clear();

        if (_block != null)
        {
            float angleDiff = DestUtil.FindAngleDifference(npc, _block.position);

            if(Vector3.Distance(npc.position, _block.position) < 3f)
            {
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
}

