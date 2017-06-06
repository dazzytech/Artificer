﻿using UnityEngine;
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
            m_transitionMap;

        protected FSMStateID m_stateID;

        [SerializeField]
        private FSM m_self;

        #endregion

        #region AI AGENT

        public List<KeyCode> Keys;

        #endregion

        #endregion

        #region ACCESSOR

        /// <summary>
        /// Returns state we are currently in
        /// </summary>
        public FSMStateID ID
        {
            get { return m_stateID; }
        }

        protected ShipInputReceiver Con
        {
            get { return m_self.Con; }
        }

        protected FSM Self
        {
            get { return m_self; }
        }

        #endregion

        #region PUBLIC INTERACTION

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

        #region VIRTUAL FUNCTIONALITY

        /// <summary>
        /// Decides if the state should transition to another on its list
        /// NPC is a reference to the npc tha is controlled by this class
        /// </summary>
        public virtual void Reason(List<Transform> objs, Transform npc) { }

        /// <summary>
        /// This method controls the behavior of the NPC in the game World.
        /// Every action, movement or communication the NPC does should be placed here
        /// NPC is a reference to the npc tha is controlled by this class
        /// </summary>
        public virtual void Act(List<Transform> objs, Transform npc) { }

        #endregion
    }
}
