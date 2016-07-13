using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * STATES AND TRANSITIONS
 * */
public enum Transition
{
    None = 0,
    Wait,
    ContinueSearch,
    ContinuePatrol,
    Follow,
    Shift,
    SawEnemy,
    ReachEnemy,
    PullOff,
    LostEnemy,
    Guard,
    GoAround,
    Eject,
}

public enum FSMStateID
{
    None = 0,
    Static,
    Searching,
    Shifting,
    Following,
    Patrolling,
    Chasing,
    Attacking,
    Scatter,
    Divert,
    CombatDivert,
    Defensive,
    Ejecting,
}

public class FSM : MonoBehaviour
{
    private List<FSMState> _fsmStates;
    
    private FSMStateID _currentStateID;
    public FSMStateID CurrentStateID { get { return _currentStateID; } }
    
    private FSMState _currentState;
    public FSMState CurrentState { get { return _currentState; } }

    public FSM()
    {
        _fsmStates = new List<FSMState>();
    }

    /*
     * STATE MANAGEMENT UTILITIES
     * */

    /// <summary>
    /// Add new state into the list
    /// </summary>
    /// <param name="fsmState">Fsm state.</param>
    public void AddFSMState(FSMState fsmState)
    {
        if (fsmState == null)
        {
            Debug.LogError("FSM ERROR: Null reference is not allowed");
        }
        
        // First state inserted is also the initial state
        // the state the machine is in when the simulation begins
        if (_fsmStates.Count == 0)
        {
            _fsmStates.Add(fsmState);
            _currentState = fsmState;
            _currentStateID = fsmState.ID;
            return;
        }
        
        foreach (FSMState state in _fsmStates)
        {
            if(state.ID == fsmState.ID)
            {
                Debug.LogError("FSM ERROR: Trying to add a state that was already instantiated");
                return;
            }
        }
        
        _fsmStates.Add(fsmState);
    }
    
    /// <summary>
    /// Deletes the state.
    /// </summary>
    /// <param name="fsmState">Fsm state.</param>
    public void DeleteState(FSMStateID fsmState)
    {
        if (fsmState == FSMStateID.None)
        {
            Debug.LogError("FSM ERROR: null id is not allowed");
            return;
        }
        
        foreach (FSMState state in _fsmStates)
        {
            if(state.ID == fsmState)
            {
                _fsmStates.Remove(state);
                return;
            }
        }
        Debug.LogError("FSM ERROR: The state passed was not on the list. Impossible to delete it");
    }
    
    /// <summary>
    /// This method tries to change the state the FSM is in based on
    /// the current state and the transition passed. If current state
    ///  doesn´t have a target state for the transition passed, 
    /// an ERROR message is printed.
    /// </summary>
    public void PerformTransition(Transition trans)
    {
        if (trans == Transition.None)
        {
            Debug.LogError("FSM ERROR: Null transition is not allowed");
            return;
        }
        
        FSMStateID id = _currentState.GetOutputState(trans);
        if (id == FSMStateID.None)
        {
            Debug.LogError("FSM ERROR: Current State does not have a target state for this transition");
            return;
        }
        
        // Update the currentStateID and currentState
        _currentStateID = id;
        foreach (FSMState state in _fsmStates)
        {
            if(state.ID == _currentStateID)
            {
                _currentState = state;
                break;
            }
        }
    }
    /*
     * AIAGENT ATTRIBUTES
     * */
    protected List<ShipAttributes> OtherShips;

    /*
     * AIAGENT VIRTUAL FUNCTIONS
     * */
	// Use this for initialization
	void Start () {	Initialize (); }
	// Update is called once per frame
	void Update () { FSMUpdate (); }
    // Called strictly once per frame
	void LateUpdate() { FSMLateUpdate ();	}

    // virtual functions for AIAgents
    protected virtual void Initialize(){}
    protected virtual void FSMUpdate(){}
    protected virtual void FSMLateUpdate(){}
}

