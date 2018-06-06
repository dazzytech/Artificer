using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using Space.Ship;
using Stations;
using Game;
using Data.Space;

namespace Space.AI
{
    /// <summary>
    /// Controls how the ai interacts with 
    /// the ship
    /// </summary>
    public enum ControlStyle { NONE, DOGFIGHTER, AUTOTARGET };

    #region STATES AND TRANSITIONS

    public enum Transition
    {
        None = 0,
        Default,
        Wait,
        Resume,
        Travel,
        Evade,
        Engage,
        Pursuit,
        Guard,
        Strafe,
        Eject,
    }

    public enum FSMStateID
    {
        None = 0,
        Static,
        Strafing,
        Following,
        Travelling,
        Pursuing,
        Attacking,
        Scatter,
        Evading,
        CombatDivert,
        Defensive,
        Ejecting,
        Custom,
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
        protected List<ShipAccessor> m_ships
            = new List<ShipAccessor>();

        protected List<StationAccessor> m_stations
            = new List<StationAccessor>();

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

        /// <summary>
        /// If we are this distance away from 
        /// home some states will traverse home
        /// </summary>
        protected float m_homeDistance = 100f;

        #endregion

        #region TEAM ATTRIBUTES

        /// <summary>
        /// The other ships that this ship is in a squad with
        /// </summary>
        //protected List<Transform> m_team;

        /// <summary>
        /// Typically the station that the 
        /// assign is spawned by to protect
        /// </summary>
        private Transform m_home;

        /// <summary>
        /// What team this AI belongs to
        /// </summary>
        protected Teams.TeamController m_team;

        #endregion

        #region HISTORICAL DATA

        /// <summary>
        /// The function previous always called 
        /// when the resume transition is invoked
        /// </summary>
        private Stack<FSMStateID> m_previousStates;

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
        protected ControlStyle m_controlCategory;

        private ShipAccessor m_ship;

        #endregion

        #endregion

        #region ACCESSOR

        public FSMStateID CurrentStateID { get { return m_currentStateID; } }

        public FSMState CurrentState { get { return m_currentState; } }

        public ShipInputReceiver Con
        {
            get { return m_ship.Input; } 
        }

        public ShipAccessor Ship
        {
            get { return m_ship; }
        }

        public Teams.TeamController Team
        {
            get { return m_team; }
        }

        public Transform Home
        {
            get { return m_home; }
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

        public float HomeDistance
        {
            get { return m_homeDistance; }
        }

        public ControlStyle Control
        {
            get { return m_controlCategory; }
        }

        #endregion

        #region MONO BEHAVIOUR

        // Use this for initialization
        void Awake()
        {
            if (!hasAuthority)
                return;

            // Assign ship references
            m_ship = GetComponent<ShipAccessor>();

            if (m_ship.Complete)
                Initialize();
            else
                // Listen for creation of ship
                m_ship.OnShipCompleted += ShipCreatedEvent;
        } 

        // Update is called once per frame
        void Update() { if (hasAuthority) FSMUpdate(); }
        // Called strictly once per frame
        void LateUpdate() { if (hasAuthority) FSMLateUpdate(); }

        #endregion

        #region PUBLIC INTERACTION

        #region INITIALIZE

        public void EstablishAgent(AgentData agent)
        {
            m_controlCategory = GetStyle(agent.Type);
            
            EstablishRanges
                (Convert.ToInt32(agent.EngageDistance),
                 Convert.ToInt32(agent.PursuitDistance),
                 Convert.ToInt32(agent.AttackDistance),
                 Convert.ToInt32(agent.PullOffDistance));       
        }

        /// <summary>
        /// Assigns our team controller to agent
        /// for team tracking
        /// </summary>
        /// <param name="team"></param>
        public void AssignTeam(Teams.TeamController team)
        {
            m_team = team;
        }

        /// <summary>
        /// Assigns the station that spawned the agent
        /// or escort
        /// </summary>
        /// <param name="home"></param>
        public void AssignHome(Transform home)
        {
            m_home = home;
        }

        /// <summary>
        /// different agent types use the target differently
        /// e.g. attack, travel to
        /// </summary>
        /// <param name="target"></param>
        public void AssignTarget(Transform target)
        {
            Target = target;
        }

        #endregion

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

                RecordState();
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
                FSMStateID newState = m_previousStates.Pop();

                //RecordState();

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
            if(m_previousStates.Count < 5)
            m_previousStates.Push(m_currentStateID);

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
            Con.ReleaseKey(Control_Config.GetKey("moveUp", "ship"));
            Con.ReleaseKey(Control_Config.GetKey("moveDown", "ship"));
            Con.ReleaseKey(Control_Config.GetKey("turnLeft", "ship"));
            Con.ReleaseKey(Control_Config.GetKey("turnRight", "ship"));
            Con.ReleaseKey(Control_Config.GetKey("strafeLeft", "ship"));
            Con.ReleaseKey(Control_Config.GetKey("strafeRight", "ship"));
            Con.ReleaseKey(Control_Config.GetKey("fire", "ship"));
        }

