using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Data.Shared;
using Space;
using Space.Ship.Components.Attributes;


namespace Space.Ship.Components.Listener
{
    public class TargeterListener : ComponentListener
    {
        TargeterAttributes _attr;
        
        void Awake()
        {
            ComponentType = "Targeter";
            _attr = GetComponent<TargeterAttributes>();
        }

        void OnDisable()
        {
        }

        void Start ()
        {
            base. SetRB();

            _attr.ComponentList = new List<ComponentListener>();
            _attr.ShipAtts = transform.parent.GetComponent<ShipAttributes>();

            // find targets if auto lock
            if (_attr.Behaviour == TargeterBehaviour.AUTOLOCKFIRE)
                StartCoroutine("FindArcTargets");
        }
    	
        // Update is called once per frame
        void Update()
        {
            // Auto engage targets if not a mouse follow behaviour
            if (_attr.Behaviour != TargeterBehaviour.MOUSECONTROL)
            {
                // Search for target to shoot
                if (FindClosestTarget())
                {
                    LerpTowardsTarget();
                    if (_attr.EngageFire)
                    {
                        // if angles are between a certain arc then 
                        // activate connected objects
                        // Find angle between this and target
                        float tAngle = Angle(transform, _attr.currentTarget, true);
                        // between min and max angle of 15 then we fire
                        if (tAngle < 5f && tAngle > -5f)
                        if (Vector3.Distance(transform.position,
                        _attr.currentTarget.position) <= _attr.AttackRange)
                            Activate();
                    }
                } else
                {
                    LerpTowardsFace();
                }
            } else
            {
                // Control the mousefollow behaviour here
                if (_attr.ShipData.CombatActive)
                {
                    LerpTowardsMouse();
                } else
                {
                    LerpTowardsFace();
                }
            }

            // Keep all components fixed to targeter
            if (_attr.ComponentList.Count == 0)
                PopulateComponentList();
            foreach (ComponentListener comp in _attr.ComponentList)
            {
                if(comp != null)
                    comp.FixToConnected();
            }
        }

        public override void Activate()
        {
            // go through each connected piece and activate them
            if (_attr.ComponentList.Count == 0)
                PopulateComponentList();
            foreach (ComponentListener comp in _attr.ComponentList)
            {
                if(comp != null)
                    comp.Activate();
            }
        }

        // TARGETING

        /// <summary>
        /// Finds targets within the targeters firing arc.
        /// </summary>
        private IEnumerator FindArcTargets()
        {
            // temp
            yield break;

            /*while (true)
            {
                RaycastHit2D[] hits =
                Physics2D.CircleCastAll(transform.position, _attr.AttackRange, Vector2.zero, 0, 1);
            
                foreach (RaycastHit2D hit in hits)
                {
                    // Only auto target heads of ships
                    // while no target grouping
                    if (_attr.ShipAtts.Targets.Contains(hit.collider.transform)
                        || hit.collider.transform.tag != "Head")
                        continue;

                    // detect if enemy
                    if (_attr.ShipAtts.AlignmentLabel == "player")
                    {
                        // use player relations
                        if (hit.transform.name
                            != "Enemy")
                            continue;
                    } else
                    {
                        // ai relation
                        if (_attr.ShipAtts.AlignmentLabel == "Enemy" && 
                            hit.transform.name == "Enemy")
                            continue;

                        if (_attr.ShipAtts.AlignmentLabel == "Friendly" &&
                            hit.transform.name != "Enemy" || hit.transform.tag == "Station")
                            continue;
                    }

                    ComponentListener comp = hit.collider.
                    transform.GetComponent<ComponentListener>();

                    if (comp != null)
                    {
                        // check not self targetting
                        if (!_attr.ShipAtts.Components.Contains(comp))
                        {
                            // Find angle between
                            float tAngle = Angle(transform, hit.collider.transform, false);
                            if (tAngle > _attr.MinAngle || tAngle < _attr.MaxAngle)
                                _attr.ShipAtts.Targets.Add(hit.collider.transform);
                        }
                    }

                    yield return null;
                }
                yield return null;
            }*/
        }

        /// <summary>
        /// Finds the closest target to the ship.
        /// </summary>
        /// <returns><c>true</c>,
        /// if closest target was found, 
        /// <c>false</c> otherwise.</returns>
        private bool FindClosestTarget()
        {
            /*_attr.currentTarget = null;
            if (_attr.ShipAtts.Targets.Count < 1)
                return false;

            float ShortestDistance = float.MaxValue;

            foreach (Transform t in _attr.ShipAtts.Targets)
            {
                // Find angle between
                float tAngle = Angle(transform, t, false);
                // between min and max angle
                if(tAngle < _attr.MinAngle || tAngle > _attr.MaxAngle)
                    continue;

                // Find distance between this and target
                float tDistance = Vector3.Distance
                    (transform.position, t.position);

                // if distance less than shortest value
                if(tDistance < ShortestDistance && tDistance < _attr.AttackRange)
                {
                    ShortestDistance = tDistance;
                    _attr.currentTarget = t;
                }
            }

            if (_attr.currentTarget != null)
                return true;
            else*/
                return false;
        }

