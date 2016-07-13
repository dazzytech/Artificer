using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SimpleFriendlyCargoController : FSM
{
    ShipInputReceiver _controller;

    public Transform target;

    protected override void Initialize()
    {
    }

    protected override void FSMUpdate()
    {
        if (CurrentState == null)
            return;
        if(CurrentState.Keys != null)
            _controller.ReceiveKey(CurrentState.Keys);
    }

    protected override void FSMLateUpdate()
    {
        if (CurrentState == null)
            return;


        if (CurrentState != null)
        {
            CurrentState.Reason(null, this.transform);
            CurrentState.Act(null, this.transform);
        }

        if (CurrentState is SearchState)
        {
            // nothing here

        }
    }

    private void ConstructFSM()
    {

        //now pass paths in constructor
        TransportState pState = new TransportState( _controller, transform, null);
        pState.AddTransition(Transition.Follow, FSMStateID.Following);
        //pState.AddTransition(Transition.SawEnemy, FSMStateID.Chasing);
        //pState.AddTransition(Transition.Eject, FSMStateID.Ejecting);
        //pState.AddTransition(Transition.GoAround, FSMStateID.Divert);    

        pState.SelectTarget(target);

        DivertState dState = new DivertState(_controller);
        dState.AddTransition(Transition.LostEnemy, FSMStateID.Searching);
        dState.AddTransition(Transition.SawEnemy, FSMStateID.Chasing);
        dState.AddTransition(Transition.Eject, FSMStateID.Ejecting);

        EjectState eState = new EjectState(_controller);
        
        AddFSMState(pState);
        AddFSMState(eState);
        AddFSMState(dState);
    }

    /*
     * ACTIONS
     * */
    public void SetController(ShipInputReceiver cont)
    {
        _controller = cont;
    }

    public void SetTransition(Transition t)
    {
        PerformTransition(t);
    }

    public void SetAlignment()
    {
        transform.SendMessage("SetShipAlignment", "Friendly");
        ConstructFSM();
    }
}

