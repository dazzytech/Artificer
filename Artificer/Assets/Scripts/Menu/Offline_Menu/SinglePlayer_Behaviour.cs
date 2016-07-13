using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

using Data.Shared;
// temp use
using Data.Space.Library;
using System;

namespace Menu
{
    public class SinglePlayer_Behaviour : MonoBehaviour 
    {
        private SinglePlayer_EventListener _listener;
        // Panel tabs for generator menu
        public GameObject ContractPanel;

        // Prefabs
        public GameObject contractPrefab;

        public RawImage ShipImage;
        public string current;

        // Contract
        public ContractPrefab SelectedContract;
        public Transform ContractList;
        public Text DescriptionText;
        public Text ObjectivesText;
        public Text RewardText;

        // Player Panel
        public GameObject ShipSelectPanel;
        public GameObject ShipItemPrefab;
        public GameObject ShipGrid;

        // Player Exp
        public Text CurrentExp;
        public Text CurrentLvl;
        public Text NextLvl;

    	// Use this for initialization
    	void Awake () {
            _listener = GetComponent<SinglePlayer_EventListener>();
    	}

        void OnEnable()
        {
            if (ShipSelectPanel.activeSelf)
                ShipSelectPanel.SetActive(false);

            //PopulateShipList();
            //BuildContractList();
           // UpdateXP();
        }

        void Start()
        {
            // Load universe instances
            // Retreive all the directories
            if (!File.Exists("Space"))
                Directory.CreateDirectory("Space");
        }
        
        // Update is called once per frame
    	void Update () 
        {
           if (!ContractPanel.activeSelf)
               ContractPanel.SetActive(true);

            /*
            // Generate Ship Image
            if (GameManager.GetPlayer.Ship != null)
            {
                if(current != GameManager.GetPlayer.Ship.Name)
                {
                    current = GameManager.GetPlayer.Ship.Name;
                    if(GameManager.GetPlayer.Ship.IconTex == null)
                        return;

                    Texture2D tex = GameManager.GetPlayer.Ship.IconTex;

                    if(tex == null)
                        return;
                    
                    ShipImage.texture = tex;
                    ShipImage.transform.localScale = new Vector3(tex.width*0.002f, tex.height*0.002f);
                }
            }*/
    	}

        private void GenerateContract(ContractData contract)
        {
            GameObject ConBtn = Instantiate(contractPrefab);
            ConBtn.transform.SetParent(ContractList);

            ContractPrefab Con = ConBtn.GetComponent<ContractPrefab>();
            Con.Contract = contract;
            Con.Name.text = contract.Name;
            Con.label.text = contract.CType.ToString();
            Con.Btn.
                onClick.AddListener(
                    delegate{_listener.SelectContract(Con);});

            if (SelectedContract == null)
                _listener.SelectContract(Con);
        }

        private void PopulateShipList()
        {
            // Will break if player doesnt exist yet
            //if (GameManager.GetPlayer == null)
                //return;

            // Clear out ship panel 
            foreach (Transform child in ShipGrid.transform)
            {
                Destroy(child.gameObject);
            }

            /*
            // Generate list of firstly using ships currently owned by the player
            foreach (ShipData ship in GameManager.GetPlayer.ShipList)
            {
                if(ship.IconTex == null)
                {
                    Texture2D newTex;
                    newTex = Resources.Load("Space/Ships/Icon/" 
                        + ship.Name, typeof(Texture2D)) as Texture2D;
                    
                    if(newTex != null)
                        ship.IconTex = newTex;
                    else
                    {
                        try
                        {
                            byte[] fileData;
                            if(File.Exists("Space/icons/" + 
                                           ship.Name));
                            {
                                string path = (System.Environment.CurrentDirectory) + "/Space/icons";
                                fileData = File.ReadAllBytes(path + "/" +
                                                             ship.Name +".png");
                                newTex = new Texture2D(2, 2);
                                newTex.LoadImage(fileData);
                                ship.IconTex = newTex;
                            }
                        }
                        catch(Exception e)
                        {
                            Debug.Log(e.Message);
                        };
                    }
                }*/

                // Create a ship item for the shop window
                /*GameObject shipItem = Instantiate(ShipItemPrefab);
                shipItem.transform.SetParent(ShipGrid.transform);
                
                ShipItemPrefab si = shipItem.GetComponent<ShipItemPrefab>();
                si.SetShipData(ship, _listener);
            }*/

            // Next generate using standard ships that the player can buy
            /*foreach (ShipData ship in ShipLibrary.GetAll())
            {
                // Check this is one the player can have and does not currently have
                if (!ship.PlayerMade || GameManager.GetPlayer.ShipList.Contains(ship))
                    continue;

                if(ship.IconTex == null)
                {
                    Texture2D newTex;
                    newTex = Resources.Load("Space/Ships/Icon/" 
                                            + ship.Name, typeof(Texture2D)) as Texture2D;

                    if(newTex != null)
                        ship.IconTex = newTex;
                }
                
                // Create a ship item for the shop window
                GameObject shipItem = Instantiate(ShipItemPrefab);
                shipItem.transform.SetParent(ShipGrid.transform);
                
                ShipItemPrefab si = shipItem.GetComponent<ShipItemPrefab>();
                si.SetShipData(ship, _listener);
            }*/
        }

        /*private void BuildContractList()
        {
            if (!ContractLibrary.ContractExists())
            {
                return;
            }

            // Clear out ship panel 
            foreach (Transform child in ContractList.transform)
            {
                Destroy(child.gameObject);
            }

            // move this as it is null
            foreach (ContractData contract in ContractLibrary.ReturnContractList())
            {
                if(GameManager.GetPlayer.Level >= contract.Requirement)
                    contract.Locked = false;
                else
                    contract.Locked = true;

                if(!contract.Locked)
                    GenerateContract(contract);
            }
        }

        private void UpdateXP()
        {
            if (GameManager.GetPlayer != null)
            {
                CurrentExp.text = string.Format("{0}:XP", GameManager.GetPlayer.XP);
                CurrentLvl.text = GameManager.GetPlayer.Level.ToString();

                NextLvl.text = string.Format("{0}:XP", GameManager.GetPlayer.NextLvl);
            }
        }*/
    }
}
