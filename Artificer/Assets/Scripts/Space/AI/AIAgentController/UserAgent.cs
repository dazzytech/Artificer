using Data.UI;
using Space.Ship;
using Space.Ship.Components.Listener;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Space.AI.Agent
{
    /// <summary>
    /// Agent behaviour that
    /// hosts the user implemented programming.
    /// </summary>
    public class UserAgent : FSM
    {
        #region ATTRIBUTES

        /// <summary>
        /// An interface for the user made script
        /// </summary>
        private ICustomScript m_userScript;

        /// <summary>
        /// reference to the controller module
        /// </summary>
        private ControlListener m_control;

        private float m_scanRadius = 5;

        private List<EntityObject> m_nearbyList = new List<EntityObject>();

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Assigns the player script and the control
        /// listener and initialize the agent
        /// </summary>
        /// <param name="userScript"></param>
        /// <param name="listener"></param>
        public void SetNPC(ICustomScript userScript, ControlListener listener)
        {
            m_userScript = userScript;

            m_control = listener;

            m_userScript.InitializeScript(transform);
        }

        #endregion

        #region FSM

        protected override void Initialize()
        {
            base.Initialize();

            InvokeRepeating("Scan", 0, 1f);
        }

        protected override void FSMUpdate()
        {
            // Perform update function
            if (m_control == null)
                return;

            m_userScript.PreLoop();

            m_userScript.PerformLoop();

            if (m_userScript.KeysPressed.Count > 0)
                Con.ReceiveKey(m_userScript.KeysPressed);
        }

        protected override void FSMLateUpdate()
        {
            foreach(KeyCode key in m_userScript.KeysReleased)
                Con.ReleaseKey(key);
        }

        #endregion

        /// <summary>
        /// Add any ships that are within range or remove any out 
        /// of range and trigger event
        /// </summary>
        private void Scan()
        {
            // loop through each ship and detect if stored
            foreach(ShipAccessor ship in m_ships)
            {
                EntityObject inRange = null;

                foreach(EntityObject entity in m_nearbyList)
                {
                    if (ship.transform.Equals(entity.Reference))
                        inRange = entity;
                }

                if(Vector3.Distance(transform.position, ship.transform.position) <= m_scanRadius)
                {
                    if(inRange == null)
                    {
                        // add to nearby list and trigger event
                        EntityObject newEntity = new EntityObject()
                        {
                            Reference = ship.transform,
                            Alignment = ship.transform.Equals(transform)? Alignment.SELF: ship.TeamID == SystemManager.Space.TeamID?
                                Alignment.FRIENDLY : Alignment.ENEMY
                        };

                        m_nearbyList.Add(newEntity);
                        m_userScript.EnterRange(newEntity);
                    }
                }
                else
                {
                    if(inRange != null)
                    {
                        // remove from range list
                        m_nearbyList.Remove(inRange);
                    }
                }
            }
        }
    }
}
