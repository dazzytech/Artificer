using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class DestroyDespatch
{
    // self alignment
    public string AlignmentLabel;

    // Last ship to attack ship
    public string AggressorTag;

    // physical destroyed ship
    public NetworkInstanceId Self;
}
