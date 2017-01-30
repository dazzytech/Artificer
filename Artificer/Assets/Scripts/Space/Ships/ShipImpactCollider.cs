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

        ShipAttributes m_ship;

        #endregion

        #region MONO BEHAVIOUR

        // Use this for initialization
        void Awake()
        {
            m_ship = GetComponent<ShipAttributes>();
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

            StartCoroutine("CycleThroughCollidersSingle", (Vector2)hit.hitPosition);
        }

        /// <summary>
        /// Similar to Hit, however damage is sent to all
        /// colliders within a radius
        /// </summary>
        /// <param name="hit"></param>
        public override void HitArea(HitData hit)
        {
            if (colliders == null)
                BuildColliders();

            // check if he have just been hit
            if (_hitD.Equals(hit))
                return;

            _hitD = hit;

            StartCoroutine("CycleThroughCollidersGroup");
        }

        /// <summary>
        /// is called on our ship then apply damage otherwise just
        /// update colours
        /// </summary>
        public void ProcessDamage(int[] damaged, HitData hData, float[] vals)
        {
            _hitD = hData;
            StartCoroutine(CycleComponentDamage(damaged, vals));

            if (isLocalPlayer)
            {
                if (GetComponent<ShipPlayerInputController>() != null)
                {
                    Camera.main.gameObject.SendMessage("ShakeCam");
                }

                // After successfully taking damage, set to under attack mode
                CmdSetUA(true);

                Invoke("AttackTimer", 20f);
            }
        }

        [Command]
        private void CmdSetUA(bool ua)
        {
            m_ship.UnderAttack = ua;
        }

        #endregion

        #region COROUTINE

        private IEnumerator CycleThroughCollidersSingle(Vector2 hitPosition)
        {
            // Store an int reference to components that were damaged
            List<int> damagedComps = new List<int>();
            List<float> damagedVals = new List<float>();

            foreach (BoxCollider2D piece in colliders)
            {
                if (piece != null)
                {
                    // Hit detection is manual due to overlap
                    // miss edge collisions
                    if((hitPosition.x <= piece.bounds.max.x
                    && hitPosition.x >= piece.bounds.min.x)
                    && (hitPosition.y <= piece.bounds.max.y
                    && hitPosition.y >= piece.bounds.min.y))
                    {
                        // Retrieve the component listener and attributes from piece obj
                        ComponentAttributes att =
                            piece.gameObject.
                            GetComponent<ComponentAttributes>();

                        damagedComps.Add(att.ID);
                        damagedVals.Add(_hitD.damage *= Random.Range(0.5f, 1.0f));
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
                msg.HitValues = damagedVals.ToArray();
                GameManager.singleton.client.Send((short)MSGCHANNEL.SHIPHIT, msg);
            }

            yield break;
        }
        
        private IEnumerator CycleThroughCollidersGroup()
        {
            // Store an int reference to components that were damaged
            List<int> damagedComps = new List<int>();
            List<float> damagedVals = new List<float>();

            Collider2D[] col = Physics2D.OverlapCircleAll(_hitD.hitPosition,
                                                             _hitD.radius);
            // We did not receive damage
            if (col.Length == 0)
                yield break;

            List<Collider2D> colList = new List<Collider2D>(col);

            foreach (BoxCollider2D piece in colliders)
            {
                if (piece != null)
                {
                    if (colList.Contains(piece))
                    {
                        // Retrieve the component listener and attributes from piece obj
                        ComponentAttributes att =
                                piece.gameObject.
                                GetComponent<ComponentAttributes>();

                        damagedComps.Add(att.ID);
                        damagedVals.Add(_hitD.damage *= Random.Range(0.5f, 1.0f));
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
                msg.HitValues = damagedVals.ToArray();
                GameManager.singleton.client.Send((short)MSGCHANNEL.SHIPHIT, msg);
            }

            yield break;
        }

        private IEnumerator CycleComponentDamage(int[] damaged, float[] dmg)
        {
            int i = 0;
            foreach (ComponentListener listener in
                GetComponent<ShipAttributes>().SelectedComponents(damaged))
            {
                listener.DamageComponent(_hitD, dmg[i++]);

                yield return null;
            }

            yield break;
        }

        /// <summary>
        /// When ship is hit by an enemy
        /// ship is in under attack mode
        /// for 20 sec after last shot
        /// </summary>
        /// <returns></returns>
        private void AttackTimer()
        {
            if (m_ship.UnderAttack)
            {
                CmdSetUA(false);
            }
        }

        #endregion
    }
}
