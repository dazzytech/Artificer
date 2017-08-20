using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Space.Ship;
using System;

namespace Space.AI
{
    [System.Serializable]
    public class FSMState
    {
        #region ATTRIBUTES

        #region STATE MANAGEMENT

        /// <summary>
        /// Possible states it is capable of transitioning to
        /// </summary>
        private Dictionary<Transition, FSMStateID>
            m_transitionMap = new Dictionary<Transition, FSMStateID>();

        protected FSMStateID m_stateID;

        [SerializeField]
        private FSM m_self;

        #endregion

        #region TIMER

        /// <summary>
        /// How long the state will be active for
        /// </summary>
        private float m_waitTime;

        /// <summary>
        /// Current time alloted since
        /// state has started
        /// </summary>
        private float m_curTime;

        /// <summary>
        /// What transition is performed
        /// when time has passed
        /// </summary>
        private Transition m_timeoutID;

        #endregion

        #region AI AGENT

        public List<KeyCode> Keys;

        // how close the angle should be (have default value)
        // todo editable
        [SerializeField]
        protected float m_angleAccuracy = 15f;

        #endregion

        #endregion

        #region ACCESSOR

        #region EXTERNAL UTILITIES

        /// <summary>
        /// The pending previous transition
        /// </summary>
        public Transition TimeoutTransition
        {
            get { return m_timeoutID; }
        }

        /// <summary>
        /// How long the 
        /// </summary>
        public float TimeoutTimer
        {
            get { return m_waitTime; }
        }

        /// <summary>
        /// Returns state we are currently in
        /// </summary>
        public FSMStateID ID
        {
            get { return m_stateID; }
        }

        #endregion

        /// <summary>
        /// Access the ship input component
        /// </summary>
        protected ShipInputReceiver Con
        {
            get { return m_self.Con; }
        }

        /// <summary>
        /// Access to the agent controller
        /// </summary>
        protected FSM Self
        {
            get { return m_self; }
        }

        /// <summary>
        /// If this state is working
        /// for only a duration
        /// </summary>
        protected bool Timed
        {
            get { return m_timeoutID != Transition.None; }
        }

        #endregion

        #region PUBLIC INTERACTION

        #region TRANSITION CONTROL

        /// <summary>
        /// Adds what behaviour will happen when 
        /// a transition is called
        /// </summary>
        /// <param name="transition"></param>
        /// <param name="id"></param>
        public void AddTransition(Transition transition, FSMStateID id)
        {
            if (transition == Transition.None || ID == FSMStateID.None)
            {
                Debug.LogWarning("FSMState : Null transition not allowed");
                return;
            }

            // siince this is a deterministic FSM
            // check if trans is already in map
            if (m_transitionMap.ContainsKey(transition))
            {
                Debug.LogWarning("FSMState ERROR: transition is already inside the map");
                return;
            }

            m_transitionMap.Add(transition, id);
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

            if (m_transitionMap.ContainsKey(trans))
            {
                m_transitionMap.Remove(trans);
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

            if (m_transitionMap.ContainsKey(trans))
            {
                return m_transitionMap[trans];
            }

            Debug.LogError("FSMState ERROR: " + trans + " Transition passed to the State was not on the list");
            return FSMStateID.None;
        }

        #endregion

        /// <summary>
        /// Sets the time duration of a state
        /// to run for that period
        /// </summary>
        /// <param name="timer"></param>
        /// <param name="timeoutTrans"></param>
        public void SetDuration(float timer, Transition timeoutTrans)
        {
            m_curTime = 0;
            m_waitTime = timer;

            m_timeoutID = timeoutTrans;
        }

        #region VIRTUAL FUNCTIONALITY

        /// <summary>
        /// Used to initialize the state and
        /// the  first thing to be called
        /// can be overriden to init vars in child
        /// </summary>
        /// <param name="selfRef"></param>
        public virtual void Initialize(FSM selfRef)
        {
            m_self = selfRef;

            AddTransition(Transition.Evade, FSMStateID.Evading);
            AddTransition(Transition.Eject, FSMStateID.Ejecting);
        }

        /// <summary>
        /// Decides if the state should transition to another on its list
        /// NPC is a reference to the npc tha is controlled by this class
        /// </summary>
        public virtual void Reason()
        {
            if (Self.transform == null)
                return;

            // Test for emergency eject
            // If ship is too damaged then depart
            if (m_self.Ship.EvacNeeded)
            {
                Self.SetTransition(Transition.Eject);
                return;
            }

            // Check if there is an object imminent with
            // the ship and attempt to evade
            Transform obsticle = DestUtil.ObjectWithinProximity
                (Self.transform, Self.PullOffDistance);

            if(obsticle != null)
            {
                // we have an obsticle too close
                // target and pulloff
                Self.Target = obsticle;
                Self.SetTransition(Transition.Evade);
                return; 
            }

            // Some states have a limited time they
            // are active
            if(Timed)
            {
                // increment time and change when ready
                m_curTime += Time.deltaTime;
                if (m_curTime >= m_waitTime)
                {
                    Self.SetTransition(m_timeoutID);
                    m_curTime = 0.0f;

                    m_timeoutID = Transition.None;
                }
            }
        }

        /// <summary>
        /// This method controls the behavior of the NPC in the game World.
        /// Every action, movement or communication the NPC does should be placed here
        /// NPC is a reference to the npc tha is controlled by this class
        /// </summary>
        public virtual void Act()
        {
            // release all keys?
        }

        #endregion

        #endregion

        #region PRIVATE UTILITIES

        /// <summary>
        /// Faces the ship at the target and
        /// fires when facing
        /// </summary>
        /// <returns>If the agent is within angle</returns>
        protected bool AimAtTarget(float angleAccuracy)
        {
            float angleDiff = Math.Angle(Self.transform, Self.Target.position);

            // Changed so that the doesnt move towards target 
            // change when applying types

            if (angleDiff >= angleAccuracy)
            {
                Con.ReleaseKey(Control_Config.GetKey("turnRight", "ship"));
                Keys.Add(Control_Config.GetKey("turnLeft", "ship"));
            }
            else if (angleDiff <= -angleAccuracy)
            {
                Con.ReleaseKey(Control_Config.GetKey("turnLeft", "ship"));
                Keys.Add(Control_Config.GetKey("turnRight", "ship"));
            }
            else
            {
                return true;
            }

            return false;
        }

        #endregion

    }
}
