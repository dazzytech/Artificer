using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class DestroyDespatch
{
    // self alignment
    public string AlignmentLabel;

    // Last ship to attack ship
    public string AggressorTag;

    // physically destroyed object
    public NetworkInstanceId Self;

    // ID object if one is assigned
    public int MiscID;
}
