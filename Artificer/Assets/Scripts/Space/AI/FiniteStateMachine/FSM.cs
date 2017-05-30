using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Space.Ship;

namespace Space.AI
{
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

    /// <summary>
    /// Basic Finite State Machine that all AI agents extend from
    /// allows each agent to be assigned a specific set of 
    /// behaviours
    /// </summary>
    public class FSM : MonoBehaviour
    {
        #region ATTRIBUTES

        #region STATE MANAGEMENT 

        [SerializeField]
        private List<FSMState> _fsmStates = new List<FSMState>();

        private FSMStateID _currentStateID;
        public FSMStateID CurrentStateID { get { return _currentStateID; } }

        private FSMState _currentState;
        public FSMState CurrentState { get { return _currentState; } }

        #endregion

        #region AGENT ATTRIBUTES

        protected List<ShipAttributes> m_ships
            = new List<ShipAttributes>();

        protected List<Transform> m_targets;

        #endregion

        protected ShipInputReceiver m_message;

        #endregion

        #region ACCESSOR

        public ShipInputReceiver Con
        {
            get { return m_message; } 
        }

        #endregion

        #region MONO BEHAVIOUR

        // Use this for initialization
        void Start() { Initialize(); }
        // Update is called once per frame
        void Update() { FSMUpdate(); }
        // Called strictly once per frame
        void LateUpdate() { FSMLateUpdate(); }

        #endregion

        #region NEED REMOVAL

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
                if (state.ID == fsmState.ID)
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
                if (state.ID == fsmState)
                {
                    _fsmStates.Remove(state);
                    return;
                }
            }
            Debug.LogError("FSM ERROR: The state passed was not on the list. Impossible to delete it");
        }

        #endregion

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
                if (state.ID == _currentStateID)
                {
                    _currentState = state;
                    break;
                }
            }
        }

        #region VIRTUAL FUNCTIONS

        // virtual functions for AIAgents
        protected virtual void Initialize()
        {
            StartCoroutine("FindShips");
        }

        protected virtual void FSMUpdate()
        {
            if (CurrentState.Keys != null)
                m_message.ReceiveKey(CurrentState.Keys);
        }

        protected virtual void FSMLateUpdate()
        {
            CurrentState.Reason(m_targets, this.transform);
            CurrentState.Act(m_targets, this.transform);
        }

        #endregion

        #region COROUTINES

        private IEnumerator FindShips()
        {
            while (true)
            {
                m_ships.Clear();

                // Get every transform
                // within _ships
                GameObject root = GameObject.Find("_ships");
                foreach (ShipAttributes child in
                         root.GetComponentsInChildren<ShipAttributes>())
                {
                    if (!m_ships.Contains(child))
                        m_ships.Add(child);

                    yield return null;
                }

                yield return null;
            }
        }

        #endregion
    }

}