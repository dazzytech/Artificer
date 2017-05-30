using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SimpleFriendlyGuardController
{/*
    ShipMessegeController _controller;
    
    // Raider variables
    public string enemyTag = "Enemy";
    public string pathTag = "PathNode";
    
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
                _controller.ProcessKey(CurrentState.Keys);
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
    }
    
    private void ConstructFSM()
    {
        Path newPath = new Path();

        newPath.GetNodesOfTag(pathTag);
        
        //now pass paths in constructor
        PatrolState pState = new PatrolState(newPath, _controller, transform);
        pState.AddTransition(Transition.Wait, FSMStateID.Static);
        pState.AddTransition(Transition.Guard, FSMStateID.Defensive);
        pState.AddTransition(Transition.Eject, FSMStateID.Ejecting);
        pState.AddTransition(Transition.GoAround, FSMStateID.Divert);
        
        DefenseState dState = new DefenseState(newPath, _controller);
        dState.AddTransition(Transition.LostEnemy, FSMStateID.Patrolling);
        dState.AddTransition(Transition.SawEnemy, FSMStateID.Chasing);
        dState.AddTransition(Transition.Eject, FSMStateID.Ejecting);
        
        IdleState iState = new IdleState(20f, _controller);
        iState.AddTransition(Transition.ContinuePatrol, FSMStateID.Patrolling);
        iState.AddTransition(Transition.Shift, FSMStateID.Shifting);
        iState.AddTransition(Transition.SawEnemy, FSMStateID.Defensive);
        iState.AddTransition(Transition.Eject, FSMStateID.Ejecting);
        
        ShiftState shState = new ShiftState(_controller);
        shState.AddTransition(Transition.Wait, FSMStateID.Static);
        shState.AddTransition(Transition.Eject, FSMStateID.Ejecting);
        
        ChaseState cState = new ChaseState(_controller);
        cState.AddTransition(Transition.LostEnemy, FSMStateID.Patrolling);
        cState.AddTransition(Transition.ReachEnemy, FSMStateID.Attacking);
        cState.AddTransition(Transition.Eject, FSMStateID.Ejecting);
        cState.AddTransition(Transition.GoAround, FSMStateID.Divert);
        
        AttackState aState = new AttackState(_controller, 2f);
        aState.AddTransition(Transition.LostEnemy, FSMStateID.Patrolling);
        aState.AddTransition(Transition.SawEnemy, FSMStateID.Chasing);
        aState.AddTransition(Transition.PullOff, FSMStateID.Scatter);
        aState.AddTransition(Transition.GoAround, FSMStateID.CombatDivert);
        aState.AddTransition(Transition.Eject, FSMStateID.Ejecting);
        
        ScatterState sState = new ScatterState(_controller);
        sState.AddTransition(Transition.LostEnemy, FSMStateID.Patrolling);
        sState.AddTransition(Transition.ReachEnemy, FSMStateID.Attacking);
        sState.AddTransition(Transition.Eject, FSMStateID.Ejecting);
        
        CombatDivertState gState = new CombatDivertState(_controller);
        gState.AddTransition(Transition.LostEnemy, FSMStateID.Patrolling);
        gState.AddTransition(Transition.SawEnemy, FSMStateID.Chasing);
        gState.AddTransition(Transition.PullOff, FSMStateID.Scatter);
        gState.AddTransition(Transition.Eject, FSMStateID.Ejecting);
        
        DivertState diState = new DivertState(_controller);
        diState.AddTransition(Transition.LostEnemy, FSMStateID.Patrolling);
        diState.AddTransition(Transition.SawEnemy, FSMStateID.Chasing);
        diState.AddTransition(Transition.Eject, FSMStateID.Ejecting);
        
        EjectState eState = new EjectState(_controller);
        
        AddFSMState(pState);
        AddFSMState(iState);
        AddFSMState(shState);
        AddFSMState(cState);
        AddFSMState(aState);
        AddFSMState(sState);
        AddFSMState(dState);
        AddFSMState(gState);
        AddFSMState(eState);
        AddFSMState(diState);
    }
    
    /*
     * ACTIONS
     * 
    public void SetController(ShipMessegeController cont)
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
    }*/
}