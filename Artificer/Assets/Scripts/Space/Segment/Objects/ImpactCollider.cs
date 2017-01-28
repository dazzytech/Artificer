using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

// Artificer defined
using Data.Space;
using Space.GameFunctions;
using Networking;

namespace Space.Segment
{
    #region HITDATA CONTAINER

    /// <summary>
    /// Container object - Stores information
    /// about a projectile to pass to a possible hit object
    /// </summary>
    [System.Serializable]
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

        #region VIRTUAL FUNCTIONS

        /// <summary>
        /// Receives hit info from server and calls client funcs
        /// </summary>
        /// <param name="hit"></param>
        public virtual void Hit(HitData hit)
        {
            // Set hitdata to our local storage
            _hitD = hit;

            _hitD.damage *= Random.Range(0.5f, 1.0f);

            // Reduce damage based on distance
            _hitD.damage -= Vector3.Distance(transform.position, _hitD.hitPosition) * 0.1f;

            SOColliderHitMessage msg = new SOColliderHitMessage();
            msg.SObjectID = this.netId;
            msg.HitD = hit;
            GameManager.singleton.client.Send((short)MSGCHANNEL.OBJECTHIT, msg);
        }

        /// <summary>
        /// The base hit area will only
        /// hit a single collider so forward to 
        /// Hit
        /// </summary>
        /// <param name="hit"></param>
        public virtual void HitArea(HitData hit)
        {
            // avoid multiple hits
            if (_hitD.Equals(hit))
                return;

            // Forward to single hit
            Hit(hit);
        }

        #endregion
    }
}
