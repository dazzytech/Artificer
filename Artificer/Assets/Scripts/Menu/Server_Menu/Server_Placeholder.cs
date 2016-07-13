using UnityEngine;
using UnityEngine.UI;

using System.Collections;

/// <summary>
/// Currently serves as a quick alterative to a full 
/// server 
/// </summary>
namespace Menu.Server
{
    public class Server_Placeholder : MonoBehaviour
    {
        public InputField address;

        void Awake()
        {
            address.text = "localhost";
        }

        public void StartHost()
        {
            Debug.Log("Placeholder");
            GameManager.CreateHostedGame();
        }

        public void JoinClient()
        {
            Debug.Log("Placeholder");
            GameManager.JoinAsClient(address.text);
        }
    }
}
