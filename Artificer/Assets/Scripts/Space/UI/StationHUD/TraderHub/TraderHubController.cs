using Data.Space;
using Data.Space.Collectable;
using Data.UI;
using Space.Teams;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

using UI;

namespace Space.UI.Station
{
    /// <summary>
    /// Interface with trader station or team assets
    /// allows player to trade
    /// </summary>
    [RequireComponent(typeof(TraderHubAttributes))]
    public class TraderHubController : HUDPanel
    {
        #region ATTRIBUTES

        [SerializeField]
        private TraderHubAttributes m_att;

        [SerializeField]
        private TraderHubEventListener m_event;

        #endregion

        #region PUBLIC INTERACTION

        #region INITIALIZE

        /// <summary>
        /// Initializes the station HUD.
        /// for now only pass ship atts
        /// Entry Point
        /// </summary>
        /// <param name="param">Parameter.</param>
        public void InitializeHUD(TeamController team)
        {
            m_att.Team = team;

            DisplayCash(SystemManager.Player.Wallet.Currency, m_att.Wallet);

            DisplayAssets(SystemManager.Player.Wallet.Assets, m_att.PlayerAssetList, m_att.PlayerAssets);

            DisplayAssets(team.Assets, m_att.TeamAssetList, m_att.TeamAssets);
        }

        #endregion

        #endregion

        #region PRIVATE UTILITIES

        #region UI DISPLAY

        /// <summary>
        /// Display the textual currency
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="wallet"></param>
        private void DisplayCash(float amount, Text wallet)
        {
            wallet.text = "test";
        }

        /// <summary>
        /// Single function to display both team
        /// and player assets
        /// </summary>
        private void DisplayAssets(ItemCollectionData[] assets, 
            Dictionary<ItemCollectionData, MaterialViewerPrefab> assetList,
            Transform container)
        {
            if (assets == null)
                return;

            if (assetList == null)
                assetList = new Dictionary<ItemCollectionData, MaterialViewerPrefab>();

            for (int i = 0; i < assets.Length; i++)
            {
                // Update storage variables
                if (!assetList.ContainsKey(assets[i]))
                    assetList.Add(assets[i], CreateAssetViewer(assets[i], container));

                // Display our requirement
                assetList[assets[i]].DisplayValue(SystemManager.Items[assets[i].Item].Name,
                    new float[1] { assets[i].Amount }, new string[2]
                    { SystemManager.Items[assets[i].Item].Description,
                        assets[i].Amount.ToString() });
            }

            // Remove any requirements that are no longer 
            // included
            for (int i = 0; i < assetList.Count; i++)
            {
                if (!assets.ToList().Contains(assetList.Keys.ToArray()[i]))
                {
                    Destroy(assetList.Values.ToArray()[i].gameObject);
                    assetList.Remove(assetList.Keys.ToArray()[i]);
                }
            }
        }

        /// <summary>
        /// Build the material viewer and
        /// return to list
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private MaterialViewerPrefab CreateAssetViewer(ItemCollectionData item, Transform container)
        {
            // Game object
            GameObject newItem = Instantiate(m_att.AssetPrefab) as GameObject;
            newItem.transform.SetParent(container, false);

            return newItem.GetComponent<MaterialViewerPrefab>();
        }

        #endregion

        #endregion
    }
}