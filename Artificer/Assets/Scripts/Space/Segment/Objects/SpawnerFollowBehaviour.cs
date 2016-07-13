using UnityEngine;
using System.Collections;

namespace Space.Segment
{
    // Script that attaches to spawner object with an object to follow
    // When object moves out of range then Follow will move spawner
    public class SpawnerFollowBehaviour : MonoBehaviour
    {
        public Transform Follow;

        public float MaxDistance;

        public float MinDistance;

        void OnDisable()
        {
            StopCoroutine("CheckDistance");
        }

        public void SetFollow(Transform follow, float min, float max)
        {
            Follow = follow;
            MinDistance = min;
            MaxDistance = max;

            StartCoroutine("CheckDistance");
        }

        IEnumerator CheckDistance()
        {
            for (;;)
            {
                if (Vector3.Distance(transform.position, Follow.position) > MaxDistance)
                {
                    Vector3 newPos = new Vector3();
                    bool outOfRange = false;
                    while (!outOfRange)
                    {
                        newPos = new Vector3
                        (Random.Range(Follow.position.x - MaxDistance, Follow.position.x + MaxDistance),
                         Random.Range(Follow.position.y - MaxDistance, Follow.position.y + MaxDistance));
                    
                        if (Vector2.Distance(newPos, Follow.position) >= MinDistance && Vector2.Distance(newPos, Follow.position) < MaxDistance)
                        {
                            outOfRange = true;
                        }
                    }

                    transform.position = newPos;
                }
                yield return null;
            }
        }
    }
}

