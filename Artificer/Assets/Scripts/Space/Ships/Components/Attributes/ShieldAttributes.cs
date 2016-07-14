using UnityEngine;
using System.Collections;

namespace Space.Ship.Components.Attributes
{
    public class ShieldAttributes : ComponentAttributes 
    {
    	[HideInInspector]
    	public GameObject ShieldGO;

        [HideInInspector]
        public float Delay = 1f;

        public float RechargeDelay = 5f;

        [HideInInspector]
        public bool Ready = true;

        public float ShieldRadius;

        public float ShieldIntegrity = 10000f;
        public float CurrentIntegrity;
        public bool Destroyed = false;
    }
}
