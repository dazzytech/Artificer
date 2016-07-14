using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Space.Ship;

public class SimpleFriendlyController: FSM
{
    ShipInputReceiver _controller;
    
    // Raider variables
    public string enemyTag = "Enemy";

    public Transform follow;

    public ShipAttributes attributes;
    
    protected override void Initialize()
    {
        OtherShips = new List<ShipAttributes>();
        // Get every transform
        // within _ships
        GameObject root = GameObject.Find("_ships");
        foreach (ShipAttributes child in
                 root.GetComponentsInChildren<ShipAttributes>())
        {
            if(child.transform != transform)
                OtherShips.Add(child);
        }
        
        attributes = GetComponent<ShipAttributes>();
        
        // init the keys
        CurrentState.Keys = new List<KeyCode>();
    }
    
    protected override void FSMUpdate()
    {
        if(CurrentState != null)
            if (CurrentState.Keys != null)
                _controller.ReceiveKey(CurrentState.Keys);

        if (CurrentState is CircleFollowState)
        {
            // Test to see if follow object is dead
            if(follow == null)
            {
                if(follow == null)
                {
                    // change state to search?
                    SetTransition(Transition.ContinuePatrol);
                }
            }
        }
    }
    
    protected override void FSMLateUpdate()
    {
        GameObject root = GameObject.Find("_ships");
        foreach (ShipAttributes child in
                 root.GetComponentsInChildren<ShipAttributes>())
        {
            if(child.transform != transform && !OtherShips.Contains(child))
                OtherShips.Add(child);
        }
        
        List<Transform> enemies = new List<Transform>();
        foreach (ShipAttributes ship in OtherShips)
        {
            if(ship == null)
            {
                OtherShips.Remove(ship);
                break;
            }
            if(ship.AlignmentLabel == enemyTag)
                enemies.Add(ship.transform);
        }
        
        if (CurrentState != null)
        {
            CurrentState.Reason(enemies, this.transform);
            CurrentState.Act(enemies, this.transform);
        } 

        if (CurrentState is SearchState)
        {
            // Test if needing to follow
            if (follow != null)
            {
                SetTransition(Transition.Follow);
                ((CircleFollowState)CurrentState).UpdateFollow(follow);
            }
        }
    }
    
    private void ConstructFSM()
    {       
        //now pass paths in constructor
        SearchState pState = new SearchState( _controller, transform, follow);
        pState.AddTransition(Transition.Follow, FSMStateID.Following);
        pState.AddTransition(Transition.SawEnemy, FSMStateID.Chasing);
        pState.AddTransition(Transition.Eject, FSMStateID.Ejecting);
        pState.AddTransition(Transition.GoAround, FSMStateID.Divert);  

        CircleFollowState cfState = new CircleFollowState(_controller, follow);
        cfState.AddTransition(Transition.ContinueSearch, FSMStateID.Searching);
        cfState.AddTransition(Transition.SawEnemy, FSMStateID.Chasing);
        cfState.AddTransition(Transition.Eject, FSMStateID.Ejecting);
        cfState.AddTransition(Transition.GoAround, FSMStateID.Divert);
        
        IdleState iState = new IdleState(20f, _controller);
        iState.AddTransition(Transition.ContinuePatrol, FSMStateID.Searching);
        iState.AddTransition(Transition.Shift, FSMStateID.Shifting);
        iState.AddTransition(Transition.SawEnemy, FSMStateID.Defensive);
        iState.AddTransition(Transition.Eject, FSMStateID.Ejecting);
        
        ShiftState shState = new ShiftState(_controller);
        shState.AddTransition(Transition.Wait, FSMStateID.Static);
        shState.AddTransition(Transition.Eject, FSMStateID.Ejecting);
        
        ChaseState cState = new ChaseState(_controller);
        cState.AddTransition(Transition.LostEnemy, FSMStateID.Searching);
        cState.AddTransition(Transition.ReachEnemy, FSMStateID.Attacking);
        cState.AddTransition(Transition.Eject, FSMStateID.Ejecting);
        cState.AddTransition(Transition.GoAround, FSMStateID.Divert);
        
        AttackState aState = new AttackState(_controller, 2f);
        aState.AddTransition(Transition.LostEnemy, FSMStateID.Searching);
        aState.AddTransition(Transition.SawEnemy, FSMStateID.Chasing);
        aState.AddTransition(Transition.PullOff, FSMStateID.Scatter);
        aState.AddTransition(Transition.GoAround, FSMStateID.CombatDivert);
        aState.AddTransition(Transition.Eject, FSMStateID.Ejecting);
        
        ScatterState sState = new ScatterState(_controller);
        sState.AddTransition(Transition.LostEnemy, FSMStateID.Searching);
        sState.AddTransition(Transition.ReachEnemy, FSMStateID.Attacking);
        sState.AddTransition(Transition.Eject, FSMStateID.Ejecting);
        
        CombatDivertState gState = new CombatDivertState(_controller);
        gState.AddTransition(Transition.LostEnemy, FSMStateID.Searching);
        gState.AddTransition(Transition.SawEnemy, FSMStateID.Chasing);
        gState.AddTransition(Transition.PullOff, FSMStateID.Scatter);
        gState.AddTransition(Transition.Eject, FSMStateID.Ejecting);
        
        DivertState diState = new DivertState(_controller);
        diState.AddTransition(Transition.LostEnemy, FSMStateID.Searching);
        diState.AddTransition(Transition.SawEnemy, FSMStateID.Chasing);
        diState.AddTransition(Transition.Eject, FSMStateID.Ejecting);
        
        EjectState eState = new EjectState(_controller);
        
        AddFSMState(pState);
        AddFSMState(cfState);
        AddFSMState(iState);
        AddFSMState(shState);
        AddFSMState(cState);
        AddFSMState(aState);
        AddFSMState(sState);
        AddFSMState(gState);
        AddFSMState(eState);
        AddFSMState(diState);
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