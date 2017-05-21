using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.IO;
// Artificer
using Data.Shared;
using Data.Space;
using Space;
using Space.CameraUtils;
using Space.UI;
using Game;
using Server;
using Steamworks;
using Networking;
using UnityEngine.Networking.NetworkSystem;
using Data.UI;
using UnityEngine.SceneManagement;
using System.Net;
using System.Net.Sockets;
using UnityEngine.Networking.Match;
using NATTraversal;
using UnityEngine.Networking.Types;

[RequireComponent(typeof(SystemAttributes))]

public class SystemManager : NATTraversal.NetworkManager
{
    #region ATTRIBUTES

    [Header("References")]

    [SerializeField]
    private SystemAttributes m_base;
    [SerializeField]
    private AssetPreloader m_preload;

    private static SystemManager m_singleton;

    #endregion

    #region ACCESSORS

    public static string Version
    {
        get { return m_singleton.m_base.Version; }
    }

    public static GameMessageHandler GameMSG
    {
        get
        {
            // Assign the playerspawn to the scene object if doesnt exist
            if (m_singleton.m_base != null)
                return m_singleton.m_base.GameMsg;
            else
                return null;
        }

        set
        {
            m_singleton.m_base.GameMsg = value;
        }
    }

    public static UIMessegeHandler UI
    {
        get
        {
            // Add new spawned ship to 
            if (m_singleton.m_base.UI == null)
                m_singleton.m_base.UI = GameObject.Find("_gui").GetComponent<UIMessegeHandler>();

            return m_singleton.m_base.UI;
        }
    }

    public static UIStateController UIState
    {
        get
        {
            // Add new spawned ship to 
            if (m_singleton.m_base.UIState == null)
                m_singleton.m_base.UIState = GameObject.Find("_gui").GetComponent<UIStateController>();

            return m_singleton.m_base.UIState;
        }
    }

    public static UIMessageController UIMsg
    {
        get
        {
            // Add new spawned ship to 
            if (m_singleton.m_base.UIMsg == null)
                m_singleton.m_base.UIMsg = GameObject.Find("_gui").GetComponent<UIMessageController>();

            return m_singleton.m_base.UIMsg;
        }
    }

    public static UIInputController UIInput
    {
        get
        {
            // Add new spawned ship to 
            if (m_singleton.m_base.UIInput == null)
                m_singleton.m_base.UIInput = GameObject.Find("_gui").GetComponent<UIInputController>();

            return m_singleton.m_base.UIInput;
        }
    }

    /// <summary>
    /// Custom network discovery attached
    /// to this object
    /// </summary>
    public static SystemNetworkDiscovery Discovery
    {
        get
        {
            if (m_singleton == null)
                return null;

            return m_singleton.m_base.Discovery;
        }
    }

    /// <summary>
    /// Returns the space manager when in
    /// the space scene
    /// </summary>
    public static SpaceManager Space
    {
        get
        {
            if (m_singleton == null)
                return null;
            else if (m_singleton.m_base.Space == null)
                if (GameObject.Find("space") == null)
                    return null;
                else
                    return (m_singleton.m_base.Space = 
                            GameObject.Find("space")
                            .GetComponent<SpaceManager>());
            else
                return m_singleton.m_base.Space;
        }
    }

    /// <summary>
    /// Returns the lobby manager
    /// when in the lobby scene
    /// </summary>
    public static ServerManager Server
    {
        get
        {
            if (m_singleton == null)
                return null;
            else if (m_singleton.m_base.Server == null)
                if (GameObject.Find("ServerViewer") == null)
                    return null;
                else
                    return (m_singleton.m_base.Server =
                            GameObject.Find("ServerViewer")
                            .GetComponent<ServerManager>());
            else
                return m_singleton.m_base.Server;
        }
    }

    public static CameraMessageHandler Background
    {
        get
        {
            if (m_singleton.m_base.CamMsgHandler == null)
                m_singleton.m_base.CamMsgHandler = GameObject.Find("PlayerCamera").GetComponent<CameraMessageHandler>();

            return m_singleton.m_base.CamMsgHandler;
        }
    }

    public static Data.UI.PlayerData Player
    {
        get
        {
            return m_singleton.m_base.Player;
        }
    }

    #endregion

    #region NETWORKMANAGER OVERRIDE

    #region SERVER SIDE

    /// <summary>
    /// When the server is successfully
    /// made then proceed to update steam lobby
    /// with connection info
    /// </summary>
    public override void OnStartServer()
    {
        base.OnStartServer();

        UpdateLobbyData();
    }

