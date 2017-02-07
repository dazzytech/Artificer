using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

using Space.Segment;

namespace Space.Projectiles
{
    public class ProjectileController : WeaponController
    {
        #region CUSTOMIZABLE ATTRIBUTES

        [SerializeField]
        private float speed = 75f;

        #endregion

        #region CALCULATION VARIABLES

        private bool destroyed;

        private float currDistance;
        private Vector3 origTransPosition;

        #endregion

        #region MONOBEHAVOUR

        // Update is called once per frame
        void Update()
        {
            if (destroyed)
                return;

            if (Aggressor == null)
                Aggressor = ClientScene.FindLocalObject(_data.Self);

            // Retrieve list of colliders That we are about to intersect with
            RaycastHit2D[] hitList = Physics2D.RaycastAll(transform.position,
                _data.Direction, speed * Time.deltaTime, maskIgnore);

            // Loop through each and discover if damaging
            if (hitList.Length != 0)
            {
                foreach (RaycastHit2D hit in hitList)
                {
                    if (hit.transform == null || Aggressor == null)
                        return;
                    // Both client and host reach here
                    // however only the player that applies damage
                    if (hasAuthority)
                    {
                        // if successful hit then break out of the loop
                        if (ApplyDamage(hit))
                        {
                            TravelBullet(hit.point);
                            return;
                        }
                    }
                    else
                    {
                        if (!hit.transform.Equals(Aggressor.transform))
                        {
                            TravelBullet(hit.point);
                            return;
                        }
                    }
                }
            }

            // Place bullet update here as there is no differing types of bullet
            TravelBullet(Vector3.zero);
        }

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Creates the bullet projectile
        /// </summary>
        /// <param name="direction">Direction.</param>
        /// <param name="range">Range.</param>
        public override void CreateProjectile(WeaponData data)
        {
            base.CreateProjectile(data);

            currDistance = 0;
            origTransPosition = transform.position;

            CmdBuildFX(data);
        }

        #region FX

        [ClientRpc]
        public override void RpcBuildFX(WeaponData data)
        {
            Instantiate(Muzzle, transform.position,// + (data.Direction * 1f) * Time.deltaTime,
                        Quaternion.LookRotation(data.Direction));

            SoundController.PlaySoundFXAt
                (transform.position, MuzzleSound);
        }

        [ClientRpc]
        public override void RpcBuildHitFX(Vector2 hit, WeaponData data)
        {
            SoundController.PlaySoundFXAt
                        (transform.position, ImpactSound);

            // Compressed plasma will explode on impact
            Instantiate(Explode, hit, Quaternion.Euler(-_data.Direction));
        }

        #endregion

        #endregion

        #region PRIVATE UTILITES

        #region BULLET UPDATE

        /// <summary>
        /// Each bullet has shared behaviour FX
        /// So create trailing bullet projectile script
        /// </summary>
        private void TravelBullet(Vector3 affix)
        {
            if (affix == Vector3.zero)
                // move the transform in the bullet direction
                transform.Translate((_data.Direction * speed) * Time.deltaTime);
            else
                transform.position = affix;
        }

        /// <summary>
        /// If contacts a collider then 
        /// invokes the object's impact collider
        /// </summary>
        /// <param name="hit"></param>
        /// <returns></returns>
        private bool ApplyDamage(RaycastHit2D hit)
        {
            // Find the ship that fired the projectile
            GameObject aggressor = ClientScene.FindLocalObject(_data.Self);

            if (aggressor == null)
            {
                return false;
            }

            if (hit.transform.Equals(aggressor.transform))
            {
                return false;
            }

            CmdBuildHitFX(hit.point, _data);

            HitData hitD = new HitData();
            hitD.damage = _data.Damage;
            hitD.hitPosition = hit.point;
            hitD.originID = _data.Self;


            // retrieve impact controller
            // and if one exists make ship process hit
            ImpactCollider IC = hit.transform.GetComponent<ImpactCollider>();
            if (IC != null)
            {
                IC.Hit(hitD);
            }

            DestroyProjectileDelay();

            return true;
        }

        private void DestroyProjectileDelay()
        {
            destroyed = true;

            // Create destroy with delay of one sec
            Invoke("DestroyProjectile", 1f);
        }

        #endregion

        #endregion
    }
}

