using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Timers;
using Data.Shared;
using Data.Space.Library;
using System.Collections.Generic;

namespace Space.UI.Spawn
{
    /// <summary>
    /// Controller for spawn selector
    /// player selects ship and spawn point 
    /// and spawn message is sent to server
    /// </summary>
    [RequireComponent(typeof(SpawnAttributes))]
    [RequireComponent(typeof(SpawnEventListener))]
    public class SpawnController : MonoBehaviour
    {
        #region ATTRIBUTES

        [SerializeField]
        private SpawnAttributes m_att;
        [SerializeField]
        private SpawnEventListener m_event;

        #endregion

        #region ACCESSORS

        public string SelectedName
        {
            get
            {
                return m_att.SelectedShip.Name;
            }
        }

        #endregion

        #region MONO BEHAVIOUR

        private void OnEnable()
        {
            // disable Spawn UI Elements
            m_att.SpawnButton.interactable = false;
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        private void Awake()
        {
            // This code should only be called once

            BuildSelection();
        }

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Called by event listener to start the spawn delay countdown
        /// </summary>
        public void EnableSpawn(int delay)
        {
            m_att.SpawnDelay = delay;

            StartCoroutine("DelaySpawn");
        }

        /// <summary>
        /// Sets the selected ship to that item
        /// for when the player spawns
        /// </summary>
        /// <param name="selected"></param>
        public void SelectShip(ShipSelectItem selected)
        {
            m_att.SelectedShip = selected;

            foreach (ShipSelectItem ship
                in m_att.ShipList)
                if (!m_att.SelectedShip.Equals(ship))
                    ship.Deselect();
                else
                    ship.Select();
        }

        #endregion

        #region PRIVATE UTILITIES

        /// <summary>
        /// Build list of spawnable ships
        /// </summary>
        private void BuildSelection()
        {
            // build a list of ships
            // we are able to select to spawn with
            foreach(string shipName in 
                m_att.SpawnableShips)
            {
                ShipData ship = ShipLibrary.GetShip(shipName);

                GameObject shipObj = 
                    Instantiate(m_att.ShipSelectPrefab);

                shipObj.transform
                    .SetParent(m_att.ShipSelectList);


                ShipSelectItem item =
                    shipObj.GetComponent<ShipSelectItem>();

                item.AssignShipData(ship);

                if(m_att.ShipList == null)
                {
                    m_att.ShipList = new List<ShipSelectItem>();

                    SelectShip(item);
                }

                m_att.ShipList.Add(item);
            }
        }

        #endregion

        #region COROUTINES

        /// <summary>
        /// Enable spawner after delay period
        /// and update timer
        /// </summary>
        /// <returns></returns>
        private IEnumerator DelaySpawn()
        {
            int seconds = 0;
            while(seconds <= m_att.SpawnDelay)
            {
                m_att.SpawnDelayText.text = string.Format
                    ("Ready to spawn in: {0:D2} sec.",
                    m_att.SpawnDelay - seconds);

                seconds++;

                yield return new WaitForSeconds(1f);
            }

            // Delay finished, enable spawn
            m_att.SpawnDelayText.text = "Ready to spawn.";

            if (!m_att.SpawnButton.interactable)
                m_att.SpawnButton.interactable = true;

            yield break;
        }

        #endregion
    }
}