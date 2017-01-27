using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

namespace Space.Projectiles
{
    [Serializable]
    public struct WeaponData
    {
        public Vector3 Direction;
        public float Distance;
        public float Damage;
        public Transform Target;
        public NetworkInstanceId Self;
    }

    public class WeaponController : NetworkBehaviour
    {
        [SyncVar]
        public WeaponData _data;

        public GameObject Explode;
        public GameObject Muzzle;

        public AudioClip MuzzleSound;

        public AudioClip ImpactSound;

        // ignore collectable layer
        public LayerMask maskIgnore;

        public virtual void CreateProjectile(WeaponData data)
        {
            CmdAssignData(data);
        }

        [Command]
        public virtual void CmdAssignData(WeaponData data)
        {
            _data = data;
        }

        [Command]
        public void CmdBuildFX(WeaponData data)
        {
            RpcBuildFX(data);
        }

        [ClientRpc]
        public virtual void RpcBuildFX(WeaponData data)
        { }

        [Command]
        public void CmdBuildHitFX(Vector2 hit, WeaponData data)
        {
            RpcBuildHitFX(hit, data);
        }

        [ClientRpc]
        public virtual void RpcBuildHitFX(Vector2 hit, WeaponData data)
        {
        }

        public virtual void CreateMissile(WeaponData data)
        {
           // SoundController.PlaySoundFXAt
                //(transform.position, MuzzleSound);
        }

        public virtual void Trigger()
        { }

        public virtual void DestroyProjectile()
        {
            CmdDestroyProjectile();
        }

        [Command]
        private void CmdDestroyProjectile()
        {
            NetworkServer.UnSpawn(this.gameObject);
            Destroy(this.gameObject);
        }
    }
}

