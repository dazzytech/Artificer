using UnityEngine;
using System.Collections;

using Data.Shared;
using Space.Segment;
using Space.Ship.Components.Attributes;

namespace Space.Ship.Components.Listener
{
    public class ShieldListener: ComponentListener{

        ShieldAttributes _attr;
    	
    	void Awake()
    	{
            ComponentType = "Shields";
            _attr = GetComponent<ShieldAttributes>();
            _attr.ShieldGO = transform.Find("ShieldObject").gameObject;
    	}

        void Start ()
    	{
            base.SetRB();

            _attr.Ready = true;
            _attr.Delay = 1f;
            _attr.Destroyed = false;
            _attr.CurrentIntegrity = _attr.ShieldIntegrity;
    	}

    	void Update()
    	{
            if (_attr.active)
            {
                if(!_attr.ShieldGO.activeSelf)
                {
                    CreateShield();

                    RaycastHit2D[] colliderList = Physics2D.CircleCastAll(transform.position, _attr.ShieldRadius, Vector2.up, 0, 1);
                    foreach (RaycastHit2D hit in colliderList)
                    {
                        if(hit.transform.Equals(transform.parent))
                        {
                            // Set Component to shielded
                            ComponentAttributes att = 
                                hit.collider.transform.gameObject.GetComponent<ComponentAttributes>();
                            if(att != null)
                                att.Shield = this;
                        }
                    }
                }
            } else
            {
                if(_attr.ShieldGO.activeSelf)
                    DestroyShield();

                RaycastHit2D[] colliderList = Physics2D.CircleCastAll(transform.position, _attr.ShieldRadius, Vector2.up, 0, 1);
                foreach (RaycastHit2D hit in colliderList)
                {
                    if(hit.transform.Equals(transform.parent))
                    {
                        // Set Component to shielded
                        hit.collider.transform.gameObject.GetComponent<ComponentAttributes>().Shield = null;
                    }
                }
            }
    	}

        public void Impact(HitData hit)
        {
            _attr.CurrentIntegrity -= hit.damage;
            
            _attr.ShieldGO.SendMessage("UpdateColour",(_attr.CurrentIntegrity / _attr.ShieldIntegrity), SendMessageOptions.DontRequireReceiver);

            _attr.ShieldGO.SendMessage("Hit", hit, 
                                       SendMessageOptions.DontRequireReceiver);

            if (_attr.CurrentIntegrity <= 0)
            {
                _attr.active = false;
                _attr.Destroyed = true;

            }
            StopCoroutine("Recharge");
            StartCoroutine("Recharge");
        }

    	public override void Activate()
    	{
            if (_attr.Ready && !_attr.Destroyed)
            {
                _attr.active = !_attr.active;
                _attr.Ready = false;
                StartCoroutine("EngageDelay");
            }
    	}
    	
    	public override void Deactivate()
    	{

    	}
    	
    	public void SetTriggerKey(string key)
    	{
    		_attr.TriggerKey =
    			Control_Config.GetKey(key, "ship");
    	}

        public void SetCombatKey(string key)
        {
            _attr.CombatKey = Control_Config
                .GetKey(key, "combat");
        }

        /// <summary>
        /// Engages the delay for key press
        /// </summary>
        /// <returns>The delay.</returns>
        private IEnumerator EngageDelay()
        {
            yield return new WaitForSeconds (_attr.Delay);
            _attr.Ready = true;
            yield return null;
        }

        /// <summary>
        /// Engages delay for shield recharge
        /// </summary>
        private IEnumerator Recharge()
        {
            yield return new WaitForSeconds (_attr.RechargeDelay);
            _attr.Destroyed = false;
            _attr.active = true;

            float amtToAdd = _attr.ShieldIntegrity - _attr.CurrentIntegrity;
            while (amtToAdd > 0)
            {
                _attr.CurrentIntegrity += 5;
                amtToAdd -= 5;
                if(_attr.CurrentIntegrity > _attr.ShieldIntegrity)
                    _attr.CurrentIntegrity = _attr.ShieldIntegrity;

                yield return null;
            }
            yield return null;
        }

        private void CreateShield()
    	{
            _attr.ShieldGO.SetActive(true);

            _attr.ShieldGO.GetComponent<ShieldController>().ConstructShield
                (_attr.ShieldRadius);
    	}

        private void DestroyShield()
        {
            _attr.ShieldGO.GetComponent<ShieldController>().DestroyShield();
        }

        public override void Destroy()
        {
            base.Destroy();
            
            Deactivate();
        }
    }
}