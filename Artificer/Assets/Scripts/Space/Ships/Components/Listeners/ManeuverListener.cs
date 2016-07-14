using UnityEngine;
using System.Collections;

// Artificer Defined
using Space.Ship.Components.Attributes;

namespace Space.Ship.Components.Listener
{
    public class ManeuverListener : ComponentListener
    {
    	ManeuverAttributes _attr;

    	void Awake()
    	{
            ComponentType = "Maneuvers";
    		_attr = GetComponent<ManeuverAttributes>();

    		SetTriggerKey ();
    	}

    	void Update()
    	{
    		// for now drag is hardcoded
    		transform.parent.transform.Translate(_attr.engineVelocity * Time.deltaTime);
    		if(!_attr.active)
    		{
    			_attr.engineVelocity *= 0.98f;
    			if (Mathf.Abs (_attr.engineVelocity.magnitude) < .01f)
    				_attr.engineVelocity = Vector3.zero;
    		}
    	}

    	// Use this for initialization
    	public override void Activate()
    	{
    		/*switch (state) {
    		case ShipAttributes.ShipState.COMBAT:
    			_attr.active = true;
    			
    			if (Input.GetKeyDown(_attr.TriggerKey [0])) {
    				// left
    				_attr.engineMotion = Vector3.left;
    			}
    			if (Input.GetKeyDown(_attr.TriggerKey [1])) 
    			{
    				// right
    				_attr.engineMotion = Vector3.right;
    			}
    			if (Input.GetKeyDown(_attr.TriggerKey [2])) 
    			{
    				// right
    				_attr.engineMotion = Vector3.down;
    			}

    			_attr.engineVelocity +=  _attr.engineMotion * _attr.acceleration;
    			if (Mathf.Abs(_attr.engineVelocity.magnitude) > _attr.maxSpeed)
    				_attr.engineVelocity = (_attr.engineVelocity.normalized 
    				       * _attr.maxSpeed) * _attr.engineMotion.magnitude;
    			break;
    		}*/
    	}

    	public override void Deactivate()
    	{
    		_attr.active = false;
    	}

    	private void SetTriggerKey()
    	{

    	}
    }

}
