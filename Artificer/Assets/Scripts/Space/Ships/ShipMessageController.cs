using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

// Artificer Defined
using Data.Shared;
using Space.Projectiles;
using Space.Segment;
using Space.Segment.Generator;
using Space.Ship.Components.Listener;
using Space.UI;

namespace Space.Ship
{
    // defines the parameters for a 
    // ship destroyed event
    public class DestroyDespatch
    {
        // self alignment
        public string AlignmentLabel;

        // Last ship to attack ship
        public string AggressorTag;

        // physical destroyed ship
        public Transform Self;
    }

    /// <summary>
    /// Ship external controller.
    /// Dispatches and listens for events from external sources
    /// </summary>
    public class ShipMessageController : NetworkBehaviour
    {
        #region ATTRIBUTES

        ShipAttributes _ship;

        // EVENT DISPATCH
        public delegate void ShipEvent(DestroyDespatch param);
        public static event ShipEvent OnShipDestroyed;

        #endregion

        #region MONO BEHAVIOUR

        // Use this for initialization
        void Awake()
        {
            _ship = GetComponent<ShipAttributes>();
        }

        void OnDestroy()
        {
            DestroyDespatch param = new DestroyDespatch();
            param.AlignmentLabel = _ship.AlignmentLabel;
            param.AggressorTag = _ship.AggressorTag;
            param.Self = transform;
            //ShipDestroyed(param);
        }

        #endregion

        #region INCOMING MESSAGES

        /// <summary>
        /// Adds material information to the 
        /// ship internal resources
        /// </summary>
        /// <param name="data"></param>
        public void AddMaterial(Dictionary<MaterialData, float> data)
        {

            // Can't add material yet
            //_ship.Ship.AddMaterial(data);

            foreach (MaterialData mat in data.Keys)
            {
                // Set to popup gui
                MessageHUD.DisplayMessege(new MsgParam("small", "You have collected: " +
                    (data[mat]).ToString("F2")
                       + " - " + mat.Element));
            }
        }

        /// <summary>
        /// Creates a list of components that is 
        /// still linked to the ship head
        /// omitting the destroyed piece.
        /// </summary>
        /// <param name="exempt"> component that was destroyed</param>
        /// <returns></returns>
        public List<ComponentListener> BuildConnections(ComponentListener exempt)
        {
            List<ComponentListener> retList
                = new List<ComponentListener>();

            List<ComponentListener> piecesToAdd
                = new List<ComponentListener>
                    (_ship.Head.GetAttributes().connectedComponents);

            while (piecesToAdd.Count != 0)
            {
                if (piecesToAdd[0] == null)
                {
                    piecesToAdd.RemoveAt(0);
                    continue;
                }

                ComponentListener p = piecesToAdd[0];

                if (!retList.Contains(p) && !p.Equals(exempt))
                {
                    retList.Add(p);
                    piecesToAdd.AddRange(p.GetAttributes().connectedComponents);
                }
                piecesToAdd.Remove(p);
            }

            return retList;
        }

        /// <summary>
        /// Adds the target to the target list
        /// first tests if the target is suitable
        /// (may add a self targeting for repair)
        /// </summary>
        /// <param name="target">Target.</param>
        public void AddTarget(Transform target)
        {
            ComponentListener comp = target.GetComponent<ComponentListener>();
            if (comp != null)
            {
                // test if self targetting
                if (_ship.Components.Contains(comp))
                {
                    if (!_ship.SelfTargeted.Contains(target))
                        _ship.SelfTargeted.Add(target);
                }
                else
                {
                    if (!_ship.Targets.Contains(target))
                        _ship.Targets.Add(target);
                }
            }
            else
            {
                if (!_ship.Targets.Contains(target))
                    _ship.Targets.Add(target);
            }
        }

        /// <summary>
        /// Called by the local object to 
        /// send a projectile to spawn to the server
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="Prefab"></param>
        /// <param name="wep"></param>
        public void SpawnProjectile(Vector3 pos,
                GameObject Prefab, WeaponData wep)
        {
            if (!isLocalPlayer) return;

            int prefabIndex = NetworkManager.singleton.spawnPrefabs.IndexOf(Prefab);

            CmdSpawnProjectile(pos, prefabIndex, wep);
        }

