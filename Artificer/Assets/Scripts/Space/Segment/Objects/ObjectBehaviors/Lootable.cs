using Data.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Space.Segment
{
    /// <summary>
    /// An object that the player can interact with
    /// to retrieve materials/components
    /// </summary>
    public class Lootable : SegmentObject
    {
        #region EVENTS

        public delegate void LootEvent(Lootable lootable);

        public static event LootEvent OnEnterRange;
        public static event LootEvent OnExitRange;

        public static event LootEvent OnLootCreated;

        #endregion

        #region ATTRIBUTES

        // Number of seconds until object dissapears
        protected float m_secondsTillRemove = 60f;

        // Amount of items that the player can retrieve
        protected Dictionary<int, float> m_yield;

        // distance the player needs to be in order to loot the ship
        protected float m_lootDistance;

        // Whether or not the player is in range
        private bool m_inRange;

        // Message that appears when player in range
        [HideInInspector]
        private PromptData m_lootPrompt;

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Display a prompt to dock
        /// </summary>
        public virtual void EnterRange()
        {
            if (SystemManager.UIPrompt != null)
            {
                if (m_lootPrompt == null)
                {
                    m_lootPrompt = new PromptData();
                    m_lootPrompt.LabelText = new string[1]
                        {"Press E to Loot Wreckage"};

                    SystemManager.UIPrompt.DisplayPrompt(m_lootPrompt);
                }
                else
                    SystemManager.UIPrompt.DisplayPrompt(m_lootPrompt.ID);
            }

            m_inRange = true;
        }

        /// <summary>
        /// Clear messages
        /// </summary>
        public virtual void ExitRange()
        {
            if (SystemManager.UIPrompt != null)
            {
                SystemManager.UIPrompt.DeletePrompt(m_lootPrompt.ID);
                m_lootPrompt = null;
            }

            m_inRange = false;
        }

        #endregion

        #region COROUTINE

        /// <summary>
        /// Catch all function that triggered provided events
        /// when in or out of range of ship
        /// </summary>
        /// <param name="InRange"></param>
        /// <param name="OutRange"></param>
        protected IEnumerator CheckRange()
        {
            // Endless loop
            while (true)
            {

                // Only proceed if the player is 
                // currently deployed
                GameObject playerObj =
                    GameObject.FindGameObjectWithTag("PlayerShip");

                // Ensure player is alive
                if (playerObj == null)
                {
                    yield return null;
                    continue;
                }

                // Find distance between station and player object
                float distance = Vector2.Distance
                    (transform.position, playerObj.transform.position);

                // determine range
                if (distance <= m_lootDistance)
                {
                    if (!m_inRange)
                    {
                        // Call the event
                        OnEnterRange(this);
                    }
                }
                else
                {
                    if (m_inRange)
                    {
                        // Call the event
                        OnExitRange(this);
                    }
                }

                yield return null;
            }
        }


        #endregion
    }
}
