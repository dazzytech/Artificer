using Data.Space.Collectable;
using Data.UI;
using Space.Ship;
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

        public static event LootEvent OnLootCompleted;

        #endregion

        #region ATTRIBUTES

        /// <summary>
        /// Number of seconds until object dissapears
        /// </summary>
        [SerializeField]
        protected float m_secondsTillRemove = 60f;

        /// <summary>
        /// Amount of items that the player can retrieve
        /// </summary>
        protected Dictionary<int, float> m_yield;

        /// <summary>
        /// distance the player needs to be in order to loot the ship
        /// </summary>
        protected float m_lootDistance = 5;

        /// <summary>
        /// Whether or not the player is in range
        /// </summary>
        private bool m_inRange;

        /// <summary>
        /// If the player is looting the object
        /// </summary>
        private bool m_looting;

        /// <summary>
        /// Message that appears when player in range
        /// </summary>
        [HideInInspector]
        public PromptData LootPrompt;

        /// <summary>
        /// The accessor to the players ship
        /// </summary>
        [HideInInspector]
        private ShipAccessor m_ship;

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Display a prompt to dock
        /// </summary>
        public virtual void EnterRange()
        {
            if (m_yield.Count == 0)
                return;

            if (SystemManager.UIPrompt != null)
            {
                if (LootPrompt == null)
                {
                    LootPrompt = new PromptData();
                    LootPrompt.LabelText = new string[1]
                        {"Press V to Loot Wreckage", };

                    SystemManager.UIPrompt.DisplayPrompt(LootPrompt);
                }
                else
                    SystemManager.UIPrompt.DisplayPrompt(LootPrompt.ID);
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
                if (LootPrompt != null)
                {
                    SystemManager.UIPrompt.DeletePrompt(LootPrompt.ID);
                    LootPrompt = null;
                }
                StopCoroutine("DelayLoot");
                StopCoroutine("DelayEnd");
            }

            m_inRange = false;

            m_looting = false;

            m_ship = null;
        }

        /// <summary>
        /// Begin the process of 
        /// depositing materials and display
        /// </summary>
        /// <param name="ship"></param>
        public void Interact(ShipAccessor ship)
        {
            if (!m_looting)
            {
                m_ship = ship;

                // Change text and update
                LootPrompt.LabelText = new string[1]
                    { "Looting Ship, Please Wait" };

                m_looting = true;

                // Set a delay 
                StartCoroutine("DelayLoot", 10f);
            }
        }

        #endregion

        #region PRIVATE UTILITIES

        /// <summary>
        /// Finished the refining delay,
        /// add the refined elements to player 
        /// storage
        /// </summary>
        private void Loot()
        {
            if (m_ship != null)
            {
                m_yield = m_ship.InsertMaterial(m_yield);
            }

            m_looting = false;

            StartCoroutine("DelayEnd",
                "Ship Looted");
        }

        /// <summary>
        /// Called by the child object to start the range checking
        /// </summary>
        protected void Initialize()
        {
            StartCoroutine("CheckRange");

            Invoke("DelayDestroy", m_secondsTillRemove);
        }

        private void DelayDestroy()
        {
            Destroy(gameObject);
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
                    StopCoroutine("DelayEnd");
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

        /// <summary>
        /// Waits for delay period
        /// and then start material deposit
        /// </summary>
        /// <returns></returns>
        private IEnumerator DelayLoot(float delay)
        {
            float timer = 0;

            LootPrompt.SliderValues = new float[1];

            while (timer < delay)
            {
                yield return null;

                timer += Time.deltaTime;

                LootPrompt.SliderValues[0] = timer / delay;

                SystemManager.UIPrompt.UpdatePrompt(LootPrompt.ID);
            }

            Loot();

            yield break;
        }

        /// <summary>
        /// Displays ending message for two seconds then  
        /// resets prompt
        /// </summary>
        /// <param name="delay"></param>
        /// <returns></returns>
        private IEnumerator DelayEnd(string endMessage)
        {
            LootPrompt.SliderValues = null;

            LootPrompt.LabelText[0] = endMessage;

            SystemManager.UIPrompt.UpdatePrompt(LootPrompt.ID);

            yield return new WaitForSeconds(2f);

            LootPrompt.LabelText = null;

            SystemManager.UIPrompt.UpdatePrompt(LootPrompt.ID);

            if (OnLootCompleted != null)
                OnLootCompleted(this);
        }


        #endregion
    }
}
