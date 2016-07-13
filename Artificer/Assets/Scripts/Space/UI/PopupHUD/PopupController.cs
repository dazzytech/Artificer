using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Data.Shared;

public class PopupController : MonoBehaviour 
{
    /*public Text Message;

    // panels
    public GameObject MissionFailed;
    public GameObject MissionPassed;
    public GameObject ShipRespawn;

    // Store reward displays
    public Text rewardText;

    enum PopupType{Win, Lose, Respawn}
    PopupType _type;

    // timer
    float _timer;

    public void SetShipRespawnCounter(float timer)
    {
        _type = PopupType.Respawn;
        _timer = timer;
    }

    public void EndGame(bool won)
    {
        if (won)
            _type = PopupType.Win;
        else
            _type = PopupType.Lose;
    }

    public void UpdateReward(RewardInfo reward)
    {
        rewardText.text = "You are awarded the following\n";
        rewardText.text += 
            "\nMaterials:\n";
        foreach (MaterialData mat in reward.Materials.Keys)
        {
            string amt;
            if((reward.Materials[mat] * mat.Density) > 1000)
                amt = ((reward.Materials[mat] * mat.Density)*0.001).ToString("F1")+"Ton(m)";
            else
                amt = (reward.Materials[mat] * mat.Density).ToString("F1")+"KG";
            
            rewardText.text += 
                string.Format(" - {0} - {1}\n", amt, mat.Name); 
        }
        
        rewardText.text += 
            "\nComponents:\n";
        
        foreach (string comp in reward.Components)
        {
            string[] comSplit = comp.Split('/');
            rewardText.text += 
                string.Format(" - {0}: {1}\n", comSplit[0],  comSplit[1]); 
        }
        
        rewardText.text += 
            string.Format("\nExp:{0}\n", reward.Xp);
    }

    void Update()
    {
        switch(_type)
        {
            case PopupType.Lose:
                if(!MissionFailed.activeSelf)
                    MissionFailed.SetActive(true);

                if(ShipRespawn.activeSelf)
                    ShipRespawn.SetActive(false);
                if(MissionPassed.activeSelf)
                    MissionPassed.SetActive(false);
                break;

            case PopupType.Win:
                if(!MissionPassed.activeSelf)
                    MissionPassed.SetActive(true);

                if(MissionFailed.activeSelf)
                    MissionFailed.SetActive(false);
                if(ShipRespawn.activeSelf)
                    ShipRespawn.SetActive(false);
                break;
            case PopupType.Respawn:
                if(!ShipRespawn.activeSelf)
                    ShipRespawn.SetActive(true);
                _timer -= Time.deltaTime;
                Message.text = "Player respawning in " + _timer.ToString("F0") + " seconds.";

                if(MissionFailed.activeSelf)
                    MissionFailed.SetActive(false);
                if(MissionPassed.activeSelf)
                    MissionPassed.SetActive(false);
                break;
        }
    }*/
}
