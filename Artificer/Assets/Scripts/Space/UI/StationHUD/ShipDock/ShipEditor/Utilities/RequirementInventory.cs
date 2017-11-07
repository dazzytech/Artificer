using UnityEngine;
using System.Collections;
using System.Collections.Generic;
// Artificer
using Data.Space;
using Data.Space.Collectable;
using System.Linq;
using UI;

namespace Space.UI.Station.Utility
{
    public class RequirementInventory : HUDPanel
    {
        #region ATTRIBUTES

        [Header("Requirement")]

        [SerializeField]
        private GameObject m_viewerPrefab;

        [SerializeField]
        private Color m_owned;

        [SerializeField]
        private Color m_notOwned;

        /// <summary>
        /// Refernce to our current stored
        /// material 
        /// </summary>
        private Dictionary<ItemCollectionData, MaterialViewerPrefab> m_reqList;

        #endregion

        #region MONOBEHAVIOUR

        void Awake()
        {
            // Set Lists
            m_reqList = new Dictionary<ItemCollectionData, MaterialViewerPrefab>();
        }

        #endregion

        #region PUBLIC INTERATION

        /// <summary>
        /// Displays the list of items that 
        /// are neededto build the ship.
        /// </summary>
        /// <param name="import">Import.</param>
        public void UpdateList(ItemCollectionData[] required)
        {
            // Add player cargo into current
            ItemCollectionData[] current = SystemManager.Player.Wallet.Assets;

            if (required == null)
                return;

            for (int i = 0; i < required.Length; i++)
            {
                // Update storage variables
                if (!m_reqList.ContainsKey(required[i]))
                    m_reqList.Add(required[i], CreateAssetViewer(required[i]));

                ItemCollectionData owned = new ItemCollectionData();

                if(current != null)
                    owned = current.FirstOrDefault
                        (x => x.Item == required[i].Item);

                // Display our requirement
                m_reqList[required[i]].DisplayValue(SystemManager.Items[required[i].Item].Name,
                    new float[2] { required[i].Amount, owned.Amount}, new string[3] 
                    { SystemManager.Items[required[i].Item].Description,
                        required[i].Amount.ToString(), owned.Amount.ToString() });

                if(required[i].Amount <= owned.Amount)
                    m_reqList[required[i]].SetColour(m_owned);
                else
                    m_reqList[required[i]].SetColour(m_notOwned);
            }

            // Remove any requirements that are no longer 
            // included
            for(int i = 0; i < m_reqList.Count; i++)
            {
                if(!required.ToList().Contains(m_reqList.Keys.ToArray()[i]))
                {
                    Destroy(m_reqList.Values.ToArray()[i].gameObject);
                    m_reqList.Remove(m_reqList.Keys.ToArray()[i]);
                    i--;
                }
            }
        }

        #endregion

        #region PRIVATE UTILITIES

        /// <summary>
        /// Build the material viewer and
        /// return to list
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private MaterialViewerPrefab CreateAssetViewer(ItemCollectionData item)
        {
            // Game object
            GameObject newItem = Instantiate(m_viewerPrefab) as GameObject;
            newItem.transform.SetParent(m_body.transform, false);

            return newItem.GetComponent<MaterialViewerPrefab>();
        }

        #endregion
    }
}