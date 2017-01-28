using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using System.Collections;
using System.Linq;

using Space.GameFunctions;
using Space.Projectiles;
using Space.Ship;
using Space.Ship.Components.Attributes;
using Networking;

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
                data.Self = _attr.Ship.instID;

                int prefabIndex = NetworkManager.singleton.spawnPrefabs.IndexOf(_attr.ProjectilePrefab);

                StartCoroutine("EngageDelay");

                ProjectileBuildMessage msg = new ProjectileBuildMessage();
                msg.PrefabIndex = prefabIndex;
                msg.Position = shotOrigin;
                msg.WData = data;
                msg.shooterID = GameManager.Space.ID;

                // Sendmsg to game to spawn projectile
                GameManager.singleton.client.Send((short)MSGCHANNEL.BUILDPROJECTILE, msg);
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
