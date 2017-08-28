using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
// Artificer
using Data.Space;
using Data.Space.Library;
using Space;
using Space.CameraUtils;
using Space.UI;
using Game;
using Server;
using Steamworks;

/// <summary>
/// Stores game parameters retrived from lobby or server
/// or created
/// </summary>
public struct GameParameters
{
    /// <summary>
    /// list of ships that can
    /// be spawned by teams immidiatly
    /// </summary>
    public ShipSpawnData[] SpawnableShips;

    /// <summary>
    /// Start inventory for player teams
    /// </summary>
    public WalletData Wallet;

    public Vector2 TeamASpawn;

    public Vector2 TeamBSpawn;
}

// STORES ALL INFORMATION OF THE GAME
// THAT DOES NOT EXIST WITHIN SCENES
// NOT INCL CINFIGURATION
public class SystemAttributes:MonoBehaviour
{
    // Game state management
    public bool LevelPaused;
    public string Version;

    // Libraries
    public ShipLibrary PrebuiltShips;
    public ItemLibrary ItemLibrary;
    public FactionLibrary FactionLibrary;

    // Server Objects
    [HideInInspector]
    public GameMessageHandler GameMsg;
    [HideInInspector]
    public GameServerEvents Events;

    // Static client accessors
    // ui
    [HideInInspector]
    public UIMessegeHandler UI;
    [HideInInspector]
    public UIStateController UIState;
    [HideInInspector]
    public UIMessageController UIMsg;

    [HideInInspector]
    public SpaceManager Space;
    public ServerManager Server;
    [HideInInspector]
    public SystemNetworkDiscovery Discovery;
    public CameraMessageHandler CamMsgHandler;

    // Player Information
    public NetworkConnection Conn;

    // UI Data information
    public Data.UI.PlayerData Player;
    public Data.UI.ServerData ServerInfo;

    // Reference to which Steam Lobby we are connected to
    public CSteamID Lobby;
    public int minimumPlayers;

    [Header ("Game Parameters")]

    /// <summary>
    /// data object passed to the match
    /// in the server to define the match
    /// variables
    /// </summary>
    public GameParameters Param;

    /// <summary>
    /// Size of the space segment we
    /// are playing in
    /// </summary>
    public Rect SegmentSize = new Rect(0, 0, 5000, 5000);

    /// <summary>
    /// Starting components that can be built 
    /// from the start
    /// </summary>
    public List<GameObject> StarterList;

    /// <summary>
    /// Ships that are in the teams inventory immediately
    /// </summary>
    public ShipSpawnData[] StarterShips;

    /// <summary>
    /// How much assets the team starts with
    /// </summary>
    public WalletData StarterAssets;

    public Vector2 TeamASpawn = new Vector2(1500, 1500);

    public Vector2 TeamBSpawn = new Vector2(3500, 3500);
}

