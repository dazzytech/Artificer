using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Space.UI
{
    public class MsgParam
    {
        public string style;
        public string messege;
        public MsgParam(string style, string messege)
        {
            this.style = style;
            this.messege = messege;
        }
    }

    public class MessageHUD: MonoBehaviour
    {
        private static MessageHUD instance = null;

        private static int messegeCount = 0;
        private static int messegeMax = 20;

        //public GameObject txtItemPrefab;
        //public Transform txtPanel;
        public Font font;

        void Awake()
        {
            if (instance == null)
                instance = this;

            messegeCount = 0;
            messegeMax = 20;
        }

        public static void DisplayMessege(MsgParam param)
        {
            GameObject disp = new GameObject();
            Text text = disp.AddComponent<Text>();
            //disp.transform.localScale = new Vector3(1f, 1f, 1f);
            //disp.transform.localPosition = new Vector3(0f, 0f, 0f);
            disp.transform.SetParent(GameObject.Find("MessageList").transform, false);

                switch (param.style)
                {
                    case "small":
                        text.color = new Color(1, 1, 1, 1);
                        text.fontSize = 8;
                        text.alignment = TextAnchor.UpperLeft;
                        break;
                    case "sm-red":
                        text.color = new Color(1, .1f, .1f, 1);
                        text.fontSize = 8;
                        text.alignment = TextAnchor.UpperLeft;
                        break;
                    case "sm-green":
                        text.color = new Color(.2f, 1, .1f, 1);
                        text.fontSize = 8;
                        text.alignment = TextAnchor.UpperLeft;
                        break;
                    case "md-green":
                        text.color = new Color(0.2f, 1, .1f, 1);
                        text.fontSize = 10;
                        text.alignment = TextAnchor.UpperLeft;
                        break;
                    case "md-yellow":
                        text.color = new Color(0.5f, .5f, .1f, 1);
                        text.fontSize = 10;
                        text.alignment = TextAnchor.UpperLeft;
                        break;
                    case "md-red":
                        text.color = new Color(1f, .1f, .1f, 1);
                        text.fontSize = 10;
                        text.alignment = TextAnchor.UpperLeft;
                        break;
                    case "bold":
                        text.color = new Color(.6f, 1f, .6f, 1f);
                        text.fontSize = 10;
                        text.fontStyle = FontStyle.Bold;
                        text.alignment = TextAnchor.UpperLeft;
                        break;
                }

            text.font = instance.font;
            text.text = param.messege;
            messegeCount++;

            // clear top messege if count reaches max
            if (messegeCount > messegeMax)
            {
                for(int i = 0; i < 5; i++)
                {
                    GameObject.Destroy(GameObject.Find("MessageList").transform.GetChild(i).gameObject);
                    messegeCount--;
                }
            }

        }

        /*private IEnumerator Count() make create wipe/clear functionality
        {
            yield return new WaitForSeconds(1f);
            messegeCount--;
        }

        private void MsgWipe()
        {
            Text text = instance.transform.Find("Messege").GetComponent<Text>();
            text.text = "";
        }*/
    }
}
