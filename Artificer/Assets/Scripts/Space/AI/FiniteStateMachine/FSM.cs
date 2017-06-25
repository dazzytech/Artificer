using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Space.Ship;
using Stations;

namespace Space.AI
{

    #region STATES AND TRANSITIONS

    public enum Transition
    {
        None = 0,
        Wait,
        Resume,
        Follow,
        Shift,
        ChaseEnemy,
        ReachEnemy,
        Evade,
        LostEnemy,
        Guard,
        Strafe,
        Eject,
    }

    public enum FSMStateID
    {
        None = 0,
        Static,
        Searching,
        Strafing,
        Following,
        Patrolling,
        Pursuing,
        Attacking,
        Scatter,
        Evading,
        CombatDivert,
        Defensive,
        Ejecting,
    }

    #endregion

    /// <summary>
    /// Basic Finite State Machine that all AI agents extend from
    /// allows each agent to be assigned a specific set of 
    /// behaviours
    /// </summary>
    public abstract class FSM : NetworkBehaviour
    {
        #region ATTRIBUTES

        #region STATE MANAGEMENT 

        private FSMStateID m_currentStateID;

        private FSMState m_currentState;

        protected List<FSMState> m_fsmStates = new List<FSMState>();

        #endregion

        #region AGENT ATTRIBUTES

        /// <summary>
        /// Stores reference to all other ships
        /// of all alignments
        /// </summary>
        protected List<ShipAttributes> m_ships
            = new List<ShipAttributes>();

        protected List<StationAttributes> m_stations
            = new List<StationAttributes>();

        /// <summary>
        /// Stores a reference to what we have targetted
        /// </summary>
        protected List<Transform> m_targets;

        /// <summary>
        /// Currently selected target
        /// </summary>
        protected Transform m_target;

        /// <summary>
        /// Target that is assignable by the states
        /// </summary>
        protected Transform m_tempTarget;

        #region DISTANCE VARIABLES
        
        /// <summary>
        /// How close we get to the target before moving to engage/pursue
        /// </summary>
        protected float m_engageDistance; 

        /// <summary>
        /// Distance before agent breaks off
        /// </summary>
        protected float m_pursuitDistance;

        /// <summary>
        /// How close a ship can get before the ship pulls off
        /// </summary>
        protected float m_pullOffDistance;

        /// <summary>
        /// How close the agent needs to be for the attack state
        /// </summary>
        protected float m_attackRange;

        #endregion

        /// <summary>
        /// The other ships that this ship is in a squad with
        /// </summary>
        protected List<Transform> m_team;

        /// <summary>
        /// What team this AI belongs to
        /// </summary>
        protected int m_teamID;

        /// <summary>
        /// Defines the behaviour of the ship defined by its class
        /// </summary>
        protected string m_shipCategory;

        /// <summary>
        /// The function previous always called 
        /// when the resume transition is invoked
        /// </summary>
        protected FSMStateID m_previousState;
        
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

        public Transform Target
        {
            get
            {
                if (m_tempTarget == null)
                    return m_target;
                else
                    return m_tempTarget;
            }
            set
            {
                m_tempTarget = value;
            }
        }

        public float PullOffDistance
        {
            get { return m_pullOffDistance; }
        }

        public float AttackRange
        {
            get { return m_attackRange; }
        }

        public float PersuitRange
        {
            get { return m_pursuitDistance; }
        }

        #endregion

        #region MONO BEHAVIOUR

        // Use this for initialization
        void Start() { if(hasAuthority) Initialize(); }
        // Update is called once per frame
        void Update() { if (hasAuthority) FSMUpdate(); }
        // Called strictly once per frame
        void LateUpdate() { if (hasAuthority) FSMLateUpdate(); }

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// called to init the ranges 
        /// for shared agent behaviour
        /// </summary>
        /// <param name="engage"></param>
        /// <param name="pursue"></param>
        /// <param name="attack"></param>
        /// <param name="pulloff"></param>
        public void EstablishRanges
            (float engage, float pursue, 
             float attack, float pulloff)
        {
            m_engageDistance = engage;
            m_pursuitDistance = pursue;
            m_attackRange = attack;
            m_pullOffDistance = pulloff;
        }

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

            fsmState.Initialize(this);

