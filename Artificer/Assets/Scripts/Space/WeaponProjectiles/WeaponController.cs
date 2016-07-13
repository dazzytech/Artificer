using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public struct WeaponData
{
    public Vector3 Direction;
    public float Distance;
    public float Damage;
    public Transform Target;
    public NetworkInstanceId Self;
}

public class WeaponController: NetworkBehaviour
{
    [SyncVar]
    protected WeaponData _data;

    public GameObject Explode;
    public GameObject Muzzle;

    public AudioClip MuzzleSound;

    public AudioClip ImpactSound;

    // ignore collectable layer
    public LayerMask maskIgnore;

    public virtual void CreateProjectile(WeaponData data)
    {}

    public virtual void CreateMissile(WeaponData data)
    {
        _data = data;
        
        SoundController.PlaySoundFXAt
            (transform.position, MuzzleSound);
    }

    public virtual void Trigger()
    {}

    public void DestroyProjectile()
    {
        NetworkServer.UnSpawn(this.gameObject);
        Destroy(this.gameObject);
    }
}

