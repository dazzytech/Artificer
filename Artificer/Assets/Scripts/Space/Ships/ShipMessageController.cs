using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

// Artificer Defined
using Data.Space;
using Space.Projectiles;
using Space.Segment;
using Space.Segment.Generator;
using Space.Ship.Components.Listener;
using Space.UI;

using Networking;
using Space.UI.Ship;
using Space.UI.Ship.Target;

namespace Space.Ship
{
    /// <summary>
    /// Ship external controller.
    /// Dispatches and listens for events from external sources
    /// </summary>
    public class ShipMessageController : NetworkBehaviour
    {
        #region ATTRIBUTES

        [SerializeField]
        private ShipAttributes m_ship;

        [SerializeField]
        private ShipAccessor m_access;

        #endregion

        #region INCOMING MESSAGES

        /// <summary>
        /// Adds material information to the 
        /// ship internal resources
        /// </summary>
        /// <param name="data"></param>
        public void CollectItem(Collectable data)
        {

            // Can't add material yet
            //_ship.Ship.AddMaterial(data);

            /*foreach (MaterialData mat in data.Keys)
            {
                // Set to popup gui
                SystemManager.UIMsg.DisplayMessege
                    ("small", "You have collected: " +
                        (data[mat]).ToString("F2")
                       + " - " + mat.Element);
            }*/
        }

        /// <summary>
        /// Creates a list of components that is 
        /// still linked to the ship head
        /// omitting the destroyed piece.
        /// </summary>
        /// <param name="exempt"> component that was destroyed</param>
        /// <returns></returns>
        public List<ComponentListener> BuildConnections(ComponentListener exempt)
        {
            List<ComponentListener> retList
                = new List<ComponentListener>();

            List<ComponentListener> piecesToAdd
                = new List<ComponentListener>
                    (m_ship.Head.GetAttributes().connectedComponents);

            while (piecesToAdd.Count != 0)
            {
                if (piecesToAdd[0] == null)
                {
                    piecesToAdd.RemoveAt(0);
                    continue;
                }

                ComponentListener p = piecesToAdd[0];

                if (!retList.Contains(p) && !p.Equals(exempt))
                {
                    retList.Add(p);
                    piecesToAdd.AddRange(p.GetAttributes().connectedComponents);
                }
                piecesToAdd.Remove(p);
            }

            return retList;
        }

        /// <summary>
        /// Adds the target to the target list
        /// first tests if the target is suitable
        /// (may add a self targeting for repair)
        /// </summary>
        /// <param name="target">Target.</param>
        public void AddTarget(Transform target)
        {
            ComponentListener comp = target.GetComponent<ComponentListener>();
            if (comp != null)
            {
                // test if self targetting
                if (m_ship.Components.Contains(comp))
                {
                    if (m_ship.SelfTarget == null)
                    {
                        // first time we selected self
                        // so create self select and target head
                        m_ship.SelfTarget = new ShipSelect();
                        m_ship.SelfTarget.Ship = m_access;
                        m_ship.SelfTarget.TargetedComponents
                            = new List<Transform>();

                        m_ship.SelfTarget.TargetedComponents.Add
                            (m_ship.Head.transform);
                    }
                    else
                    {
                        // we are already target so add selected piece
                        if (!m_ship.SelfTarget.TargetedComponents.Contains(target))
                            m_ship.SelfTarget.TargetedComponents.Add(target);
                        else
                        {
                            m_ship.SelfTarget.TargetedComponents.Remove(target);
                            if (m_ship.SelfTarget.TargetedComponents.Count == 0)
                                m_ship.SelfTarget = null;
                        }
                    }
                }
                else
                {
                    // Determine if our target ship list already contains 
                    // the ship parent
                    // retreive ship attributes from removed cmp
                    ShipAccessor ship = target.GetComponentInParent<ShipAccessor>();
                    // Is there a ship attached to this?
                    if (ship != null)
                    {
                        // This will return our ship selection
                        // if this is already selected.
                        ShipSelect selected = m_ship.TargetedShips
                            .FirstOrDefault(o => o.Ship.Equals(ship));

                        if (selected != null)
                        {
                            // This ship has already been selected
                            // Add our selected component (if not already selected)
                            if (!selected.TargetedComponents.Contains(target))
                                selected.TargetedComponents.Add(target);
                            else
                            {
                                selected.TargetedComponents.Remove(target);
                                if (selected.TargetedComponents.Count == 0)
                                    m_ship.TargetedShips.Remove(selected);
                                selected = null;
                            }
                        }
                        else
                        {
                            // This is a newly selected component
                            // Create a new shipselect object
                            selected = new ShipSelect();
                            selected.Ship = ship;
                            selected.TargetedComponents = new List<Transform>();
                            // Add the head as a target bu default
                            selected.TargetedComponents.Add
                                (ship.Head);

                            // Now add this to selection
                            m_ship.TargetedShips.Add(selected);
                        }
                    }
                }
            }
            else
            {
                // If not a ship, ignore for now
                //if (!m_ship.Targets.Contains(target))
                  //  m_ship.Targets.Add(target);
            }
        }

