using UnityEngine;
using System.Collections;
using System.Collections.Generic;
// Artificer Defined 
using Data.Shared;
using Data.Space.Library;
using Space.Ship.Components.Listener;
using Space.Ship.Components.Attributes;

namespace Construction.ShipEditor
{
    /// <summary>
    /// Ship requirements utility.
    /// stores a list of required material 
    /// responsible for adding requirements and removing requirements 
    /// 
    /// Store list of existing components, do not detract when removed
    /// add amount to player cargo instead and remove component
    /// </summary>
    public class ShipRequirementsUtility
    {
        // shared dictionary of requirements
        public Dictionary<MaterialData, float> Requirements;

        // Keep track of materials that were reimbursed
        public Dictionary<MaterialData, float> Reimbursed;

        // private list of existing components
        private List<Data.Shared.Component> Existing;

        // Keep track of adding components to existing
        // ship or not.
        public bool StoreExisting;

        public ShipRequirementsUtility()
        {
            Requirements = new Dictionary<MaterialData, float>();
            Existing = new List<Data.Shared.Component>();
            Reimbursed = new Dictionary<MaterialData, float>();
            StoreExisting = false;
        }

        // Private interaction 

        /// <summary>
        /// Adds the existing component so that required materials are
        /// ignored.
        /// </summary>
        /// <param name="newComp">New comp.</param>
        private void AddExistingComponent(Data.Shared.Component newComp)
        {
            if (!Existing.Contains(newComp))
                Existing.Add(newComp);
        }
        
        /// <summary>
        /// Adds the requirements of the component to the requirements
        /// total list
        /// </summary>
        /// <param name="newComp">New comp.</param>
        private void AddNewComponent(Data.Shared.Component newComp)
        {
            // Retreive respective object GO
            GameObject GO = Resources.Load("Space/Ships/" + newComp.Folder + "/" + newComp.Name, typeof(GameObject)) as GameObject;
            ComponentAttributes att = GO.GetComponent<ComponentAttributes>();

            // Process materials
            if(att.RequiredMats != null)
            {
                foreach(ConstructInfo info in att.RequiredMats)
                {
                    /*MaterialData mat = ElementLibrary.ReturnElement(info.material);
                    if(mat == null)
                        continue;
                    if(Requirements.ContainsKey(mat))
                    {
                        Requirements[mat] += info.amount;
                    }
                    else
                    {
                        Requirements.Add(mat, info.amount);
                    }*/
                }
            }
        }

        // interation with editor

        /// <summary>
        /// Adds the component to requirement object.
        /// </summary>
        /// <param name="newComp">New comp.</param>
        public void AddComponent(Data.Shared.Component newComp)
        {
            if (StoreExisting)
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
        public void RemoveComponent(Data.Shared.Component newComp)
        {
            // Retrive corresponding GO
            GameObject GO = Resources.Load("Space/Ships/" + newComp.Folder + "/" + newComp.Name, typeof(GameObject)) as GameObject;
            ComponentAttributes att = GO.GetComponent<ComponentAttributes>();
            if (Existing.Contains(newComp))
            {
                // this was an original piece
                // Reimburse player data
                if (att.RequiredMats != null)
                {
                    // Create dictionary list
                    Dictionary<MaterialData, float> ReturnAmt = new Dictionary<MaterialData, float>();
                    foreach (ConstructInfo info in att.RequiredMats)
                    {
                        /*MaterialData mat = ElementLibrary.ReturnElement(info.material);

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
                        }*/
                    }
                    // Reimburse player
                    //GameManager.GetPlayer.AddMaterial(ReturnAmt);
                }
                // Remove component from list
                Existing.Remove(newComp);
            } else
            {
                // This is not an original piece and can decrement requirements
                if (att.RequiredMats != null)
                {
                    // Remove amount from store
                    /*foreach(ConstructInfo info in att.RequiredMats)
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
                    }*/
                }
            }
        }

        /// <summary>
        /// Called when 
        /// </summary>
        /// <param name="saved">If set to <c>true</c> saved.</param>
        public void Clear(bool saved)
        {
            if (!saved)
            {
                // Changes were not saved so take material away from player
                // Remove amount from store
                /*foreach(MaterialData mat in Reimbursed.Keys)
                {
                    if(GameManager.GetPlayer.Cargo.ContainsKey(mat))
                    {
                        GameManager.GetPlayer.Cargo[mat] -= Reimbursed[mat];
                        // Remove entry if empty
                        if(GameManager.GetPlayer.Cargo[mat] <= 0)
                            GameManager.GetPlayer.Cargo.Remove(mat);
                    }
                }*/
            }

            Requirements.Clear();
            Reimbursed.Clear();
            Existing.Clear();
        }
    }
}

