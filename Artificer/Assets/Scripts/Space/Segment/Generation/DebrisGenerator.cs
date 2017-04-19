using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Data.Space;

namespace Space.Segment.Generator
{
    /// <summary>
    /// Central manager for generating debris
    /// called by the server functions
    /// also keeps track of total debris count
    /// for performance
    /// </summary>
    public class DebrisGenerator : NetworkBehaviour
    {
        #region ATTRIBUTES

        // reference to self in static functs
        public static DebrisGenerator instance;

        //private static SegmentObject wData = new SegmentObject();
        public GameObject WreckagePrefab;

        // Track the total number of debris
        private static int maxWreckage = 10;

        // num of current debris
        private static int currentWreckage;

        #endregion

        #region MONOBEHAVIOUR

        void OnEnable()
        {
            //SegmentObjectBehaviour.Destroyed += Destroyed;
        }
        
        void OnDisable()
        {
            //SegmentObjectBehaviour.Destroyed -= Destroyed;
        }

        void Start()
        {
            if (instance == null)
                instance = this;

            currentWreckage = 0;
        }

        /*wData._symbols = new string[100];
        int i = 0;
        for (; i < 7; i++)
            wData._symbols[i] = "Sn";
        for (; i < 14; i++)
            wData._symbols[i] = "Cu";
        for (; i < 30; i++)
            wData._symbols[i] = "Fe";
        for (; i < 35; i++)
            wData._symbols[i] = "Ni";
        for (; i < 45; i++)
            wData._symbols[i] = "Co";
        for (; i < 50; i++)
            wData._symbols[i] = "Zn";
        for (; i < 70; i++)
            wData._symbols[i] = "Pb";
        for (; i < 80; i++)
            wData._symbols[i] = "Al";
        for (; i < 87; i++)
            wData._symbols[i] = "Ag";
        for (; i < 95; i++)
            wData._symbols[i] = "Au";
        for (; i < 100; i++)
            wData._symbols[i] = "Pt";


    }



    public void Destroyed(SegmentObject obj)
    {
        // do nothing
        if(obj.Equals(wData))
            currentWreckage--;
    }*/

        #endregion

        #region SERVER MESSAGES

        /// <summary>
        /// Takes component ID list, generates 
        /// the debris from the components and initializes
        /// the controller.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="Comps"></param>
        /// <param name="playerID"></param>
        /// <param name="Vel"></param>
        [Server]
        public static void SpawnShipDebris(Vector3 position, int[] Comps,
            NetworkInstanceId playerID, Vector2 Vel)
        {
            if (currentWreckage >= maxWreckage)
                return;

            GameObject destroyed = Instantiate(instance.WreckagePrefab);
            destroyed.transform.parent = instance.transform;
            destroyed.transform.position = position;

            Rigidbody2D rb = destroyed.GetComponent<Rigidbody2D>();
            rb.AddForce(Vel);

            // Add attribute scripts
            //SegmentObjectBehaviour obj = destroyed.AddComponent<SegmentObjectBehaviour>();
            //obj.Create(50, wData);
            
            NetworkServer.Spawn(destroyed);

            // send build msg
            DestroyedComponent dc
                = destroyed.GetComponent<DestroyedComponent>();
            //dPC.prospect = wData._symbols;

            dc.SetWreckage(Comps, playerID);

            currentWreckage++;
        }

        #endregion
    }
}

