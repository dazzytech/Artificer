using Data.UI;
using System.Collections;
using System.Collections.Generic;
using UI;
using UI.Effects;
using UnityEngine;

namespace Space.UI.IDE
{
    /// <summary>
    /// Interacts with the scripting UI
    /// placing nodes and drawing links
    /// </summary>
    public class ScriptUI : MonoBehaviour
    {
        #region ATTIBUTES

        [SerializeField]
        private EditorManager m_con;

        [SerializeField]
        private Material m_lineMat;

        [Header("UI Prefabs")]

        [SerializeField]
        private GameObject m_NodePrefab;

        [Header("UI Elements")]

        [SerializeField]
        private Transform m_scriptEditor;

        #endregion

        private void OnEnable()
        {
            Initialize();
        }

        #region PUBLIC INTERACTION

        public void Initialize()
        {
            StartCoroutine("RenderLines");
        }

        public void UpdateUI()
        {
            if (EditorManager.SelectedObj != null)
            {
                if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
                {
                    // Deselect object if we click outside selected piece or component window
                    if (!RectTransformExtension.InBounds
                        (EditorManager.SelectedObj.GetComponent<RectTransform>(),
                        Input.mousePosition))
                    {
                        EditorManager.SelectedObj = null;
                        return;
                    }
                }
            }
        }

        public NodePrefab PlaceNode(NodeData node)
        {
            GameObject nodeGO = Instantiate(m_NodePrefab);
            NodePrefab newNode = nodeGO.GetComponent<NodePrefab>();
            newNode.InitializeNode(node);
            nodeGO.transform.SetParent(m_scriptEditor);

            return newNode;
        }

        #endregion

        private void DrawLine(Vector3 a, Vector3 b, Color col)
        {
            GL.Begin(GL.LINES);
            m_lineMat.SetPass(0);
            GL.Color(col);
            GL.Vertex3(a.x, Screen.height - a.y, a.z);
            GL.Vertex3(b.x, Screen.height - b.y, b.z);
            GL.End();
        }

        #region COROUTINES

        private IEnumerator RenderLines()
        {
            while(true)
            {
                yield return new WaitForEndOfFrame();

                if(EditorManager.IODraggingLink != null)
                {
                    DrawLine
                        (EditorManager.IODraggingLink.IconBounds.position,
                        Input.mousePosition, EditorManager.IODraggingLink.Colour);
                }
                foreach(LinkRender line in m_con.RenderList)
                {
                    DrawLine(line.Start.IconBounds.position, line.End.IconBounds.position,
                        line.Colour);
                }

                yield return null;
            }
        }

        #endregion
    }
}