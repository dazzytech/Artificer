using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

// Artificer defined
using Data.Space;

namespace Space.Segment
{
    #region HITDATA CONTAINER

    /// <summary>
    /// Container object - Stores information
    /// about a projectile to pass to a possible hit object
    /// </summary>
    public struct HitData
    {
        public Vector3 hitPosition;
        public float damage;
        public float radius;
        public NetworkInstanceId originID;
        public int hitComponent;
    }

    #endregion

    /// <summary>
    /// Add on function that manages server objects being hit by
    /// projectiles. 
    /// </summary>
    public class ImpactCollider : NetworkBehaviour
    {
        #region ATTRIBUTES

        [SyncVar]
        public HitData _hitD;

        #endregion

        #region EXTERNAL FUNCTIONS

        /// <summary>
        /// Receives hit info from server and calls client funcs
        /// </summary>
        /// <param name="hit"></param>
        [Server]
        public void Hit(HitData hit)
        {
            // Set hitdata to our local storage
            _hitD = hit;
            RpcHit();
        }

        #endregion

        #region VIRTUAL FUNCTIONS

        /// <summary>
        /// Client function called by server 
        /// to handle projectile hits on our object
        /// process the damage on the local object and then
        /// update the remote clients
        /// </summary>
        /// <param name="hitpoint">The vector 3D point
        /// where the projectile intesected with the colliders </param>
        [ClientRpc]
        public virtual void RpcHit()
        { }

        /// <summary>
        /// Similar to RpcHit, however damage is sent to all
        /// colliders within a radius
        /// </summary>
        /// <param name="hit"></param>
        /*[ClientRpc]
        public void RpcHitArea(HitData hit)
        {
            // add distance damage reduction
            foreach (BoxCollider2D piece in colliders)
            {
                if(piece != null)
                {
                    Collider2D col = Physics2D.OverlapCircle(hit.hitPosition,
                                                             hit.radius);
                    if(col != null)
                    {
                        if(col.Equals(piece))
                        {
                            piece.gameObject.SendMessage("DamageComponent", hit);
                        }
                    }
                }
            }

            if (GetComponent<ShipPlayerInputController>() != null)
            {
                Camera.main.gameObject.SendMessage("ShakeCam");
            }
        }*/

        #endregion
    }
}
