using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Data.Space;

namespace Space.Segment
{
    /*public class AsteroidSpawner : SpawnerBehaviour
    {
        public static AsteroidData Asteroid;
        public static AsteroidGenerator AstGen;
        public static Vector2 Target;

        private static List<SegmentObject> _ast = new List<SegmentObject>();
        public static int Max = 15;

        void OnEnable()
        {
            SegmentObjectBehaviour.Destroyed += RockDestroyed;
        }

        void OnDisable()
        {
            SegmentObjectBehaviour.Destroyed -= RockDestroyed;
        }

        void OnDestroy()
        {
            Disengage();
        }

        public void RockDestroyed(SegmentObject obj)
        {
            if (_ast.Contains(obj))
            {
                _ast.Remove(obj);
            }
        }

        public override void Trigger()
        {
            if (Target == null)
            {
                Target = GameObject.FindGameObjectWithTag("Player").transform.position;
                if (Target == null)
                    return;
            }

            if (_ast.Count < Max)
            {
                Vector2 Velocity = (Target - new Vector2(transform.position.x, transform.position.y))
                    * Random.Range(.05f, .10f);
                GameObject Ast = AstGen.GenerateSingle 
                    (Asteroid, transform.position, Velocity);

                SegmentObject AstRef = new SegmentObject();
                AstRef._type = "asteroid";
                AstRef._texturePath = "Space/asteroid";

                SegmentObjectBehaviour obj = Ast.AddComponent<SegmentObjectBehaviour>();
                obj.Create(200f, AstRef);
                _ast.Add(AstRef);
            }
        }
    }*/
}