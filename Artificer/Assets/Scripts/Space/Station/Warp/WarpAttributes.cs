using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Stations
{
    /// <summary>
    /// Contains attributes unique to a warp
    /// station
    /// </summary>
    public class WarpAttributes : StationAttributes
    {
        // Radius within warps detect other warps
        public float WarpRadius;     
    }
}