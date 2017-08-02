using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Space.Projectiles;
using Space.Ship.Components.Attributes;
using Networking;
using Data.Space;
using Space.UI.Ship;
using Space.UI.Ship.Target;

namespace Space.Ship.Components.Listener
{
    public class LauncherListener : ComponentListener
    {
        LauncherAttributes _att;

        #region PUBLIC INTERACTION

        public override void Activate()
        {
            if (_att.readyToFire)
            {
                StartCoroutine("EngageDelay");

                StartCoroutine("LaunchRockets"); 
            }
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

        [Command]
        private void CmdBuildRocket(int prefabID, Vector3 shotOrigin,
            WeaponData wData, uint playerNetID)
        {
            GameObject Prefab = NetworkManager.singleton.spawnPrefabs[prefabID];

            GameObject GO = Instantiate(Prefab, shotOrigin, Quaternion.identity) as GameObject;

            // Projectile can run command to display self
            NetworkServer.SpawnWithClientAuthority(GO,
                SystemManager.Space.PlayerConn
                (new NetworkInstanceId(playerNetID)));

            GO.GetComponent<MissileController>().CreateProjectile(wData);
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
            Transform[] firePs = transform.Cast<Transform>().Where
                (c=>c.gameObject.tag == "Fire").ToArray();
            
            if(firePs.Length == 0)
            {
                Debug.Log("No fire point!");
                StopCoroutine("LaunchRockets");
                yield return null;
            }

            for(int i=0;i < _att.Rockets; i++)
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

                _att.readyToFire = false;

                WeaponData data = new WeaponData();
                data.Damage = _att.WeaponDamage;
                data.Direction = forward;
                data.Distance = -1;
                data.Self = _att.Ship.NetworkID;

                int prefabID = NetworkManager.singleton.spawnPrefabs.IndexOf(_att.ProjectilePrefab);

                // if targets are removed mid fire
                if (_att.Ship.TargetedShips.Count == 0)
                {
                    yield break;
                }

                ShipSelect targetShip = _att.Ship.TargetedShips[Random.Range(0,
                   _att.Ship.TargetedShips.Count)];

                Transform target = targetShip.TargetedComponents[Random.Range(0,
                   targetShip.TargetedComponents.Count)];

                NetworkInstanceId netTarget = target
                    .GetComponent<NetworkIdentity>().netId;

                if(netTarget.Value == 0)
                    netTarget = target.parent.GetComponent<NetworkIdentity>().netId;

                data.Target = netTarget;

                CmdBuildRocket(prefabID, shotOrigin,
                    data, SystemManager.Space.NetID);

                yield return new WaitForSeconds(_att.RocketDelay);
            }
        }

        #endregion
    }
}
