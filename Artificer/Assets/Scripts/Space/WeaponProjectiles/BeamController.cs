using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

using Space.Segment;

namespace Space.Projectiles
{
    /// <summary>
    /// Attached to the beam prefab and 
    /// is used to create the beam effect.
    /// </summary>
    public class BeamController : WeaponController
    {
        public enum BeamType
        {
            FIZZLE,
            FADE,
        }

        #region PARTICLES

        private ParticleSystem _system;
        private ParticleSystem.Particle[] _points;

        #endregion

        #region CUSTOMIZABLE ATTRIBUTES
        
        [SerializeField]
        private int pointsPerUnit = 8;
        [SerializeField]
        private float seconds = .3f;
        [SerializeField]
        private Color particleColor = new Color(1f, 0f, 0f, 1f);
        [SerializeField]
        private Color startFadeColor = new Color(.5f, 0f, 0f, 1f);
        [SerializeField]
        private Color fadeColor = Color.black;
        [SerializeField]
        private float fadeSpeed = 0.01f;
        [SerializeField]
        private float particleSize = 0.1f;
        [SerializeField]
        private float fizzleDistance = 0.05f;
        [SerializeField]
        private BeamType beamType = BeamType.FADE;

        #endregion

        #region CALCULATION VARIABLES
        
        private int pointCount = 0;
        private float allotedTime;

        #endregion

        #region MONOBEHAVIOUR

        void Awake()
        {
            _system = GetComponent<ParticleSystem>();
        }
        
        void Update()
        {
            switch (beamType)
            {
                case BeamType.FADE:
                    UpdateFade();
                    break;
                case BeamType.FIZZLE:
                    UpdateFizzle();
                    break;
            }

            if (_system != null && _points != null)
                _system.SetParticles(_points, _points.Length);
        }

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Called from weapon controller 
        /// </summary>
        /// <param name="data"></param>
        public override void CreateProjectile(WeaponData data)
        {
            base.CreateProjectile(data);

            // Damage whatever is hit
            ApplyDamage(data);

            // Create destroy with delay
            Invoke("DestroyProjectile", seconds);
        }

        /// <summary>
        /// Server functions for applying damage for
        /// colliders within the 
        /// </summary>
        public void ApplyDamage(WeaponData data)
        {
            // Find the ship that fired the projectile
            GameObject aggressor = ClientScene.FindLocalObject(data.Self);

            RaycastHit2D[] hitList = Physics2D.RaycastAll(transform.position, data.Direction, data.Distance, maskIgnore);

            if (hitList.Length != 0)
            {
                foreach (RaycastHit2D hit in hitList)
                {
                    if (hit.transform.Equals(aggressor.transform))
                    {
                        continue;
                    }

                    // Show hit decals on clients
                    CmdBuildHitFX(hit.point, data);

                    // create hitdata
                    HitData hitD = new HitData();
                    hitD.damage = data.Damage;
                    hitD.hitPosition = hit.point;
                    hitD.originID = data.Self;

                    // retrieve impact controller
                    // and if one exists make ship process hit
                    ImpactCollider IC = hit.transform.GetComponent<ImpactCollider>();
                    if (IC != null)
                    {
                        IC.Hit(hitD);

                        data.Distance = Vector2.Distance(transform.position, hit.point);
                        break;
                    }
                }
            }

            CmdBuildFX(data);
        }

        #endregion

        #region PROJECTILE CREATION

        /// <summary>
        /// Builds the beam and muzzle on the client
        /// </summary>
        /// <param name="data"></param>
        [ClientRpc]
        public override void RpcBuildFX(WeaponData data)
        {
            // Local muzzle visible
            Instantiate(Muzzle, transform.position, Quaternion.Euler(data.Direction));

            SoundController.PlaySoundFXAt
                (transform.position, MuzzleSound);

            switch (beamType)
            {
                case BeamType.FADE:
                    allotedTime = 0f;
                    BuildBeam(data);
                    break;
                case BeamType.FIZZLE:
                    BuildBeam(data);
                    break;
            }
        }

       
        /// <summary>
        /// Creates hit decals whem projectile hits a collider
        /// </summary>
        /// <param name="hit"></param>
        [ClientRpc]
        public override void RpcBuildHitFX(Vector2 hit, WeaponData data)
        {
            SoundController.PlaySoundFXAt
                        (hit, ImpactSound);

            // display hit on the server
            Instantiate(Explode, hit, Quaternion.Euler(data.Direction));
        }


        /// <summary>
        /// Builds a beam
        /// </summary>
        /// <param name="mag">Mag.</param>
        /// <param name="origin">Origin.</param>
        public void BuildBeam(WeaponData data)
        {
            // Prepare to draw beam
            float mag = data.Distance;
            Vector3 origin = transform.position;

            pointCount = (int)mag * pointsPerUnit;

            float bulletStep = mag / pointCount;

            _points = new ParticleSystem.Particle[pointCount];
            _system = GetComponent<ParticleSystem>();

            for (int i = 0; i < pointCount; i++)
            {
                Vector3 position = origin + data.Direction * (i * bulletStep);
                position.z = 0;
                _points[i].position = position;
                _points[i].color = particleColor;
                _points[i].size = particleSize;
            }
            _system.SetParticles(_points, _points.Length);
        }

        #endregion

        #region PROJECTILE UPDATES

        /// <summary>
        /// Arranges the particles to appear to fizzle away
        /// </summary>
        public void UpdateFizzle()
        {
            for (int i = 0; i < pointCount; i++)
            {
                Vector2 position = _points[i].position +
                    Random.insideUnitSphere * fizzleDistance;
                _points[i].color = Color.Lerp(_points[i].color, fadeColor, fadeSpeed);
                _points[i].position = position;
            }
        }

        /// <summary>
        /// Arranges the particles to appear to fade
        /// </summary>
        public void UpdateFade()
        {
            allotedTime += Time.deltaTime;

            for (int i = 0; i < pointCount; i++)
            {
                if (allotedTime < seconds * .3f)
                    _points[i].color = Color.Lerp(_points[i].color, startFadeColor, fadeSpeed);
                else
                    _points[i].color = Color.Lerp(_points[i].color, fadeColor, fadeSpeed);
            }
        }

        #endregion
    }
}

