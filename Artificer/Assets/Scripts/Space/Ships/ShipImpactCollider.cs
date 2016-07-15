using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

// Artificer defined
using Data.Space;
using Space.Segment;
using Space.Ship.Components.Attributes;
using Space.Ship.Components.Listener;

namespace Space.Ship
{
    class ShipImpactCollider : ImpactCollider
    {
        #region ATTRIBUTES

        // Store a base list of colliders
        // in some cases this could be only one
        private List<BoxCollider2D> colliders;

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

        [Server]
        public override void Hit(HitData hit)
        {
            base.Hit(hit);

            if (colliders == null)
                BuildColliders();

            StartCoroutine("CycleThroughCollidersSingle");
        }

        /// <summary>
        /// Client function called by server 
        /// to handle projectile hits on our object
        /// process the damage on the local object and then
        /// update the remote clients
        /// </summary>
        /// <param name="hitpoint">The vector 3D point
        /// where the projectile intesected with the colliders </param>
        [ClientRpc]
        public override void RpcHit()
        {
            if (!isLocalPlayer)
                return;

            if (GetComponent<ShipPlayerInputController>() != null)
            {
                Camera.main.gameObject.SendMessage("ShakeCam");
            }
        }

        /// <summary>
        /// Similar to RpcHit, however damage is sent to all
        /// colliders within a radius
        /// </summary>
        /// <param name="hit"></param>
        [ClientRpc]
        public override void RpcHitArea()
        {
            if (!isLocalPlayer)
                return;

            if (GetComponent<ShipPlayerInputController>() != null)
            {
                Camera.main.gameObject.SendMessage("ShakeCam");
            }

            StartCoroutine("CycleThroughCollidersGroup");
        }

        /// <summary>
        /// is called on our ship then apply damage otherwise just
        /// update colours
        /// </summary>
        [ClientRpc]
        public void RpcProcessDamage(int[] damaged)
        {
            if (isLocalPlayer)
                StartCoroutine("CycleComponentDamage", damaged);
            else
                StartCoroutine("CycleComponentColours", damaged);
        }

        #endregion

        #region COROUTINE

        [Server]
        private IEnumerator CycleThroughCollidersSingle()
        {
            // Store an int reference to components that were damaged
            List<int> damagedComps = new List<int>();

            foreach (BoxCollider2D piece in colliders)
            {
                if (piece != null)
                {
                    if (piece.bounds.Contains(_hitD.hitPosition))
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

            if (damagedComps.Count > 0)
                RpcProcessDamage(damagedComps.ToArray());

            yield return null;
        }

        [Server]
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
