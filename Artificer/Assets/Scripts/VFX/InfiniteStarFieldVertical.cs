﻿using UnityEngine;
using System.Collections;

public class InfiniteStarFieldVertical: MonoBehaviour {
		
	public Transform tx;
	private ParticleSystem.Particle[] points;

	public int starsMax = 100;
	public float starSizeMax = 0.1f;
	public float starSizeMin = 0.01f;
	public float starDistance = 10f;
	public float starClipDistance = 1f;
	private float starDistanceSqr;

	// Use this for initialization
	void Start () {
		starDistanceSqr = starDistance * starDistance;
	}
	
	
	private void CreateStars() {
		points = new ParticleSystem.Particle[starsMax];
		
		for (int i = 0; i < starsMax; i++) {
			Vector3 position = Random.insideUnitSphere * starDistance + tx.position;
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
			if ((points[i].position - tx.position).sqrMagnitude > starDistanceSqr) {

				Vector3 position = points[i].position *(0.9f*-Mathf.Sign((points[i].position - tx.position).sqrMagnitude));
				position.z = 0;
				points[i].position = position;
			}
			else
			{
				Vector3 position = points[i].position +
					new Vector3(0.1f * points[i].size, 0.0f, 0.0f);
				position.z = 0;
				points[i].position = position;
			}
		}
		
		GetComponent<ParticleSystem>().SetParticles ( points, points.Length );
		
	}
}
