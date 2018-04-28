using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
// Artificer
using Data.Space;
using Space.UI.Ship;
using Space.Segment;
using Space.Ship;
using Space.Teams;
using System.Collections.Generic;
using System;
using Space.Ship.Components.Listener;
using Space.UI.Station;
using Space.UI.Spawn;
using Space.UI.Teams;
using Space.UI.Proxmity;
using Data.UI;
using Space.UI.Prompt;

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
        private StationMessageHandler m_stationMsg;

        #region PLAYHUD BEHAVIOUR

        [Header("PLAYHUD Behaviours")]

        [SerializeField]
        private ShipHUD m_ship;

        [SerializeField]
        private ProximityHUD m_proximity;

        [SerializeField]
        private BuildHUD m_build;

        [SerializeField]
        private IndicatorHUD m_indicator;

        #endregion

        #endregion

        #endregion  

        #region PLAYRECT UI MESSAGES

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
            List<DeployData> options)
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

        #region STATION RECT MESSAGES

        /// <summary>
        /// Prompts the staton object to enable the ship
        /// viewer portion of the station HUD. 
        /// </summary>
        /// <param name="ship"></param>
        public void InitializeStation(ShipAccessor ship)
        {
            m_stationMsg.InitializeShipViewer(ship);
        }

        public void InitializeTradeHub(TeamController team)
        {
            m_stationMsg.InitializeTradingHub(team);
        }

        #endregion
    }
}