            // First state inserted is also the initial state
            // the state the machine is in when the simulation begins
            if (m_fsmStates.Count == 0)
            {
                m_fsmStates.Add(fsmState);
                m_currentState = fsmState;
                m_currentStateID = fsmState.ID;
                return;
            }

            foreach (FSMState state in m_fsmStates)
            {
                if (state.ID == fsmState.ID)
                {
                    Debug.LogError("FSM ERROR: Trying to add a state that was already instantiated");
                    return;
                }
            }

            m_fsmStates.Add(fsmState);
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

            foreach (FSMState state in m_fsmStates)
            {
                if (state.ID == fsmState)
                {
                    m_fsmStates.Remove(state);
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
        public void SetTransition(Transition trans)
        {
            if (trans == Transition.None)
            {
                Debug.LogError("FSM ERROR: Null transition is not allowed");
                return;
            }

            // If we are resuming then state will
            // be previously ran
            if (trans == Transition.Resume)
            {
                m_currentStateID = m_previousState;
            }
            else
            {
                // Else we have a new state
                FSMStateID id = m_currentState.GetOutputState(trans);
                if (id == FSMStateID.None)
                {
                    Debug.LogError("FSM ERROR: Current State does not have a target state for this transition");
                    return;
                }

                // Keep track of our last state incase we 
                // want to return to it
                m_previousState = m_currentStateID;

                // Update the currentStateID and currentState
                m_currentStateID = id;
            }

            foreach (FSMState state in m_fsmStates)
            {
                if (state.ID == m_currentStateID)
                {
                    m_currentState = state;
                    break;
                }
            }
        }

        #endregion

        #region VIRTUAL FUNCTIONS

        #region FSM

        // virtual functions for AIAgents
        protected virtual void Initialize()
        {
            StartCoroutine("FindShips");

            m_message = GetComponent<ShipInputReceiver>();
        }

        protected virtual void FSMUpdate()
        {
            if (CurrentState.Keys != null)
                m_message.ReceiveKey(CurrentState.Keys);
        }

        protected virtual void FSMLateUpdate()
        { 
            CurrentState.Reason();
            CurrentState.Act();
        }

        #endregion

        /// <summary>
        /// A one shot coroutine that finds the target
        /// closest to this agent
        /// </summary>
        /// <param name="maxDistance"></param>
        /// <returns></returns>
        protected virtual void GetClosestTarget(float maxDistance)
        {
            if (m_targets == null)
                return;

            // This is used to find the closest object 
            float minDistance = float.MaxValue;

            // If target exists include it
            if (m_target != null)
                minDistance = Vector3.Distance(transform.position,
                    m_target.position);

            foreach (Transform target in m_targets)
            {
                // basic error proofing 
                if (target == null)
                    continue;

                // Discover the distance between this and target
                float distance = Vector3.Distance(transform.position, target.position);

                // skip if this exceeds maximum distance 
                if (distance > maxDistance)
                    continue;

                // Check if this is closer then our current target
                if (distance < minDistance)
                {
                    // This object is closer

                    // Assign as target
                    m_target = target;

                    // Assign distance
                    minDistance = distance;
                }
            }
        }

        #endregion

        #region COROUTINES

        private IEnumerator FindShips()
        {
            while (true)
            {
                // Clear any m_ships that are now null
                int i = 0;
                while(i < m_ships.Count)
                {
                    if (m_ships[i] == null)
                        m_ships.RemoveAt(i);

                    yield return null;
                }

                // Get every ship and store if we dont have them
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

        private IEnumerator FindStations()
        {
            while (true)
            {
                // Clear any m_ships that are now null
                int i = 0;
                while (i < m_stations.Count)
                {
                    if (m_stations[i] == null)
                        m_stations.RemoveAt(i);

                    yield return null;
                }

                // Get every ship and store if we dont have them
                GameObject root = GameObject.Find("Team_A");
                foreach (StationAttributes child in
                         root.GetComponentsInChildren<StationAttributes>())
                {
                    if (!m_stations.Contains(child))
                        m_stations.Add(child);

                    yield return null;
                }

                root = GameObject.Find("Team_B");
                foreach (StationAttributes child in
                         root.GetComponentsInChildren<StationAttributes>())
                {
                    if (!m_stations.Contains(child))
                        m_stations.Add(child);

                    yield return null;
                }

                yield return null;
            }
        }

        #endregion
    }

}