using UnityEngine;
using System.Collections;
// Artificer
using Data.Space;

namespace Data.Space.Library
{
    // STORES A COPY OF ALL SHIP BUILDS
    public class ShipLibrary {
    	static ShipData[] _data;

    	/// <summary>
    	/// Used by ShipDataImporter
    	/// </summary>
    	/// <param name="data">Data.</param>
    	public void AssignData(ShipData[] data)
    	{
    		_data = data;
    	}

    	/// <summary>
    	/// Gets the ship from the library
    	/// </summary>
    	/// <returns>The ship.</returns>
    	/// <param name="index">Index.</param>
    	public static ShipData GetShip(int index)
    	{
    		if(index >= 0 && index < _data.Length)
    			return _data[index];

    		Debug.Log ("Ship Library - Get Ship: index falls out of bounds!");
    		return new ShipData();
    	}

        public static ShipData GetShip(string name)
        {
            if (_data == null)
            {
                Debug.Log ("Ship Library - Get Ship: Data object has not been created");
                return new ShipData();
            }

            foreach (ShipData ship in _data)
                if (ship.Name == name)
                    return ship;
            
            Debug.Log ("Ship Library - Get Ship: name of ship does not exist!" + name);
            return new ShipData();
        }

        public static ShipData[] GetAll()
        {
            if (_data == null)
            {
                Debug.Log ("Ship Library - Get Ship: Data object has not been created");
                return null;
            }

            return _data;
        }
    }
}