using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Space.Ship;

public class SimpleEnemyController : FSM
{
    ShipInputReceiver _controller;

    // Raider variables
    public string pathTag = "PathNode";

    // Other raiders within pirate group
    public SimpleEnemyController[] otherRaiders;

    // if false then follow raider that is the lead
    public bool isLead;

    public Transform follow;

    public Transform target;
    public float trackDistance = 300f;

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
    }

    protected override void FSMUpdate()
    {
        if (CurrentState == null)
            return;
        if(CurrentState.Keys != null)
            _controller.ReceiveKey(CurrentState.Keys);

        if (CurrentState is CircleFollowState)
        {
            // Test to see if follow object is dead
            if(follow == null)
            {
                // follow obj is dead
                // See if there is a new lead
                foreach(SimpleEnemyController raider in otherRaiders)
                {
                    if(raider == null)
                        continue;

                    if(raider.isLead) follow = raider.transform;
                }
                if(follow == null)
                {
                    // there is no new lead
                    isLead = true;
                    // change state to search?
                    SetTransition(Transition.ContinueSearch);
                }
            }
        }
    }

    protected override void FSMLateUpdate()
    {
        if (CurrentState == null)
            return;

        OtherShips = new List<ShipAttributes>();
        // Get every transform
        // within _ships
        GameObject root = GameObject.Find("_ships");
        foreach (ShipAttributes child in
                 root.GetComponentsInChildren<ShipAttributes>())
        {
            if(!OtherShips.Contains(child))
                OtherShips.Add(child);
        }

        List<Transform> enemies = new List<Transform>();

        // Add station target
        if(target != null)
            enemies.Add(target);

        foreach (ShipAttributes ship in OtherShips)
        {
            if(ship == null)
            {
                OtherShips.Remove(ship);
                break;
            }
            if(ship.AlignmentLabel != "Enemy")
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
            else if(target != null)
            {
                ((SearchState)CurrentState).SelectTarget(target);
                if(Vector3.Distance(target.position,
                                    transform.position) > trackDistance)
                {
                    ((SearchState)CurrentState).DeselectTarget(transform);
                    //target = null; // dont remove targets for now to combat them
                }
            }
        }
    }

    private void ConstructFSM()
    {
        Path newPath = new Path();
        newPath.GetNodesOfTag(pathTag);

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
        
        ChaseState cState = new ChaseState(_controller);
        cState.AddTransition(Transition.LostEnemy, FSMStateID.Searching);
        cState.AddTransition(Transition.ReachEnemy, FSMStateID.Attacking);
        cState.AddTransition(Transition.Eject, FSMStateID.Ejecting);
        cState.AddTransition(Transition.GoAround, FSMStateID.Divert);
        
        AttackState aState = new AttackState(_controller, 3f);
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

        DivertState dState = new DivertState(_controller);
        dState.AddTransition(Transition.LostEnemy, FSMStateID.Searching);
        dState.AddTransition(Transition.SawEnemy, FSMStateID.Chasing);
        dState.AddTransition(Transition.Eject, FSMStateID.Ejecting);

        EjectState eState = new EjectState(_controller);
        
        AddFSMState(pState);
        AddFSMState(cfState);
        AddFSMState(cState);
        AddFSMState(aState);
        AddFSMState(sState);
        AddFSMState(gState);
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
        transform.SendMessage("SetShipAlignment", "Enemy");
        ConstructFSM();
    }

    public void SetGroup(ArrayList raidList)
    {
        otherRaiders = new SimpleEnemyController[raidList.Count];
        int i = 0;
        foreach (SimpleEnemyController raider in raidList)
        {
            otherRaiders[i++] = raider;
            if(raider.isLead) follow = raider.transform;
        }
        if (follow == null)
            isLead = true;

        //ConstructFSM();
    }
}

