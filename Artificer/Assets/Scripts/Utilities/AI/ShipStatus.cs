using UnityEngine;
using System.Collections;

public class ShipStatus
{
    /// <summary>
    /// Evac is needed.
    /// Tests the ship attributes to see is ship
    /// functions correctly
    /// </summary>
    /// <returns><c>true</c>, if evaced is needed, <c>false</c> otherwise.</returns>
    public static bool EvacNeeded(Transform ship)
    {
        ShipAttributes att = ship.GetComponent<ShipAttributes>();

        if (att.Engines == 0)
            return true;

        if (att.Weapons == 0)
            return true;

        if (att.Rotors == 0)
            return true;

        return false;
    }
}

