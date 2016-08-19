using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using System.Collections;
using System.Linq;

using Space.GameFunctions;
using Space.Projectiles;
using Space.Ship;
using Space.Ship.Components.Attributes;

namespace Space.Ship.Components.Listener
{
    #region NETWORK MESSAGE OBJECTS 

    /// <summary>
    /// Message sent when projectile is spawned by server
    /// with reference for accessing new projectile
    /// </summary>
    public class ProjectileSpawnedMessage : MessageBase
    {
        public NetworkInstanceId Projectile;
        public WeaponData WData;
    }

    #endregion

    public class WeaponListener : ComponentListener 
    {
        WeaponAttributes _attr;
    	
        void Awake()
    	{
            ComponentType = "Weapons";
    		_attr = GetComponent<WeaponAttributes>();

            GameManager.singleton.client.RegisterHandler(MsgType.Highest + 11, ProjectileCreated);
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

                int prefabIndex = NetworkManager.singleton.spawnPrefabs.IndexOf(_attr.ProjectilePrefab);

                StartCoroutine("EngageDelay");

                ProjectileBuildMessage msg = new ProjectileBuildMessage();
                msg.PrefabIndex = prefabIndex;
                msg.Position = shotOrigin;
                msg.WData = data;
                msg.shooterID = GameManager.Space.ID;

                // Sendmsg to game to spawn projectile
                GameManager.singleton.client.Send(MsgType.Highest + 12, msg);
            }
    	}

        public void ProjectileCreated(NetworkMessage msg)
        {
            Debug.Log("bullet made");
            // retreive message
            ProjectileSpawnedMessage projMsg = msg.ReadMessage<ProjectileSpawnedMessage>();

            // find our projectile
            GameObject GO = ClientScene.FindLocalObject
                (projMsg.Projectile);

            // client side projectile building
            GO.GetComponent<WeaponController>().CreateProjectile(projMsg.WData);
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
