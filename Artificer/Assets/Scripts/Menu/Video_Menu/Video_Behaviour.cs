using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Menu
{
    [RequireComponent(typeof(Video_EventListener))]

    public class Video_Behaviour : HUDPanel
    {
        // Resolution list item prefab
        public GameObject ResolutionItemPrefab;
        // Resolution list
        public Transform ResolutionList;

        // UI ELEMENTS
        // Stores quaility level
        public Text QualityLabel;
        public Toggle Fullscreen;
        public Text vSync;
        public Text AA;
        public int aa;
        public Text TexQual;
        public int tex;

        // event listener
        private Video_EventListener _listener;

        void Awake()
        {
            // Assign listener obj
            _listener = GetComponent<Video_EventListener>();
        }

        // Use this for initialization
        protected override void OnEnable()
        {
            base.OnEnable();

    	    // Build the resolution list using all possible resolutions
            if (ResolutionList != null)
            {
                // delete previous
                foreach (Transform child in ResolutionList.transform)
                    Destroy(child.gameObject);

                foreach(Resolution res in Screen.resolutions)
                {
                    if(res.width < Video_Config.MIN_WIDTH || res.height < Video_Config.MIN_HEIGHT)
                        continue;

                    // create resolution prefab
                    ResolutionListItem Res =
                        CreateButton(res);

                    // Highlight current resolution
                    if(Video_Config.Video.IsCurrent(Res.resolution))
                        _listener.SelectResolution(Res);
                }
            }

            // Toggle fullscreen with default/loaded settings
            _listener.SetFullscreen(Screen.fullScreen);

            // Toggle VSync
            vSync.text = Video_Config.Video.VSync.ToString();

            QualityLabel.text = Video_Config.Video.Settings;

            aa = QualitySettings.antiAliasing;
            AA.text = Video_Config.Video.GetAA;
            tex = QualitySettings.masterTextureLimit;
            TexQual.text = Video_Config.Video.GetTexQual;
        }
    	

        /// <summary>
        /// Creates the button from resolution prefab.
        /// </summary>
        /// <returns>The button.</returns>
        /// <param name="res">Res.</param>
        public ResolutionListItem CreateButton(Resolution res)
        {
            GameObject ResBtn = Instantiate(ResolutionItemPrefab) as GameObject;
            ResBtn.transform.SetParent(ResolutionList, false);

            ResolutionListItem Res = ResBtn.GetComponent<ResolutionListItem>();
            Res.SetResolution(res, _listener);
            Res.button.image.color = new Color(0f,0f,0f,0f);
            Res.button.
            onClick.AddListener(
                    delegate{_listener.SelectResolution(Res);});

            return Res;
        }
    }
}

