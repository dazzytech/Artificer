using UnityEngine;
using System.Collections;
using Space;

public class ProjectileController : WeaponController
{
    // bullet vars
    public float speed = 100f;
   
    //time
    float seconds;
    float currSeconds;
    
    // Update is called once per frame
    void Update ()
    {
        // move the transform in the bullet direction
        transform.Translate((_data.Direction * speed) * Time.deltaTime);

        currSeconds += Time.deltaTime;
        if (currSeconds >= seconds)
            DestroyProjectile();

        RaycastHit2D[] hitList = Physics2D.RaycastAll(transform.position, -_data.Direction, speed*Time.deltaTime, maskIgnore);
        
        if (hitList.Length != 0)
        {
            foreach(RaycastHit2D hit in hitList)
            {
                if(hit.transform.Equals(_data.Self))
                {
                    continue;
                }

                SoundController.PlaySoundFXAt
                    (transform.position, ImpactSound);

                HitData hitD = new HitData();
                hitD.damage = _data.Damage;
                hitD.hitPosition = hit.point;
                hitD.originID = _data.Self;
                hit.transform.gameObject.SendMessage("Hit", hitD, SendMessageOptions.DontRequireReceiver);
                DestroyProjectile();
                Instantiate(Explode, hit.point, Quaternion.Euler(_data.Direction));

                break;
            }
        }
    }

    /// <summary>
    /// Creates the bullet appearance then.
    /// 
    /// </summary>
    /// <param name="direction">Direction.</param>
    /// <param name="range">Range.</param>
    public override void CreateProjectile(WeaponData data)
    {
        base.CreateProjectile(data);
        Instantiate(Muzzle, transform.position + (_data.Direction * 1f) 
                    * Time.deltaTime, Quaternion.LookRotation(_data.Direction));

        transform.Translate(_data.Direction*Time.deltaTime);

        seconds = data.Distance / speed;
        currSeconds = 0;
    }
}

