using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class SkillItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public int Code;
    public int Level = 1;
    public bool Passive = false;
    public string ShortcutKey = "<color=#666>- %1 -</color>";
    public GameObject DescriptionWindow;
    public MagicShortcutManager ShortcutMgr;
    public bool DisplayAbove = false;
    public LocalizationManager Localization;

    bool DescWinOpen = false;
    int ManaCost;
    Sprite Icon;
    GUIController GUI;
    CanvasScaler Scaler;

    private void Awake()
    {
        if (DescriptionWindow != null)
        {
            DescriptionWindow.SetActive(false);
        }
    }

    private void Start()
    {
        if(Localization == null)
        {
            Localization = transform.Find("/LocalizationManager").GetComponent<LocalizationManager>();
        }

        var playerCharactersNode = transform.Find("/PlayerCharacters");
        if(playerCharactersNode == null)
        {
            return;
        }

        string playerSkin = playerCharactersNode.GetComponent<PlayerSkinSelector>().GetPlayerSkin();

        GUI = playerCharactersNode.transform.Find(playerSkin).gameObject.GetComponent<PlayerMovement>().GUI;
        Scaler = GUI.GetCanvasScaler();

        ShortcutKey = ShortcutKey.Replace("%1", Localization.Translate("N/A"));
    }

    public string GetTitle()
    {
        switch (Code)
        {
            case 9001:
                return Localization.Translate("MAGIC_FIREBALL");
            case 9002:
                return Localization.Translate("MAGIC_ICYFIRE");
            case 9003:
                return Localization.Translate("MAGIC_FREEZER");
            case 9101:
                return Localization.Translate("MAGIC_TOMBRAIDER");
            case 9102:
                return Localization.Translate("MAGIC_LIGHTNINGSHIELD");
            case 9103:
                return Localization.Translate("MAGIC_CURSEDARMOR");
            case 9201:
                return Localization.Translate("MAGIC_FIRESHIELD");
            case 9202:
                return Localization.Translate("MAGIC_KISSOFTHEDRAGON");
            case 9203:
                return Localization.Translate("MAGIC_CHAINSHIELD");
            case 9301:
                return Localization.Translate("MAGIC_FIREARROW");
            case 9302:
                return Localization.Translate("MAGIC_ICYDREAMS");
            case 9303:
                return Localization.Translate("MAGIC_LIGHTNINGHARPOON");
        }

        return Localization.Translate("DETAILS");
    }

    public string GetDescription()
    {
        ManaCost = 12;

        string param = "";
        string param2 = "";

        switch (Code)
        {
            case 9001:
                switch(Level)
                {
                    case 1:
                        ManaCost = 20;
                        param = "6-8";
                        break;
                    case 2:
                        ManaCost = 21;
                        param = "10-14";
                        break;
                    case 3:
                        ManaCost = 22;
                        param = "20-26";
                        break;
                }

                return Localization.Translate("MAGIC_FIREBALL_DESC_L1").Replace("%1", param);
            case 9002:
                switch (Level)
                {
                    case 1:
                        ManaCost = 20;
                        param = "4-6";
                        param2 = "3";
                        break;
                    case 2:
                        ManaCost = 21;
                        param = "8-11";
                        param2 = "4";
                        break;
                    case 3:
                        ManaCost = 22;
                        param = "14-18";
                        param2 = "5";
                        break;
                }

                return Localization.Translate("MAGIC_ICYFIRE_DESC_L1").Replace("%1", param).Replace("%2", param2);
            case 9003:
                switch (Level)
                {
                    case 1:
                        ManaCost = 30;
                        param = "3";
                        break;
                    case 2:
                        ManaCost = 32;
                        param = "5";
                        break;
                    case 3:
                        ManaCost = 36;
                        param = "9";
                        break;
                }

                return Localization.Translate("MAGIC_FREEZER_DESC_L1").Replace("%1", param);
            case 9101:
                return Localization.Translate("MAGIC_TOMBRAIDER_DESC_L1");
            case 9102:
                return Localization.Translate("MAGIC_LIGHTNINGSHIELD_DESC_L1");
            case 9103:
                switch (Level)
                {
                    case 1:
                        ManaCost = 20;
                        param = "10";
                        break;
                    case 2:
                        ManaCost = 23;
                        param = "25";
                        break;
                    case 3:
                        ManaCost = 28;
                        param = "60";
                        break;
                }

                return Localization.Translate("MAGIC_CURSEDARMOR_DESC_L1").Replace("%1", param);
            case 9201:
                switch (Level)
                {
                    case 1:
                        ManaCost = 20;
                        param = "2";
                        break;
                    case 2:
                        ManaCost = 23;
                        param = "5";
                        break;
                    case 3:
                        ManaCost = 28;
                        param = "15";
                        break;
                }

                return Localization.Translate("MAGIC_FIRESHIELD_DESC_L1").Replace("%1", param);
            case 9202:
                switch (Level)
                {
                    case 1:
                        ManaCost = 20;
                        param = "3";
                        break;
                    case 2:
                        ManaCost = 23;
                        param = "7";
                        break;
                    case 3:
                        ManaCost = 28;
                        param = "21";
                        break;
                }

                return Localization.Translate("MAGIC_KISSOFTHEDRAGON_DESC_L1").Replace("%1", param);
            case 9203:
                return Localization.Translate("MAGIC_CHAINSHIELD_DESC_L1");
            case 9301:
                switch (Level)
                {
                    case 1:
                        ManaCost = 1;
                        param = "1";
                        break;
                    case 2:
                        ManaCost = 3;
                        param = "3";
                        break;
                    case 3:
                        ManaCost = 5;
                        param = "5";
                        break;
                }

                return Localization.Translate("MAGIC_FIREARROW_DESC_L1").Replace("%1", param);
            case 9302:
                switch (Level)
                {
                    case 1:
                        ManaCost = 3;
                        param = "2";
                        break;
                    case 2:
                        ManaCost = 10;
                        param = "5";
                        break;
                    case 3:
                        ManaCost = 25;
                        param = "10";
                        break;
                }

                return Localization.Translate("MAGIC_ICYDREAMS_DESC_L1").Replace("%1", param);
            case 9303:
                switch (Level)
                {
                    case 1:
                        ManaCost = 1;
                        param = "1";
                        break;
                    case 2:
                        ManaCost = 3;
                        param = "3";
                        break;
                    case 3:
                        ManaCost = 5;
                        param = "5";
                        break;
                }

                return Localization.Translate("MAGIC_LIGHTNINGHARPOON_DESC_L1").Replace("%1", param);
        }

        return "- " + Localization.Translate("N/A") + " -";
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(DescriptionWindow == null)
        {
            return;
        }

        DescriptionWindow.SetActive(true);
        DescWinOpen = true;
        if (DisplayAbove)
        {
            DescriptionWindow.transform.position = new Vector2(Input.mousePosition.x - 48, Input.mousePosition.y + 135);
        }
        else
        {
            DescriptionWindow.transform.position = new Vector2(Input.mousePosition.x - 48, Input.mousePosition.y - 145);
        }

        try
        {
            DescriptionWindow.transform.Find("Caption/Text").GetComponent<Text>().text = GetTitle();
            DescriptionWindow.transform.Find("Text").GetComponent<Text>().text = GetDescription();
            DescriptionWindow.transform.Find("TxtCost").GetComponent<Text>().text = ManaCost.ToString();
            DescriptionWindow.transform.Find("TxtPassive").gameObject.SetActive(Passive);
            DescriptionWindow.transform.Find("TxtKey").GetComponent<Text>().text = ShortcutKey;
        } catch
        {

        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (DescriptionWindow == null)
        {
            return;
        }

        DescriptionWindow.SetActive(false);
        DescWinOpen = false;
    }


    // Update is called once per frame
    void Update()
    {
        if (DescWinOpen && DescriptionWindow != null && Scaler != null)
        {
            if (DisplayAbove)
            {
                DescriptionWindow.transform.position = new Vector2(Input.mousePosition.x - 48 * (Screen.width / Scaler.referenceResolution.x), Input.mousePosition.y + 125 * (Screen.height / Scaler.referenceResolution.y));
                //DescriptionWindow.transform.position = new Vector2(Input.mousePosition.x - 48, Input.mousePosition.y + 135);
            }
            else
            {
                DescriptionWindow.transform.position = new Vector2(Input.mousePosition.x - 48 * (Screen.width / Scaler.referenceResolution.x), Input.mousePosition.y - 145 * (Screen.height / Scaler.referenceResolution.y));
                //DescriptionWindow.transform.position = new Vector2(Input.mousePosition.x - 48, Input.mousePosition.y - 145);
            }

            Icon = GetComponent<Image>().sprite;

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                ShortcutMgr.SetMagicToShortcutKey(Code, 1, Icon, Passive, ShortcutKey);
                ShortcutKey = "<color=#ff0>1</color>";
                DescriptionWindow.transform.Find("TxtKey").GetComponent<Text>().text = ShortcutKey;
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                ShortcutMgr.SetMagicToShortcutKey(Code, 2, Icon, Passive, ShortcutKey);
                ShortcutKey = "<color=#ff0>2</color>";
                DescriptionWindow.transform.Find("TxtKey").GetComponent<Text>().text = ShortcutKey;
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                ShortcutMgr.SetMagicToShortcutKey(Code, 3, Icon, Passive, ShortcutKey);
                ShortcutKey = "<color=#ff0>3</color>";
                DescriptionWindow.transform.Find("TxtKey").GetComponent<Text>().text = ShortcutKey;
            }

            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                ShortcutMgr.SetMagicToShortcutKey(Code, 4, Icon, Passive, ShortcutKey);
                ShortcutKey = "<color=#ff0>4</color>";
                DescriptionWindow.transform.Find("TxtKey").GetComponent<Text>().text = ShortcutKey;
            }

            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                ShortcutMgr.SetMagicToShortcutKey(Code, 5, Icon, Passive, ShortcutKey);
                ShortcutKey = "<color=#ff0>5</color>";
                DescriptionWindow.transform.Find("TxtKey").GetComponent<Text>().text = ShortcutKey;
            }

            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                ShortcutKey = "<color=#ff0>6</color>";
                DescriptionWindow.transform.Find("TxtKey").GetComponent<Text>().text = ShortcutKey;
            }

            if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                ShortcutKey = "<color=#ff0>7</color>";
                DescriptionWindow.transform.Find("TxtKey").GetComponent<Text>().text = ShortcutKey;
            }

            if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                ShortcutKey = "<color=#ff0>8</color>";
                DescriptionWindow.transform.Find("TxtKey").GetComponent<Text>().text = ShortcutKey;
            }

            if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                ShortcutKey = "<color=#ff0>9</color>";
                DescriptionWindow.transform.Find("TxtKey").GetComponent<Text>().text = ShortcutKey;
            }

            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                ShortcutKey = "<color=#ff0>0</color>";
                DescriptionWindow.transform.Find("TxtKey").GetComponent<Text>().text = ShortcutKey;
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            /*switch(Code)
            {

            }*/
        }
    }
}
