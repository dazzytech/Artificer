using UnityEngine;
using System.Collections;

namespace ShipComponents
{
    public class WeaponAttributes : ComponentAttributes 
    {
    	public float WeaponDamage;
    	public float WeaponRange;
    	public float WeaponDelay;

    	public bool readyToFire;
    	public GameObject ProjectilePrefab;
    }
}