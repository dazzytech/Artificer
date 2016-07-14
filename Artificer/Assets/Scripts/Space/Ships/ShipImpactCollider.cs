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

            StartCoroutine("CycleThroughColliders");
        }

        [Command]
        public void CmdProcessDamage(int[] damaged)
        {
            RpcProcessDamage(damaged);
        }

        /// <summary>
        /// sent to this object on other clients and 
        /// </summary>
        [ClientRpc]
        public void RpcProcessDamage(int[] damaged)
        {
            if (isLocalPlayer)
                return;

            StartCoroutine("CycleComponentColours", damaged);
        }

        #endregion

        #region COROUTINE

        private IEnumerator CycleThroughColliders()
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

                        // Consider doing the damage processing here
                        // Create a utility for retreiving the component listener item.
                        piece.gameObject.SendMessage("DamageComponent", _hitD);

                        damagedComps.Add(att.ID);

                        Debug.Log("Damaged Component: " + piece.name);
                    }
                }

                yield return null;
            }

            if (damagedComps.Count > 0)
                CmdProcessDamage(damagedComps.ToArray());

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
