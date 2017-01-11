using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
// Artificer Defined
using Data.Space;
using Data.Shared;
using Space.Ship.Components.Listener;

namespace Space.Ship
{
    /// <summary>
    /// Central storage point for attributes for ship objects
    /// </summary>
    public class ShipAttributes : NetworkBehaviour
    {
        #region SHIP ATTRIBUTES

        public List<ComponentListener> Components;

        public ComponentListener Head;

        // allegance
        // friendly, enemy, player
        // Assigned by the local player to the ship
        public string AlignmentLabel;

        // Store a reference to the ships data
        public ShipData Ship;

        // Last ship to attack ship
        [SyncVar]
        public string AggressorTag;

        //[SyncVar]             why does this work?
        public bool ShipDocked;

        public NetworkInstanceId instID;

        #endregion

        #region COMPONENT GETTERS

        // Getter functions
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

        public List<Transform> Targets;
        public List<Transform> SelfTargeted;
        public List<Transform> HighlightedTargets;
        public Rect HighlightRect;
        public float TargetDistance;

        #endregion
    }
}

