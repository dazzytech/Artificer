using UnityEngine;
using System.Collections;

namespace ShipComponents
{
    public class ManeuverAttributes : ComponentAttributes
    {
    	[HideInInspector] 
    	public Vector3 engineMotion;
    	[HideInInspector]
    	public Vector3 engineVelocity;

    	public float acceleration;
    	public float maxSpeed;
    }
}

