using UnityEngine;
using System.Collections;

namespace ShipComponents
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
