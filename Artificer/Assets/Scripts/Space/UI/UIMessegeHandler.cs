using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
// Artificer
using Data.Space;
using Space.UI.Ship;
using Space.UI.Tracker;
using Space.Segment;
using Space.Ship;
using Space.Teams;
using System.Collections.Generic;
using System;
using Space.Ship.Components.Listener;
using Space.UI.Station.Map;
using Space.UI.Station;
using Space.UI.Spawn;
using Space.UI.Teams;

namespace Space.UI
{
    /// <summary>
    /// Redirects incoming messages
    /// from non-UI elements as well was server messages
    /// </summary>
    public class UIMessegeHandler : NetworkBehaviour
    {
        #region ATTRIBUTES

        #region UI BEHAVIOURS

        [Header("UI Behaviours")]

        [SerializeField]
        private TeamSelectController m_team;

        [SerializeField]
        private SpawnController m_spawn;

        [SerializeField]
        private StationMessageHandler m_stationMsg;

        #region PLAYHUD BEHAVIOUR

        [Header("PLAYHUD Behaviours")]

        [SerializeField]
        private PlayHUD m_play;

        [SerializeField]
        private BuildHUD m_build;

        [SerializeField]
        private IndicatorHUD m_indicator;

        [SerializeField]
        private TrackerHUD m_tracker;

        #endregion

        #endregion

        #endregion  

        #region PLAYRECT UI MESSAGES

        /// <summary>
        /// Prompts the GUI to retreive ship
        /// attributes and update the HUD
        /// </summary>
        public void BuildShipData()
        {
            m_play.BuildShipData();
        }

        /// <summary>
        /// Adds a peice for the TrackerHUD to track
        /// a transform dependant on its tag
        /// </summary>
        /// <param name="piece"></param>
        public void AddUIPiece(Transform piece)
        {
            m_tracker.AddUIPiece(piece);
        }

        /// <summary>
        /// Creates a popup text displaying
        /// damage taken by collider
        /// </summary>
        /// <param name="postion"></param>
        /// <param name="amount"></param>
        public void DisplayIntegrityChange
            (Vector2 postion, float amount)
        {
            m_indicator.IndicateIntegrity(postion, amount);
        }

        /// <summary>
        /// displays construction
        /// heads up display
        /// </summary>
        /// <param name="deployFunction"></param>
        /// <param name="options"></param>
        public void DisplayBuildWheel(Deploy deployFunction, 
            List<string> options)
        {
            m_build.InitializeHUD(deployFunction, options);
        }

        #region PLAYRECT RPC

        [ClientRpc]
        public void RpcAddRemotePlayer(NetworkInstanceId instID)
        {
            // retreive our version of that network instance 
            GameObject otherPlayer = ClientScene.FindLocalObject(instID);

            // if this is our ship we dont want to add it to UI
            if (otherPlayer.GetComponent<NetworkIdentity>().isLocalPlayer)
                return;

            // Add to UI
            AddUIPiece(otherPlayer.transform);
        }

        #endregion

        #endregion

        #region TEAM PICKER MESSAGES

        /// <summary>
        /// Sets the team picker rect to show 
        /// which team objects can be chosen
        /// </summary>
        /// <param name="teamA"></param>
        /// <param name="teamB"></param>
        public void SetTeamOptions(int teamA, int teamB)
        {
            m_team.SetTeams(new int[2] { teamA, teamB });
        }

        #endregion

        #region SPAWN PICKER MESSAGES

        /// <summary>
        /// Set the HUD spawn button to enable after a certain period
        /// </summary>
        /// <param name="delay"></param>
        public void SetSpawnDelay(int delay)
        {
            m_spawn.EnableSpawn(delay);
        }

        #endregion

        #region STATION RECT MESSAGES

        /// <summary>
        /// prompts the station object to enable
        /// the warp map portion of the Station HUD
        /// </summary>
        public void InitializeWarpMap
            (List<uint> surroundingWarpGates,
            Transform homeGate)
        {
            m_stationMsg.InitializeWarpMap
                (surroundingWarpGates, homeGate);
        }

        /// <summary>
        /// Prompts the staton object to enable the ship
        /// viewer portion of the station HUD. 
        /// </summary>
        /// <param name="ship"></param>
        public void InitializeStationHUD(ShipAccessor ship)
        {
            m_stationMsg.InitializeShipViewer(ship);
        }

        #endregion
    }
}
