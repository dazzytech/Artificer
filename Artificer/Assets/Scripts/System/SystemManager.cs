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
using Space.GameFunctions;

[RequireComponent(typeof(SystemAttributes))]

public class SystemManager: NetworkManager 
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
            if (m_singleton.m_base.GameMsg == null)
                m_singleton.m_base.GameMsg = GameObject.Find("game")
                    .GetComponent<GameMessageHandler>();
            return m_singleton.m_base.GameMsg;
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
                return null;   
            else
                return m_singleton.m_base.Space;
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

    #endregion

    #region NETWORKMANAGER OVERRIDE

    #region SERVER SIDE

    /// <summary>
    /// Initailze the player and add it to the 
    /// team spawner to allocate it a position
    /// for the teamspawner to later add the spawn
    /// </summary>
    /// <param name="conn"></param>
    /// <param name="playerControllerId"></param>
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        Debug.Log("Server Add Player");
        // error cause goes to lobby
        //GameMSG.AddNewPlayer
        //   (playerControllerId, conn);
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
            Space.InitializeSpaceParameters();
            GameMSG.InitializeGameParameters();
        }
        else if (sceneName == "LobbyScene")
        {
            // Use this override to initialize and
            // broadcast your game through NetworkDiscovery
            m_base.Discovery.Initialize();
            m_base.Discovery.StartAsServer();
        }
    }

    /// <summary>
    /// Called when player quits out of a game
    /// </summary>
    /// <param name="conn"></param>
    /*public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        GameMSG.RemovePlayer(conn);
    }*/

    /*
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
    }*/

    #endregion

    #region CLIENT SIDE

    /// <summary>
    /// Called on each client when the server changes
    /// the server and assigns the space manager when 
    /// entering the server scene
    /// </summary>
    /// <param name="conn"></param>
    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        base.OnClientSceneChanged(conn);

        // If we switched to space then assign our
        // space manager
        if (networkSceneName == "SpaceScene")
        {
            m_singleton.m_base.Space
                = GameObject.Find("space").GetComponent<SpaceManager>();

            if (m_singleton.m_base.Space == null)
                Debug.Log("Error: System Manager - Client Scene Changed: " +
                    "SpaceManager not found in space scene.");
        }
    }

    /*public override void OnStartClient(NetworkClient client)
    {
        Debug.Log("Start Client");
        base.OnStartClient(client);
    }

    public override void OnClientNotReady(NetworkConnection conn)
    {
        Debug.Log("Client Not Ready");
        base.OnClientNotReady(conn);
    }

    public override void OnClientError(NetworkConnection conn, int errorCode)
    {
        Debug.Log("Client Error");
        base.OnClientError(conn, errorCode);
    }*/

    #endregion

    #endregion

    #region MONO BEHAVIOUR

    void Start()
    {
        if (m_singleton == null)
            m_singleton = this;
        else
            Destroy(gameObject);

        m_preload.PreloadAssets();
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

    /// <summary>
    /// For now just creates a host on local host
    /// </summary>
    public static void CreateServer()
    {
        // check if the network is already active
        if (m_singleton.isNetworkActive)
            return;

        // Set the IP the Net Manager is going to use to host a game to OUR IP address and Port 7777
        m_singleton.networkAddress = Network.player.ipAddress;
        m_singleton.networkPort = 7777;

        // in future set text popup to enter name
        // If game name was set...
        //if (ServerName.text != "")
            //... get the provided name
            //_name = ServerName.text;
        //else
        //{
            // ELSE set a game name for our user
        string name = "Game:" + Random.Range(0, 10000);
        //ServerName.text = _name;
        //}
        // This sets the data part of the OnReceivedBroadcast() event 
        m_singleton.m_base.Discovery.broadcastData = name;

        // Startup the host
        m_singleton.TryHost();
    }

    public static void JoinAsClient(string serverAddress)
    {
        // Artificer uses port 7777

        NetworkManager.singleton.networkAddress = serverAddress;

        m_singleton.TryClient();
    }

    public static void Disconnect()
    {
        if (singleton.client != null)
            singleton.StopClient();
        else
            singleton.StopHost();
    }

    #endregion

    #region PRIVATE UTILITIES

    #region HOST/CLIENT CONTROLS

    // Tries to host a game
    // This method is called by the user clicking Host Game on the main menu
    private void TryHost()
    {
        networkSceneName = "";
        NetworkServer.SetAllClientsNotReady();
        ClientScene.DestroyAllClientObjects();
        SystemManager.m_singleton.StartHost();
    }

    // Tries to connect as a client
    // This method is called by the user clicking Join Game on a server info panel
    private void TryClient()
    {
        networkSceneName = "";
        NetworkServer.SetAllClientsNotReady();
        ClientScene.DestroyAllClientObjects();
        SystemManager.m_singleton.StartClient();
    }

    // Leaves the lobby we are connected to (host and client)
    // This method is called by the user clicking Leave Lobby from within the lobby slot panel
    private void LeaveLobby()
    {
        networkSceneName = "";
        m_base.Discovery.StopBroadcast();
        SystemManager.m_singleton.StopClient();
        SystemManager.m_singleton.StopHost();
    }

    // Leaves the game we are in (host and client)
    // This method is called by the user clicking Exit Game from within the in-game UI
    private void LeaveGame()
    {
        networkSceneName = "";
        SystemManager.m_singleton.StopClient();
        SystemManager.m_singleton.StopHost();
    }

    //http://molx.us/2016/03/28/1/   above from here

    #endregion

    #endregion
}
