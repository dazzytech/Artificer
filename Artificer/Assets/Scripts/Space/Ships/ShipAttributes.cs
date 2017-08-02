using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
// Artificer Defined
using Data.Space;
using Space.Ship.Components.Listener;
using Space.UI.Ship;
using Space.Ship.Components.Attributes;
using Space.UI.Ship.Target;

namespace Space.Ship
{
    /// <summary>
    /// Central storage point for attributes for ship objects
    /// </summary>
    public class ShipAttributes : NetworkBehaviour
    {
        #region SHIP ATTRIBUTES

        public ComponentListener Head;

        [SyncVar]
        public bool hasSpawned;

        // Store a reference to the ships data
        [SyncVar]
        public ShipData Ship;

        // Last ship to attack ship
        [SyncVar]
        public int AggressorID;

        [SyncVar]             //why doesnt this work?
        public bool ShipDocked;

        [SyncVar]
        public bool InCombat;
        
        [SyncVar]
        public int TeamID;

        /// <summary>
        /// The netID if the ship this is attached to
        /// </summary>
        [SyncVar]
        public NetworkInstanceId NetworkID;

        #endregion

        #region COMPONENTS

        public ComponentListener[] Components
        {
            get { return GetComponentsInChildren<ComponentListener>(); }
        }

        public int Engines
        {
            get
            {
                int returnValue = 0;
                foreach (ComponentListener comp in Components)
                {
                    if (comp is EngineListener)
                        returnValue++;
                }

                return returnValue;
            }
        }

        public int Weapons
        {
            get
            {
                int returnValue = 0;
                foreach (ComponentListener comp in Components)
                {
                    if (comp is WeaponListener)
                        returnValue++;
                }

                return returnValue;
            }
        }

        public int Rotors
        {
            get
            {
                int returnValue = 0;
                foreach (ComponentListener comp in Components)
                {
                    if (comp is RotorListener)
                        returnValue++;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Returns all Targeter components 
        /// within a ship
        /// </summary>
        public List<TargeterListener> Targeter
        {
            get
            {
                List<TargeterListener> targets = new List<TargeterListener>();
                foreach (ComponentListener comp in Components)
                {
                    if (comp is TargeterListener)
                        targets.Add(comp as TargeterListener);
                }

                return targets;
            }
        }

        /// <summary>
        /// Returns all Storage components 
        /// within a ship
        /// </summary>
        public List<StorageListener> Storage
        {
            get
            {
                List<StorageListener> storage = new List<StorageListener>();
                foreach (ComponentListener comp in Components)
                {
                    if (comp is StorageListener)
                        storage.Add(comp as StorageListener);
                }

                return storage;
            }
        }

        /// <summary>
        /// Return the first instance of the collector
        /// object
        /// </summary>
        public CollectorListener Collector
        {
            get
            {
                foreach (ComponentListener comp in Components)
                {
                    if (comp is CollectorListener)
                        return comp as CollectorListener;
                }
                return null;
            }
        }

        public bool Aligned
        {
            get
            {
                // If the whole ship follows the 
                // then find the angle of the 
                if (Ship.CombatResponsive)
                {
                    float tAngle = Math.Angle(transform,
                            Camera.main.ScreenToWorldPoint(Input.mousePosition));
                    // between min and max angle of 15 then we fire
                    if (tAngle < 5f && tAngle > -5f)
                        return true;
                    else
                        return false;
                }
                else
                {
                    foreach (TargeterListener targeter in Targeter)
                        if (((TargeterAttributes)targeter.GetAttributes()).Aligned)
                            return true;
                    return false;
                }
            }
        }

        #endregion

        #region TARGETTING ATTRIBUTES

        public List<ShipSelect> TargetedShips;

        public ShipSelect SelfTarget;

        public Rect HighlightRect;
        public float TargetDistance;

        public Transform Target;

        #endregion
    }
}

