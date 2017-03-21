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

    public static UIMessegeHandler GUI
    {
        get
        {
            // Add new spawned ship to 
            if (m_singleton.m_base.GUIMsg == null)
                m_singleton.m_base.GUIMsg = GameObject.Find("_gui").GetComponent<UIMessegeHandler>();

            return m_singleton.m_base.GUIMsg;
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
        else if (sceneName == "LobbyScene")
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
    }

    /// <summary>
    /// Called when player quits out of a game
    /// </summary>
    /// <param name="conn"></param>
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);

        GameMSG.RemovePlayer(conn);
    }

    public override void OnStopHost()
    {
        base.OnStopHost();

        GameObject.Destroy(m_base.GameMsg.gameObject);
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
    }

    /// <summary>
    /// Called on each client when the server changes
    /// the server and assigns the space manager when 
    /// entering the server scene
    /// </summary>
    /// <param name="conn"></param>
    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        base.OnClientSceneChanged(conn);

        ClientScene.AddPlayer(0);

        // listen for when server is assigned an ID
        m_singleton.client.RegisterHandler((short)MSGCHANNEL.NEWID, OnNewIDMessage);
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
    public static void CreateOnlineServer
        (ServerData newServer, CSteamID lobbyID)
    {
        // check if the network is already active
        if (m_singleton.isNetworkActive)
            return;

        m_singleton.m_base.Lobby = lobbyID;

        m_singleton.m_base.Player.IsHost = true;

        m_singleton.m_base.ServerInfo = newServer;

        m_singleton.onlineScene = "SpaceScene";

        // Startup the host
        m_singleton.StartHostAll(m_singleton.
            m_base.ServerInfo.ServerName, 2);
    }

    /// <summary>
    /// For now just creates a host on local host
    /// </summary>
    public static void CreateLANServer
        (string serverName)
    {
        // check if the network is already active
        if (m_singleton.isNetworkActive)
            return;

        // Build the Server Data we will use
        ServerData newServer = new ServerData();
        newServer.ServerIP = Network.player.ipAddress;
        newServer.ServerPort = 7777;
        newServer.ServerVersion = m_singleton.m_base.Version;
        newServer.ServerName = serverName;

        m_singleton.m_base.Player.IsHost = true;

        m_singleton.m_base.ServerInfo = newServer;

        // This sets the data part of the OnReceivedBroadcast() event 
        m_singleton.m_base.Discovery.broadcastData = newServer.ServerName;

        // Set the IP the Net Manager is going to use to host a game to OUR IP address and Port 7777
        m_singleton.networkAddress = newServer.ServerIP;
        m_singleton.networkPort = newServer.ServerPort;

        m_singleton.onlineScene = "ServerScene";

        // Startup the host
        m_singleton.TryHost();
    }

    #endregion

    #region JOIN SERVER

    public static void JoinOnlineClient
        (string externalIP, string internalIP,
        ulong guid, string matchID, CSteamID lobbyID)
    {
        m_singleton.onlineScene = "SpaceScene";

        m_singleton.StartClientAll
            (externalIP, internalIP, guid);

        m_singleton.m_base.Lobby = lobbyID;
    }

    public static void JoinLANClient(string serverAddress)
    {
        // Artificer uses port 7777
        m_singleton.networkAddress = serverAddress;
        m_singleton.networkPort = 7777;

        m_singleton.onlineScene = "ServerScene";

        m_singleton.TryClient();
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
        SteamMatchmaking.SetLobbyData(m_base.Lobby, "guid", NATHelper.singleton.guid.ToString());
        SteamMatchmaking.SetLobbyData(m_base.Lobby, "publicIP", m_singleton.externalIP);
        SteamMatchmaking.SetLobbyData(m_base.Lobby, "internalIP", m_singleton.hostInternalIP);

        // set game to running
        SteamMatchmaking.SetLobbyData(m_base.Lobby, "running", "true");
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
        m_singleton.StartHost();
    }

    // Tries to connect as a client
    // This method is called by the user clicking Join Game on a server info panel
    private void TryClient()
    {
        networkSceneName = "";
        NetworkServer.SetAllClientsNotReady();
        ClientScene.DestroyAllClientObjects();
        m_singleton.StartClient();
    }

    // Leaves the lobby or match we are connected to (host and client)
    // This method is called by the user clicking Leave Lobby or Game
    private void LeaveLobby()
    {
        networkSceneName = "";

        if(m_base.Discovery.running)
            m_base.Discovery.StopBroadcast();

        SystemManager.m_singleton.StopClient();
        SystemManager.m_singleton.StopHost();
    }

    #endregion

    #endregion
}
