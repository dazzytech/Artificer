﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.IO;
// Artificer
using Data.Shared;
using Data.Space;
using Space.SpawnManagers;
using Space.UI;
using Space.GameFunctions;
using Data.Space.DataImporter;
using Data.Space.Library;

[RequireComponent(typeof(GameBaseAttributes))]

public class GameManager: NetworkManager 
{

    #region ATTRIBUTES

	private static
		GameBaseAttributes _base;
    private static
        GameAssetPreloader _preload;

    #endregion

    #region MONOGAME

    void Start()
    {
        _base = GetComponent<GameBaseAttributes>();
        _preload = GetComponent<GameAssetPreloader>();

        _preload.PreloadAssets();
    }

    void OnApplicationQuit() 
    {
        // We have nothing to save
        //Save();
    }

    #endregion

    #region PUBLIC INTERACTION

    /// <summary>
    /// For now just creates a host on local host
    /// </summary>
    public static void CreateHostedGame()
    {
        // Artificer uses port 7777
        NetworkManager.singleton.networkPort = 7777;

        NetworkManager.singleton.networkAddress = "localhost";

        NetworkManager.singleton.StartHost(); 
    }

    public static void JoinAsClient(string serverAddress)
    {
        // Artificer uses port 7777
        NetworkManager.singleton.networkPort = 7777;

        //NetworkManager.singleton.networkAddress = serverAddress;
        NetworkManager.singleton.networkAddress = "localhost";

        NetworkManager.singleton.StartClient();
    }

    public static void Disconnect()
    {
        if (singleton.client != null)
            singleton.StopClient();
        else
            singleton.StopHost();
    }

    public static void SpawnPlayerShip()
    {

    }

    #endregion

    #region GAME MANAGER INTERNAL INTERACTION

    #endregion

    #region NETMANAGER OVERRIDE

    // SERVER SIDE

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

    /// <summary>
    /// Initailze the player and add it to the 
    /// team spawner to allocate it a position
    /// for the teamspawner to later add the spawn
    /// </summary>
    /// <param name="conn"></param>
    /// <param name="playerControllerId"></param>
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        Debug.Log("Adding Player");

        // Add the initial ship data to the ship attributes
        //playerGO.GetComponent<ShipAttributes>().Ship = ShipLibrary.GetShip("Mammoth XI");

        GameMSG.AddNewPlayer
            (playerControllerId, conn); 
    }

    public override void OnServerReady(NetworkConnection conn)
    {
        Debug.Log("Client is Ready");

        base.OnServerReady(conn);
    }

    // CLIENT SIDE

    public override void OnStartClient(NetworkClient client)
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
    }

    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        Debug.Log("Scene Changed");

        base.OnClientSceneChanged(conn);
    }

    #endregion

    #region STATIC ACCESSORS

    /* THESE DONT APPLY ANYMORE
    public static void SetContract(int contractID)
    {
        ContractData temp = ContractLibrary.ReturnContract(contractID);
        if (temp != null)
            _base.Contract = temp;
        else
            Debug.Log("GameManager - SetContract: contract does not exist!");
    }

    public static void NextContract()
    {
        ContractData temp = ContractLibrary.ReturnContract(_base.Contract.ID+1);
        if (temp != null)
        {
            if(!temp.Locked)
                _base.Contract = temp;
            else
                _base.Contract = ContractLibrary.ReturnContract(0);
        }
        else
        {
            Debug.Log("GameManager - SetContract: contract does not exist!");
            _base.Contract = ContractLibrary.ReturnContract(0);
        }
    }*/

    public static string Version
    {
        get {return _base.Version;}
    }

    public static GameMessageHandler GameMSG
    {
        get
        {
            // Assign the playerspawn to the scene object if doesnt exist
            if (_base.GameMsg == null)
                _base.GameMsg = GameObject.Find("game")
                    .GetComponent<GameMessageHandler>();
            return _base.GameMsg;
        }
    }

    public static UIMessegeHandler GUI
    {
        get
        {
            // Add new spawned ship to 
            if (_base.GUIMsg == null)
                _base.GUIMsg = GameObject.Find("_gui").GetComponent<UIMessegeHandler>();

            return _base.GUIMsg;
        }
    }

    #endregion

    /// <summary>
    /// Copy player and segment 
    /// data into the stored attributes 
    /// and serializes the objects
    /// </summary>
    /* NOT NEEDED?
    public static void Save()
    {
        /* DONT DO ANYTHING
        first manage the player data
        if (GameObject.FindGameObjectWithTag("PlayerShip") 
            != null)
        {
            _base.Player.LocalUp = GameObject.FindGameObjectWithTag
                ("PlayerShip").transform.up;
        }
        // serialize player object
        Serializer.Save<PlayerData>("Space/Player_Data", _base.Player);
    }*/
}
