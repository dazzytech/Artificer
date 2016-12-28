using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using System.Collections;
using System.Collections.Generic;

// Artificer defined
using Data.Space;
using Space.Segment;
using Space.GameFunctions;
using Space.Ship.Components.Attributes;
using Space.Ship.Components.Listener;
using Networking;

namespace Space.Ship
{
    class ShipImpactCollider : ImpactCollider
    {
        #region ATTRIBUTES

        // Store a base list of colliders
        // in some cases this could be only one
        private List<BoxCollider2D> colliders;

        #endregion

        #region MONO BEHAVIOUR

        void Awake()
        {
            GameManager.singleton.client.RegisterHandler((short)MSGCHANNEL.PROCESSSHIPHIT, ProcessHitMsg);
        }

        #endregion

        #region COLLIDER RETREIVAL

        // Start function retreives all collider objects and stores them
        // for later hit detection
        public void BuildColliders()
        {
            // Create new list item
            colliders = new List<BoxCollider2D>();

            // If the base obj doesn't have a collider
            // we have a ship object so store colliders for all components
            foreach (BoxCollider2D comps in GetComponentsInChildren<Collider2D>())
            {
                colliders.Add(comps);
            }
        }

        #endregion

        #region IMPACT COLLISION

        /// <summary>
        /// Called on remote ship for client side
        /// collision detection
        /// </summary>
        /// <param name="hit"></param>
        public override void Hit(HitData hit)
        {
            if (colliders == null)
                BuildColliders();

            _hitD = hit;

            StartCoroutine("CycleThroughCollidersSingle");
        }

        public override void ProcessHitMsg(NetworkMessage msg)
        {
            ShipColliderHitMessage colMsg = msg.ReadMessage<ShipColliderHitMessage>();

            GameObject HitObj = ClientScene.FindLocalObject(colMsg.ShipID);
            if (HitObj != null)
            {
                HitObj.transform.GetComponent<ShipImpactCollider>()
                .ProcessDamage(colMsg.HitComponents, colMsg.HitD);
            }
        }

        /// <summary>
        /// Similar to RpcHit, however damage is sent to all
        /// colliders within a radius
        /// </summary>
        /// <param name="hit"></param>
        /*[ClientRpc]
        public override void RpcHitArea()
        {
            if (!isLocalPlayer)
                return;

            if (GetComponent<ShipPlayerInputController>() != null)
            {
                Camera.main.gameObject.SendMessage("ShakeCam");
            }

            StartCoroutine("CycleThroughCollidersGroup");
        }*/

        /// <summary>
        /// is called on our ship then apply damage otherwise just
        /// update colours
        /// </summary>
        public void ProcessDamage(int[] damaged, HitData hData)
        {
            _hitD = hData;

            if (isLocalPlayer)
            {
                StartCoroutine("CycleComponentDamage", damaged);
                if (GetComponent<ShipPlayerInputController>() != null)
                {
                    Camera.main.gameObject.SendMessage("ShakeCam");
                }
            }
            else
                StartCoroutine("CycleComponentColours", damaged);
        }

        #endregion

        #region COROUTINE

        private IEnumerator CycleThroughCollidersSingle()
        {
            // Store an int reference to components that were damaged
            List<int> damagedComps = new List<int>();

            foreach (BoxCollider2D piece in colliders)
            {
                if (piece != null)
                {
                    if (piece.OverlapPoint(_hitD.hitPosition))
                    {
                        // Retrieve the component listener and attributes from piece obj
                        ComponentAttributes att =
                            piece.gameObject.
                            GetComponent<ComponentAttributes>();

                        damagedComps.Add(att.ID);
                    }
                }

                yield return null;
            }

            // Send message to server to process damage
            if (damagedComps.Count > 0)
            {
                ShipColliderHitMessage msg = new ShipColliderHitMessage();
                msg.HitComponents = damagedComps.ToArray();
                msg.ShipID = this.netId;
                msg.HitD = _hitD;
                GameManager.singleton.client.Send((short)MSGCHANNEL.SHIPHIT, msg);
            }

            yield return null;
        }
        
        private IEnumerator CycleThroughCollidersGroup()
        {
            // Store an int reference to components that were damaged
            /*List<int> damagedComps = new List<int>();

            foreach (BoxCollider2D piece in colliders)
            {
                if (piece != null)
                {
                    Collider2D col = Physics2D.OverlapCircle(_hitD.hitPosition,
                                                             _hitD.radius);
                    if (col != null)
                    {
                        if (col.Equals(piece))
                        {
                            // Retrieve the component listener and attributes from piece obj
                            ComponentAttributes att =
                                    piece.gameObject.
                                    GetComponent<ComponentAttributes>();

                            // consider creating a new hit data with reduced damage based
                            // on distance
                            piece.gameObject.SendMessage("DamageComponent", _hitD);

                            damagedComps.Add(att.ID);
                        }
                    }
                }

                yield return null;
            }

            if (damagedComps.Count > 0)
                RpcProcessDamage(damagedComps.ToArray());*/

            yield return null;
        }

        private IEnumerator CycleComponentDamage(int[] damaged)
        {
            foreach (ComponentListener listener in
                GetComponent<ShipAttributes>().SelectedComponents(damaged))
            {
                listener.DamageComponent(_hitD);

                yield return null;
            }

            yield return null;
        }

        private IEnumerator CycleComponentColours(int[] damaged)
        {
            foreach (ComponentListener listener in
                GetComponent<ShipAttributes>().SelectedComponents(damaged))
            {
                listener.GetAttributes().Integrity -= _hitD.damage;
                listener.SetColour(listener.GetAttributes().Integrity);

                yield return null;
            }

            yield return null;
        }

        #endregion
    }
}
