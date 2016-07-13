using UnityEngine;
using System.Collections;

using Space;
using Data.Shared;

namespace ShipComponents
{
    public class RotorListener : ComponentListener{

    	RotorAttributes _attr;
    	
    	void Awake()
    	{
            ComponentType = "Rotors";
    		_attr = GetComponent<RotorAttributes>();
    	}

        void OnDisable()
        {}

        void Start ()
    	{
            base.SetRB();
    		InitEmitter ();
    	}

    	void Update()
    	{
            // Perform automated turning if in combat mode
            if (_attr.Ship.CombatActive && _attr.Ship.CombatResponsive)
            {
                float difference = CalcAngle();
                if (Mathf.Sign(transform.localEulerAngles.z - 180) < 0)
                {
                    if (difference < -5f)
                    {
                        Activate();
                    } else
                        Deactivate();
                } else
                {
                    if (difference > 5f)
                    {
                        Activate();
                    } else
                        Deactivate();
                }

                if(difference < 1f && difference > -1f)
                    rb.angularVelocity = 0f;
            }

            // Apply rotation force
            float turnAmount = _attr.turnSpeed * Mathf.Sign(transform.localEulerAngles.z - 180);

            rb.AddTorque(turnAmount * Time.deltaTime);
            

            // add friction
    		if (!_attr.active)
            {
                _attr.turnSpeed = 0f;          
                rb.angularDrag = 1f;
            }
    	}

    	public override void Activate()
    	{
    		_attr.turnSpeed += _attr.turnAcceleration;

            if (Mathf.Abs(_attr.turnSpeed) > _attr.maxTurnSpeed)
                _attr.turnSpeed = _attr.maxTurnSpeed;

    		_attr.emitter.emit = true;
    		_attr.active = true;
    	}
    	
    	public override void Deactivate()
    	{
            if (_attr.emitter != null)
            {
                _attr.emitter.emit = false;
                _attr.active = false;
            }
    	}

    	public void SetTriggerKey(string key)
    	{
    		_attr.TriggerKey =
    			Control_Config.GetKey(key, "ship");

    		_attr.turnVector = new Vector3();
    		_attr.turnVector.z = -transform.right.y;
    	}

    	private void InitEmitter()
    	{
    		_attr.emitter = transform.Find
    			("Engine").GetComponent
    				<EllipsoidParticleEmitter> ();
    	}

        private float CalcAngle()
        {
            Vector2 dest = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 pos = transform.parent.position;
            float angle = Mathf.Atan2(dest.y-pos.y, dest.x-pos.x)*180 / Mathf.PI -90;
            return Mathf.DeltaAngle(transform.parent.eulerAngles.z, angle);
        }

        public override void Destroy()
        {
            base.Destroy();
            
            Deactivate();
        }
    }
}