using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using UI;

/// <summary>
/// Where the player wants to spawn
/// </summary>
public class SpawnSelectItem : SelectableHUDItem
{
    [Header("Spawn")]
    public int SpawnID;
}
