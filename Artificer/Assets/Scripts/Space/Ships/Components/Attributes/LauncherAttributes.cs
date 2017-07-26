using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Space.Ship.Components.Attributes
{
    public class LauncherAttributes : ComponentAttributes 
    {
        public float WeaponDamage;
        public float WeaponDelay;
        public float AttackRange;

        public float MaxAngle;
        public float MinAngle;

        public int Rockets;
        public float RocketDelay = .5f;
        
        public bool readyToFire;
        public GameObject ProjectilePrefab;
        
        public bool AutoTarget = true;
    }
}
