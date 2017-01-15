using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

// Artificer Defined
using Data.Shared;
using Space.Projectiles;
using Space.Segment;
using Space.Segment.Generator;
using Space.Ship.Components.Listener;
using Space.UI;

using Networking;

namespace Space.Ship
{
    /// <summary>
    /// Ship external controller.
    /// Dispatches and listens for events from external sources
    /// </summary>
    public class ShipMessageController : NetworkBehaviour
    {
        #region ATTRIBUTES

        ShipAttributes _ship;

        #endregion

        #region MONO BEHAVIOUR

        // Use this for initialization
        void Awake()
        {
            _ship = GetComponent<ShipAttributes>();
        }

        #endregion

        #region INCOMING MESSAGES

        /// <summary>
        /// Adds material information to the 
        /// ship internal resources
        /// </summary>
        /// <param name="data"></param>
        public void AddMaterial(Dictionary<MaterialData, float> data)
        {

            // Can't add material yet
            //_ship.Ship.AddMaterial(data);

            foreach (MaterialData mat in data.Keys)
            {
                // Set to popup gui
                GameManager.GUI.DisplayMessege(new MsgParam("small", "You have collected: " +
                    (data[mat]).ToString("F2")
                       + " - " + mat.Element));
            }
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
                    (_ship.Head.GetAttributes().connectedComponents);

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
                if (_ship.Components.Contains(comp))
                {
                    if (!_ship.SelfTargeted.Contains(target))
                        _ship.SelfTargeted.Add(target);
                }
                else
                {
                    if (!_ship.Targets.Contains(target))
                        _ship.Targets.Add(target);
                }
            }
            else
            {
                if (!_ship.Targets.Contains(target))
                    _ship.Targets.Add(target);
            }
        }

        /// <summary>
        /// Ship has docked at a station therefore should 
        /// be interactive as well change appearance to indicate
        /// </summary>
        public void DisableShip()
        {
            // First disable any form of input through
            // player interaction
            ShipPlayerInputController input = GetComponent<ShipPlayerInputController>();

            if (input == null)
                return;

            input.enabled = false;

            // Begin the process of hiding components on all components
            CmdDisableComponents();

            GetComponent<Rigidbody2D>().constraints = 
                RigidbodyConstraints2D.FreezeAll;


            // disable any automatic functioning components
            foreach(TargeterListener listener in _ship.Targeter)
            {
                listener.enabled = false;
            }
        }

        /// <summary>
        /// When ship has undocked we add player functionality once
        /// again
        /// </summary>
        public void EnableShip()
        {
            // First enable
            // player interaction
            ShipPlayerInputController input = GetComponent<ShipPlayerInputController>();

            if (input == null)
                return;

            input.enabled = true;

            // Begin the process of showing all components
            CmdEnableComponents();

            GetComponent<Rigidbody2D>().constraints =
                RigidbodyConstraints2D.None;


            // disable any automatic functioning components
            foreach (TargeterListener listener in _ship.Targeter)
            {
                listener.enabled = true;
            }
        }

        #region CMD & RPC

        [Command]
        private void CmdDisableComponents()
        {
            _ship.ShipDocked = true;
            RpcDisableComponents();
        }

        [ClientRpc]
        private void RpcDisableComponents()
        {
            // loop through each component and change visual back
            foreach (ComponentListener listener in _ship.Components)
            {
                listener.HideComponent();
            }
        }

        [Command]
        private void CmdEnableComponents()
        {
            _ship.ShipDocked = false;
            RpcEnableComponents();
        }

        [ClientRpc]
        private void RpcEnableComponents()
        {
            // loop through each component and change visual back
            foreach (ComponentListener listener in _ship.Components)
            {
                listener.ShowComponent();
            }
        }

        #endregion

        #endregion

        #region SHIP INITIALIZATION

        /// <summary>
        /// Adds the components to list
        /// stored within shipattributes and
        /// defines the head
        /// </summary>
        public void AddComponentsToList()
        {
            foreach (Transform child in transform)
            {
                ComponentListener comp =
                    child.gameObject.
                        GetComponent<ComponentListener>();
                if (comp != null)
                {
                    // Store component in attributes 
                    // and sent attributes to component
                    _ship.Components.Add(comp);
                    comp.SetShip(_ship);
                }

                if (child.tag == "Head")
                    _ship.Head = comp;
            }
        }

        /// <summary>
        /// Sets the faction/team the ship is aligned to
        /// </summary>
        /// <param name="alignment"></param>
        public void SetShipAlignment(string alignment)
        {
            _ship.AlignmentLabel = alignment;
        }

        #endregion

        #region SHIP DESTRUCTION

        /// <summary>
        /// Adds the entire ship components to
        /// the debris generator. 
        /// </summary>
        public void ShipDestroyed()
        {
            int[] dead = new int[_ship.Components.Count];
            int i = 0;
            foreach (ComponentListener listener in _ship.Components)
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
                _ship.AggressorTag = aggressor.tag;
            }

            // Find the corresponding transform

            ComponentListener listener = null;

            foreach (ComponentListener temp in _ship.Components)
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
            if (listener.Equals(_ship.Head))
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

            foreach (ComponentListener p in _ship.Components)
            {
                if (!connected.Contains(p) && !p.Equals(_ship.Head))
                {
                    // remove piece
                    dead.Add(p.GetAttributes().ID);
                }
            }

            // if player ship we need to destroy ship
            if ((_ship.Engines == 0 || _ship.Rotors == 0) && _ship.Weapons == 0)
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
            msg.AggressorTag = _ship.AggressorTag;
            msg.AlignmentLabel = _ship.AlignmentLabel;
            msg.SelfID = netId;
            msg.ID = GameManager.Space.ID;

            GameManager.singleton.client.Send((short)MSGCHANNEL.SHIPDESTROYED, msg);
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
