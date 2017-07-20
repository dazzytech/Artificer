using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

using Space.Segment;
using Space.Ship;

namespace Space.Projectiles
{
    public class PlasmaController : WeaponController
    {
        // The three different plasma types behave slightly different
        public enum PlasmaType { DEFAULT, COMPRESSED, FUSION };

        #region PARTICLES

        private ParticleSystem _system;
        private ParticleSystem.Particle[] points;

        #endregion

        #region CUSTOMIZABLE ATTRIBUTES

        [SerializeField]
        private float radius = 0f;
        [SerializeField]
        private float followRadius = 0f;
        [SerializeField]
        private float followTurnSpeed = 0f;
        [SerializeField]
        private float speed = 75f;
        [SerializeField]
        private float trailLength = 1f;
        [SerializeField]
        private int pointCount = 50;
        [SerializeField]
        private PlasmaType PType;

        #endregion

        #region CALCULATION VARIABLES

        private Vector3 origTransPosition;
        private float bulletStep;
        private float currDistance;

        [SyncVar]
        private bool destroyed;

        #endregion

        #region MONOBEHAVIOUR

        void Awake()
        {
            BuildProjectile();
        }

        // Update is called once per frame
        void Update()
        {
            if (destroyed)
                return;

            if(Aggressor == null)
                Aggressor = ClientScene.FindLocalObject(_data.Self);

            // if plasmatype is fusion then detect any transforms within a certain radius
            if (PType == PlasmaType.FUSION)
            {
                UpdateFusion();
            }
            else if(PType == PlasmaType.COMPRESSED)
            {
                UpdateCompressed();
            }
            else
            {
                UpdateProjectile();
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

            RpcBuildFX(data);
        }

        #region FX

        [ClientRpc]
        public override void RpcBuildFX(WeaponData data)
        {
            Instantiate(Muzzle, transform.position + (data.Direction * 1f) * Time.deltaTime,
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
            Instantiate(Explode, transform.position, Quaternion.Euler(-_data.Direction));
        }

        #endregion

        #endregion

        #region PRIVATE UTILITY

        /// <summary>
        /// Initialized the particle system 
        /// and init calculation variables on all clients
        /// </summary>
        private void BuildProjectile()
        {
            _system = GetComponent<ParticleSystem>();
            _system.simulationSpace
                = ParticleSystemSimulationSpace.Local;

            points = new ParticleSystem.Particle[pointCount];

            points[pointCount - 1].position = Vector3.zero;
            points[pointCount - 1].color = new Color(1f, 1f, 1f, 1f);
            points[pointCount - 1].size = .2f;

            currDistance = 0;

            destroyed = false;

            // Figure out a bullet step
            bulletStep = trailLength / (pointCount - 1);

            for (int i = 0; i < pointCount - 1; i++)
            {
                points[i].position = Vector3.zero;

                points[i].color = Color.Lerp(points[pointCount - 1].color,
                    new Color(0.66f, 0.164f, 0.015f, 1f), bulletStep * i);

                points[i].size = points[pointCount - 1].size -
                    (points[pointCount - 1].size / pointCount) * i;
            }

            GetComponent<ParticleSystem>().SetParticles(points, points.Length);
            origTransPosition = transform.position;
        }

        #region PROJECTILE UPDATES

        #region FUSION

        private void UpdateFusion()
        {
            FusionCurve();

            RaycastHit2D[] hitList = Physics2D.RaycastAll(transform.position,
                _data.Direction, speed * Time.deltaTime, maskIgnore);

            if (hitList.Length != 0)
            {
                foreach (RaycastHit2D hit in hitList)
                {
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

            TravelBullet(Vector3.zero);
        }

        /// <summary>
        /// curves the plasma bolt to an object.
        /// </summary>
        /// <returns><c>true</c>, if curve was successful, <c>false</c> otherwise.</returns>
        private bool FusionCurve()
        {
            if (Aggressor == null)
            {
                //DestroyProjectileDelay();
                return false;
            }

            // use follow radius to find an object within range
            RaycastHit2D collider = Physics2D.CircleCast(transform.position, followRadius, Vector2.up, 0, maskIgnore);

            if (collider.transform != null)
                if (!collider.transform.Equals(Aggressor))
                {
                    if (collider.transform.tag == "Enemy")
                    {
                        Vector3 newDir = (collider.transform.position - transform.position).normalized;
                        newDir.z = 0;

                        if (Vector3.Angle(collider.transform.position, transform.position) < 1)
                            _data.Direction = newDir;
                        else
                            _data.Direction = Vector3.Lerp(_data.Direction, newDir, Time.deltaTime * followTurnSpeed).normalized;

                        return true;
                    }
                }
            return false;
        }

        #endregion

        private void UpdateProjectile()
        {
            RaycastHit2D[] hitList = Physics2D.RaycastAll(transform.position,
                _data.Direction, speed * Time.deltaTime, maskIgnore);

            if (hitList.Length != 0)
            {
                foreach (RaycastHit2D hit in hitList)
                {
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

            TravelBullet(Vector3.zero);
        }

        private void UpdateCompressed()
        {
            RaycastHit2D[] hitList = Physics2D.RaycastAll(transform.position, _data.Direction,
                speed * Time.deltaTime, maskIgnore);

            if (hitList.Length != 0)
            {
                foreach (RaycastHit2D hit in hitList)
                {
                    // Both client and host reach here
                    // however only the player that applies damage
                    if (hasAuthority)
                    {
                        // if successful hit then break out of the loop
                        if (ApplyDamageArea(hit))
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
            TravelBullet(Vector3.zero);
        }

        #endregion

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

            float travel = ((transform.position - origTransPosition).sqrMagnitude);
            travel *= bulletStep;
            currDistance += travel;

            if (travel > bulletStep)
                travel = bulletStep;

            for (int i = 0; i < pointCount - 1; i++)
            {
                points[i].position = -_data.Direction * (travel * i);
            }

            GetComponent<ParticleSystem>().SetParticles(points, points.Length);

            if (currDistance > _data.Distance * _data.Distance)
                DestroyProjectileDelay();
        }

        #endregion

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

            if (hit.transform.Equals(Aggressor.transform))
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

        private bool ApplyDamageArea(RaycastHit2D hit)
        {
            if (hit.transform.Equals(Aggressor.transform))
            {
                return false;
            }

            CmdBuildHitFX(hit.point, _data);

            HitData hitD = new HitData();
            hitD.damage = _data.Damage;
            // maybe this should be center of impact
            hitD.hitPosition = hit.point;
            hitD.radius = radius;
            hitD.originID = _data.Self;

            RaycastHit2D[] colliderList = Physics2D.CircleCastAll(transform.position, radius, Vector2.up, 0, maskIgnore);
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

            DestroyProjectileDelay();

            return true;
        }

        /// <summary>
        /// Stops the bullet functionality and destroys
        /// bullet after trail dies
        /// </summary>
        public void DestroyProjectileDelay()
        {
            destroyed = true;

            _system.Clear();

            // Create destroy with delay of one sec
            Invoke("DestroyProjectile", 1f);
        }

        #endregion
    }
}
