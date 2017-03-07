using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Data.Shared;
using Data.Space.Library;

namespace Menu
{
    [RequireComponent(typeof(SinglePlayer_Behaviour))]

    public class SinglePlayer_EventListener : MonoBehaviour
    {
        private SinglePlayer_Behaviour _controller;

        void Awake()
        {
            _controller = GetComponent<SinglePlayer_Behaviour>();
        }


        public void SelectContract(ContractPrefab contract)
        {
            if(_controller == null)
                _controller = GetComponent<SinglePlayer_Behaviour>();

            if (_controller.SelectedContract != null)
            {
                // Set button color to normal
                _controller.SelectedContract.Btn.image.color 
                    = new Color(0.56f, 0.53f, 0.41f); 
            }

            contract.Btn.image.color 
                = new Color(0.66f, 0.63f, 0.51f);
            _controller.DescriptionText.text 
                = contract.Contract.Description;

            _controller.ObjectivesText.text = "";

            _controller.RewardText.text = "";

            _controller.ObjectivesText.text += 
                "Primary Objectives:\n\n";

            _controller.RewardText.text += 
                "Primary Rewards:\n";

            // Create a list to collect reward items
            Dictionary<MaterialData, float> mats = new Dictionary<MaterialData, float>();

            List<string> comps = new List<string>();

            float xp = 0;

            // process each primary mission

            /*foreach (MissionData m in contract.Contract.PrimaryMissions)
            {
                _controller.ObjectivesText.text +=
                    m.Objectives();
                _controller.ObjectivesText.text += "\n";

                if(m.Reward == null)
                    continue;

                if(m.Reward.Materials != null)
                {
                    foreach(MaterialData mat in m.Reward.Materials.Keys)
                    {
                        if(mats.ContainsKey(mat))
                        {
                            mats[mat] += m.Reward.Materials[mat];
                        }
                        else
                        {
                            mats.Add(mat, m.Reward.Materials[mat]);
                        }
                    }
                }

                if(m.Reward.Components != null)
                {
                    foreach(string comp in m.Reward.Components)
                    {
                        if(!comps.Contains(comp))
                            comps.Add(comp);
                    }
                }

                xp += m.Reward.Xp;
            }

            // Display combined rewards to window
            _controller.RewardText.text += 
                "Materials:\n";

            foreach (MaterialData mat in mats.Keys)
            {
                string amt;
                if((mats[mat] * mat.Density) > 1000)
                    amt = ((mats[mat] * mat.Density)*0.001).ToString("F1")+"Ton(m)";
                else
                    amt = (mats[mat] * mat.Density).ToString("F1")+"KG";

                _controller.RewardText.text += 
                    string.Format(" - {0} - {1}\n", amt, mat.Name); 
            }

            _controller.RewardText.text += 
                "\nComponents:\n";

            foreach (string comp in comps)
            {
                string[] comSplit = comp.Split('/');
                _controller.RewardText.text += 
                    string.Format(" - {0}: {1}\n", comSplit[0],  comSplit[1]); 
            }

            _controller.RewardText.text += 
                string.Format("Exp:{0}\n", xp);

            // Clear lists
            mats.Clear();
            comps.Clear();
            xp = 0;

            _controller.RewardText.text += 
                "\nOptional Rewards:\n";

            _controller.ObjectivesText.text += 
                "\nOptional Objectives:\n\n";
            
            foreach (MissionData e in contract.Contract.OptionalMissions)
            {
                _controller.ObjectivesText.text +=
                    e.Objectives();
                _controller.ObjectivesText.text += "\n";

                if(e.Reward == null)
                    continue;

                if(e.Reward.Materials != null)
                {
                    foreach(MaterialData mat in e.Reward.Materials.Keys)
                    {
                        if(mats.ContainsKey(mat))
                        {
                            mats[mat] += e.Reward.Materials[mat];
                        }
                        else
                        {
                            mats.Add(mat, e.Reward.Materials[mat]);
                        }
                    }
                }
                
                if(e.Reward.Components != null)
                {
                    foreach(string comp in e.Reward.Components)
                    {
                        if(!comps.Contains(comp))
                            comps.Add(comp);
                    }
                }

                xp += e.Reward.Xp;
            }

            // Display combined rewards to window
            _controller.RewardText.text += 
                "Materials:\n";
            
            foreach (MaterialData mat in mats.Keys)
            {
                string amt;
                if((mats[mat] * mat.Density) > 1000)
                    amt = ((mats[mat] * mat.Density)*0.001).ToString("F1")+"Ton(m)";
                else
                    amt = (mats[mat] * mat.Density).ToString("F1")+"KG";
                
                _controller.RewardText.text += 
                    string.Format(" - {0} - {1}\n", amt, mat.Name); 
            }
            
            _controller.RewardText.text += 
                "\nComponents:\n";
            
            foreach (string comp in comps)
            {
                string[] comSplit = comp.Split('/');
                _controller.RewardText.text += 
                    string.Format(" - {0}: {1}\n", comSplit[0],  comSplit[1]); 
            }

            _controller.RewardText.text += 
                string.Format("Exp:{0}\n", xp);*/

            _controller.SelectedContract = contract;
        }

