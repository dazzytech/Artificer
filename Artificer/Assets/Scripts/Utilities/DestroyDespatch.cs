using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class DestroyDespatch
{
    // self alignment
    public int SelfTeam;

    // Last ship to attack ship
    public int AggressorID;

    // physically destroyed object
    public NetworkInstanceId Self;

    // ID object if one is assigned
    public int MiscID;
}
