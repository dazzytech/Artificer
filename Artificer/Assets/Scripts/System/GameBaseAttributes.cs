using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
// Artificer
using Data.Shared;
using Data.Space;
using Data.Space.Library;
using Space;
using Space.UI;
using Space.GameFunctions;

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
public class GameBaseAttributes:MonoBehaviour
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
    public UIMessegeHandler GUIMsg;
    public SpaceManager Space;
}

