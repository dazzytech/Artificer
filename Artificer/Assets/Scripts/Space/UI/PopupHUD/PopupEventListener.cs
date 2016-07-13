using UnityEngine;
using System.Collections;

/// <summary>
/// Popup event listener.
/// listens for button presses
/// </summary>
public class PopupEventListener : MonoBehaviour 
{
    public void ExitToMenu()
    {
        GameManager.Disconnect();
    }
}
