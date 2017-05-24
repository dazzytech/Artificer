using UnityEngine;
using System.Collections;
using System.Collections.Generic;
// Artificer
using Data.Space;

// THIS MAY BE THE MAIN STORAGE ATTRIBUTE OF THE PLAYER
// SHIP DATA, LOCATIONAL DATA AND RESOURCES
namespace Data.Shared
{
    [System.Serializable]
    public class PlayerData
    {
    	//-										// What ships the player has
    	private List<ShipData> 
    		_storedShips;
    	//-										// Where the player is in space 
    	private Vector2 
    		_spaceCoordinates;

        private Vector2                         // Where the player is within segment
            _localCoordinates;

        private Vector2                         // The up vector
            _localUp;
            
        //-                                     // Player storage
        //public Dictionary<MaterialData, float> Cargo;

        //public Dictionary<Data.Shared.Component, int> StoredComps;

    	private ShipData _currentShip;

        // -                                    // Stores requirements for each level
        private float[] levelReqs = new float[10]
        {100f, 200f, 400f, 800f, 1600f, 2400f, 4800f, 8600f, 16000f, 32000f};

        public List<int> Completedlevels;

        public float XP;

        public int Level;

        public List<string> Components;

    	/*public void AddShip(ShipData newShip)
    	{
    		if (_storedShips == null)
    			_storedShips = new List<ShipData> ();
    		_storedShips.Add (newShip);

    		if (_currentShip == null)
    			_currentShip = newShip;
    	}

        public void SetShip(int shipIndex)
        {
            if(shipIndex >= 0 && shipIndex < _storedShips.Count)
                _currentShip = _storedShips [shipIndex];
        }

        public void SetShip(string shipName)
        {
            foreach (ShipData ship in _storedShips)
                if (ship.Name == shipName)
                    _currentShip = ship;
        }

        public void AddMaterial(Dictionary<MaterialData, float> data)
        {
            if(Cargo == null)
               Cargo = new Dictionary<MaterialData, float>();
            
            foreach (MaterialData mat in data.Keys)
            {
                float valueToAdd = data[mat];
                
                if(Cargo.ContainsKey(mat))
                    Cargo[mat] += valueToAdd;  // add amount to current supply
                else if(valueToAdd > 0)
                    Cargo.Add(mat, valueToAdd);
            }
        }

        public void AddComponent(List<string> comps)
        {
            if (Components == null)
                Components = new List<string>();

            foreach (string comp in comps)
            {
                if(!Components.Contains(comp))
                    Components.Add(comp);
            }
        }

        public void AddXP(float xp)
        {
            XP += xp;

            if (XP >= levelReqs [Level])
                Level++;
        }



    	// GET AND SET FUNCTIONS

        public float NextLvl
        {
            get{ return levelReqs [Level] - XP;}
        }

    	public ShipData Ship
    	{
    		get { return _currentShip;}
    	}

        public List<ShipData> ShipList
        {
            get 
            { 
                if(_storedShips == null)
                    _storedShips = new List<ShipData>();
                return _storedShips;
            }
            set { _storedShips = value;}
        }

        /// <summary>
        /// Gets the location in space coordinates of the
        /// player
        /// </summary>
    	public Vector2 SpaceLocation
    	{
            get { return _spaceCoordinates;}
            set { _spaceCoordinates = value;}
    	}

        /// <summary>
        /// Gets the local location coordinates of the
        /// player
        /// </summary>
        public Vector2 LocalLocation
        {
            get { return _localCoordinates;}
            set { _localCoordinates = value;}
        }

        public Vector2 LocalUp
        {
            get { return _localUp;}
            set { _localUp = value;}
        }*/
    }
}

