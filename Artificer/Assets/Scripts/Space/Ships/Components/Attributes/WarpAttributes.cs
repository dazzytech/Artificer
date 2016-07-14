using UnityEngine;
using System.Collections;

namespace Space.Ship.Components.Attributes
{
    public class WarpAttributes : ComponentAttributes
    {
        // distance
        public float WarpDistance;
        public float MinDistance;
        public float MaxDistance;

        //timer
        public float WarpDelay;
        public float TimeCount;

        public float WarpWarmUp;
        
        public bool readyToFire;

        public GameObject WarpFXPrefab;
    }
}
