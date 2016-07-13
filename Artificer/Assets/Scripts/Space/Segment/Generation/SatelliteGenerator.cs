using UnityEngine;
using System.Collections;
using Data.Space;

namespace Space.Segment
{
    public class SatelliteGenerator : MonoBehaviour
    {
        /*public GameObject GenerateSatellite(SegmentObject satellite)
        {
            // Create game object for station
            GameObject newSatellite = new GameObject();
            newSatellite.name = satellite._name;
            newSatellite.tag = "Satellite";
            newSatellite.transform.parent = this.transform;
            newSatellite.transform.position = satellite._position;
            
            // Create visual element
            SpriteRenderer render = newSatellite.AddComponent<SpriteRenderer>();
            Sprite satelliteimg = Resources.Load(satellite._texturePath, typeof(Sprite)) as Sprite;
            render.sprite = satelliteimg;
            render.sortingLayerName = "BackgroundObjects";
            
            // colliders
            newSatellite.AddComponent<BoxCollider2D> ();
            Rigidbody2D r = newSatellite.AddComponent<Rigidbody2D> ();
            r.gravityScale = 0;
            
            return newSatellite;
        }*/
    }
}

