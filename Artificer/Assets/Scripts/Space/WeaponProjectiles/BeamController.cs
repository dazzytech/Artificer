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
        // enumerator for deteriming the type of beam
        public enum BeamType
        {
            FIZZLE,
            FADE,
        }

        // Particles
        private ParticleSystem _system;
        private ParticleSystem.Particle[] _points;

        // Customizable variables
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

        // Calculation Variables
        private int pointCount = 0;
        private float allotedTime;

        void Awake()
        {
            _system = GetComponent<ParticleSystem>();
        }

        // Update is called once per frame
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

            if (_system != null)
                _system.SetParticles(_points, _points.Length);
        }

        [Server]
        public override void CreateProjectile(WeaponData data)
        {
            if (!isServer)
                return;

            // Damage whatever is hit
            ApplyDamage(data);

            // Build projectile item
            RpcBuildFX(data);

            // Create destroy with delay
            Invoke("DestroyProjectile", seconds);
        }

        // create a clientrpc for showing particles on clients?
        [ClientRpc]
        public void RpcBuildFX(WeaponData data)
        {
            // Add data here too
            _data = data;

            // Local muzzle visible
            Instantiate(Muzzle, transform.position, Quaternion.Euler(data.Direction));

            SoundController.PlaySoundFXAt
                (transform.position, MuzzleSound);

            switch (beamType)
            {
                case BeamType.FADE:
                    allotedTime = 0f;
                    BuildBeam();
                    break;
                case BeamType.FIZZLE:
                    BuildBeam();
                    break;
            }
        }

        /// <summary>
        /// Server functions for applying damage for
        /// colliders within the 
        /// </summary>
        [Server]
        public void ApplyDamage(WeaponData data)
        {
            _data = data;

            // Find the ship that fired the projectile
            GameObject aggressor = NetworkServer.FindLocalObject(_data.Self);

            RaycastHit2D[] hitList = Physics2D.RaycastAll(transform.position, _data.Direction, _data.Distance, maskIgnore);

            if (hitList.Length != 0)
            {
                foreach (RaycastHit2D hit in hitList)
                {
                    if (hit.transform.Equals(aggressor.transform))
                    {
                        continue;
                    }

                    // Show hit decals on clients
                    RpcBuildHitFX(hit.point);

                    // create hitdata
                    HitData hitD = new HitData();
                    hitD.damage = _data.Damage;
                    hitD.hitPosition = hit.point;
                    hitD.originID = _data.Self;

                    // retrieve impact controller
                    // and if one exists make ship process hit
                    ImpactCollider IC = hit.transform.GetComponent<ImpactCollider>();
                    if (IC != null)
                    {
                        Debug.Log("Object Hit: " + hit.transform.name);
                        IC.Hit(hitD);

                        _data.Distance = Vector2.Distance(transform.position, hit.point);

                        // stop searching for hit points
                        return;
                    }
                }
            }
        }

        [ClientRpc]
        public void RpcBuildHitFX(Vector2 hit)
        {
            SoundController.PlaySoundFXAt
                        (hit, ImpactSound);

            // display hit on the server
            Instantiate(Explode, hit, Quaternion.Euler(_data.Direction));
        }

        /// <summary>
        /// Builds the ray from the laser origin and then
        /// moves the origin to within the camera if is outside
        /// cropping the laser to within main viewport
        /// </summary>
        /// <param name="mag">Mag.</param>
        /// <param name="origin">Origin.</param>
        /*public void CropToCamera(out float mag, out Vector3 origin)
        {
            Vector3 hitPoint = _data.Direction * _data.Distance;

            mag = _data.Distance;

            //float camDistance;
            origin = transform.position;

            Ray ray = new Ray(transform.position, _data.Direction);

            Bounds b = 
                CameraExtensions.OrthographicBounds
                    (Camera.main);

            if (b.IntersectRay(ray, out camDistance))
            {
                if(camDistance < mag && !b.Contains(transform.position))
                {
                    transform.Translate(_data.Direction * (camDistance+1));
                    // using new transform
                    origin = transform.position - _data.Direction;
                    mag = (origin -
                           hitPoint).magnitude;
                }
            }
        }*/

        /// <summary>
        /// Builds a beam
        /// </summary>
        /// <param name="mag">Mag.</param>
        /// <param name="origin">Origin.</param>
        public void BuildBeam()
        {
            // Prepare to draw beam
            float mag = _data.Distance;
            Vector3 origin = transform.position;

            pointCount = (int)mag * pointsPerUnit;

            float bulletStep = mag / pointCount;

            _points = new ParticleSystem.Particle[pointCount];
            _system = GetComponent<ParticleSystem>();

            for (int i = 0; i < pointCount; i++)
            {
                Vector3 position = origin + _data.Direction * (i * bulletStep);
                position.z = 0;
                _points[i].position = position;
                _points[i].color = particleColor;
                _points[i].size = particleSize;
            }
            _system.SetParticles(_points, _points.Length);
        }

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
    }
}

