using UnityEngine;
using System.Collections;

public class shipShake : MonoBehaviour {

	Vector3 homePosition;

	void Start()
	{
		homePosition = transform.position;

		StartCoroutine (ShakeShip());
	}

	public IEnumerator ShakeShip()
	{
		while (true) {
			Vector3 trans = new Vector3 ();
			trans.x = homePosition.x + (Random.insideUnitCircle.x * 0.005f);
			trans.y = homePosition.y + (Random.insideUnitCircle.y * 0.005f);
            trans.z = homePosition.z;

			transform.position = trans;
			yield return new WaitForSeconds (0.001f);
		}
	}
}
