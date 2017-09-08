using System.Collections;
using System.Collections.Generic;
using Space.Ship;
using UnityEngine;
using UnityEngine.Networking;
using Data.Space;
using System.Linq;
using Data.Space.Collectable;

namespace Stations
{
    public class DepotController : StationController
    {
        #region MONOBEHAVIOUR

        public override void Awake()
        {
            base.Awake();

            m_att.ProximityMessage = "Hold Enter to deposit storage";

            m_att.Type = STATIONTYPE.DEPOT;
        }

        #endregion

        #region PUBLIC INTERACTION

        [Server]
        public override void Initialize(NetworkInstanceId newTeam, bool ignore)
        {
            // For now call the base class till actions are different
            base.Initialize(newTeam, true);

            m_att.IsDepositing = false;

            // Here we initialise the delay list
            m_att.DepositDelayLists = new Dictionary<int, float>(4)
            {
                { 34, .1f }, // Plain asteroid .1 second
                { 35, .4f }, // high val asteroid
                { 36, .2F }, // plut
                { 37, .5f }  // fragments
            };
        }

        #region PLAYER

        public override void Dock(ShipAccessor ship)
        {
            // Do nothing
        }

        public override void UnDock(ShipAccessor ship)
        {
            // do nothing
        }

        /// <summary>
        /// Display custom message
        /// </summary>
        public override void EnterRange()
        {
            SystemManager.UIMsg.DisplayPrompt(string.Format
                ("Press {0} to deposit ship storage.", Control_Config.GetKey("interact", "sys")));
        }

        /// <summary>
        /// If we are depositing then cancel
        /// </summary>
        public override void ExitRange()
        {
            base.ExitRange();

            if (m_att.IsDepositing)
            {
                m_att.IsDepositing = false;
                StopAllCoroutines();
            }
        }

        /// <summary>
        /// Begin the process of 
        /// depositing materials and display
        /// </summary>
        /// <param name="ship"></param>
        public override void Interact(ShipAccessor ship)
        {
            if (!m_att.IsDepositing)
            {
                m_att.Ship = ship;

                SystemManager.UIMsg.DisplayPrompt("Depositing materials, Please wait");

                m_att.IsDepositing = true;

                // Set a float based on the assets
                // contained within the ship
                float timer = 0;

                foreach(ItemCollectionData item in m_att.Ship.GetMaterials
                    (m_att.DepositDelayLists.Keys.ToArray()))
                {
                    // time is determine by our delay list
                    // if delay is 0.01 then 10 will be a second to deposits
                    float delay = m_att.DepositDelayLists[item.Item] * item.Amount;

                    timer += delay;
                }

                StartCoroutine("DelayDeposit", timer);
            }
        }

        #endregion

        #endregion

        #region PRIVATE ACCESSORS

        /// <summary>
        /// Returns the attributes in warp type
        /// </summary>
        protected new DepotAttributes m_att
        {
            get
            {
                if (transform == null)
                    return null;
                else if (transform.GetComponent<DepotAttributes>() != null)
                    return transform.GetComponent<DepotAttributes>();
                else
                    return null;
            }
        }

        #endregion

        #region PRIVATE UTILITIES

        /// <summary>
        /// Finished the refining delay,
        /// add the refined elements to player 
        /// storage
        /// </summary>
        private void Deposit()
        {
            if(m_att.Ship != null)
            {
                // Get list of items from the ship
                foreach(ItemCollectionData item in m_att.Ship.RemoveMaterials
                    (m_att.DepositDelayLists.Keys.ToArray()))
                {
                    // retreive a number of item keys using the 
                    // material compisiiton
                    for (int i = 0; i < 3; i++)
                    {
                        int newItem = ((MaterialItem)SystemManager.Items[item.Item]).GetRandom();

                        float amount = item.Amount * Random.Range(0.1f, 3f);

                        // add the generated amount to our pending wallet
                        m_att.PendingDeposit.Deposit(new ItemCollectionData[]
                        { new ItemCollectionData(){ Item = newItem , Amount = amount, Exist = true } });
                    }
                }

                WalletData temp = SystemManager.Wallet;

                temp.Deposit(m_att.PendingDeposit.Assets);

                SystemManager.Wallet = temp;
            }

            SystemManager.UIMsg.ClearPrompt();

            m_att.IsDepositing = false;
        }

        #endregion

        #region COROUTINE

        /// <summary>
        /// Waits for delay period
        /// and then start material deposit
        /// </summary>
        /// <returns></returns>
        private IEnumerator DelayDeposit(float delay)
        {
            yield return new WaitForSeconds(delay);

            Deposit();

            yield break;
        }

        #endregion
    }
}
