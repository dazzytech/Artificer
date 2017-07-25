using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Space.Segment
{
    /// <summary>
    /// used to create fragments 
    /// that break apart
    /// </summary>
    public class Debris : MonoBehaviour
    {
        // Use this for initialization
        void Start()
        {
            Invoke("Die", 10f);
        }

        public void Initialize()
        {
            gameObject.layer = LayerMask.NameToLayer("Collectable");
            GetComponent<SpriteRenderer>().sortingLayerName = "BackgroundObjects";
        }

        protected void Die()
        {
            Destroy(this.gameObject);
        }
    }
}
