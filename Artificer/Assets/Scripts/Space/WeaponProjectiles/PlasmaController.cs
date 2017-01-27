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

        private Vector3 projPosition;
        private Vector3 origTransPosition;
        private float bulletStep;
        private float currDistance;
        private bool destroyed;

        #endregion

        #region MONOBEHAVIOUR

        void Awake()
        {
            _system = GetComponent<ParticleSystem>();
        }

        // Update is called once per frame
        void Update()
        {
            if (points == null || destroyed)
                return;

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

            CmdBuildFX(data);
        }

        #endregion

        #region PROJECTILE CREATION

        [ClientRpc]
        public override void RpcBuildFX(WeaponData data)
        {
            Instantiate(Muzzle, transform.position + (data.Direction * 1f) * Time.deltaTime,
                        Quaternion.LookRotation(data.Direction));

            SoundController.PlaySoundFXAt
                (transform.position, MuzzleSound);

            BuildProjectile(data);
        }

        [ClientRpc]
        public override void RpcBuildHitFX(Vector2 hit, WeaponData data)
        {
            SoundController.PlaySoundFXAt
                        (transform.position, ImpactSound);

            // Compressed plasma will explode on impact
            Instantiate(Explode, transform.position, Quaternion.Euler(-_data.Direction));
        }

        public void BuildProjectile(WeaponData data)
        {
            transform.Translate(_data.Direction * Time.deltaTime);

            GetComponent<ParticleSystem>().simulationSpace
                = ParticleSystemSimulationSpace.Local;
            points = new ParticleSystem.Particle[pointCount];

            // First create the main projectile particle
            projPosition = Vector3.zero;

            currDistance = 0;

            destroyed = false;

            points[pointCount - 1].position = projPosition;
            points[pointCount - 1].color = new Color(1f, 1f, 1f, 1f);
            points[pointCount - 1].size = .2f;

            // Figure out a bullet step
            bulletStep = trailLength / (pointCount - 1);

            for (int i = 0; i < pointCount - 1; i++)
            {
                points[i].position = projPosition;

                points[i].color = Color.Lerp(points[pointCount - 1].color, 
                    new Color(0.66f, 0.164f, 0.015f, 1f), bulletStep * i);

                points[i].size = points[pointCount - 1].size - 
                    (points[pointCount - 1].size / pointCount) * i;
            }

            GetComponent<ParticleSystem>().SetParticles(points, points.Length);
            origTransPosition = transform.position;
        }

        #endregion

        #region PROJECTILE UPDATES

        private void UpdateFusion()
        {
            FusionCurve();

            TravelBullet();

            RaycastHit2D[] hitList = Physics2D.RaycastAll(transform.position, 
                -_data.Direction, speed * Time.deltaTime, maskIgnore);

            if (hitList.Length != 0)
            {
                foreach (RaycastHit2D hit in hitList)
                {
                    // if successful hit then break out of the loop
                    if(ApplyDamage(hit))
                        break;
                }
            }
        }

        private void UpdateProjectile()
        {
            TravelBullet();

            RaycastHit2D[] hitList = Physics2D.RaycastAll(transform.position,
                -_data.Direction, speed * Time.deltaTime, maskIgnore);

            if (hitList.Length != 0)
            {
                foreach (RaycastHit2D hit in hitList)
                {
                    // if successful hit then break out of the loop
                    if (ApplyDamage(hit))
                        break;
                }
            }
        }

        private void UpdateCompressed()
        {
            TravelBullet();

            RaycastHit2D[] hitList = Physics2D.RaycastAll(transform.position, -_data.Direction, speed * Time.deltaTime, maskIgnore);

            if (hitList.Length != 0)
            {
                foreach (RaycastHit2D hit in hitList)
                {
                    if (hit.transform.Equals(_data.Self))
                    {
                        continue;
                    }

                    CmdBuildHitFX(hit.point, _data);
                    
                    RaycastHit2D[] colliderList = Physics2D.CircleCastAll(transform.position, radius, Vector2.up, 0, maskIgnore);
                    foreach (RaycastHit2D hitB in colliderList)
                    {
                        if (hit.transform != null)
                        {
                            HitData hitD = new HitData();
                            hitD.damage = _data.Damage;
                            hitD.hitPosition = hitB.point;
                            hitD.radius = radius;
                            hitD.originID = _data.Self;
                            hitB.transform.gameObject.SendMessage("HitArea", hitD, SendMessageOptions.DontRequireReceiver);
                        }
                        DestroyProjectile();
                        break;
                    }
                }
            }
        }

        #region SHARED

        /// <summary>
        /// Each bullet has shared behaviour FX
        /// So create trailing bullet projectile script
        /// </summary>
        private void TravelBullet()
        {
            // move the transform in the bullet direction
            transform.Translate((_data.Direction * speed) * Time.deltaTime);

            float travel = ((transform.position - origTransPosition).sqrMagnitude);
            travel *= bulletStep;
            currDistance += travel;

            if (travel > bulletStep)
                travel = bulletStep;

            for (int i = 0; i < pointCount - 1; i++)
            {
                points[i].position = projPosition - _data.Direction * (travel * i);
            }

            GetComponent<ParticleSystem>().SetParticles(points, points.Length);

            if(currDistance > _data.Distance)
                DestroyProjectileDelay();
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

        #endregion

        #region PRIVATE

        /// <summary>
        /// curves the plasma bolt to an object.
        /// </summary>
        /// <returns><c>true</c>, if curve was successful, <c>false</c> otherwise.</returns>
        private bool FusionCurve()
        {
            // Get home alignment
            if (_data.Self == null)
            {
                Destroy(gameObject);
                return false;
            }
            string align = NetworkServer.FindLocalObject(_data.Self).GetComponent<ShipAttributes>().AlignmentLabel;
            // use followradius to find an object within range
            RaycastHit2D collider = Physics2D.CircleCast(transform.position, followRadius, Vector2.up, 0, maskIgnore);

            if (collider.transform != null)
                if (!collider.transform.Equals(_data.Self))
                {
                    // Retreive ship info
                    ShipAttributes sa = collider.transform.GetComponent<ShipAttributes>();
                    if (sa != null)
                        if (sa.AlignmentLabel == "Enemy" && align != "Enemy" ||
                           sa.AlignmentLabel != "Enemy" && align == "Enemy")
                        {
                            Vector3 newDir = collider.transform.position - transform.position;
                            newDir.z = 0;
                            Vector3 dir = Vector3.Slerp(_data.Direction, newDir, Time.deltaTime * (followTurnSpeed * (10f - newDir.magnitude)));
                            transform.Translate((dir.normalized * speed) * Time.deltaTime);
                            _data.Direction = dir;

                            return true;
                        }
                }
            return false;
        }

        #endregion

        public void DestroyProjectileDelay()
        {
            destroyed = true;

            points = null;

            _system.Clear();

            // Create destroy with delay of one sec
            Invoke("DestroyProjectile", 1f);
        }

        #endregion
    }
}
