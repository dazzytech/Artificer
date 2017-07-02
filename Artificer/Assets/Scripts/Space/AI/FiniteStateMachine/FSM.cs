using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Space.Ship;
using Stations;
using Game;

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

        private List<FSMState> m_fsmStates = new List<FSMState>();

        #endregion

        #region AGENT ATTRIBUTES

        #region TARGET ATTRIBUTES

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
        protected List<Transform> m_targets = new List<Transform>();

        /// <summary>
        /// Currently selected target
        /// </summary>
        protected Transform m_target;

        /// <summary>
        /// Target that is assignable by the states
        /// </summary>
        protected Transform m_tempTarget;

        #endregion

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

        #region TEAM ATTRIBUTES

        /// <summary>
        /// The other ships that this ship is in a squad with
        /// </summary>
        protected List<Transform> m_team;

        /// <summary>
        /// What team this AI belongs to
        /// </summary>
        protected int m_teamID;

        #endregion

        #region HISTORICAL DATA

        /// <summary>
        /// The function previous always called 
        /// when the resume transition is invoked
        /// </summary>
        private FSMStateID m_previousStateID;

        /// <summary>
        /// If the previous state only ran for an alloted 
        /// time then we will only want to run it for that
        /// period again
        /// </summary>
        private float m_previousTimer;

        /// <summary>
        /// If the previous state only ran for a 
        /// duration then we may want the same
        /// trans to occur on timeout
        /// </summary>
        private Transition m_previousTimeout;

        #endregion

        #endregion

        #region SHIP ATTRIBUTES

        /// <summary>
        /// Defines the behaviour of the ship defined by its class
        /// TODO: GET THIS FROM SHIP DATA
        /// </summary>
        protected string m_shipCategory;

        private ShipInputReceiver m_message;

        private ShipAttributes m_att;

        #endregion

        #endregion

        #region ACCESSOR

        public FSMStateID CurrentStateID { get { return m_currentStateID; } }

        public FSMState CurrentState { get { return m_currentState; } }

        public ShipInputReceiver Con
        {
            get { return m_message; } 
        }

        public ShipAttributes Att
        {
            get { return m_att; }
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
        void Start()
        {
            if (!hasAuthority)
                return;

            // Assign ship references
            m_message = GetComponent<ShipInputReceiver>();
            m_att = GetComponent<ShipAttributes>();

            // Listen for creation of ship
            SystemManager.Events.EventShipCreated += ShipCreatedEvent;
        } 

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

            ReleaseAllKeys();

            // If we are resuming then state will
            // be previously ran
            if (trans == Transition.Resume)
            {
                // create temp id to store new so not overwritten
                FSMStateID newState = m_previousStateID;

                RecordState();

                m_currentStateID = newState;

                ApplyChanges();

                if (m_previousTimeout == Transition.None)
                    m_currentState.SetDuration
                        (m_previousTimer, m_previousTimeout);
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

                RecordState();

                // Update the currentStateID and currentState
                m_currentStateID = id;

                ApplyChanges();
            }
        }

        /// <summary>
        /// invokes the set transition function above
        /// but then sets it to only occur for a set period
        /// before resuming
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="timer"></param>
        public void SetTransition(Transition trans, float timer, Transition timeout)
        {
            SetTransition(trans);

            // Set our new current state to operate on a timer
            m_currentState.SetDuration(timer, timeout);
        }

        #endregion

        #region PRIVATE UTILTIES

        /// <summary>
        /// Makes the new state
        /// the current state
        /// </summary>
        private void ApplyChanges()
        {
            foreach (FSMState state in m_fsmStates)
            {
                if (state.ID == m_currentStateID)
                {
                    m_currentState = state;
                    break;
                }
            }
        }

        /// <summary>
        /// Takes a snapshot of the current state
        /// for resuming
        /// </summary>
        private void RecordState()
        {
            // Keep track of our last state incase we 
            // want to return to it
            m_previousStateID = m_currentStateID;

            // Clear timer info
            m_previousTimer = 0f;
            m_previousTimeout = Transition.None;

            // snapshot timer info if needed
            if (m_currentState.TimeoutTransition != Transition.None)
                m_previousTimeout =
                    m_currentState.TimeoutTransition;

            if (m_currentState.TimeoutTimer != 0)
                m_previousTimer =
                    m_currentState.TimeoutTimer;
        }

        /// <summary>
        /// Called to release all keys 
        /// that are being currently processed
        /// </summary>
        private void ReleaseAllKeys()
        {
            m_message.ReleaseKey(Control_Config.GetKey("moveUp", "ship"));
            m_message.ReleaseKey(Control_Config.GetKey("moveDown", "ship"));
            m_message.ReleaseKey(Control_Config.GetKey("turnLeft", "ship"));
            m_message.ReleaseKey(Control_Config.GetKey("turnRight", "ship"));
            m_message.ReleaseKey(Control_Config.GetKey("strafeLeft", "ship"));
            m_message.ReleaseKey(Control_Config.GetKey("strafeRight", "ship"));
            m_message.ReleaseKey(Control_Config.GetKey("fire", "ship"));
        }

        #endregion

        #region VIRTUAL FUNCTIONS

        #region FSM
        
        protected virtual void Initialize()
        {
            SystemManager.Events.EventShipDestroyed
                    += ShipDestroyedEvent;

            InvokeRepeating("SeekShips", 0, 1f);
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

        #region TARGET UTILITIES

        /// <summary>
        /// Overridden by child agents
        /// to assign targets from their requirements
        /// </summary>
        protected virtual void SeekTargets()
        {}

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

            SeekTargets();

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

        #endregion

        #region EVENTS

        /// <summary>
        /// Finds out if our target was destroyed
        /// </summary>
        /// <param name="DD"></param>
        private void ShipDestroyedEvent(DestroyDespatch DD)
        {
            Transform destroyed =
                ClientScene.FindLocalObject(DD.Self).transform;

            if (destroyed == m_target)
                m_target = null;
            if (destroyed == m_tempTarget)
                m_tempTarget = null;

            if (m_targets.Contains(destroyed))
                m_targets.Remove(destroyed);

            if (m_ships.Contains(destroyed.GetComponent<ShipAttributes>()))
                m_ships.Remove(destroyed.GetComponent<ShipAttributes>());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="att"></param>
        private void ShipCreatedEvent(CreateDispatch CD)
        {
            if(netId.Value == CD.Self)
            {
                SystemManager.Events.EventShipCreated -= ShipCreatedEvent;
                Initialize();
            }
        }

        #endregion

        #region COROUTINES

        private void SeekShips()
        {
                // Clear any m_ships that are now null
                for(int i = 0;i < m_ships.Count; i++)
                    if (m_ships[i] == null)
                        m_ships.RemoveAt(i);

                // Get every ship and store if we dont have them
                GameObject root = GameObject.Find("_ships");
                foreach (ShipAttributes child in
                         root.GetComponentsInChildren<ShipAttributes>())
                {
                    if (!m_ships.Contains(child))
                        m_ships.Add(child);
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