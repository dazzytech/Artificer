using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Space.AI;
using Space.Ship;
using Data.UI;

namespace Space.AI
{
    /// <summary>
    /// The base class for user created scripts
    /// contains default attributes and event listeners
    /// interacted with by the CustomAgent
    /// </summary>
    public abstract class ICustomScript
    {
        #region ATTRIBUTES

        public List<KeyCode> KeysPressed;

        public List<KeyCode> KeysReleased;

        protected EntityObject Player;

        protected EntityObject Self;

        #endregion

        #region ACCESSOR

        #endregion

        /// <summary>
        /// Called by custom agent to 
        /// assign default parameters etc
        /// </summary>
        public void InitializeScript(Transform transform)
        {
            KeysPressed = new List<KeyCode>();
            KeysReleased = new List<KeyCode>();

            Player = new EntityObject()
            {
                Reference = GameObject.FindGameObjectWithTag("PlayerShip").transform,
                Alignment = Alignment.FRIENDLY
            };

            Self = new EntityObject()
            {
                Reference = transform,
                Alignment = Alignment.SELF
            };
        }

        public void PreLoop()
        {
            KeysPressed.Clear();
            KeysReleased.Clear();
        }

        /// <summary>
        /// Called by the CustomAgent
        /// </summary>
        public abstract void PerformLoop();

        #region SHIP EVENTS

        public abstract void EnterRange(EntityObject entity);

        /// <summary>
        /// Called by custom agent when the ship
        /// takes damage
        /// </summary>
        //public abstract void ComponentDamaged();

        #endregion

        #region PRIVATE UTILITIES
        
        /// <summary>
        /// returns the position of the transform component
        /// of an entity object
        /// </summary>
        public Vector2 GetPosition(EntityObject entityObject)
        {
            return entityObject.Reference.position;
        }

        /// <summary>
        /// Turns the ship to face a vector point, 
        /// returns true is angle is below certain amount
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        protected bool TurnTo(Vector2 point)
        {
            float difference = Math.Angle(Self.Reference,
                   point);

            if (difference > 5f)
            {
                KeysPressed.Add(Control_Config.GetKey("turnLeft", "ship"));
                KeysReleased.Add(Control_Config.GetKey("turnRight", "ship"));
            }
            else if (difference < -5f)
            {
                KeysPressed.Add(Control_Config.GetKey("turnRight", "ship"));
                KeysReleased.Add(Control_Config.GetKey("turnLeft", "ship"));
            }
            else
            {
                KeysReleased.Add(Control_Config.GetKey("turnLeft", "ship"));
                KeysReleased.Add(Control_Config.GetKey("turnRight", "ship"));

                return true;
            }


                return false;
        }

        #endregion
    }
}
