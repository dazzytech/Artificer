﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Artificer
using Stations;
using Space.Teams;
using UnityEngine.Networking;

namespace Space.UI.Ship
{
    /// <summary>
    /// Resposible for adding station information
    /// to the HUD
    /// </summary>
    public class StationHUD : BasePanel
    {
        #region ATTRIBUTES

        // Stop duplicate HUD elements
        private List<uint> m_addedIDs = new List<uint>();

        #region HUD ELEMENTS

        [Header("HUD Elements")]

        [SerializeField]
        private Transform m_stationList;

        #endregion

        #region PREFABS

        [Header("HUD Prefabs")]

        [SerializeField]
        private GameObject m_stationPrefab;

        #endregion

        #region ACCESSORS

        private TeamController Team
        {
            get
            {
                if (SystemManager.Space != null)
                    return SystemManager.Space.Team;
                else
                    return null;
            }
        }

        #endregion

        #endregion

        #region MONO BEHAVIOUR

        // Use this for initialization
        void OnEnable()
        {
            if (Team != null)
            {
                // event listener here
                Team.EventStationListChanged += GenerateStationList;
                // regenerate team list incase of changes
                GenerateStationList();
            }
        }

        // Update is called once per frame
        void OnDisable()
        {
            if (Team != null)
            {
                // event listener here
                Team.EventPlayerListChanged -= GenerateStationList;
            }
        }

        void Awake()
        {
            StationPrefab.Base = this;
        }

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Remove ID from list so new ship can be added
        /// while previous ship HUD is being removed
        /// </summary>
        /// <param name="ID"></param>
        public void RemoveID(uint ID)
        {
            if (m_addedIDs.Contains(ID))
            {
                m_addedIDs.Remove(ID);
            }
        }

        #endregion

        #region INTERNAL FUNCTIONALITY

        private void GenerateStationList()
        {
            if (m_stationList == null)
            {
                Debug.Log("ERROR: Station HUD - GenerateStationList: " +
                    "Station List HUD has not been assigned.");
                return;
            }

            // Loop through each player and assign a friendly prefab
            foreach (uint ID in Team.Stations)
            {
                // Skip if local player
                if (ID == SystemManager.Space.NetID || m_addedIDs.Contains(ID))
                    continue;

                NetworkInstanceId netID = new NetworkInstanceId(ID);

                // Get ship object from network id
                GameObject friendlyObj = ClientScene.FindLocalObject(netID);

                if (friendlyObj == null)
                {
                    Debug.Log("ERROR: Station HUD - GenerateStationList: " +
                        "station not found in client scene.");
                }
                else
                {
                    AddUIPiece(friendlyObj.GetComponent<StationController>(), ID);
                }
            }
        }

        /// <summary>
        /// Adds station to the HUD display
        /// </summary>
        /// <param name="piece"></param>
        private void AddUIPiece(StationController controller, uint ID)
        {
            GameObject newStation = Instantiate(m_stationPrefab);
            newStation.transform.SetParent(m_stationList, false);

            StationPrefab tracker = newStation.GetComponent<StationPrefab>();
            tracker.DefineStation(controller, ID);

            m_addedIDs.Add(ID);
        }

        #endregion
    }
}