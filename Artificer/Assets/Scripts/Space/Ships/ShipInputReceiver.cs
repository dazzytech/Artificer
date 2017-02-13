using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
// Artificer Defined
using Space.Generator;
using Space.Ship;
using Space.Ship.Components.Listener;
using Space.Ship.Components.Attributes;
using Space.UI.Ship;

namespace Space.Ship
{

    [RequireComponent(typeof(ShipAttributes))]
    public class ShipInputReceiver : NetworkBehaviour {

        #region ATTRIBUTES

        // ship references
        ShipAttributes _ship;
        ShipMessageController _msg;

        // delays key when switching combat states
        bool inputDelay;

        // Used to track active rotors in combat mode
        List<int> activeRotors = new List<int>();

        #endregion

        #region MONO BEHAVIOUR

        void Awake()
        {
            _ship = GetComponent<ShipAttributes>();
            _msg = GetComponent<ShipMessageController>();

            inputDelay = false;

            GetComponent<Rigidbody2D>().angularDrag = 0.9f;

            // init lists
            /*_ship.Targets = new List<Transform>();
            _ship.SelfTargeted = new List<Transform>();
            _ship.HighlightedTargets = new List<Transform>();*/
            _ship.TargetedShips = new List<ShipSelect>();
            _ship.TargetDistance = 300f;

            // rotor vars
            activeRotors = new List<int>();
        }

        void Update()
        {
            if (_ship == null)
                return;

            List<Transform> remove = new List<Transform>();

            // Remove targets if they are null
            // Or are further away than our targeting distance
            for(int iBase = 0; iBase < _ship.TargetedShips.Count; iBase++)
            {
                ShipSelect selected = _ship.TargetedShips[iBase];

                if (selected.Ship == null)
                {
                    // ship has been destroyed
                    // remove this selection and revert index for
                    // component
                    _ship.TargetedShips.RemoveAt(iBase);
                    selected = null;
                    iBase--;
                    continue;
                }

                // Ship is still alive, however detect if
                // ship is still in tracking range
                if (Vector3.Distance(transform.position, 
                    selected.Ship.transform.position)
                   > _ship.TargetDistance)
                {
                    // We are out of tracking range
                    // remove this selection and revert index for
                    // component
                    _ship.TargetedShips.RemoveAt(iBase);
                    selected = null;
                    iBase--;
                    continue;
                }

                // If the ship is still being tracked then 
                // check if we are still tracking components
                for (int iComp = 0; iComp < selected.TargetedComponents.Count; iComp++)
                {
                    Transform component = selected.TargetedComponents[iComp];

                    // check the component is destroyed
                    if (component == null)
                    {
                        // if so then delete this item
                        // revert index
                        selected.TargetedComponents.RemoveAt(iComp);
                        component = null;
                        iComp--;
                        continue;
                    }
                }

                // Are there components left?
                if (selected.TargetedComponents.Count <= 0)
                {
                    // if not then we don't care about this ship
                    // remove this selection and revert index for
                    // component
                    _ship.TargetedShips.RemoveAt(iBase);
                    selected = null;
                    iBase--;
                    continue;
                }
            }
            

            // Remove self targeted components if null
            /*foreach (Transform t in _ship.SelfTargeted)
            {
                if (t == null)
                {
                    remove.Add(t);
                    continue;
                }
            }

            // Remove Targets
            foreach (Transform t in remove)
            {
                _ship.SelfTargeted.Remove(t);
            }*/

            // if player ship find out if turning
            // by checking if rotor is active
            // also send message to server if true
            _ship.Ship.Aligned = true;
            List<int> inactiveRotors = new List<int>();
            foreach (ComponentListener list in _ship.Components)
            {
                if (list is RotorListener)
                {
                    int id = list.GetAttributes().ID;
                    if (list.GetAttributes().active)
                    {
                        _ship.Ship.Aligned = false;
                        if (!activeRotors.Contains(id))
                            activeRotors.Add(id);
                    }
                    else if (activeRotors.Contains(id))
                    {
                        inactiveRotors.Add(id);
                        activeRotors.Remove(id);
                    }
                }
            }

            if (_ship.Ship.CombatActive)
            {
                if (activeRotors.Count > 0)
                    CmdProcessComps(activeRotors.ToArray());

                if (inactiveRotors.Count > 0)
                    CmdReleaseComps(inactiveRotors.ToArray());
            }
        }

