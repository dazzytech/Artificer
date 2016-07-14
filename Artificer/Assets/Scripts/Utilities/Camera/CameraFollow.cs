using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
	public Transform objToFollow;
	public Transform thisTransform;
	public float PosDamp = 4f;

	// Use this for initialization
	void Awake ()
	{
		objToFollow = null;
		thisTransform = GetComponent<Transform>();
	}

	public void SetFollowObj(Transform newObj)
	{
		if (newObj == null) {
			Debug.Log 
				("CameraFollow - SetFollowObj: Object doesnt exist!");
			return;
		}

		objToFollow = newObj;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		if (objToFollow != null) {
			//Get output velocity
			Vector3 Velocity = Vector3.zero;

			thisTransform.position = 
				Vector3.SmoothDamp(thisTransform.position,
				    objToFollow.position, ref Velocity,
					PosDamp* Time.fixedDeltaTime);

			thisTransform.position = 
				new Vector3(thisTransform.position.x, 
			        thisTransform.position.y, -10f);

		}
	}
}

