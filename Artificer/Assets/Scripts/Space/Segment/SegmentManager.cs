using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

using Data.Space;
using Space.CameraUtils;
using Space.Ship;
using Space.UI;

namespace Space.Segment
{
    [RequireComponent(typeof(SegmentObjectManager))]
    /// <summary>
    /// Segment behaviour.
    /// responsible for automated tasks and dispatching events
    /// e.g. What segment the player is currently, loading and unloading data
    /// </summary>
    public class SegmentManager 
        : NetworkBehaviour
    {
        #region NESTED SYNCLIST CLASS

        // sync list callback
        public void SyncListGOAdded(SyncListSO.Operation op, int index)
        {
            /*MessageHUD.DisplayMessege(new MsgParam("sm-green", ("\nName: " + _att.SegObjs[index]._name +
                 ", Position: " + _att.SegObjs[index]._position.ToString() +
                 ", Type: " + _att.SegObjs[index]._type + "\n")));*/
        }

        #endregion

        #region ATTRIBUTES

        [SerializeField]
        private SegmentObjectManager m_gen;
        [SerializeField]
        private SegmentAttributes m_att;

        private bool m_segInit;

        #endregion

        #region ACCESSOR
        
        public SegmentObjectManager Generator
        {
            get { return m_gen; }
        }
         
        #endregion

        #region MONO BEHAVIOUR

        void OnEnable()
        {
            SpaceManager.OnPlayerUpdate += OnPlayerUpdate;
        }
        
        void OnDisable()
        {
            SpaceManager.OnPlayerUpdate -= OnPlayerUpdate;
        }

        void Start()
        {
            // Segment is seperate from game functions 
            // so is encapsulated
            m_gen.GenerateBase();

            m_att.SegObjs.Callback += SyncListGOAdded;

            m_segInit = false;
        }

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Starts the process of making 
        /// objects within range visable
        /// </summary>
        private void EnterSegment()
        {
            m_gen.StartSegmentCycle();
            m_segInit = true;
        }

        /// <summary>
        /// Builds the space segment on the server
        /// and  syncs the segment info across the network
        /// </summary>
        [Server]
        public void InitializeSegment(GameParameters param)
        {
            // Initialize space segment if not created - server's job
            //SystemManager.GUI.DisplayMessege(new MsgParam("bold", "Generating space..."));
            SegmentObjectData[] sObjs = SegmentDataBuilder.BuildNewSegment(param);

            foreach (SegmentObjectData sObj in sObjs)
                m_att.SegObjs.Add(sObj);

            m_gen.GenerateServerObjects();

            // Initialize parellax objects
            SyncPI[] pItems = SegmentDataBuilder.BuildNewBackground();


            foreach (SyncPI pItem in pItems)
                m_att.BGItem.Add(pItem);
        }

        #endregion

        #region EVENT

        /// <summary>
        /// Raises the player update event.
        /// also updates cursor
        /// </summary>
        /// <param name="player">Player.</param>
        private void OnPlayerUpdate(Transform playerShip)
        {
            // initialize segment 
            if (!m_segInit)
                EnterSegment();
        }

        #endregion
    }
}

