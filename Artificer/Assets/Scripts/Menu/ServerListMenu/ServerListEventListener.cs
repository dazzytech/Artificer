using UnityEngine;
using System.Collections;

namespace Menu.Server
{
    [RequireComponent(typeof(ServerListController))]
    [RequireComponent(typeof(ServerListAttributes))]
    public class ServerListEventListener : MonoBehaviour
    {
        #region ATTRIBUTES

        [SerializeField]
        private ServerListAttributes m_att;

        [SerializeField]
        private ServerListController m_con;

        #endregion
    }
}
