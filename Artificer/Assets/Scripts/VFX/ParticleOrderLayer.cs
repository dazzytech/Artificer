using UnityEngine;
using System.Collections;

public class ParticleOrderLayer : MonoBehaviour {

    public string layer;

    void Start ()
    {
        //Change particle layer to layer you want
        foreach (ParticleSystem ps in GetComponentsInChildren<ParticleSystem>())
        {
            ps.GetComponent<Renderer>().sortingLayerName = layer;
        }
    }
}
