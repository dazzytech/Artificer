using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Space.Ship;

public abstract class FSMState 
{
    protected Dictionary<Transition, FSMStateID> 
        _stateMap = new Dictionary<Transition, FSMStateID>();
    protected FSMStateID _stateID;
    public FSMStateID ID { get { return _stateID; } }
   
    //generic AIAgent Attributes
    protected ShipInputReceiver _messege;
    public List<KeyCode> Keys;

    public void AddTransition(Transition transition, FSMStateID id)
    {
        if (transition == Transition.None || ID == FSMStateID.None)
        {
            Debug.LogWarning("FSMState : Null transition not allowed");
            return;
        }

        // siince this is a deterministic FSM
        // check if trans is already in map
        if (_stateMap.ContainsKey(transition))
        {
            Debug.LogWarning("FSMState ERROR: transition is already inside the map");
            return;
        }

        _stateMap.Add(transition, id);
    }

    /// <summary>
    /// This method deletes a pair transition-state from this state´s map.
    /// If the transition was not inside the state´s map, an ERROR message is printed.
    /// </summary>
    public void DeleteTransition(Transition trans)
    {
        // check null
        if (trans == Transition.None)
        {
            Debug.LogError("FSMState ERROR: NullTransition is not allowed");
            return;
        }

        if (_stateMap.ContainsKey(trans))
        {
            _stateMap.Remove(trans);
            return;
        }
        Debug.LogError("FSMState ERROR: Transition passed was not on this State´s List");
    }

    /// <summary>
    /// This method returns the new state the FSM should be if
    ///    this state receives a transition  
    /// </summary>
    public FSMStateID GetOutputState(Transition trans)
    {
        if (trans == Transition.None)
        {
            Debug.LogError("FSMState ERROR: NullTransition is not allowed");
            return FSMStateID.None;
        }

        if (_stateMap.ContainsKey(trans))
        {
            return _stateMap[trans];
        }

        Debug.LogError("FSMState ERROR: " + trans+ " Transition passed to the State was not on the list");
        return FSMStateID.None;
    }


    // STAY IN FOR FUTURE REFERENCE

    /// <summary>
    /// Sets the message controller for the
    /// ship outside of constructor
    /// </summary>
    /// <param name="m">M.</param>
    public void SetMessage(ShipInputReceiver m)
    {
        _messege = m;
    }

    /// <summary>
    /// Sets the path for the state.
    /// used by just a few states
    /// </summary>
    /// <param name="npc">Npc.</param>
    /// <param name="path">Path.</param>
    public virtual void SetPath(Transform npc, Path path)
    {   
        // Do nothing
    }

    /// <summary>
    /// Decides if the state should transition to another on its list
    /// NPC is a reference to the npc tha is controlled by this class
    /// </summary>
    public abstract void Reason(List<Transform> objs, Transform npc);

    /// <summary>
    /// This method controls the behavior of the NPC in the game World.
    /// Every action, movement or communication the NPC does should be placed here
    /// NPC is a reference to the npc tha is controlled by this class
    /// </summary>
    public abstract void Act(List<Transform> objs, Transform npc);

}
