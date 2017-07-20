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

        Transform Target;

        protected Rigidbody2D rocketRigidbody;

        #region MONOBEHAVIOUR

        // Use this for initialization
        void Awake()
        {
            rocketRigidbody = GetComponent<Rigidbody2D>();
        }

        void Start()
        {
            if(!hasAuthority)
                return;

            Invoke("ApplyDamageArea", MissileFuse);
        }

        void FixedUpdate()
        {
            if (Target == null)
                return;

            Quaternion newRotation = Quaternion.LookRotation(transform.position - Target.position, Vector3.forward);
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
            RaycastHit2D hit = Physics2D.Raycast(transform.position, -_data.Direction, MissileSpeed * Time.deltaTime, maskIgnore);

            if (hit.transform != null)
            {
                // Both client and host reach here
                // however only the player that applies damage
                if (hasAuthority)
                {
                    // if successful hit then break out of the loop
                    ApplyDamageArea(hit);
                }
            }
        }

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Creates the bullet appearance then.
        /// </summary>
        /// <param name="direction">Direction.</param>
        /// <param name="range">Range.</param>
        public override void CreateProjectile(WeaponData data)
        {
            base.CreateProjectile(data);

            GameObject targetObj = ClientScene.FindLocalObject(data.Target);

            if (targetObj == null)
            {
                DestroyProjectile();
                return;
            }
            Target = targetObj.transform;

            RpcBuildFX(data);
        }

        #endregion

        #region PRIVATE UTILITY

        private bool ApplyDamageArea(RaycastHit2D hit)
        {
            CancelInvoke("ApplyDamageArea");

            // Find the ship that fired the projectile
            GameObject aggressor = ClientScene.FindLocalObject(_data.Self);

            if (aggressor == null)
            {
                return false;
            }

            if (hit.transform == aggressor.transform)
                return false;

            CmdBuildHitFX(hit.point, _data);

            HitData hitD = new HitData();
            hitD.damage = _data.Damage;
            // maybe this should be center of impact
            hitD.hitPosition = hit.point;
            hitD.radius = Radius;
            hitD.originID = _data.Self;

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

            return true;
        }

        #endregion
    }
}

