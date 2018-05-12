using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;


namespace Space.UI.Warp
{
    /// <summary>
    /// Prefab that is swapped with a map object that the player can warp to
    /// </summary>
    public class WarpSelectHUDItem : SelectableHUDItem
    {
        /// <summary>
        /// The object that this warp icon represents
        /// </summary>
        [Header("Warp")]
        public Transform Reference;
    }
}