        /// <summary>
        /// Server command that creates a projectile and then 
        /// initializes it
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="prefabIndex"></param>
        /// <param name="wep"></param>
        [Command]
        public void CmdSpawnProjectile(Vector3 pos,
                int prefabIndex, WeaponData wep)
        {
            GameObject Prefab = NetworkManager.singleton.spawnPrefabs[prefabIndex];

            GameObject GO = Instantiate(Prefab, pos, Quaternion.identity) as GameObject;

            NetworkHash128 GOID = GO.GetComponent<NetworkIdentity>().assetId;

            NetworkServer.Spawn(GO);

            GO.GetComponent<WeaponController>().CreateProjectile(wep);
        }

        #endregion

        #region SHIP INITIALIZATION

        /// <summary>
        /// Adds the components to list
        /// stored within shipattributes and
        /// defines the head
        /// </summary>
        public void AddComponentsToList()
        {
            foreach (Transform child in transform)
            {
                ComponentListener comp =
                    child.gameObject.
                        GetComponent<ComponentListener>();
                if (comp != null)
                {
                    // Store component in attributes 
                    // and sent attributes to component
                    _ship.Components.Add(comp);
                    comp.SetShip(_ship);
                }

                if (child.tag == "Head")
                    _ship.Head = comp;
            }
        }

        /// <summary>
        /// Sets the faction/team the ship is aligned to
        /// </summary>
        /// <param name="alignment"></param>
        public void SetShipAlignment(string alignment)
        {
            _ship.AlignmentLabel = alignment;
        }

        #endregion

        #region SHIP DESTRUCTION

        /// <summary>
        /// Adds the entire ship components to
        /// the debris generator. 
        /// </summary>
        public void ShipDestroyed()
        {
            int[] dead = new int[_ship.Components.Count];
            int i = 0;
            foreach (ComponentListener listener in _ship.Components)
            {
                listener.Destroy();
                dead[i++] = listener.GetAttributes().ID;
            }

            SendMessage("BuildColliders");

            CmdRemoveComponent(dead, this.transform.position);
        }

        /// <summary>
        /// Loops through each connection omitting the 
        /// Destroyed component. Then 
        /// adds the unconnected components to the debris generator.
        /// </summary>
        /// <param name="hit">hit.</param>
        public void DestroyComponent(HitData hit)
        {
            // Set the aggressor
            GameObject aggressor = NetworkServer.FindLocalObject(hit.originID);
            if (aggressor != null)
            {
                _ship.AggressorTag = aggressor.tag;
            }

            // Find the corresponding transform

            ComponentListener listener = null;

            foreach (ComponentListener temp in _ship.Components)
            {
                if (temp.GetAttributes().ID == hit.hitComponent)
                {
                    listener = temp;
                    break;
                }
            }

            // Check we were successful in obtaining the destroyed head
            if (listener == null)
                return;

            // first test if head was killed
            if (listener.Equals(_ship.Head))
            {
                ShipDestroyed();
                return;
            }

            // Create a list of instance IDs current connected to head
            List<ComponentListener> connected = BuildConnections(listener);
            if (connected.Count == 0)
            {
                ShipDestroyed();
                return;
            }

            List<int> dead = new List<int>();

            foreach (ComponentListener p in _ship.Components)
            {
                if (!connected.Contains(p) && !p.Equals(_ship.Head))
                {
                    // remove piece
                    dead.Add(p.GetAttributes().ID);
                }
            }

            // if player ship we need to destroy ship
            if ((_ship.Engines == 0 || _ship.Rotors == 0) && _ship.Weapons == 0)
                ShipDestroyed();
            else
                CmdRemoveComponent(dead.ToArray(), listener.transform.position);
        }

        /// <summary>
        /// Runs on server to create debris using destroyed 
        /// parts and sends rpc to rebuild all ship colliders
        /// </summary>
        /// <param name="dead"></param>
        /// <param name="position"></param>
        [Command]
        public void CmdRemoveComponent(int[] dead, Vector3 position)
        {
            DebrisGenerator.SpawnShipDebris(position,
                dead, this.netId, (position -
                transform.position).normalized * 10);

            RpcRemoveComponent(dead);
        }

        /// <summary>
        /// Rebuild collider list for hit detection.
        /// </summary>
        /// <param name="dead"></param>
        [ClientRpc]
        public void RpcRemoveComponent(int[] dead)
        {
            SendMessage("BuildColliders");
        }

        #endregion
    }
}
