using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Space.CameraUtils
{
    public class StarFieldController : MonoBehaviour
    {
        private Transform tx;
        private ParticleSystem.Particle[] points;
        private Vector3 prevPos;

        public int starsMax = 100;
        public float starSizeMax = 0.2f;
        public float starSizeMin = 0.01f;
        Bounds cBounds;

        public bool isRunning;

        // Use this for initialization
        void Start()
        {
            Profiler.maxNumberOfSamplesPerFrame = -1;
            isRunning = false;
        }

        private void CreateStars()
        {
            tx = gameObject.transform;
            prevPos = tx.position;
            points = new ParticleSystem.Particle[starsMax];
            cBounds = CameraExtensions.OrthographicBounds(GetComponentInParent<Camera>());

            for (int i = 0; i < starsMax; i++)
            {
                float x = Random.Range(-1f, 1f) * cBounds.extents.x;
                float y = Random.Range(-1f, 1f) * cBounds.extents.y;
                points[i].position = new Vector3(x, y, 0);
                points[i].color = new Color(Random.Range(.5f, 1f), Random.Range(.2f, 1f), 1, 1);
                points[i].startSize = Random.Range(0, 100) == 99 ? 0.5f : Random.Range(starSizeMin, starSizeMax);
            }

            cBounds.center = new Vector3(0,0);

            // extend so stars come from out of frame
            cBounds.extents *= 1.01f;
        }


        // Update is called once per frame
        // switch to coroutine
        void Update()
        {
            if (!isRunning)
            {
                points = null;
                return;
            }

            if (points == null)
                CreateStars();

            for (int i = 0; i < starsMax; i++)
            {
                if (!cBounds.Contains(points[i].position))
                {
                    Vector3 position = new Vector3();
                    position = points[i].position * (0.99f * -Mathf.Sign((points[i].position - tx.position).sqrMagnitude));
                    position.z = 0.0f;
                    points[i].position = position;
                }
                else
                {
                    Vector3 movement = (prevPos - tx.position) * 0.01f;
                    Vector3 position = points[i].position +
                        new Vector3(movement.x * Random.Range(0.1f, 0.3f), movement.y * Random.Range(0.1f, 0.3f), 0.0f);
                    position.z = 0;
                    points[i].position = position;
                }
            }

            prevPos = tx.position;
            GetComponent<ParticleSystem>().SetParticles(points, points.Length);
        }
    }
}
