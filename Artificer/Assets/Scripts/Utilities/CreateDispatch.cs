using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

[System.Serializable]
public class CreateDispatch
{
    /// <summary>
    /// Net Instance ID of the ship created
    /// </summary>
    public uint Self;

    /// <summary>
    /// The ID key of the player spawning the object
    /// </summary>
    public int PlayerID;

    /// <summary>
    /// the alignment label
    /// </summary>
    public int TeamID;
}

