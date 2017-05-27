using UnityEngine;
using System.Collections;

using Space.Ship.Components.Attributes;

namespace Space.Ship.Components.Listener
{
    public class CollectorListener : ComponentListener
    {
        CollectorAttributes _attr;
        GameObject _GravityWell;

        void Awake()
        {
            ComponentType = "Collectors";
            _attr = GetComponent<CollectorAttributes>();
        }
        
        void Start ()
        {
            base.SetRB();
        }
        
        void Update()
        {
            // Begin process of searching for collectable objects within in range
            RaycastHit2D[] hits =
                    Physics2D.CircleCastAll(transform.position,
                                            _attr.Radius, Vector2.up, 0, (1 << 8));

            if(hits.Length > 0)
            {
                foreach (RaycastHit2D hit in hits)
                {
                    if (hit.transform.GetComponent<Collectable>()
                       != null)
                    {
                        Vector3 newPos = hit.transform.position;
                        newPos -= (hit.transform.position -
                                   transform.position).normalized * _attr.PullForce * Time.deltaTime;

                        hit.transform.position = newPos;

                        // test if in pickup range and send to storage is successful
                        if(Vector3.Distance(hit.transform.position, transform.position) > 1f)
                        {
                            // close enough to collect
                            transform.parent.SendMessage("CollectItem", 
                                hit.transform.GetComponent<Collectable>());
                        }
                    }
                }
            }
        }
    }
}
