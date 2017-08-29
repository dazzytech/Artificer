using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class DestroyDespatch
{
    /// <summary>
    /// Team that the destroyed object
    /// belonged to
    /// </summary>
    public int SelfTeamID;

    /// <summary>
    /// Network ID of the ship that was destroyed
    /// </summary>
    public NetworkInstanceId SelfID;

    /// <summary>
    /// Team that the aggressive ship
    /// belongs too
    /// </summary>
    public int AggressorTeamID;

    /// <summary>
    /// Network id of the object that
    /// destroyed the ship
    /// </summary>
    public NetworkInstanceId AggressorID;

    ///  <summary>
    ///  ID object if one is assigned
    /// </summary>
    public int MiscID;
}