        #endregion

        #region KEYBOARD INPUT

        /// <summary>
        /// Processes a single key.
        /// relays to main receiver
        /// </summary>
        /// <param name="key">Key.</param>
        public void ReceiveKey(KeyCode key)
        {
            List<KeyCode> keys = new List<KeyCode>();
            keys.Add(key);
            ReceiveKey(keys);
        }

        /// <summary>
        /// Receives a list of keys that the player pressed
        /// and compares them against all components' triggers
        /// Activates components player intends to activate
        /// </summary>
        /// <param name="keys">Pressed Keys</param>
        public void ReceiveKey(List<KeyCode> keys)
        {
            if (Time.timeScale == 0)
                return;

            List<int> comps = new List<int>();

            foreach (KeyCode key in keys)
            {
                if (key == KeyCode.None)
                    return;

                if (key == Control_Config.GetKey("eject", "ship"))
                {
                    _msg.ShipDestroyed();
                }

                if (!inputDelay)
                {
                    if (key == Control_Config.GetKey("switchtocombat", "ship"))
                    {
                        _ship.Ship.CombatActive = !_ship.Ship.CombatActive;

                        // Switch off every component to stop sticking
                        foreach (ComponentListener listener in _ship.Components)
                            listener.Deactivate();

                        inputDelay = true;
                        StartCoroutine("EngageDelay");
                    }
                }

                // Components
                if (_ship.Ship.CombatActive)
                {
                    foreach (ComponentListener listener in _ship.Components)
                    {
                        ComponentAttributes att = listener.GetAttributes();
                        if (att != null && !att.active)
                        {
                            if (!(listener is WeaponListener) && !(listener is LauncherListener))
                            {
                                if (!_ship.Ship.CombatResponsive)
                                {
                                    if (att.TriggerKey == key)
                                    {
                                        listener.Activate();
                                        comps.Add(listener.GetAttributes().ID);
                                    }

                                }
                                else
                                {
                                    if (att.CombatKey != KeyCode.None)
                                    {
                                        if (att.CombatKey == key)
                                        {
                                            listener.Activate();
                                            comps.Add(att.ID);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                // Not every component will have a combat key
                                if (att.CombatKey != KeyCode.None)
                                {
                                    if (att.CombatKey == key)
                                    {
                                        listener.Activate();
                                        comps.Add(att.ID);
                                    }
                                }
                            }
                        }
                    }

                    if (comps.Count > 0)
                        CmdProcessComps(comps.ToArray());

                    return;
                }

                foreach (ComponentListener listener in _ship.Components)
                {
                    if (listener != null)
                    {
                        ComponentAttributes att = listener.GetAttributes();

                        if (att != null && !att.active)
                            if (att.TriggerKey == key)
                            {
                                listener.Activate();
                                comps.Add(att.ID);
                            }
                    }
                }

                
            }

            if (comps.Count > 0)
                CmdProcessComps(comps.ToArray());
        }

        /// <summary>
        /// Relays activated components to 
        /// client machines
        /// </summary>
        [Command]
        private void CmdProcessComps(int[] comps)
        {
            RpcProcessComps(comps);
        }

        /// <summary>
        /// Gives the remote player ships 
        /// the illusion of activated components
        /// e.g. engine exhausts
        /// </summary>
        /// <param name="comps"></param>
        [ClientRpc]
        private void RpcProcessComps(int[] comps)
        {
            if (isLocalPlayer)
                return;

            foreach (ComponentListener listener in _ship.SelectedComponents(comps))
            {
                if (listener is RotorListener || listener is EngineListener)
                {
                    if(listener.GetAttributes() != null)
                        listener.GetAttributes().emitter.emit = true;
                }
            }
        }

        /// <summary>
        /// Receives a list of keys that are released
        /// and deactivates any components that triggers
        /// correspond with the released keys
        /// </summary>
        /// <param name="key"></param>
        public void ReleaseKey(KeyCode key)
        {
            if (key != KeyCode.None)
            {
                List<int> comps = new List<int>();

                // Components
                if (_ship.Ship.CombatActive)
                {
                    foreach (ComponentListener listener in _ship.Components)
                    {
                        ComponentAttributes att = listener.GetAttributes();

                        if (att != null && att.active)
                        {
                            if (!(listener is WeaponListener) && !(listener is LauncherListener))
                            {
                                if (!_ship.Ship.CombatResponsive)
                                {
                                    if (listener.GetAttributes().TriggerKey == key)
                                    {
                                        listener.Deactivate();
                                        comps.Add(att.ID);
                                    }
                                }
                                else
                                {
                                    // Not every component will have a combat key
                                    if (att.CombatKey != KeyCode.None)
                                    {
                                        if (att.CombatKey == key)
                                        {
                                            listener.Deactivate();
                                            comps.Add(att.ID);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                // Not every component will have a combat key
                                if (att.CombatKey != KeyCode.None)
                                {
                                    if (att.CombatKey == key)
                                    {
                                        listener.Deactivate();
                                        comps.Add(att.ID);
                                    }
                                }
                            }
                        }
                    }

                    if (comps.Count > 0)
                        CmdReleaseComps(comps.ToArray());

                    return;
                }

                foreach (ComponentListener listener in _ship.Components)
                {
                    if (listener != null)
                    {
                        ComponentAttributes att = listener.GetAttributes();
                        if (att != null && att.active)
                            if (att.TriggerKey == key)
                            {
                                listener.Deactivate();
                                comps.Add(listener.GetAttributes().ID);
                            }

                    }
                }

                if (comps.Count > 0)
                    CmdReleaseComps(comps.ToArray());
            }
        }

        /// <summary>
        /// Relays deactivated components to
        /// client players
        /// </summary>
        [Command]
        private void CmdReleaseComps(int[] comps)
        {
            RpcReleaseComps(comps);
        }

        /// <summary>
        /// Runs on clients and turns of 
        /// visual activation for remote player ships
        /// </summary>
        /// <param name="comps"></param>
        [ClientRpc]
        private void RpcReleaseComps(int[] comps)
        {
            if (isLocalPlayer)
                return;

            foreach (ComponentListener listener in _ship.SelectedComponents(comps))
            {
                if (listener is RotorListener || listener is EngineListener)
                {
                    listener.GetAttributes().emitter.emit = false;
                }
            }
        }

        #endregion

        #region MOUSE INPUT

        /// <summary>
        /// Process a single click from the player input
        /// 
        /// </summary>
        /// <param name="position">Position.</param>
        public void SingleClick(Vector2 position)
        {
            if (!inputDelay)
            {
                if (_ship.Ship.CombatActive)
                    return;

                Collider2D hit =
                    Physics2D.OverlapPoint
                        (position);

                if (hit != null)
                {
                    _msg.AddTarget(hit.transform);
                }
                inputDelay = true;
                StartCoroutine("EngageDelay");
            }

        }

        /// <summary>
        /// Delete all the targets within the ships attributes
        /// </summary>
        public void ClearTargets()
        {
            if (!inputDelay)
            {
                //_ship.SelfTargeted.Clear();
                //_ship.Targets.Clear();
                _ship.TargetedShips.Clear();
                inputDelay = true;
                StartCoroutine("EngageDelay");
            }
        }

        #endregion

        #region COROUTRINES

        private IEnumerator EngageDelay()
        {
            yield return new WaitForSeconds(1f);
            inputDelay = false;
            yield return null;
        }

        #endregion
    }
}
