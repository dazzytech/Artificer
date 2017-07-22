using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
// Artificer Defined
using Data.Space;
using Space.Ship.Components.Listener;
using Space.UI.Ship;
using Space.Ship.Components.Attributes;

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

        #region COMPONENT GETTERS

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

        public List<LauncherListener> Launchers
        {
            get
            {
                List<LauncherListener> targets = new List<LauncherListener>();
                foreach (ComponentListener comp in Components)
                {
                    if (comp is LauncherListener)
                        targets.Add(comp as LauncherListener);
                }

                return targets;
            }
        }

        public List<ShieldListener> Shields
        {
            get
            {
                List<ShieldListener> targets = new List<ShieldListener>();
                foreach (ComponentListener comp in Components)
                {
                    if (comp is ShieldListener)
                        targets.Add(comp as ShieldListener);
                }

                return targets;
            }
        }

        public WarpListener Warp
        {
            get
            {
                foreach (ComponentListener comp in Components)
                {
                    if (comp is WarpListener)
                        return comp as WarpListener;
                }
                return null;
            }
        }

        public ShipGenerator Generator
        {
            get { return GetComponent<ShipGenerator>(); }
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

        #region COMPONENT UTILITIES

        /// <summary>
        /// Returns a list of components from a list of IDs
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public ComponentListener[] SelectedComponents(int[] list)
        {
            List<ComponentListener> retList = new List<ComponentListener>();

            // Add components with corresponding IDs 
            // to return list
            foreach (ComponentListener listener in Components)
            {
                if (listener.GetAttributes() != null)
                {
                    foreach (int i in list)
                    {
                        if (listener.GetAttributes().ID == i)
                        {
                            retList.Add(listener);
                        }
                    }
                }
            }

            return retList.ToArray();
        }

        #endregion

        #region TARGETTING ATTRIBUTES

        public List<ShipSelect> TargetedShips;

        public Rect HighlightRect;
        public float TargetDistance;

        public Transform Target;

        #endregion

        #region ACCESSORS

        /// <summary>
        /// returns int labeling the state the station is currently in
        /// 0 - Safe
        /// 1 - Under Attack
        /// 2 - Docked
        /// </summary>
        public int Status
        {
            get
            {
                if (ShipDocked)
                    return 2;
                if (InCombat)
                    return 1;
                else
                    return 0;
            }
        }

        public float Distance
        {
            get
            {
                // Retrieve player object and check if 
                // Player object currently exists
                GameObject playerTransform =
                    GameObject.FindGameObjectWithTag("PlayerShip");

                if (playerTransform == null)
                {
                    return -1;
                }

                // return distance
                return Vector3.Distance(this.transform.position,
                    playerTransform.transform.position);
            }
        }

        #endregion
    }
}

