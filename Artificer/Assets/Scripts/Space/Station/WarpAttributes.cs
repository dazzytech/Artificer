using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stations
{
    /// <summary>
    /// Contains attributes unique to a warp
    /// station
    /// </summary>
    public class WarpAttributes : StationAttributes
    {
        // Exit point of warp
        public Vector2 ExitPoint;

        // Radius for ship warping
        public float WarpRadius;

        // How long until cancel build without exit
        public float WaitForExit;
    }
}