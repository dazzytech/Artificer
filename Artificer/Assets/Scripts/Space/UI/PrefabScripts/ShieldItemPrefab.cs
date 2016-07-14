using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// Artificer Defined
using Space.Ship.Components.Listener;
using Space.Ship.Components.Attributes;

namespace Space.UI
{
    public class ShieldItemPrefab : MonoBehaviour
    {
        public Text Label;
        public Slider Amount;

        public ShieldListener listener;

        public void SetShield(ShieldListener list)
        {
            listener = list;
        }
    	
        // Update is called once per frame
        void Update()
        {
    	    if (listener != null)
            {
                ShieldAttributes att = listener.GetAttributes() as ShieldAttributes;
                if(listener.GetAttributes().active)
                {
                    if(!Amount.gameObject.activeSelf)
                        Amount.gameObject.SetActive(true);

                    if(Label.gameObject.activeSelf)
                        Label.gameObject.SetActive(false);

                    Amount.value = (att.CurrentIntegrity/att.ShieldIntegrity);

                }
                else
                {
                    if(att.Destroyed)
                    {
                        if(Amount.gameObject.activeSelf)
                            Amount.gameObject.SetActive(false);
                        
                        if(!Label.gameObject.activeSelf)
                            Label.gameObject.SetActive(true);
                        
                        Label.text = "Recharging";
                    }
                    else
                    {
                        if(Amount.gameObject.activeSelf)
                            Amount.gameObject.SetActive(false);
                    
                        if(!Label.gameObject.activeSelf)
                            Label.gameObject.SetActive(true);
                    
                        Label.text = "Ready";
                    }
                }
            }
        }
    }
}

