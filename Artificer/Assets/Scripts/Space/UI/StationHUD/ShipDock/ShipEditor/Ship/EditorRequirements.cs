using UnityEngine;
using System.Collections;
using System.Collections.Generic;
// Artificer Defined 
using Data.Space;
using Data.Space.Library;
using Space.Ship.Components.Listener;
using Space.Ship.Components.Attributes;
using UnityEngine.UI;
using Data.Space.Collectable;

namespace Space.UI.Station.Editor
{
    /// <summary>
    /// Ship requirements utility.
    /// stores a list of required material 
    /// responsible for adding requirements and removing requirements 
    /// 
    /// Store list of existing components, do not detract when removed
    /// add amount to player cargo instead and remove component
    /// </summary>
    public class EditorRequirements
    {
        #region ATTRIBUTES

        /// <summary>
        /// Accumulated cost of the ship
        /// </summary>
        private WalletData m_requirements;

        #region CURRENT COMPONENTS

        /// <summary>
        /// if true then we are editing 
        /// adding components that are already
        /// existing
        /// </summary>
        private bool m_current;

        /// <summary>
        /// Components that do not effect
        /// the cost when added or removed
        /// </summary>
        private List<int> m_existingComponents;

        #endregion

        #endregion

        #region ACCESSOR

        /// <summary>
        /// set if we are storing current 
        /// or new components
        /// </summary>
        public bool Current
        {
            set { m_current = value; }
        }

        public int Cost
        {
            get { return m_requirements.Currency; }
        }

        #endregion

        public EditorRequirements()
        {
            m_existingComponents = new List<int>();
            m_requirements = new WalletData();
            m_current = false;
        }

        #region PUBLIC INTERACTION

        /// <summary>
        /// Adds the component to requirement object.
        /// </summary>
        /// <param name="newComp">New comp.</param>
        public void AddComponent(ComponentData newComp)
        {
            if (m_current)
                AddExistingComponent(newComp);
            else
                AddNewComponent(newComp);
        }

        /// <summary>
        /// Removes the component.
        /// if component is existing then remove component
        /// and reimburse player, else remove amount from requirements
        /// </summary>
        /// <param name="newComp">New comp.</param>
        public void RemoveComponent(ComponentData newComp)
        {
            if (m_existingComponents.Contains(newComp.InstanceID))
            {
                /*
                // this was an original piece
                // Reimburse player data
                if (att.RequiredMats != null)
                {
                    // Create dictionary list
                    Dictionary<MaterialData, float> ReturnAmt = new Dictionary<MaterialData, float>();
                    foreach (ConstructInfo info in att.RequiredMats)
                    {
                        MaterialData mat = ElementLibrary.ReturnElement(info.material);

                        if(mat == null)
                            continue;

                        if (ReturnAmt.ContainsKey(mat))
                        {
                            ReturnAmt [mat] += info.amount;
                        } else
                        {
                            ReturnAmt.Add(mat, info.amount);
                        }
                        // Add amount to reimburse tracker
                        if (Reimbursed.ContainsKey(mat))
                        {
                            Reimbursed [mat] += info.amount;
                        } else
                        {
                            Reimbursed.Add(mat, info.amount);
                        }
                    }
                    // Reimburse player
                    //SystemManager.GetPlayer.AddMaterial(ReturnAmt);
                }*/
                // Remove component from list
                m_existingComponents.Remove(newComp.InstanceID);
            } else
            {
                m_requirements.Purchase(newComp.Cost);

                /*
                // This is not an original piece and can decrement requirements
                if (att.RequiredMats != null)
                {
                    // Remove amount from store
                    foreach(ConstructInfo info in att.RequiredMats)
                    {
                        MaterialData mat = ElementLibrary.ReturnElement(info.material);

                        if(mat == null)
                            continue;

                        if(Requirements.ContainsKey(mat))
                        {
                            Requirements[mat] -= info.amount;
                            // Remove entry if empty
                            if(Requirements[mat] <= 0)
                                Requirements.Remove(mat);
                        }
                    }
                }*/
            }
        }

        /// <summary>
        /// Deletes any component data stored
        /// </summary>
        /// <param name="saved">If set to <c>true</c> saved.</param>
        public void Clear(bool saved)
        {
            if (!saved)
            {
                m_requirements.Clear();
                // Changes were not saved so take material away from player
                // Remove amount from store
                /*foreach(MaterialData mat in Reimbursed.Keys)
                {
                    if(SystemManager.GetPlayer.Cargo.ContainsKey(mat))
                    {
                        SystemManager.GetPlayer.Cargo[mat] -= Reimbursed[mat];
                        // Remove entry if empty
                        if(SystemManager.GetPlayer.Cargo[mat] <= 0)
                            SystemManager.GetPlayer.Cargo.Remove(mat);
                    }
                }*/
            }

            m_existingComponents.Clear();
        }

        #endregion

        #region PRIVATE UTILITIES

        /// <summary>
        /// Adds the existing component so that required materials are
        /// ignored.
        /// </summary>
        /// <param name="newComp">New comp.</param>
        private void AddExistingComponent(ComponentData newComp)
        {
            if (!m_existingComponents.Contains(newComp.InstanceID))
                m_existingComponents.Add(newComp.InstanceID);
        }
        
        /// <summary>
        /// Adds the requirements of the component to the requirements
        /// total list
        /// </summary>
        /// <param name="newComp">New comp.</param>
        private void AddNewComponent(ComponentData newComp)
        {
            // Add component value to cost
            m_requirements.Deposit(newComp.Cost);

            // Process materials
            if(newComp.requirements != null)
            {
                foreach(ItemCollectionData item in newComp.requirements)
                {
                    if(item.Item != -1)
                        m_requirements.Deposit(item.Item, item.Amount);
                }
            }
        }

        #endregion
    }
}

