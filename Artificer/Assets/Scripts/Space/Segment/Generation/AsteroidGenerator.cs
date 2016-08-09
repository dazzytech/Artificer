using UnityEngine;
using System.Collections;
using Data.Space;

namespace Space.Segment.Generator
{
    public class AsteroidGenerator : MonoBehaviour
    {
        public GameObject GenerateField(SegmentObject aData)
        {
            GameObject field = new GameObject ();
            field.name = "asteroidBelt";
            field.tag = "Asteroid";
            field.transform.parent = this.transform;
            field.transform.position = aData._position;
            
            // for now only spawn one asteroid to make sure it works
            int ACount = 0;
            while (ACount < aData._count) {
                GameObject asteroid = (GameObject)Instantiate (Resources.Load (aData._texturePath));
                asteroid.transform.parent = field.transform;
                // Give a random location and size;
                Vector2 location = new Vector2
                    (Random.Range (0f, aData._size.x),
                     Random.Range (0f, aData._size.y));
                asteroid.transform.localPosition = location;
                
                float scale = Random.Range (0f, 5f);
                asteroid.transform.localScale =
                    new Vector3(scale,
                                scale, 1f);
                asteroid.GetComponent<Rigidbody2D>().mass = scale;
                
               // AsteroidBehaviour behaviour = asteroid.AddComponent<AsteroidBehaviour>();
                //behaviour.rockDensity = 20f * scale;
                //behaviour.prospect = aData._symbols;
                ACount++;
            }

            return field;
        }

        public GameObject GenerateSingle(SegmentObject aData, 
                                         Vector2 position, Vector2 velocity)
        {
            GameObject asteroid = (GameObject)Instantiate
                (Resources.Load (aData._texturePath));

            asteroid.transform.parent = this.transform;
            // Give a random location and size;
            Vector2 location = position;
            asteroid.transform.position = location;
            
            float scale = Random.Range (1f, 10f);
            asteroid.transform.localScale =
                new Vector3(scale,
                            scale, 1f);

            Rigidbody2D rb = asteroid.GetComponent<Rigidbody2D>();
            rb.mass = scale;
            rb.AddForce(velocity, ForceMode2D.Force);
            
            AsteroidBehaviour behaviour = asteroid.AddComponent<AsteroidBehaviour>();
            //behaviour.rockDensity = 20f * scale;
            //behaviour.prospect = aData._symbols;

            return asteroid;
        }
    }
}
