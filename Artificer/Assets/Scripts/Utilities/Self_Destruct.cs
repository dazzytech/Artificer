using UnityEngine;
using System.Collections;

public class Self_Destruct : MonoBehaviour {

    public float seconds = 3f;
	// Use this for initialization
	void Awake()
    {
        Destroy(this.gameObject, seconds);
    }
}
