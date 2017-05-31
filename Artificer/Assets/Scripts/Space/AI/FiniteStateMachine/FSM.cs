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
    public abstract class FSM : MonoBehaviour
    {
        #region ATTRIBUTES

        #region STATE MANAGEMENT 

        private FSMStateID m_currentStateID;

        private FSMState m_currentState;

        #endregion

        #region AGENT ATTRIBUTES

        protected List<ShipAttributes> m_ships
            = new List<ShipAttributes>();

        protected List<Transform> m_targets;

        #endregion

        protected ShipInputReceiver m_message;

        #endregion

        #region ACCESSOR

        public FSMStateID CurrentStateID { get { return m_currentStateID; } }

        public FSMState CurrentState { get { return m_currentState; } }

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

        /// <summary>
        /// This method tries to change the state the FSM is in based on
        /// the current state and the transition passed. If current state
        ///  doesn´t have a target state for the transition passed, 
        /// an ERROR message is printed.
        /// </summary>
        public void SetTransition(Transition trans)
        {
            if (trans == Transition.None)
            {
                Debug.LogError("FSM ERROR: Null transition is not allowed");
                return;
            }

            FSMStateID id = m_currentState.GetOutputState(trans);
            if (id == FSMStateID.None)
            {
                Debug.LogError("FSM ERROR: Current State does not have a target state for this transition");
                return;
            }

            // Update the currentStateID and currentState
            m_currentStateID = id;
            foreach (FSMState state in StateMap())
            {
                if (state.ID == m_currentStateID)
                {
                    m_currentState = state;
                    break;
                }
            }
        }

        #region VIRTUAL FUNCTIONS

        protected abstract List<FSMState> StateMap();

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