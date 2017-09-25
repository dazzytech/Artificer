using System.Collections;
using System.Collections.Generic;
using Space.Ship;
using UnityEngine;
using UnityEngine.Networking;
using Data.Space;
using System.Linq;
using Data.Space.Collectable;
using Data.UI;

namespace Stations
{
    public class DepotController : StationController
    {
        #region ACCESSORS

        /// <summary>
        /// Returns the attributes 
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

        /// <summary>
        /// Quick accessor to materials that can be deposited
        /// </summary>
        private ItemCollectionData[] Depositable
        {
            get
            {
                return m_att.Ship.GetMaterials(m_att.DepositDelayLists.
                Keys.ToArray());
            }
        }

        #endregion

        #region MONOBEHAVIOUR

        public override void Awake()
        {
            base.Awake();

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
        public override void EnterRange(ShipAccessor ship)
        {
            m_att.Ship = ship;

            // Start listening for storage change
            m_att.Ship.OnStorageChanged += StorageChanged;

            m_att.InRange = true;

            Prompt();
        }

        /// <summary>
        /// If we are depositing then cancel
        /// </summary>
        public override void ExitRange(ShipAccessor ship)
        {
            m_att.Ship = null;

            if (m_att.InteractPrompt != null)
            {
                SystemManager.UIPrompt.DeletePrompt(m_att.InteractPrompt.ID);
                m_att.InteractPrompt = null;
            }

            if (m_att.IsDepositing)
            {
                m_att.IsDepositing = false;
                StopAllCoroutines();

                m_att.Ship.OnStorageChanged -= StorageChanged;
            }

            m_att.InRange = false;
        }

        /// <summary>
        /// Begin the process of 
        /// depositing materials and display
        /// </summary>
        /// <param name="ship"></param>
        public override void Interact(ShipAccessor ship)
        {
            if (!m_att.IsDepositing && Depositable.Length > 0)
            {
                // Change text and update
                m_att.InteractPrompt.LabelText = new string[1]
                    { "Depositing materials, Please wait" };

                m_att.IsDepositing = true;

                // Set a float based on the assets
                // contained within the ship
                float timer = 0;

                foreach(ItemCollectionData item in Depositable)
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

            m_att.Ship.OnStorageChanged -= StorageChanged;

            StartCoroutine("DelayEnd",
                "Deposit successful.");
        }

        /// <summary>
        /// Displays different prompts based
        /// on the inventory of the ship
        /// </summary>
        private void Prompt(bool reset = false)
        {
            if (m_att.InteractPrompt == null)
            {
                m_att.InteractPrompt = new PromptData();
                // Check that the player has storage that can be deposited
                if (Depositable.Length > 0)
                    m_att.InteractPrompt.LabelText = new string[1]
                        {string.Format
                        ("Press {0} to deposit ship storage.",
                        Control_Config.GetKey("interact", "sys"))};
                else
                    m_att.InteractPrompt.LabelText = new string[1]
                        {"You have no materials that may be deposited here."};

                SystemManager.UIPrompt.DisplayPrompt(m_att.InteractPrompt);
            }
            else
            {
                // Check that the player has storage that can be deposited
                if (Depositable.Length > 0)
                    m_att.InteractPrompt.LabelText = new string[1]
                        {string.Format
                        ("Press {0} to deposit ship storage.",
                        Control_Config.GetKey("interact", "sys"))};
                else
                    m_att.InteractPrompt.LabelText = new string[1]
                        {"You have no materials that may be deposited here."};

                if(reset)
                    SystemManager.UIPrompt.UpdatePrompt(m_att.InteractPrompt.ID);
                else
                    SystemManager.UIPrompt.DisplayPrompt(m_att.InteractPrompt.ID);
            }
                
        }

        #endregion

        #region EVENTS

        /// <summary>
        /// When the player updates their storage
        /// while depositing then stop the deposit and update the 
        /// prompt
        /// </summary>
        public void StorageChanged()
        {
            if (m_att.IsDepositing)
            {
                StopCoroutine("DelayDeposit");

                StartCoroutine("DelayEnd",
                    "Deposit cancelled stored due to change in storage.");
            }
            else
                Prompt(true);
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
            float timer = 0;

            m_att.InteractPrompt.SliderValues = new float[1];

            while (timer < delay)
            {
                yield return null;

                timer += Time.deltaTime;

                m_att.InteractPrompt.SliderValues[0] = timer / delay;

                SystemManager.UIPrompt.UpdatePrompt(m_att.InteractPrompt.ID);
            }

            Deposit();

            yield break;
        }

        /// <summary>
        /// Displays ending message for two seconds then  
        /// resets prompt
        /// </summary>
        /// <param name="delay"></param>
        /// <returns></returns>
        private IEnumerator DelayEnd(string endMessage)
        {
            m_att.InteractPrompt.SliderValues = null;

            m_att.InteractPrompt.LabelText[0] = endMessage;

            SystemManager.UIPrompt.UpdatePrompt(m_att.InteractPrompt.ID);

            yield return new WaitForSeconds(2f);

            m_att.IsDepositing = false;

            m_att.Ship.OnStorageChanged += StorageChanged;

            Prompt(true);
        }

        #endregion
    }
}
