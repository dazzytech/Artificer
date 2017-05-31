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
        #region DICTIONARY

        // Create dummy entry type
        [Serializable]
        public class Entry : GenericDictionaryItem<Transition, FSMStateID>
        {
        }

        [System.Serializable]
        public class StateDictionary: GenericDictionary<Transition, FSMStateID, Entry> { }

        #endregion

        #region ATTRIBUTES

        #region STATE MANAGEMENT

        /// <summary>
        /// Possible states it is capable of transitioning to
        /// </summary>
        [SerializeField]
        private StateDictionary
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
