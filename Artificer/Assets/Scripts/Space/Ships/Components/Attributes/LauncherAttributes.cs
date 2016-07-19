using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Space.Ship.Components.Attributes
{
    public class LauncherAttributes : ComponentAttributes 
    {
        public float WeaponDamage;
        public float WeaponRange;
        public float WeaponDelay;
        public float AttackRange;

        public int Rockets;
        public float RocketDelay = .5f;
        
        public bool readyToFire;
        public GameObject ProjectilePrefab;

        // Auto target items
        //\public ShipAttributes Ship;
        
        public bool AutoTarget = true;
    }
}
