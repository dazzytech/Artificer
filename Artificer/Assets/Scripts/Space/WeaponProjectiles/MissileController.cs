using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

using Space.Segment;

namespace Space.Projectiles
{
    public class MissileController : WeaponController
    {
        #region CUSTOMIZABLE ATTRIBUTES

        public float MissileSpeed = 5f;
        public float MissileFuse;

        #endregion

        #region CALCULATION VARIABLES

        float turn = 2.5f;
        float lastTurn = 0f;
        public float Radius = 3f;

        #endregion

        private Transform m_target;

        [SerializeField]
        private Rigidbody2D rocketRigidbody;

        #region MONOBEHAVIOUR

        void FixedUpdate()
        {
            if (m_target == null)
                return;

            if (Vector3.Distance(transform.position, m_target.position) < 5)
            {
                turn = 0f;
                return;
            }

            Quaternion newRotation = Quaternion.LookRotation(transform.position - m_target.position, Vector3.forward);
            newRotation.x = 0.0f;
            newRotation.y = 0.0f;
            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * turn);
            rocketRigidbody.velocity = transform.up * MissileSpeed;
            if (turn < 40f)
            {
                lastTurn += Time.deltaTime * Time.deltaTime * 50f;
                turn += lastTurn;
            }
        }

        void Update()
        {
            if (!hasAuthority)
                return;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, -_data.Direction, MissileSpeed * Time.deltaTime, maskIgnore);

            if (hit.transform != null)
            {
                // if successful hit then break out of the loop
                ApplyDamageArea(hit);
            }
        }

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Creates the bullet appearance then.
        /// </summary>
        /// <param name="direction">Direction.</param>
        /// <param name="range">Range.</param>
        [Server]
        public override void CreateProjectile(WeaponData data)
        {
            base.CreateProjectile(data);

            RpcSetTarget(data.Target.Value);

            RpcBuildFX(data);
        }

        #endregion

        #region PRIVATE UTILITY

        private bool ApplyDamageArea(RaycastHit2D hit)
        {
            // Find the ship that fired the projectile
            GameObject aggressor = ClientScene.FindLocalObject(_data.Self);

            if (aggressor == null)
            {
                return false;
            }

            if (hit.transform == aggressor.transform)
                return false;

            ExplodeRocket(hit.point);

            return true;
        }

        #endregion

        #region PRIVATE UTILITIES

        private void SelfDestruct()
        {
            ExplodeRocket(transform.position);
        }

        private void ExplodeRocket(Vector3 hitPos)
        {
            HitData hitD = new HitData();
            hitD.damage = _data.Damage;
            hitD.hitPosition = hitPos;
            hitD.radius = Radius;
            hitD.originID = _data.Self;

            CmdBuildHitFX(hitPos, _data);

            RaycastHit2D[] colliderList =
                Physics2D.CircleCastAll(transform.position, Radius, Vector2.up, 0, maskIgnore);

            foreach (RaycastHit2D hitB in colliderList)
            {
                if (hitB.transform != null)
                {
                    // retrieve impact controller
                    ImpactCollider IC = hitB.transform.GetComponent<ImpactCollider>();
                    if (IC != null)
                    {
                        IC.HitArea(hitD);
                    }
                }
            }

            DestroyProjectile();
        }

        #endregion

        #region CLIENT

        /// <summary>
        /// Creates the target on 
        /// each client
        /// </summary>
        /// <param name="targetID"></param>
        [ClientRpc]
        public void RpcSetTarget(uint targetID)
        {
            GameObject targetObj = 
                ClientScene.FindLocalObject
                    (new NetworkInstanceId(targetID));

            m_target = targetObj.transform;

            if(hasAuthority)
                Invoke("SelfDestruct", MissileFuse);
        }

        #endregion
    }
}

