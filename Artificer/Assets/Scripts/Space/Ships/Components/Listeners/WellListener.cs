using UnityEngine;
using System.Collections;

using Space.Ship.Components.Attributes;

namespace Space.Ship.Components.Listener
{
    public class WellListener : ComponentListener
    {
        WellAttributes _attr;
        GameObject _GravityWell;

        void Awake()
        {
            ComponentType = "Wells";
            _attr = GetComponent<WellAttributes>();
            _attr.GravityEngaged = false;
        }
        
        void Start ()
        {
            base.SetRB();
        }
        
        void Update()
        {
            if(_attr.GravityEngaged)
            {
                if(_GravityWell != null)
                    _GravityWell.transform.position = transform.position;
                RaycastHit2D[] hits = 
                    Physics2D.CircleCastAll(transform.position,
                                            _attr.WellRadius, Vector2.up, 0, (1<<8));

                foreach(RaycastHit2D hit in hits)
                {
                    if(hit.transform.GetComponent<CollectableRockBehaviour>() 
                       != null)
                    {
                        Vector3 newPos = hit.transform.position;
                        newPos -= (hit.transform.position -
                                   transform.position).normalized *  _attr.PullForce*Time.deltaTime;

                        hit.transform.position = newPos;
                    }
                }
            }
        }
        
        public override void Activate()
        {
            if (!_attr.GravityEngaged)
            {
                _attr.GravityEngaged = true;

                _GravityWell = (GameObject)
                    Instantiate(_attr.WellFXPrefab,
                                transform.position, Quaternion.identity);
            }
        }

        public override void Deactivate()
        {
            _attr.GravityEngaged = false;

            Destroy(_GravityWell);
        }
        
        public void SetTriggerKey(string key)
        {
            _attr.TriggerKey = Control_Config
                .GetKey(key, "ship");
        }
    }
}
