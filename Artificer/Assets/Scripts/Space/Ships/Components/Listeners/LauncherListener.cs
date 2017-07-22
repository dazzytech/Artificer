using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Space.Projectiles;
using Space.Ship.Components.Attributes;
using Networking;
using Data.Space;

namespace Space.Ship.Components.Listener
{
    public class LauncherListener : ComponentListener
    {
        LauncherAttributes _att;

        #region PUBLIC INTERACTION

        public override void Activate()
        {
            /*if (_attr.readyToFire && _attr.Ship.Targets.Count != 0)
            {
                StartCoroutine("EngageDelay");

                StartCoroutine("LaunchRockets"); 
            }*/
        }

        #endregion

        #region PRIVATE UTILITIES

        protected override void InitializeComponent()
        {
            base.InitializeComponent();

            ComponentType = "Launchers";
            _att = GetComponent<LauncherAttributes>();

            if (hasAuthority)
            {
                _att.AutoTarget = _att.Data.AutoLock;

                _att.readyToFire = true;

                if (_att.AutoTarget)
                    StartCoroutine(FindArcTargets
                        (_att.AttackRange, _att.MinAngle, _att.MaxAngle, _att.Ship));
            }
        }

        #endregion

        #region COROUTINE

        /// <summary>
        /// Once fired the coroutine will
        /// reenable the weapon after an alotted time
        /// </summary>
        /// <returns></returns>
        private IEnumerator EngageDelay()
        {
            yield return new WaitForSeconds (_att.WeaponDelay);
            _att.readyToFire = true;
            yield return null;
        }
        
        private IEnumerator LaunchRockets()
        {
            // temp
            yield break;
            /*Transform[] firePs = transform.Cast<Transform>().Where
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
                data.Self = _attr.Ship.instID;

                int prefabIndex = NetworkManager.singleton.spawnPrefabs.IndexOf(_attr.ProjectilePrefab);

                // if targets are removed mid fire
                if (_attr.Ship.Targets.Count == 0)
                {
                    StopCoroutine("LaunchRockets");
                    yield return null;
                }

                //Transform Target = _attr.Ship.Targets[Random.Range(0,
                 //   _attr.Ship.Targets.Count)];

                NetworkInstanceId netTarget = Target
                    .GetComponent<NetworkIdentity>().netId;

                if(netTarget.Value == 0)
                    netTarget = Target.parent.GetComponent<NetworkIdentity>().netId;

                data.Target = netTarget;

                StartCoroutine("EngageDelay");

                ProjectileBuildMessage msg = new ProjectileBuildMessage();
                msg.PrefabIndex = prefabIndex;
                msg.Position = shotOrigin;
                msg.WData = data;
                msg.shooterID = SystemManager.Space.ID;

                // Sendmsg to game to spawn projectile
                SystemManager.singleton.client.Send((short)MSGCHANNEL.BUILDPROJECTILE, msg);

                yield return new WaitForSeconds(_attr.RocketDelay);
            }*/
        }

        #endregion
    }
}