        /// <summary>
        /// Sets our ship in a combative 
        /// agaisnt the passed transform
        /// </summary>
        /// <param name="Combatant"></param>
        public void SetCombatant(Transform Combatant)
        {
            if (!hasAuthority)
                return; 

            m_ship.Target = Combatant;

            CmdSetCombat(true);

            CancelInvoke("AttackTimer");

            Invoke("AttackTimer", 20f);
        }

        /// <summary>
        /// When ship is hit by an enemy
        /// ship is in under attack mode
        /// for 20 sec after last shot
        /// </summary>
        /// <returns></returns>
        private void AttackTimer()
        {
            if (m_ship.InCombat)
            {
                CmdSetCombat(false);
            }

            m_ship.Target = null;
        }

        #region CMD & RPC

        [Command]
        public void CmdDisableComponents()
        {
            m_ship.ShipDocked = true;
            RpcDisableComponents();
        }

        [ClientRpc]
        public void RpcDisableComponents()
        {
            // loop through each component and change visual back
            foreach (ComponentListener listener in m_ship.Components)
            {
                listener.HideComponent();
            }
        }

        [Command]
        public void CmdEnableComponents()
        {
            m_ship.ShipDocked = false;
            RpcEnableComponents();
        }

        [ClientRpc]
        public void RpcEnableComponents()
        {
            // loop through each component and change visual back
            foreach (ComponentListener listener in m_ship.Components)
            {
                listener.ShowComponent();
            }
        }

        [Command]
        private void CmdSetCombat(bool ic)
        {
            m_ship.InCombat = ic;
        }

        #endregion

        #endregion

        #region SHIP DESTRUCTION

        /// <summary>
        /// Adds the entire ship components to
        /// the debris generator. 
        /// </summary>
        public void ShipDestroyed()
        {
            int[] dead = new int[m_ship.Components.Length];
            int i = 0;
            foreach (ComponentListener listener in m_ship.Components)
            {
                listener.Destroy();
                dead[i++] = listener.GetAttributes().ID;
            }

            SendMessage("BuildColliders");

            CmdRemoveComponent(dead, this.transform.position);

            Destroy();
        }

        /// <summary>
        /// Loops through each connection omitting the 
        /// Destroyed component. Then 
        /// adds the unconnected components to the debris generator.
        /// </summary>
        /// <param name="hit">hit.</param>
        public void DestroyComponent(HitData hit)
        {
            // Set the aggressor
            GameObject aggressor = NetworkServer.FindLocalObject(hit.originID);

            if (aggressor != null)
            {
                m_ship.AggressorID = aggressor.GetComponent<ShipAttributes>().TeamID;
            }

            // Find the corresponding transform

            ComponentListener listener = null;

            foreach (ComponentListener temp in m_ship.Components)
            {
                if (temp.GetAttributes().ID == hit.hitComponent)
                {
                    listener = temp;
                    break;
                }
            }

            // Check we were successful in obtaining the destroyed head
            if (listener == null)
                return;

            // first test if head was killed
            if (listener.Equals(m_ship.Head))
            {
                ShipDestroyed();
                return;
            }

            // Create a list of instance IDs current connected to head
            List<ComponentListener> connected = BuildConnections(listener);
            if (connected.Count == 0)
            {
                ShipDestroyed();
                return;
            }

            List<int> dead = new List<int>();

            foreach (ComponentListener p in m_ship.Components)
            {
                if (!connected.Contains(p) && !p.Equals(m_ship.Head))
                {
                    // remove piece
                    dead.Add(p.GetAttributes().ID);
                }
            }

            // if player ship we need to destroy ship
            if ((m_ship.Engines == 0 || m_ship.Rotors == 0) && m_ship.Weapons == 0)
                ShipDestroyed();
            else
                CmdRemoveComponent(dead.ToArray(), listener.transform.position);
        }

        /// <summary>
        /// Triggers destroyed event 
        /// </summary>
        private void Destroy()
        {
            // Send message to server for updating
            ShipDestroyMessage msg = new ShipDestroyMessage();
            msg.AggressorTeam = m_ship.AggressorID;
            msg.SelfTeam = m_ship.TeamID;
            msg.SelfID = netId;
            msg.ID = SystemManager.Space.ID;

            SystemManager.singleton.client.Send((short)MSGCHANNEL.SHIPDESTROYED, msg);
        }

        #region CMD & RPC

        /// <summary>
        /// Runs on server to create debris using destroyed 
        /// parts and sends rpc to rebuild all ship colliders
        /// </summary>
        /// <param name="dead"></param>
        /// <param name="position"></param>
        [Command]
        private void CmdRemoveComponent(int[] dead, Vector3 position)
        {
            DebrisGenerator.SpawnShipDebris(position,
                dead, this.netId, (position -
                transform.position).normalized * 10);

            RpcRemoveComponent(dead);
        }

        /// <summary>
        /// Rebuild collider list for hit detection.
        /// </summary>
        /// <param name="dead"></param>
        [ClientRpc]
        private void RpcRemoveComponent(int[] dead)
        {
            SendMessage("BuildColliders");
        }

        #endregion

        #endregion
    }
}
