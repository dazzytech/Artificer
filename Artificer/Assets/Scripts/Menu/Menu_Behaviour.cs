using UnityEngine;
using System.Collections;

namespace Menu
{
    [RequireComponent(typeof(Menu_Attributes))]

    public class Menu_Behaviour : MonoBehaviour
    {
    	private Menu_Attributes _attributes;

        // CurrentStored State
        public MenuState CurrentState;

        // Delegate event for state changed
        public delegate void ChangeState(MenuState newState);

        public event ChangeState OnStateChanged;

    	void Awake()
    	{
            // Add Attributes
    		_attributes = GetComponent<Menu_Attributes> ();
    	}

    	// Use this for initialization
    	void Start ()
    	{
            _attributes.MenuState = MenuState.None;
            CurrentState = MenuState.None;
            OnStateChanged(MenuState.None);

            // music
            SoundController.ClearPlayList();
            SoundController.PlayMusic(0);
    	}
    	
        void Update()
        {
            if (!_attributes.MenuState.Equals(CurrentState))
                OnStateChanged(CurrentState);
        }
    }
}