    /// <summary>
    /// Initailze the player and add it to the 
    /// team spawner to allocate it a position
    /// for the teamspawner to later add the spawn
    /// </summary>
    /// <param name="conn"></param>
    /// <param name="playerControllerId"></param>
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        GameMSG.AddNewPlayer
           (playerControllerId, conn);
    }

    /// <summary>
    /// Initialize the game parameters if server 
    /// scene changes to space
    /// If Lobby, initialized the discovery tool 
    /// and start our broadcast
    /// </summary>
    /// <param name="sceneName"></param>
    public override void OnServerSceneChanged(string sceneName)
    {
        // Initialize space scene
        if (sceneName == "SpaceScene")
        {
            // switch to space scene
            GameMSG.SceneChanged("play");
        }
        else if (sceneName == "ServerScene")
        {
            // Use this override to initialize and
            // broadcast your game through NetworkDiscovery
            m_base.Discovery.Initialize();
            m_base.Discovery.StartAsServer();

            // Initialize the Game Manager within
            // the lobby
            GameMSG.Initialize();
            GameMSG.SceneChanged("lobby");

            m_base.Server = GameObject.Find("ServerViewer")
                .GetComponent<ServerManager>();

            // Change to server data?
            Server.InitializeLobby(m_base.ServerInfo);
        }
        base.OnServerSceneChanged(sceneName);
    }

    /// <summary>
    /// Called when player quits out of a game
    /// </summary>
    /// <param name="conn"></param>
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        GameMSG.RemovePlayer(conn);

        base.OnServerDisconnect(conn);
    }

    public override void OnStopHost()
    {
        base.OnStopHost();

        GameObject.Destroy(m_base.GameMsg.gameObject);

        if (m_base.Lobby != CSteamID.Nil)
        {
            SteamMatchmaking.LeaveLobby(m_base.Lobby);
            m_base.Lobby = CSteamID.Nil;
        }
    }

    /// <summary>
    /// Called when the server is unsuccessful in creation
    /// displays a popup alerting the user
    /// </summary>
    /// <param name="conn"></param>
    /// <param name="errorCode"></param>
    public override void OnServerError(NetworkConnection conn, int errorCode)
    {
        Debug.Log(errorCode.ToString());

        base.OnServerError(conn, errorCode);
    }

    #endregion

    #region CLIENT SIDE

    /*
    /// <summary>
    /// When client connection is unsuccessful then display
    /// a popup and leave steam lobby
    /// </summary>
    /// <param name="conn"></param>
    /// <param name="errorCode"></param>
    public override void OnClientError(NetworkConnection conn, int errorCode)
    {
        Debug.Log("Client Error");

        base.OnClientError(conn, errorCode);
    }*/

    /// <summary>
    /// Called on each client when the server changes
    /// the server and assigns the space manager when 
    /// entering the server scene
    /// </summary>
    /// <param name="conn"></param>
    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        base.OnClientSceneChanged(conn);

        m_singleton.client.RegisterHandler((short)MSGCHANNEL.NEWID, OnNewIDMessage);

        // initialize differently between space and lobby
        if(m_base.Space != null)
            m_base.Space.InitializeSpaceParameters();
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        if (m_base.Lobby != CSteamID.Nil)
        {
            SteamMatchmaking.LeaveLobby(m_base.Lobby);
            m_base.Lobby = CSteamID.Nil;
        }

        base.OnClientDisconnect(conn);
    }

    #endregion

    #endregion

    #region MONO BEHAVIOUR

    private void OnEnable()
    {
        // Develop our player data
        m_base.Player = new Data.UI.PlayerData();

        string name;

        if (SteamManager.Initialized)
        {
            // Base our player information on Steam Account

            // retreive steam name
            name = SteamFriends.GetPersonaName();
        }
        else
        {
            // Not connected to Steam
            name = "Non-Steam Player";
        }

        m_base.Player.PlayerName = name;
    }

    public override void Start()
    {
        if (m_singleton == null)
            m_singleton = this;
        else
            Destroy(gameObject);

        m_preload.PreloadAssets();

        base.Start();
    }

    #endregion

    #region PUBLIC INTERACTION

    #region NETWORK DISCOVERY

    /// <summary>
    /// Manually starting the discovery tool to listen for a broadcast
    /// called when server list starts
    /// </summary>
    public static void StartListening()
    {
        // Check we are initialized
        if (m_singleton == null)
            return;

        if (!m_singleton.m_base.Discovery.running)
        {
            // Use this method to start listening for a local game
            m_singleton.m_base.Discovery.Initialize();
            m_singleton.m_base.Discovery.StartAsClient();
        }
    }

    /// <summary>
    /// Stops the discovery component from listening for a broadcast
    /// called when server list closes
    /// </summary>
    public static void StopListening()
    {
        // Check we are initialized
        if (m_singleton == null)
            return;

        if (m_singleton.m_base.Discovery.running)
        {

            // Use this method to stop listening for a local game
            m_singleton.m_base.Discovery.StopBroadcast();
        }
    }

    #endregion

    #region CREATE SERVER

    /// <summary>
    /// Called from steam match maker
    /// with pre created Server data
    /// create reference to steam lobby
    /// </summary>
    /// <param name="newServer"></param>
    public static void CreateServer
        (string serverName, CSteamID lobbyID)
    {
        // check if the network is already active
        if (m_singleton.isNetworkActive)
            return;

        m_singleton.m_base.Player.IsHost = true;

        // Build the Server Data we will use
        ServerData newServer = new ServerData();
        newServer.ServerVersion = m_singleton.m_base.Version;
        newServer.ServerName = serverName;

        m_singleton.m_base.ServerInfo = newServer;

        m_singleton.m_base.Lobby = lobbyID;
        if (lobbyID != CSteamID.Nil)
        {
            m_singleton.onlineScene = "SpaceScene";
        }
        else
            m_singleton.onlineScene = "ServerScene";

        m_singleton.TryHost();
    }

    #endregion

    #region JOIN SERVER

    public static void JoinClient
        (string externalIP, string internalIP,
        ulong guid, CSteamID lobbyID)
    {
        m_singleton.onlineScene = "SpaceScene";

        m_singleton.m_base.Lobby = lobbyID;

        m_singleton.TryClient
            (externalIP, internalIP, guid);
    }

    public static void JoinClient(ServerData serverData)
    {
        m_singleton.m_base.ServerInfo = serverData;

        m_singleton.onlineScene = "ServerScene";

        m_singleton.TryClient
            (serverData.PublicIP, serverData.InternalIP,
                serverData.GUID);
    }

    #endregion

    /// <summary>
    /// For now only leave the lobby we have
    /// </summary>
    public static void Disconnect()
    {
        m_singleton.LeaveLobby();

        // Clear Server Data for us
        m_singleton.m_base.ServerInfo = null;
    }

    /// <summary>
    /// Switches from Lobby scene to player scene
    /// but still broadcasts for client connections
    /// </summary>
    public static void StartMatch()
    {
        m_singleton.ServerChangeScene("SpaceScene");
    }

    #region SERVER MESSAGES

    /// <summary>
    /// Stores the ID assigned from the game controller
    /// </summary>
    /// <param name="netMsg"></param>
    public void OnNewIDMessage(NetworkMessage netMsg)
    {
        // Retreive variables and display options
        IntegerMessage im = netMsg.ReadMessage<IntegerMessage>();

        // Store our id on the server
        m_base.Player.PlayerID = im.value;
    }

    #endregion

    #endregion

    #region PRIVATE UTILITIES

    #region LOBBY CONTROLS

    /// <summary>
    /// places information in the steam
    /// lobby information. sets lobby to running
    /// </summary>
    private void UpdateLobbyData()
    {
        m_base.ServerInfo.GUID = NATHelper.singleton.guid;
        m_base.ServerInfo.PublicIP = m_singleton.externalIP;
        m_base.ServerInfo.InternalIP = m_singleton.hostInternalIP;

        if (m_base.Lobby != CSteamID.Nil)
        {
            SteamMatchmaking.SetLobbyData(m_base.Lobby, "guid", m_base.ServerInfo.GUID.ToString());
            SteamMatchmaking.SetLobbyData(m_base.Lobby, "publicIP", m_base.ServerInfo.PublicIP);
            SteamMatchmaking.SetLobbyData(m_base.Lobby, "privateIP", m_base.ServerInfo.InternalIP);
        }
        else
        {
            m_singleton.m_base.Discovery.broadcastData =
                string.Format("{0}/{1}/{2}/{3}",
                new string[] {m_base.ServerInfo.ServerName,
                    m_base.ServerInfo.PublicIP, m_base.ServerInfo.InternalIP,
                    m_base.ServerInfo.GUID.ToString() });
        }
    }

    #endregion

    #region HOST/CLIENT CONTROLS

    // Tries to host a game
    // This method is called by the user clicking Host Game on the main menu
    private void TryHost()
    {
        networkSceneName = "";
        NetworkServer.SetAllClientsNotReady();
        ClientScene.DestroyAllClientObjects();
        m_singleton.StartHostAll(m_singleton.
            m_base.ServerInfo.ServerName, 2);
    }

    // Tries to connect as a client
    // This method is called by the user clicking Join Game on a server info panel
    private void TryClient(string externalIP, string internalIP,
        ulong guid)
    {
        networkSceneName = "";
        NetworkServer.SetAllClientsNotReady();
        ClientScene.DestroyAllClientObjects();

        m_singleton.StartClientAll
            (externalIP, internalIP,
                guid);
    }

    // Leaves the lobby or match we are connected to (host and client)
    // This method is called by the user clicking Leave Lobby or Game
    private void LeaveLobby()
    {
        networkSceneName = "";

        if(m_base.Discovery.running)
            m_base.Discovery.StopBroadcast();

        m_singleton.StopHost();
    }

    #endregion

    #endregion
}
