using UnityEngine;
using System.Collections;

namespace ShipComponents
{
    public class RotorAttributes : ComponentAttributes 
    {
    	
    	[HideInInspector]
    	public Vector3 strafeVelocity;
    	[HideInInspector]
    	//public EllipsoidParticleEmitter emitter;

    	public float turnSpeed;
    	public float maxTurnSpeed;
    	public float turnAcceleration;

    	[HideInInspector]
    	public Vector3 turnVector;
    }
}
