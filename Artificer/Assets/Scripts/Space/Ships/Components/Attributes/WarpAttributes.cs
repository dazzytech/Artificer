using UnityEngine;
using System.Collections;

namespace Space.Ship.Components.Attributes
{
    public class WarpAttributes : ComponentAttributes
    {
        #region WARP DISTANCE 

        public float MinDistance;
        public float MaxDistance;

        #endregion


        #region TIMER VARIABLES

        /// <summary>
        /// How long between each warp engagements
        /// </summary>
        public float WarpDelay;

        /// <summary>
        /// A timer count that displaying how long the warp can fire again
        /// </summary>
        public float TimeCount;

        /// <summary>
        /// How long the warp takes to 'fire up'
        /// </summary>
        public float WarpWarmUp;

        #endregion

        #region WARPING

        /// <summary>
        /// Location that the warp will place the 
        /// </summary>
        public Vector2 WarpPoint;

        /// <summary>
        /// Change to accessor
        /// </summary>
        public bool WarpReady;

        public bool CustomWarpPoints;

        public GameObject WarpFXPrefab;

        #endregion
    }
}
