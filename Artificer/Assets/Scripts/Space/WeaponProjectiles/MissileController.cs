using UnityEngine;
using System.Collections;

using Space.Segment;

namespace Space.Projectiles
{
    public class MissileController : WeaponController
    {
        public float MissileSpeed = 5f;
        public float MissileFuse;

        float turn = 2.5f;
        float lastTurn = 0f;
        public float Radius = 3f;

        protected Rigidbody2D rocketRigidbody;

        // Use this for initialization
        void Awake()
        {
            rocketRigidbody = GetComponent<Rigidbody2D>();
        }

        void Start()
        {
            Invoke("Trigger", MissileFuse);
        }

        void FixedUpdate()
        {
            if (_data.Target == null)
                return;

            Quaternion newRotation = Quaternion.LookRotation(transform.position - _data.Target.position, Vector3.forward);
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
                if (!hit.transform.Equals(_data.Self))
                {
                    Trigger();
                }
            }
        }

        public override void Trigger()
        {
            CancelInvoke("Trigger");

            SoundController.PlaySoundFXAt
                (transform.position, ImpactSound);

            Instantiate(Explode, transform.position, Quaternion.identity);

            RaycastHit2D[] colliderList = Physics2D.CircleCastAll(transform.position, Radius, Vector2.up, 0, maskIgnore);
            foreach (RaycastHit2D hit in colliderList)
            {
                if (hit.transform.Equals(_data.Self))
                {
                    continue;
                }

                if (hit.transform != null)
                {
                    HitData hitD = new HitData();
                    hitD.damage = _data.Damage;
                    hitD.radius = Radius;
                    hitD.hitPosition = hit.point;
                    hitD.originID = _data.Self;
                    hit.transform.gameObject.SendMessage("HitArea", hitD, SendMessageOptions.DontRequireReceiver);
                }
            }
            DestroyProjectile();
        }
    }
}

