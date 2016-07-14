using UnityEngine;
using System.Collections;

namespace Space.Ship.Components.Attributes
{
    public class WellAttributes : ComponentAttributes
    {
        public float WellRadius;
        public float PullForce;
        public GameObject WellFXPrefab;

        [HideInInspector]
        public bool GravityEngaged;
    }
}