        /// <summary>
        /// Lerps the targeter angle towards the target.
        /// </summary>
        private void LerpTowardsTarget()
        {
            Quaternion turretRotation = 
                Quaternion.LookRotation((transform.position
                                        - _attr.currentTarget.position), transform.TransformDirection
                                        (Vector3.forward));
            
            transform.rotation = 
                Quaternion.Slerp (transform.rotation,
                                  new Quaternion(0, 0, turretRotation.z, turretRotation.w), 
                                  Time.deltaTime * _attr.turnSpeed);
        }

        // NOT-TARGETING
        private void LerpTowardsFace()
        {
            Quaternion turretRotation = 
                Quaternion.LookRotation(-FindHomeUp(),
                transform.TransformDirection
                (Vector3.forward));
            
            transform.rotation = 
                Quaternion.Slerp (transform.rotation,
                new Quaternion(0, 0, turretRotation.z, turretRotation.w), 
                Time.deltaTime * _attr.turnSpeed);
        }

        private void LerpTowardsMouse()
        {
            Debug.Log(FindHomeUp());

            // Find angle between
            float tAngle = Angle(transform, Camera.main.ScreenToWorldPoint(Input.mousePosition));
            // between min and max angle
            if (tAngle < _attr.MinAngle || tAngle > _attr.MaxAngle)
                return;

            Quaternion turretRotation = 
                Quaternion.LookRotation((transform.position
                - Camera.main.ScreenToWorldPoint(Input.mousePosition)), transform.TransformDirection
                (Vector3.forward));
            
            transform.rotation = 
                Quaternion.Slerp (transform.rotation,
                new Quaternion(0, 0, turretRotation.z, turretRotation.w), 
                Time.deltaTime * _attr.turnSpeed);
        }



        // UTIL FUNCTIONS 

        /// <summary>
        /// Finds the origin vector of the component
        /// 
        /// </summary>
        private Vector3 FindHomeUp()
        {
            Vector3 homeUp = transform.parent.up;

            if(_attr.homeForward == Vector3.down)
                homeUp = -transform.parent.up;
            if(_attr.homeForward == Vector3.left)
                homeUp = -transform.parent.right;
            if(_attr.homeForward == Vector3.right)
                homeUp = transform.parent.right;

            return homeUp;
        }

        private float FindHomeAngle()
        {
            float euler = transform.parent.eulerAngles.z;
            
            if (_attr.homeForward == Vector3.down)
                euler += 180;
            if (_attr.homeForward == Vector3.left)
                euler += 90;
            if (_attr.homeForward == Vector3.right)
                euler -= 90;
            
            return euler;
        }


        /// <summary>
        /// Finds the angle difference between the two objects.
        /// </summary>
        /// <returns>The angle difference.</returns>
        /// <param name="trans">Trans.</param>
        /// <param name="dest">Destination.</param>
        float Angle(Transform trans, Transform dest, bool local)
        {
            Vector2 pos = trans.position;
            Vector2 destPos = dest.position;
            float angle = Mathf.Atan2(destPos.y-pos.y, destPos.x-pos.x)*180 / Mathf.PI -90;
            if(local)
                return Mathf.DeltaAngle(trans.eulerAngles.z, angle);
            else
                return Mathf.DeltaAngle(FindHomeAngle(), angle);
        }

        float Angle(Transform trans, Vector2 point)
        {
            Vector2 pos = trans.position;
            float angle = Mathf.Atan2(point.y-pos.y, point.x-pos.x)*180 / Mathf.PI -90;
            return Mathf.DeltaAngle(FindHomeAngle(), angle);
        }

        /// <summary>
        /// Adds any listeners of a certain type
        /// to the component list
        /// </summary>
        private void PopulateComponentList()
        {
            foreach (ComponentListener comp in _attr.connectedComponents)
            {
                // tracker type - weapon
                if(comp is WeaponListener)
                {
                    _attr.ComponentList.Add(comp);
                    // Clear triggers if weapon is not mouse controlled
                    if(_attr.Behaviour != TargeterBehaviour.MOUSECONTROL)
                    {
                        comp.GetAttributes().TriggerKey = KeyCode.None;
                        comp.GetAttributes().CombatKey = KeyCode.None;
                    }
                }
            }
        }
    }
}
