using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Space.CameraUtils
{
    /// <summary>
    /// presents a single large image with a galaxy background
    /// and scrolls it when player moves 
    /// </summary>
    public class GalaxyScroller : MonoBehaviour
    {
        public Texture2D[] Textures;

        public GameObject Background;

        #region MONO BEHAVIOUR

        // Use this for initialization
        void Awake()
        {
            Object[] objs = Resources.LoadAll("Textures/GalaxyTextures", typeof(Texture2D));
            Textures = new Texture2D[objs.Length];
            int i = 0;
            foreach (Object ob in objs)
                Textures[i++] = ob as Texture2D;

            Background = new GameObject();
            Background.layer = LayerMask.NameToLayer("SpaceElements");
            Background.AddComponent<SpriteRenderer>();
            Background.transform.SetParent(this.transform);
        }

        void OnDestroy()
        {
            StopCoroutine("CycleScroller");
        }

        #endregion

        #region PUBLIC INTERACTION

        public void AssignBG(int index, Camera cam)
        {
            Texture2D newBG = Textures[index];

            SpriteRenderer sr = Background.GetComponent<SpriteRenderer>();

            sr.sprite =
                Sprite.Create(newBG, new Rect(0, 0, newBG.width, newBG.height),
                new Vector2(0.5f, 0.5f));
            sr.sortingOrder = -1;
            sr.sortingLayerName = "Background";

            Background.transform.localScale = new Vector3(1, 1, 1);

            float width = sr.sprite.bounds.size.x;
            float height = sr.sprite.bounds.size.y;


            float worldScreenHeight = cam.orthographicSize * 2f;
            float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

            Vector3 newScale = new Vector3();

            Vector3 xWidth = Background.transform.localScale;
            xWidth.x = worldScreenWidth / width;
            Background.transform.localScale = xWidth;
            newScale.x = worldScreenWidth / width;
            Vector3 yHeight = Background.transform.localScale;
            yHeight.y = worldScreenHeight / height;
            Background.transform.localScale = yHeight;
            newScale.y = worldScreenHeight / height;

            Background.transform.localScale = newScale;
        }

        #endregion

        #region COROUTINES

        /// <summary>
        /// When the playership is active, detect if the 
        /// </summary>
        /// <returns></returns>
        private IEnumerator CycleScroller()
        {
            yield return null;
        }

        #endregion
    }
}