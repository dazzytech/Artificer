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
    // nothing in here
    public ShipSpawnData[] SpawnableShips;
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
    public AITemplateLibrary AILibrary;

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

    // Builder
    public List<GameObject> StarterList;

    public ShipSpawnData[] StarterShips;

    public GameParameters Param;

    // Player Information
    public NetworkConnection Conn;

    // UI Data information
    public Data.UI.PlayerData Player;
    public Data.UI.ServerData ServerInfo;

    // Reference to which Steam Lobby we are connected to
    public CSteamID Lobby;
    public int minimumPlayers;
}