        public void StartContract()
        {
            if (_controller.SelectedContract != null)
            {
                //SystemManager.SetContract(_controller.SelectedContract.Contract.ID);
            }
        }

        public void EnterComponentBuilder()
        {
        }

        public void OpenShipPanel()
        {
            if (!_controller.ShipSelectPanel.activeSelf)
                _controller.ShipSelectPanel.SetActive(true);
        }

        public void CloseShipPanel()
        {
            if (_controller.ShipSelectPanel.activeSelf)
                _controller.ShipSelectPanel.SetActive(false);
        }

        public void ClearPlayerData()
        {
            Popup_Dialog.ShowPopup("Confirm Player Reset", "Are you sure you wish to reset your entire progress? " +
                "It cannot be undone", DialogType.YESNO);
            Popup_Dialog.OnDialogEvent += ConfirmDeletion;
        }

        public void ConfirmDeletion(DialogResult result, object ignore)
        {
            if (result == DialogResult.YES)
            {
                string path = ("Space/Player_Data");
                File.Delete(path);
                //SystemManager.ResetPlayer();
            }

            Menu.Popup_Dialog.OnDialogEvent -= ConfirmDeletion;
        }

        public void BuildShip(string ship)
        {

            ShipData item = ShipLibrary.GetShip(ship);

            // TEST THAT WE HAVE ENOUGH MATERIAL
            foreach (string matName in item.GetRequirements().Keys)
            {
                /*if(SystemManager.GetPlayer.Cargo == null)
                    SystemManager.GetPlayer.Cargo = 
                        new System.Collections.Generic.Dictionary<MaterialData, float>();

                MaterialData mat = ElementLibrary.ReturnElement(matName);

                if(!SystemManager.GetPlayer.Cargo.ContainsKey(mat))
                    return;

                if(!(SystemManager.GetPlayer.Cargo[mat] >= item.GetRequirements()[matName]))
                    return;*/
            }

            // EXPEND MATERIAL IN CARGO
            foreach (string matName in item.GetRequirements().Keys)
            {
                /*if(SystemManager.GetPlayer.Cargo == null)
                    SystemManager.GetPlayer.Cargo = 
                        new System.Collections.Generic.Dictionary<MaterialData, float>();

                MaterialData mat = ElementLibrary.ReturnElement(matName);
                SystemManager.GetPlayer.Cargo[mat] -= item.GetRequirements()[matName];
            */}

            // Add ship to inventory
            //SystemManager.GetPlayer.AddShip(item);
            // Set Ship to selected
            //SystemManager.GetPlayer.SetShip(item.Name);

            // Refresh button items in panel
            foreach (Transform child in _controller.ShipGrid.transform)
            {
                child.gameObject.SendMessage("Refresh");
            }
        }

        public void SelectShip(string ship)
        {
            // Set Ship to selected
            //SystemManager.GetPlayer.SetShip(ship);

            // Refresh button items in panel
            foreach (Transform child in _controller.ShipGrid.transform)
            {
                child.gameObject.SendMessage("Refresh");
            }
        }
    }
}

