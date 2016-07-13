using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EjectState : FSMState
{
    public EjectState(ShipInputReceiver m)
    {
        _messege = m;
        _stateID = FSMStateID.Ejecting;
        Keys = new List<KeyCode>();
    }

    public override void Reason(List<Transform> enemies, Transform npc)
    {}

    public override void Act(List<Transform> enemies, Transform npc)
    {
        Keys.Clear();

        Keys.Add(Control_Config.GetKey("eject", "ship"));
    }
}

