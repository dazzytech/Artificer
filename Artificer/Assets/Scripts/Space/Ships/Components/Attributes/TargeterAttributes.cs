using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Space.Ship.Components.Listener;

namespace Space.Ship.Components.Attributes
{
    /// <summary>
    /// Targeter behaviour.
    /// determines how the targeter behaves.
    /// </summary>
    public enum TargeterBehaviour
    {
        AUTOLOCKFIRE,
        MOUSECONTROL,
        TARGETSELECTOR,
    }

    public class TargeterAttributes : ComponentAttributes
    {
        //Angle limits
        public float MaxAngle;
        public float MinAngle;

        // turn variables for facing position
        public float turnSpeed;
        public float AttackRange;

        public Transform currentTarget;

        public Vector3 homeForward;

        // Components to be activated via targeter
        public List<ComponentListener> ComponentList;

        public ShipAttributes ShipAtts;

        public TargeterBehaviour Behaviour;

        // Bool that pauses fire
        public bool EngageFire;
    }
}
