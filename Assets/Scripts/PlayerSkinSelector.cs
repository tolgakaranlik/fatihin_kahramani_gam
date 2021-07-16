using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSkinSelector : MonoBehaviour
{
    public string PlayerSkin = "ArcherFemale";
    public GameObject RTT;
    public GUIController GUI;
    public DialogueManager DialogueMgr;

    void Start()
    {
        // Skin
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(transform.GetChild(i).name == GetPlayerSkin());
        }

        // RTT
        for (int i = 0; i < RTT.transform.childCount; i++)
        {
            RTT.transform.GetChild(i).gameObject.SetActive((RTT.transform.GetChild(i).name == GetPlayerSkin() + "RTT") || RTT.transform.GetChild(i).name == "RTTCam");
        }

        GameObject player = GetPlayerObject();
        GUI.Equipment = player.GetComponent<PlayerEquipment>();
        GUI.Attributes = player.GetComponent<CharAttributes>();
        DialogueMgr.DefaultSprite = player.GetComponent<Image>().sprite;
    }

    public GameObject GetPlayerObject()
    {
        return transform.Find(GetPlayerSkin()).gameObject;
    }

    public string GetPlayerSkin()
    {
        if (PlayerSkin == "auto")
        {
            switch (PlayerPrefs.GetInt("SelectedCharacter"))
            {
                case 0:
                    PlayerSkin = "ArcherFemale";
                    break;
                case 1:
                    PlayerSkin = "MageMale";
                    break;
            }
        }
        
        return PlayerSkin;
    }
}
