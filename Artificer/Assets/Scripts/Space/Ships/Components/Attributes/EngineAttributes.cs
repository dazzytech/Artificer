using UnityEngine;
using System.Collections;

namespace ShipComponents
{
    public class EngineAttributes : ComponentAttributes
    {
    	[HideInInspector] 
    	public Vector3 engineMotion;
    	[HideInInspector]
    	public Vector3 engineVelocity;
    	[HideInInspector]
    	//public EllipsoidParticleEmitter emitter;
    	
    	public float acceleration;
    	public float maxSpeed;
    }
}
