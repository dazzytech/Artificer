using UnityEngine;
using System.Collections;

using Space.Ship.Components.Attributes;

namespace Space.Ship.Components.Listener
{
    public class EngineListener : ComponentListener
    {
    	EngineAttributes _attr;

    	void Awake()
    	{
            ComponentType = "Engines";
    		_attr = GetComponent<EngineAttributes>();
    	}

    	void Start ()
    	{
            base.SetRB();
    		InitEmitter ();
    	}

    	void Update()
    	{
            _attr.engineVelocity = transform.up * _attr.engineVelocity.magnitude;
            rb.AddForce
                (_attr.engineVelocity*Time.deltaTime, ForceMode2D.Force);

    		// for now drag is hardcoded
    		if(!_attr.active)
    		{
    			_attr.engineVelocity *= 0.98f;
    			if (Mathf.Abs (_attr.engineVelocity.magnitude) < .01f)
    				_attr.engineVelocity = Vector3.zero;

                rb.drag = 0.9f;
    		} else
            {
                _attr.engineMotion = transform.up;
                _attr.engineVelocity += _attr.engineMotion * _attr.acceleration;
                if (Mathf.Abs(_attr.engineVelocity.magnitude) > _attr.maxSpeed)
                    _attr.engineVelocity = (_attr.engineVelocity.normalized
                                            * _attr.maxSpeed) * _attr.engineMotion.magnitude;
            }
    	}

    	public override void Activate()
    	{
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
    		_attr.TriggerKey = Control_Config
    			   .GetKey(key, "ship");

    		_attr.engineMotion = transform.up;
    	}

        public void SetCombatKey(string key)
        {
            _attr.CombatKey = Control_Config
                .GetKey(key, "combat");
        }

    	private void InitEmitter()
    	{
    		_attr.emitter = transform.Find
    			("Engine").GetComponent
    				<EllipsoidParticleEmitter> ();
    	}

        public override void Destroy()
        {
            base.Destroy();

            Deactivate();
        }
    }
}