        private ControlStyle GetStyle(string control)
        {
            switch (control)
            {
                case "dogfighter":
                    return ControlStyle.DOGFIGHTER;
                case "autotarget":
                    return ControlStyle.AUTOTARGET;
                default:
                    return ControlStyle.NONE;
            }
        }

        #endregion

        #region VIRTUAL FUNCTIONS

        #region FSM
        
        protected virtual void Initialize()
        {
            SystemManager.Events.EventShipDestroyed
                    += ShipDestroyedEvent;

            InvokeRepeating("SeekShips", 0, 1f);

            m_previousStates = new Stack<FSMStateID>();

            switch (m_controlCategory)
            {
                case ControlStyle.DOGFIGHTER:
                    m_ship.SetTargetAttributes(false);
                    break;
                case ControlStyle.AUTOTARGET:
                    m_ship.SetTargetAttributes(true, 0);
                    break;
            }
        }

        protected virtual void FSMUpdate()
        {
            if (CurrentState.Keys != null)
                Con.ReceiveKey(CurrentState.Keys);
        }

        protected virtual void FSMLateUpdate()
        {
            if (CurrentState != null)
            {
                CurrentState.Reason();
                CurrentState.Act();
            }
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
        protected virtual void GetClosestTarget(float maxDistance, Vector3 origin)
        {
            if (m_targets == null)
                return;

            SeekTargets();

            // This is used to find the closest object 
            float minDistance = float.MaxValue;

            // If target exists include it
            if (m_target != null)
                minDistance = Vector3.Distance(origin,
                    m_target.position);

            foreach (Transform target in m_targets)
            {
                // basic error proofing 
                if (target == null)
                    continue;

                // Discover the distance between this and target
                float distance = Vector3.Distance(origin, target.position);

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
                ClientScene.FindLocalObject(DD.SelfID).transform;

            if (destroyed == m_target)
                m_target = null;
            if (destroyed == m_tempTarget)
                m_tempTarget = null;

            if (m_targets.Contains(destroyed))
                m_targets.Remove(destroyed);

            if (m_ships.Contains(destroyed.GetComponent<ShipAccessor>()))
                m_ships.Remove(destroyed.GetComponent<ShipAccessor>());
        }

        /// <summary>
        /// When ship is built we will
        /// initialize the agent
        /// only runs once
        /// </summary>
        /// <param name="att"></param>
        private void ShipCreatedEvent()
        {
                m_ship.OnShipCompleted -= ShipCreatedEvent;
                Initialize();
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
            foreach (ShipAccessor child in
                        root.GetComponentsInChildren<ShipAccessor>())
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
                foreach (StationAccessor child in
                         root.GetComponentsInChildren<StationAccessor>())
                {
                    if (!m_stations.Contains(child))
                        m_stations.Add(child);

                    yield return null;
                }

                root = GameObject.Find("Team_B");
                foreach (StationAccessor child in
                         root.GetComponentsInChildren<StationAccessor>())
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