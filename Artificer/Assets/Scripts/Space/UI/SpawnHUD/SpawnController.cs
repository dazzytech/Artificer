using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using System.Timers;
using Data.Space;
using Data.Space.Library;
using System.Collections.Generic;
using Space.Map;
using Stations;

using UI;

namespace Space.UI.Spawn
{
    /// <summary>
    /// Controller for spawn selector
    /// player selects ship and spawn point 
    /// and spawn message is sent to server
    /// </summary>
    [RequireComponent(typeof(SpawnAttributes))]
    [RequireComponent(typeof(SpawnEventListener))]
    public class SpawnController : HUDPanel
    {
        #region ATTRIBUTES

        [SerializeField]
        private SpawnAttributes m_att;
        [SerializeField]
        private SpawnEventListener m_event;

        #endregion

        #region ACCESSORS

        public ShipData SelectedShip
        {
            get
            {
                return m_att.SelectedShip.Ship;
            }
        }

        public int SelectedShipID
        {
            get
            {
                return m_att.SelectedShip.ID;
            }
        }

        public int SelectedSpawn
        {
            get { return m_att.SelectedSpawn.SpawnID; }
        }

        #endregion

        #region MONO BEHAVIOUR

        protected override void OnEnable()
        {
            base.OnEnable();

            if (m_att.SpawnButtonCurrency.interactable)
                m_att.SpawnButtonCurrency.interactable = false;

            if (SystemManager.PlayerShips == null)
                return; 

            BuildSelection();
            BuildStations();

            StartCoroutine("DetectSpawnReady"); 
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            while (m_att.ShipList.Count > 0)
            {
                Destroy(m_att.ShipList[0].gameObject);
                m_att.ShipList.RemoveAt(0);
            }
        }

        private void Awake()
        {
            // This code should only be called once
            m_att.Map.InitializeMap(new MapObjectType[] { MapObjectType.SHIP });

            if (SystemManager.Space.TeamID == 1)
                m_att.Map.RotateMap(new Vector2(0, -1));
        }

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Sets the selected ship to that item
        /// for when the player spawns
        /// </summary>
        /// <param name="selected"></param>
        public void SelectShip(ShipUIPrefab selected)
        {
            m_att.SelectedShip = selected;

            foreach (ShipUIPrefab ship
                in m_att.ShipList)
                if (!m_att.SelectedShip.Equals(ship))
                    ship.Deselect();
                else
                    ship.Select();

            // Invoke the function to display 
            // the cost to build the ship
            DisplayCost();

            // Update the spawn setting based on any change
        }

        public void SelectSpawn(SpawnSelectItem selected)
        {
            m_att.SelectedSpawn = selected;

            foreach(SpawnSelectItem spawn in m_att.SpawnList)
                if (!m_att.SelectedSpawn.Equals(spawn))
                    spawn.Deselect();
                else
                    spawn.Select();
        }

        #endregion

        #region PRIVATE UTILITIES

        /// <summary>
        /// Build list of spawnable ships
        /// </summary>
        private void BuildSelection()
        {
            int shipIndex = 0;
            // build a list of ships
            // we are able to select to spawn with
            foreach(ShipSpawnData spawn in SystemManager.PlayerShips)
            {
                // Only spawn ships we own
                if (!spawn.Owned)
                {
                    shipIndex++;
                    continue;
                }
                

                GameObject shipObj = 
                    Instantiate(m_att.ShipSelectPrefab);

                shipObj.transform
                    .SetParent(m_att.ShipSelectList);

                ShipUIPrefab item =
                    shipObj.GetComponent<ShipUIPrefab>();

                item.Initialize(m_event.ShipSelected);

                item.AssignShip(shipIndex++);

                if (m_att.ShipList == null)
                {
                    m_att.ShipList = new List<ShipUIPrefab>();
                }

                m_att.ShipList.Add(item);
            }

            SelectShip(m_att.ShipList[0]);
        }

        /// <summary>
        /// Creates spawn prefabs for all stations with 
        /// spawn capability and sends them to the map
        /// </summary>
        private void BuildStations()
        {
            // create a list to store these in future
            foreach (uint netId in SystemManager.Space.Team.Stations)
            {
                GameObject stationObj = ClientScene.FindLocalObject
                    (new NetworkInstanceId(netId));

                int SpawnID = stationObj.GetComponent
                    <StationAccessor>().SpawnID;

                if (SpawnID != -1)
                {
                    // Station is spawnable, create prefab instance
                    GameObject spawnObj = Instantiate(m_att.StationSelectPrefab);

                    // Initialize the selectable item
                    SpawnSelectItem spawnItem = 
                        spawnObj.GetComponent<SpawnSelectItem>();
                    spawnItem.Initialize(m_event.SpawnSelected);
                    spawnItem.SpawnID = SpawnID;

                    // Add new prefab to list
                    if (m_att.SpawnList == null)
                        m_att.SpawnList = new List<SpawnSelectItem>();

                    m_att.SpawnList.Add(spawnItem);

                    // Update map
                    MapObject mObj = SystemManager.Space.GetMapObject
                        (stationObj.transform);

                    if(mObj != null)
                        m_att.Map.DeployPrefab(mObj, spawnObj);
                }
            }

            SelectSpawn(m_att.SpawnList[0]);
        }

        /// <summary>
        /// when a ship is selected. display the cost to build 
        /// the ship to the viewer panel
        /// </summary>
        private void DisplayCost()
        {
            m_att.CurrentShipCost.text = string.Format
                ("Deploy Ship\nCost: ¤ {0}",  m_att.SelectedShip.Cost);
        }

        /// <summary>
        /// Displays how much cash 
        /// player has
        /// </summary>
        private void DisplayWallet()
        {
            m_att.PlayerCurrency.text = string.Format
                ("Player Wallet: ¤ {0}", SystemManager.Player.Wallet.Currency);
        }

        #endregion 

        #region COROUTINE

        /// <summary>
        /// Runs in the background
        /// updates the player currency and spawn state
        /// </summary>
        /// <returns></returns>
        private IEnumerator DetectSpawnReady()
        {
            while(true)
            {
                DisplayWallet();

                if (m_att.SelectedShip != null)
                {
                    if (SystemManager.Player.Wallet.Currency
                        >= m_att.SelectedShip.Cost &&
                        m_att.SelectedShip.Progress >= 1f)
                    {
                        if (!m_att.SpawnButtonCurrency.interactable)
                            m_att.SpawnButtonCurrency.interactable = true;
                    }
                    else
                        if (m_att.SpawnButtonCurrency.interactable)
                            m_att.SpawnButtonCurrency.interactable = false;
                }
                else
                {
                    if (m_att.SpawnButtonCurrency.interactable)
                        m_att.SpawnButtonCurrency.interactable = false;
                }

                yield return null;
            }
        }

        #endregion
    }
}