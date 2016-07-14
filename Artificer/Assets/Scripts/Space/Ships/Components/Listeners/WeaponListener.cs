using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Linq;

using Space.Projectiles;
using Space.Ship;
using Space.Ship.Components.Attributes;

namespace Space.Ship.Components.Listener
{
    public class WeaponListener : ComponentListener 
    {
    	WeaponAttributes _attr;
    	
        void Awake()
    	{
            ComponentType = "Weapons";
    		_attr = GetComponent<WeaponAttributes>();
    	}
    	
    	void Start ()
    	{
            base. SetRB();
    		_attr.readyToFire = true;
    	}

        void Update()
        {
        }

    	public override void Activate()
    	{
    		if (_attr.readyToFire) 
            {
                Transform[] firePs = transform.Cast<Transform>().Where
                    (c=>c.gameObject.tag == "Fire").ToArray();

                if(firePs.Length == 0)
                {
                    Debug.Log("No fire point!");
                    return;
                }

                Vector3 shotOrigin = firePs[Random.Range(0, firePs.Length)].position;

    			Vector3 forward = transform.TransformDirection (Vector3.up);

               // GameObject projectile;

                _attr.readyToFire = false;
        
                WeaponData data = new WeaponData();
                data.Damage = _attr.WeaponDamage;
                data.Direction = forward;
                data.Distance = _attr.WeaponRange;
                data.Self = _attr.ShipAtt.instID;

                StartCoroutine("EngageDelay");

                // Here is where we instruct the server to fire our weapon
                // pass weapon data and our prefab (or prefab info)

                transform.parent.GetComponent<ShipMessageController>()
                    .SpawnProjectile(shotOrigin, _attr.ProjectilePrefab, data);
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
    }
}
