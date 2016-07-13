using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ShipComponents
{
    public class LauncherListener : ComponentListener
    {
        LauncherAttributes _attr;

        void Awake()
        {
            ComponentType = "Launchers";
            _attr = GetComponent<LauncherAttributes>();
        }
        
        void Start ()
        {
            base. SetRB();
            _attr.Ship = transform.parent.GetComponent<ShipAttributes>();
            _attr.readyToFire = true;

            if (_attr.AutoTarget)
                StartCoroutine("FindArcTargets");
        }

        void Update()
        {

        }
        
        public override void Activate()
        {
            if (_attr.readyToFire && _attr.Ship.Targets.Count != 0)
            {
                StartCoroutine("EngageDelay");

                StartCoroutine("LaunchRockets"); 
            }
        }
        
        public override void Deactivate()
        {
            
        }
        
        private IEnumerator EngageDelay()
        {
            yield return new WaitForSeconds (_attr.WeaponDelay);
            _attr.readyToFire = true;
            yield return null;
        }
        
        public void SetTriggerKey(string key)
        {
            _attr.TriggerKey = Control_Config
                .GetKey(key, "ship");
        }
        
        public void SetCombatKey(string key)
        {
            _attr.CombatKey = Control_Config
                .GetKey(key, "combat");
        }

        /// <summary>
        /// Finds targets within the targeters firing arc.
        /// </summary>
        private IEnumerator FindArcTargets()
        {
            while (true)
            {
                RaycastHit2D[] hits =
                Physics2D.CircleCastAll(transform.position, _attr.AttackRange, Vector2.zero, 0, 1);
            
                foreach (RaycastHit2D hit in hits)
                {
                    // Only auto target heads of ships
                    // while no target grouping
                    if (_attr.Ship.Targets.Contains(hit.collider.transform)
                        || hit.collider.transform.tag != "Head")
                        continue;

                    // detect if enemy
                    if (_attr.Ship.AlignmentLabel == "Player")
                    {
                        // use player relations
                        if (hit.transform.name
                            != "Enemy")
                            continue;
                    } else
                    {
                        // ai relation
                        if (_attr.Ship.AlignmentLabel == "Enemy" && 
                            hit.transform.name == "Enemy")
                            continue;

                        if (_attr.Ship.AlignmentLabel == "Friendly" && 
                            hit.transform.name != "Enemy" || hit.transform.tag == "Station")
                            continue;
                    }

                    ComponentListener comp = hit.collider.
                    transform.GetComponent<ComponentListener>();

                    if (comp != null)
                    {
                        // check not self targetting
                        if (!_attr.Ship.Components.Contains(comp))
                        {
                                _attr.Ship.Targets.Add(hit.collider.transform);
                        }
                    }

                    yield return null;
                }
                yield return null;
            }
            yield return null;
        }

        private IEnumerator LaunchRockets()
        {
            Transform[] firePs = transform.Cast<Transform>().Where
                (c=>c.gameObject.tag == "Fire").ToArray();
            
            if(firePs.Length == 0)
            {
                Debug.Log("No fire point!");
                StopCoroutine("LaunchRockets");
                yield return null;
            }

            for(int i=0;i < _attr.Rockets; i++)
            {
                float rotation=0f;
                if(i%2==0)
                {
                    rotation=Random.Range(-45f, -10f);
                }
                else
                {
                    rotation=Random.Range(10f, 45f);
                }

                Vector3 shotOrigin = firePs[Random.Range(0, firePs.Length)].position;
                    
                Vector3 forward = transform.TransformDirection (Vector3.up);
                    
                GameObject projectile;

                _attr.readyToFire = false;

                WeaponData data = new WeaponData();
                data.Damage = _attr.WeaponDamage;
                data.Direction = forward;
                data.Distance = _attr.WeaponRange;
                data.Self = this.GetComponentInParent<NetworkBehaviour>().netId;

                // if targets are removed mid fire
                if (_attr.Ship.Targets.Count == 0)
                {
                    StopCoroutine("LaunchRockets");
                    yield return null;
                }

                data.Target = _attr.Ship.Targets[Random.Range(0, _attr.Ship.Targets.Count)];

                StartCoroutine("EngageDelay");

                projectile = Instantiate(_attr.ProjectilePrefab, shotOrigin,
                                                Quaternion.Euler(0f, 0f, rotation)) as GameObject;

                projectile.SendMessage("CreateMissile", data, 
                                           SendMessageOptions.RequireReceiver);

                yield return new WaitForSeconds(_attr.RocketDelay);
            }
        }
    }
}
