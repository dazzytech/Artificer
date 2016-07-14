using UnityEngine;
using System.Collections;

public class EngineSortingLayer : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<EllipsoidParticleEmitter>().GetComponent<Renderer>().sortingLayerName = "BackgroundObjects";
	}
}
