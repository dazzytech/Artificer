using UnityEngine;
using System.Collections;
// Artificer
using Data.Space;
using Space.Generator;

public class StationBehaviour : MonoBehaviour
{/*
    //private SegmentObject _station;

    private float _integrity = 10000f;

    public bool Enter = false;

    public SegmentObject Station
    {
        set { _station = value; }
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if(Enter)
            GameObject.Find("space").SendMessage("StationReached", 
                collider.transform.parent, SendMessageOptions.DontRequireReceiver);
    }

    public void OnTriggerExit2D(Collider2D collider)
    {
    }

    public void Hit(HitData hit)
    {
        _integrity -= hit.damage;
    }

    public void HitArea(HitData hit)
    {
        Hit(hit);
    }


    public bool Functional
    {
        get{ return _integrity > 0;}
    }

    public float NormalizedHealth
    {
        get { return _integrity / 10000f;}
    }*/
}

