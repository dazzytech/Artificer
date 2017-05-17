using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
// Artificer
using Data.Shared;
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

    // CLIENT INFO
    public NetworkClient mClient;
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
    public ElementLibrary ElementLibrary;
    public FactionLibrary FactionLibrary;

    // Server Objects
    public GameMessageHandler GameMsg;

    // Static client accessors
    // ui
    public UIMessegeHandler UI;
    public UIStateController UIState;
    public UIMessageController UIMsg;
    public UIInputController UIInput;

    public SpaceManager Space;
    public ServerManager Server;
    public SystemNetworkDiscovery Discovery;
    public CameraMessageHandler CamMsgHandler;

    // UI Data information
    public Data.UI.PlayerData Player;
    public Data.UI.ServerData ServerInfo;

    // Reference to which Steam Lobby we are connected to
    public CSteamID Lobby;
}

