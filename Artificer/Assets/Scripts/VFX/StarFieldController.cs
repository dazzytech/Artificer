using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StarFieldController : MonoBehaviour {
	private Transform tx;
	private ParticleSystem.Particle[] points;
	private Vector3 prevPos;
	
	public int starsMax = 100;
	public float starSizeMax = 0.2f;
	public float starSizeMin = 0.01f;
	public float starDistance = 10f;
	public float starClipDistance = 1f;
	private float starDistanceSqr;
	
	// Use this for initialization
	void Start () {
        starDistance = Camera.main.orthographicSize * 2;
        starsMax = Mathf.RoundToInt((starDistance / 10f) * 100f);
		starDistanceSqr = starDistance * starDistance;
	}
	
    public void Resize()
    {
        starDistance = Camera.main.orthographicSize * 2;
        starsMax = Mathf.RoundToInt((starDistance / 10f) * 100f);
        starDistanceSqr = starDistance * starDistance;
        CreateStars();
    }
	
	private void CreateStars() {
		tx = gameObject.transform;
		prevPos = tx.position;
		points = new ParticleSystem.Particle[starsMax];
		
		for (int i = 0; i < starsMax; i++) {
			Vector3 position = Random.insideUnitSphere * starDistance;
			position.z = 0;
			points[i].position = position;
			points[i].color = new Color(1,1,1, 1);
			points[i].size = Random.Range(starSizeMin, starSizeMax);
		}
	}
	
	
	// Update is called once per frame
	void Update () {
		if ( points == null ) CreateStars();
		for (int i = 0; i < starsMax; i++) {
			if ((points[i].position).sqrMagnitude > starDistanceSqr) {

				Vector3 position = new Vector3();
				position = points[i].position *(0.9f*-Mathf.Sign((points[i].position - tx.position).sqrMagnitude));
				position.z = 0.0f;
				points[i].position = position;
			}
			else
			{
				Vector3 movement = (prevPos - tx.position)* 0.5f;
				Vector3 position = points[i].position +
					new Vector3(movement.x * points[i].size, movement.y * points[i].size, 0.0f);
				position.z = 0;
				points[i].position = position;
			}
		}

		prevPos = tx.position;
		GetComponent<ParticleSystem>().SetParticles ( points, points.Length );
	}
}
