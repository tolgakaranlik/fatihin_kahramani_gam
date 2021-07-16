using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.UI;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

public class GUIController : MonoBehaviour
{
    public GameObject InventoryWindow;
    public GameObject EquipmentWindow;
    public GameObject SkillsWindow;
    public GameObject MainMenuWindow;
    public GameObject DialogWindow;
    public GameObject DescriptionWindowInventory;
    public GameObject DescriptionWindowEquipment;
    public GameObject DescriptionWindowShop;
    public GameObject RTTArea;
    public GameObject InventoryMesage;
    public GameObject DlgAutoSpeech;
    public GameObject ShopWindow;
    public GameObject QuestWindow;
    public GameObject QuestItem;
    public GameObject QuestItemParent;
    public GameObject QuestDetails;
    public GameObject QuestNoItems;
    public GameObject QuestSelectToDisplayDetails;
    public GameObject WinMap;
    public GameObject LevelUpDisplay;
    public Text GoldDisplay;
    public InventoryManager Inventory;
    public QuestManager QuestMgr;
    public PlayerEquipment Equipment;
    public Slider GaugeLife;
    public Slider GaugeMana;
    public Slider GaugeXP;
    public CharAttributes Attributes;

    public delegate bool ShopItemClickDelegate(InventoryItem param);

    [HideInInspector]
    public bool GUIItemClicked;

    GameObject[][] InventoryItems;
    GameObject[] InventoryEquipment;
    DialogItem[] DialogResponses;
    DialogItem NextAutoSpeechItem;
    TweenerCore<float, float, FloatOptions> AutoSpeechCountdown;
    
    bool AutoSpeechCoroutinePaused;
    bool CanDisplayInventoryMessage;
    int AutoSpeechIndex = 0;
    bool DialogueOpen;
    bool EquipmentOpen;
    bool InventoryOpen;
    bool SkillsOpen;
    bool QuestsOpen;
    bool ShopOpen;
    bool MapOpen;
    bool MenuOpen;

    const int EQUIPMENT_HEAD = 0;
    const int EQUIPMENT_SHOULDER = 1;
    const int EQUIPMENT_TORSO = 2;
    const int EQUIPMENT_CLOAK = 3;
    const int EQUIPMENT_GLOVES = 4;
    const int EQUIPMENT_ABDOMEN = 5;
    const int EQUIPMENT_FEET = 6;
    const int EQUIPMENT_LEGS = 7;
    const int EQUIPMENT_LEFT_HAND = 8;
    const int EQUIPMENT_RIGHT_HAND = 9;

    const int EQUIPMENT_NECK = 11;
    const int EQUIPMENT_RING1 = 12;
    const int EQUIPMENT_RING2 = 13;
    const int EQUIPMENT_RING3 = 14;
    const int EQUIPMENT_RING4 = 15;

    // Start is called before the first frame update
    void Start()
    {
        MainMenuWindow.SetActive(false);
        EquipmentWindow.SetActive(false);
        InventoryWindow.SetActive(false);
        InventoryMesage.SetActive(false);
        LevelUpDisplay.SetActive(false);
        DlgAutoSpeech.SetActive(false);
        SkillsWindow.SetActive(false);
        DialogWindow.SetActive(false);
        QuestWindow.SetActive(false);
        ShopWindow.SetActive(false);
        RTTArea.SetActive(false);
        WinMap.SetActive(false);

        CanDisplayInventoryMessage = true;
        GUIItemClicked = false;
        EquipmentOpen = false;
        InventoryOpen = false;
        DialogueOpen = false;
        SkillsOpen = false;
        MenuOpen = false;
        ShopOpen = false;
        MapOpen = false;

        // Inventory items
        InventoryItems = new GameObject[Inventory.SlotWidth][];
        for (int i = 0; i < Inventory.SlotWidth; i++)
        {
            InventoryItems[i] = new GameObject[Inventory.SlotWidth];
            for (int j = 0; j < Inventory.SlotHeight; j++)
            {
                InventoryItems[i][j] = transform.Find("WinInventory/Background/IconPlace" + (i + 1) + "_" + (j + 1)).gameObject;
            }
        }

        // Inventory equipment
        InventoryEquipment = new GameObject[10]; // 15
        for (int i = 0; i < InventoryEquipment.Length; i++)
        {
            InventoryEquipment[i] = transform.Find("WinEquipment/IconEquipment_" + (i + 1)).gameObject;
        }

        // Dialog responses
        DialogResponses = new DialogItem[6];
        for (int i = 0; i < DialogResponses.Length; i++)
        {
            DialogResponses[i] = null;
        }

        QuestNoItems.SetActive(true);
        QuestItem.SetActive(false);
        DisplayQuestDetail(-1);
    }

    public void DisplaySkills()
    {
        HideQuests();

        if (DialogueOpen)
        {
            HideDialogue();
        }

        var trans = SkillsWindow.GetComponent<RectTransform>();
        trans.anchoredPosition = new Vector2(0f, 940f);
        SkillsWindow.SetActive(true);

        DOTween.To(() => trans.anchoredPosition, x => trans.anchoredPosition = x, new Vector2(0f, 45f), 0.35f);
        SkillsOpen = true;
    }

    public void DisplayShop(string title, InventoryItem[] items, string buttonCaption, bool prizeComparison, ShopItemClickDelegate clickAction)
    {
        HideDialogue();
        HideInventory();
        HideEquipment();

        ShopOpen = true;
        ShopWindow.SetActive(true);
        ShopWindow.transform.Find("Text").GetComponent<Text>().text = title;

        GameObject item;
        int price;
        if (items != null)
        {
            // enumerate items to sell
            for (int i = 0; i < Math.Min(10, items.Length); i++)
            {
                // fill occupied slots
                price = prizeComparison ? (int)Mathf.Ceil(items[i].SellValue * 1.5f) : items[i].SellValue;

                item = ShopWindow.transform.Find("IconItem_" + (i + 1)).gameObject;
                item.SetActive(true);
                item.transform.GetChild(0).GetComponent<Image>().sprite = items[i].Glyph;
                item.transform.GetChild(1).GetComponent<Text>().text = items[i].GetTitle();
                item.transform.GetChild(2).GetComponent<Text>().text = price.ToString("N0");
                item.transform.GetChild(4).transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = buttonCaption;
                item.transform.GetChild(4).GetComponent<Button>().onClick.RemoveAllListeners();

                var newComponent = item.transform.GetChild(0).gameObject.AddComponent<InventoryItem>();
                newComponent.Copy(items[i]);
                newComponent.DescriptionWindow = DescriptionWindowShop;

                if (prizeComparison)
                {
                    if (items[i].GetInventoryManager().Gold > price)
                    {
                        item.transform.GetChild(2).GetComponent<Text>().color = Color.yellow;
                    }
                    else
                    {
                        item.transform.GetChild(2).GetComponent<Text>().color = Color.red;
                    }
                } else
                {
                    item.transform.GetChild(2).GetComponent<Text>().color = Color.yellow;
                }

                var action = clickAction;
                var param = items[i];
                item.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(delegate
                {
                    GUIItemClicked = true;

                    //var result = action?.Invoke(param);
                    action?.Invoke(param);
                });
            }

            if (items.Length < 10)
            {
                for (int i = items.Length; i < 10; i++)
                {
                    // clear non used slots
                    item = ShopWindow.transform.Find("IconItem_" + (i + 1)).gameObject;
                    item.SetActive(false);
                }
            }
        } else
        {
            // no items to sell
            for (int i = 0; i < 10; i++)
            {
                // clear non used slots
                item = ShopWindow.transform.Find("IconItem_" + (i + 1)).gameObject;
                item.SetActive(false);
            }
        }
    }

    public void PrepareItems()
    {
        for (int i = 0; i < Inventory.SlotWidth; i++)
        {
            for (int j = 0; j < Inventory.SlotHeight; j++)
            {
                if (Inventory.Slots[0][i][j] != null)
                {
                    InventoryItems[i][j].transform.GetChild(0).GetComponent<Image>().sprite = Inventory.Slots[0][i][j].Glyph;
                    InventoryItems[i][j].transform.GetChild(0).GetComponent<Image>().color = Color.white;

                    InventoryItem newComponent = InventoryItems[i][j].GetComponent<InventoryItem>();
                    if (newComponent == null)
                    {
                        newComponent = InventoryItems[i][j].AddComponent<InventoryItem>();
                    }

                    newComponent.Copy(Inventory.Slots[0][i][j]);
                    newComponent.DescriptionWindow = DescriptionWindowInventory;
                    newComponent.ItemSource = InventoryItem.ItemSourceType.Inventory;
                    newComponent.ItemSourceParam1 = i;
                    newComponent.ItemSourceParam2 = j;
                }
                else
                {
                    if (InventoryItems[i][j].GetComponent<InventoryItem>() != null)
                    {
                        Destroy(InventoryItems[i][j].GetComponent<InventoryItem>());
                    }

                    InventoryItems[i][j].transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 0);
                }
            }
        }
    }

    public void PrepareEquipment()
    {
        // Head
        if (Equipment.Head != null)
        {
            InventoryEquipment[EQUIPMENT_HEAD].GetComponent<Image>().color = Color.white; 
            InventoryEquipment[EQUIPMENT_HEAD].transform.Find("Icon").GetComponent<Image>().sprite = Equipment.Head.Glyph;
            InventoryEquipment[EQUIPMENT_HEAD].transform.Find("Icon").GetComponent<Image>().color = Color.white;

            if(InventoryEquipment[EQUIPMENT_HEAD].GetComponent<InventoryItem>() != null)
            {
                Destroy(InventoryEquipment[EQUIPMENT_HEAD].GetComponent<InventoryItem>());
            }

            var newComponent = InventoryEquipment[EQUIPMENT_HEAD].AddComponent<InventoryItem>();
            newComponent.Copy(Equipment.Head);
            newComponent.DescriptionWindow = DescriptionWindowEquipment;
            newComponent.ItemSource = InventoryItem.ItemSourceType.Equipment;
            newComponent.ItemSourceParam1 = EQUIPMENT_HEAD;

            Equipment.Head = newComponent;
        }
        else
        {
            InventoryEquipment[EQUIPMENT_HEAD].GetComponent<Image>().color = new Color(1, 1, 1, 0);
            InventoryEquipment[EQUIPMENT_HEAD].transform.Find("Icon").GetComponent<Image>().color = new Color(1, 1, 1, 0);
        }

        // Shoulder
        if (Equipment.Shoulder != null)
        {
            InventoryEquipment[EQUIPMENT_SHOULDER].GetComponent<Image>().color = Color.white; 
            InventoryEquipment[EQUIPMENT_SHOULDER].transform.Find("Icon").GetComponent<Image>().sprite = Equipment.Shoulder.Glyph;
            InventoryEquipment[EQUIPMENT_SHOULDER].transform.Find("Icon").GetComponent<Image>().color = Color.white;

            if (InventoryEquipment[EQUIPMENT_SHOULDER].GetComponent<InventoryItem>() != null)
            {
                Destroy(InventoryEquipment[EQUIPMENT_SHOULDER].GetComponent<InventoryItem>());
            }

            var newComponent = InventoryEquipment[EQUIPMENT_SHOULDER].AddComponent<InventoryItem>();
            newComponent.Copy(Equipment.Shoulder);
            newComponent.DescriptionWindow = DescriptionWindowEquipment;
            newComponent.ItemSource = InventoryItem.ItemSourceType.Equipment;
            newComponent.ItemSourceParam1 = EQUIPMENT_SHOULDER;

            Equipment.Shoulder = newComponent;
        }
        else
        {
            InventoryEquipment[EQUIPMENT_SHOULDER].GetComponent<Image>().color = new Color(1, 1, 1, 0);
            InventoryEquipment[EQUIPMENT_SHOULDER].transform.Find("Icon").GetComponent<Image>().color = new Color(1, 1, 1, 0);
        }

        // Torso
        if (Equipment.Torso != null)
        {
            InventoryEquipment[EQUIPMENT_TORSO].GetComponent<Image>().color = Color.white; 
            InventoryEquipment[EQUIPMENT_TORSO].transform.Find("Icon").GetComponent<Image>().sprite = Equipment.Torso.Glyph;
            InventoryEquipment[EQUIPMENT_TORSO].transform.Find("Icon").GetComponent<Image>().color = Color.white;

            if (InventoryEquipment[EQUIPMENT_TORSO].GetComponent<InventoryItem>() == null)
            {
                InventoryEquipment[EQUIPMENT_TORSO].AddComponent<InventoryItem>();
            }

            var newComponent = InventoryEquipment[EQUIPMENT_TORSO].GetComponent<InventoryItem>();
            newComponent.Copy(Equipment.Torso);
            newComponent.DescriptionWindow = DescriptionWindowEquipment;
            newComponent.ItemSource = InventoryItem.ItemSourceType.Equipment;
            newComponent.ItemSourceParam1 = EQUIPMENT_TORSO;

            Equipment.Torso = newComponent;
        }
        else
        {
            InventoryEquipment[EQUIPMENT_TORSO].GetComponent<Image>().color = new Color(1, 1, 1, 0);
            InventoryEquipment[EQUIPMENT_TORSO].transform.Find("Icon").GetComponent<Image>().color = new Color(1, 1, 1, 0);
        }

        // Cloak
        if (Equipment.Cloak != null)
        {
            InventoryEquipment[EQUIPMENT_CLOAK].GetComponent<Image>().color = Color.white; 
            InventoryEquipment[EQUIPMENT_CLOAK].transform.Find("Icon").GetComponent<Image>().sprite = Equipment.Cloak.Glyph;
            InventoryEquipment[EQUIPMENT_CLOAK].transform.Find("Icon").GetComponent<Image>().color = Color.white;

            if (InventoryEquipment[EQUIPMENT_CLOAK].GetComponent<InventoryItem>() != null)
            {
                Destroy(InventoryEquipment[EQUIPMENT_CLOAK].GetComponent<InventoryItem>());
            }

            var newComponent = InventoryEquipment[EQUIPMENT_CLOAK].AddComponent<InventoryItem>();
            newComponent.Copy(Equipment.Cloak);
            newComponent.DescriptionWindow = DescriptionWindowEquipment;
            newComponent.ItemSource = InventoryItem.ItemSourceType.Equipment;
            newComponent.ItemSourceParam1 = EQUIPMENT_CLOAK;

            Equipment.Cloak = newComponent;
        }
        else
        {
            InventoryEquipment[EQUIPMENT_CLOAK].GetComponent<Image>().color = new Color(1, 1, 1, 0);
            InventoryEquipment[EQUIPMENT_CLOAK].transform.Find("Icon").GetComponent<Image>().color = new Color(1, 1, 1, 0);
        }

        // Gloves
        if (Equipment.Gloves != null)
        {
            InventoryEquipment[EQUIPMENT_GLOVES].GetComponent<Image>().color = Color.white; 
            InventoryEquipment[EQUIPMENT_GLOVES].transform.Find("Icon").GetComponent<Image>().sprite = Equipment.Gloves.Glyph;
            InventoryEquipment[EQUIPMENT_GLOVES].transform.Find("Icon").GetComponent<Image>().color = Color.white;

            if (InventoryEquipment[EQUIPMENT_GLOVES].GetComponent<InventoryItem>() == null)
            {
                InventoryEquipment[EQUIPMENT_GLOVES].AddComponent<InventoryItem>();
            }

            var newComponent = InventoryEquipment[EQUIPMENT_GLOVES].GetComponent<InventoryItem>();
            newComponent.Copy(Equipment.Gloves);
            newComponent.DescriptionWindow = DescriptionWindowEquipment;
            newComponent.ItemSource = InventoryItem.ItemSourceType.Equipment;
            newComponent.ItemSourceParam1 = EQUIPMENT_GLOVES;

            Equipment.Gloves = newComponent;
        }
        else
        {
            InventoryEquipment[EQUIPMENT_GLOVES].GetComponent<Image>().color = new Color(1, 1, 1, 0);
            InventoryEquipment[EQUIPMENT_GLOVES].transform.Find("Icon").GetComponent<Image>().color = new Color(1, 1, 1, 0);
        }

        // Abdomen
        if (Equipment.Abdomen != null)
        {
            InventoryEquipment[EQUIPMENT_ABDOMEN].GetComponent<Image>().color = Color.white; 
            InventoryEquipment[EQUIPMENT_ABDOMEN].transform.Find("Icon").GetComponent<Image>().sprite = Equipment.Abdomen.Glyph;
            InventoryEquipment[EQUIPMENT_ABDOMEN].transform.Find("Icon").GetComponent<Image>().color = Color.white;

            if (InventoryEquipment[EQUIPMENT_ABDOMEN].GetComponent<InventoryItem>() != null)
            {
                Destroy(InventoryEquipment[EQUIPMENT_ABDOMEN].GetComponent<InventoryItem>());
            }

            var newComponent = InventoryEquipment[EQUIPMENT_ABDOMEN].AddComponent<InventoryItem>();
            newComponent.Copy(Equipment.Abdomen);
            newComponent.DescriptionWindow = DescriptionWindowEquipment;
            newComponent.ItemSource = InventoryItem.ItemSourceType.Equipment;
            newComponent.ItemSourceParam1 = EQUIPMENT_ABDOMEN;

            Equipment.Abdomen = newComponent;
        }
        else
        {
            InventoryEquipment[EQUIPMENT_ABDOMEN].GetComponent<Image>().color = new Color(1, 1, 1, 0);
            InventoryEquipment[EQUIPMENT_ABDOMEN].transform.Find("Icon").GetComponent<Image>().color = new Color(1, 1, 1, 0);
        }

        // Feet
        if (Equipment.Feet != null)
        {
            InventoryEquipment[EQUIPMENT_FEET].GetComponent<Image>().color = Color.white; 
            InventoryEquipment[EQUIPMENT_FEET].transform.Find("Icon").GetComponent<Image>().sprite = Equipment.Feet.Glyph;
            InventoryEquipment[EQUIPMENT_FEET].transform.Find("Icon").GetComponent<Image>().color = Color.white;

            if (InventoryEquipment[EQUIPMENT_FEET].GetComponent<InventoryItem>() == null)
            {
                InventoryEquipment[EQUIPMENT_FEET].AddComponent<InventoryItem>();
            }

            var newComponent = InventoryEquipment[EQUIPMENT_FEET].GetComponent<InventoryItem>();
            newComponent.Copy(Equipment.Feet);
            newComponent.DescriptionWindow = DescriptionWindowEquipment;
            newComponent.ItemSource = InventoryItem.ItemSourceType.Equipment;
            newComponent.ItemSourceParam1 = EQUIPMENT_FEET;

            Equipment.Feet = newComponent;
        }
        else
        {
            InventoryEquipment[EQUIPMENT_FEET].GetComponent<Image>().color = new Color(1, 1, 1, 0);
            InventoryEquipment[EQUIPMENT_FEET].transform.Find("Icon").GetComponent<Image>().color = new Color(1, 1, 1, 0);
        }

        // Legs
        if (Equipment.Legs != null)
        {
            InventoryEquipment[EQUIPMENT_LEGS].GetComponent<Image>().color = Color.white;
            InventoryEquipment[EQUIPMENT_LEGS].transform.Find("Icon").GetComponent<Image>().sprite = Equipment.Legs.Glyph;
            InventoryEquipment[EQUIPMENT_LEGS].transform.Find("Icon").GetComponent<Image>().color = Color.white;

            if (InventoryEquipment[EQUIPMENT_LEGS].GetComponent<InventoryItem>() != null)
            {
                Destroy(InventoryEquipment[EQUIPMENT_LEGS].GetComponent<InventoryItem>());
            }

            var newComponent = InventoryEquipment[EQUIPMENT_LEGS].AddComponent<InventoryItem>();
            newComponent.Copy(Equipment.Legs);
            newComponent.DescriptionWindow = DescriptionWindowEquipment;
            newComponent.ItemSource = InventoryItem.ItemSourceType.Equipment;
            newComponent.ItemSourceParam1 = EQUIPMENT_LEGS;

            Equipment.Legs = newComponent;
        }
        else
        {
            InventoryEquipment[EQUIPMENT_LEGS].GetComponent<Image>().color = new Color(1, 1, 1, 0);
            InventoryEquipment[EQUIPMENT_LEGS].transform.Find("Icon").GetComponent<Image>().color = new Color(1, 1, 1, 0);
        }

        // Left hand
        if (Equipment.LeftHand != null)
        {
            InventoryEquipment[EQUIPMENT_LEFT_HAND].GetComponent<Image>().color = Color.white;
            InventoryEquipment[EQUIPMENT_LEFT_HAND].transform.Find("Icon").GetComponent<Image>().sprite = Equipment.LeftHand.Glyph;
            InventoryEquipment[EQUIPMENT_LEFT_HAND].transform.Find("Icon").GetComponent<Image>().color = Color.white;

            if (InventoryEquipment[EQUIPMENT_LEFT_HAND].GetComponent<InventoryItem>() != null)
            {
                Destroy(InventoryEquipment[EQUIPMENT_LEFT_HAND].GetComponent<InventoryItem>());
            }

            var newComponent = InventoryEquipment[EQUIPMENT_LEFT_HAND].AddComponent<InventoryItem>();
            newComponent.Copy(Equipment.LeftHand);
            newComponent.DescriptionWindow = DescriptionWindowEquipment;
            newComponent.ItemSource = InventoryItem.ItemSourceType.Equipment;
            newComponent.ItemSourceParam1 = EQUIPMENT_LEFT_HAND;

            Equipment.LeftHand = newComponent;
        }
        else
        {
            InventoryEquipment[EQUIPMENT_LEFT_HAND].GetComponent<Image>().color = new Color(1, 1, 1, 0);
            InventoryEquipment[EQUIPMENT_LEFT_HAND].transform.Find("Icon").GetComponent<Image>().color = new Color(1, 1, 1, 0);
        }

        // Right hand
        if (Equipment.RightHand != null || (Equipment.LeftHand != null && Equipment.LeftHand.TwoHanded))
        {
            var item = Equipment.RightHand;
            if(item == null)
            {
                item = Equipment.LeftHand;
            }

            InventoryEquipment[EQUIPMENT_RIGHT_HAND].GetComponent<Image>().color = Color.white;
            InventoryEquipment[EQUIPMENT_RIGHT_HAND].transform.Find("Icon").GetComponent<Image>().sprite = item.Glyph;
            InventoryEquipment[EQUIPMENT_RIGHT_HAND].transform.Find("Icon").GetComponent<Image>().color = Color.white;

            if (InventoryEquipment[EQUIPMENT_RIGHT_HAND].GetComponent<InventoryItem>() != null)
            {
                Destroy(InventoryEquipment[EQUIPMENT_RIGHT_HAND].GetComponent<InventoryItem>());
            }

            var newComponent = InventoryEquipment[EQUIPMENT_RIGHT_HAND].AddComponent<InventoryItem>();
            newComponent.Copy(item);
            newComponent.DescriptionWindow = DescriptionWindowEquipment;
            newComponent.ItemSource = InventoryItem.ItemSourceType.Equipment;
            newComponent.ItemSourceParam1 = EQUIPMENT_RIGHT_HAND;

            Equipment.RightHand = newComponent;
        }
        else
        {
            InventoryEquipment[EQUIPMENT_RIGHT_HAND].GetComponent<Image>().color = new Color(1, 1, 1, 0);
            InventoryEquipment[EQUIPMENT_RIGHT_HAND].transform.Find("Icon").GetComponent<Image>().color = new Color(1, 1, 1, 0);
        }

        // Neck
        /*if (Equipment.Neck != null)
        {
            InventoryEquipment[EQUIPMENT_NECK].GetComponent<Image>().sprite = Equipment.Neck.Glyph;
            InventoryEquipment[EQUIPMENT_NECK].GetComponent<Image>().color = Color.white;
        }
        else
        {
            InventoryEquipment[EQUIPMENT_NECK].GetComponent<Image>().color = new Color(1, 1, 1, 0);
        }*/

        // Ring 1
        // Ring 2
        // Ring 3
        // Ring 4
        // Ring 5
    }

    public void DisplayInventory()
    {
        if (DialogueOpen)
        {
            HideDialogue();
        }

        // Initialize displays
        PrepareItems();

        // Display the window
        var trans = InventoryWindow.GetComponent<RectTransform>();
        trans.anchoredPosition = new Vector2(400f, 53f);
        InventoryWindow.SetActive(true);

        DOTween.To(() => trans.anchoredPosition, x => trans.anchoredPosition = x, new Vector2(-350f, 53f), 0.35f);
        InventoryOpen = true;
    }

    public void DisplayEquipment()
    {
        if (DialogueOpen)
        {
            HideDialogue();
        }

        PrepareEquipment();
        RTTArea.SetActive(true);

        // Display character skills
        EquipmentWindow.transform.Find("Attr1/Value").GetComponent<Text>().text = Attributes.AttrStrength.ToString(); // itemlardan gelen ilaveleri de yeşille yanına yazdır
        EquipmentWindow.transform.Find("Attr2/Value").GetComponent<Text>().text = Attributes.AttrSpeed.ToString(); // itemlardan gelen ilaveleri de yeşille yanına yazdır
        EquipmentWindow.transform.Find("Attr3/Value").GetComponent<Text>().text = Attributes.AttrStamina.ToString(); // itemlardan gelen ilaveleri de yeşille yanına yazdır
        EquipmentWindow.transform.Find("Attr4/Value").GetComponent<Text>().text = Attributes.AttrLife.ToString(); // itemlardan gelen ilaveleri de yeşille yanına yazdır
        EquipmentWindow.transform.Find("Attr5/Value").GetComponent<Text>().text = Attributes.AttrArmor.ToString(); // itemlardan gelen ilaveleri de yeşille yanına yazdır
        EquipmentWindow.transform.Find("Attr6/Value").GetComponent<Text>().text = Attributes.AttrMagic.ToString(); // itemlardan gelen ilaveleri de yeşille yanına yazdır

        var trans = EquipmentWindow.GetComponent<RectTransform>();
        trans.anchoredPosition = new Vector2(-400f, 53f);
        EquipmentWindow.SetActive(true);

        DOTween.To(() => trans.anchoredPosition, x => trans.anchoredPosition = x, new Vector2(340f, 53f), 0.35f);
        EquipmentOpen = true;
    }

    public void DisplayMenu()
    {
        if (InventoryOpen)
        {
            HideInventory();
        }

        if (EquipmentOpen)
        {
            HideEquipment();
        }

        if (SkillsOpen)
        {
            HideSkills();
        }

        if(DialogueOpen)
        {
            HideDialogue();
        }

        if (QuestsOpen)
        {
            HideQuests();
        }

        var trans = MainMenuWindow.GetComponent<RectTransform>();
        trans.anchoredPosition = new Vector2(0f, 853f);
        MainMenuWindow.SetActive(true);

        DOTween.To(() => trans.anchoredPosition, x => trans.anchoredPosition = x, new Vector2(0f, 35f), 0.35f);
        MenuOpen = true;

        StartCoroutine(FreezeTime(0.35f));
    }

    public void HideSkills()
    {
        StartCoroutine(HideSkillsNow());
    }

    public void HideInventory()
    {
        StartCoroutine(HideInventoryNow());
    }

    public void HideEquipment()
    {
        StartCoroutine(HideEquipmentNow());
    }

    public void HideMenu()
    {
        Time.timeScale = 1;
        StartCoroutine(HideMenuNow());
    }

    public void HideShop()
    {
        GUIItemClicked = true;

        ShopOpen = false;
        ShopWindow.SetActive(false);
    }

    private IEnumerator FreezeTime(float secs)
    {
        yield return new WaitForSeconds(secs);

        Time.timeScale = 0;
    }

    private IEnumerator HideSkillsNow()
    {
        var trans = SkillsWindow.GetComponent<RectTransform>();
        DOTween.To(() => trans.anchoredPosition, x => trans.anchoredPosition = x, new Vector2(0f, 940f), 0.35f);
        SkillsOpen = false;

        yield return new WaitForSeconds(0.35f);

        SkillsWindow.SetActive(false);
    }

    private IEnumerator HideInventoryNow()
    {
        var trans = InventoryWindow.GetComponent<RectTransform>();
        DOTween.To(() => trans.anchoredPosition, x => trans.anchoredPosition = x, new Vector2(350f, 53f), 0.35f);
        InventoryOpen = false;

        yield return new WaitForSeconds(0.35f);

        InventoryWindow.SetActive(false);
    }

    private IEnumerator HideEquipmentNow()
    {
        var trans = EquipmentWindow.GetComponent<RectTransform>();
        DOTween.To(() => trans.anchoredPosition, x => trans.anchoredPosition = x, new Vector2(-340f, 53f), 0.35f);
        EquipmentOpen = false;

        yield return new WaitForSeconds(0.35f);

        RTTArea.SetActive(false);
        EquipmentWindow.SetActive(false);
    }

    private IEnumerator HideMenuNow()
    {
        var trans = MainMenuWindow.GetComponent<RectTransform>();
        DOTween.To(() => trans.anchoredPosition, x => trans.anchoredPosition = x, new Vector2(0f, 853f), 0.35f);
        MenuOpen = false;

        yield return new WaitForSeconds(0.35f);

        MainMenuWindow.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetButtonUp("Inventory"))
        {
            ToggleInventory();
        }

        if (Input.GetButtonUp("Equipment"))
        {
            ToggleEquipment();
        }

        if (Input.GetButtonUp("Skills"))
        {
            ToggleSkills();
        }

        if (Input.GetButtonUp("Quests"))
        {
            ToggleQuests();
        }

        if (Input.GetButtonUp("GameMenu"))
        {
            ToggleMenu();
        }

        if (Input.GetButtonUp("Map"))
        {
            ToggleMap();
        }

        if (DlgAutoSpeech.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            try
            {
                AutoSpeechIndex += 1;
                AutoSpeechCountdown.Kill();
            }
            catch
            {

            }

            DisplayDialogue(NextAutoSpeechItem);
        }
    }

    public void ToggleInventory()
    {
        GUIItemClicked = true;

        switch (InventoryOpen)
        {
            case false:
                DisplayInventory();
                break;
            case true:
                HideInventory();
                break;
        }
    }

    public void ToggleEquipment()
    {
        GUIItemClicked = true;

        switch (EquipmentOpen)
        {
            case false:
                DisplayEquipment();
                break;
            case true:
                HideEquipment();
                break;
        }
    }

    public void ToggleSkills()
    {
        GUIItemClicked = true;

        switch (SkillsOpen)
        {
            case false:
                DisplaySkills();
                break;
            case true:
                HideSkills();
                break;
        }
    }

    public void ToggleQuests()
    {
        GUIItemClicked = true;

        switch (QuestsOpen)
        {
            case false:
                DisplayQuests();
                break;
            case true:
                HideQuests();
                break;
        }
    }

    public void ToggleMenu()
    {
        GUIItemClicked = true;

        switch (MenuOpen)
        {
            case false:
                DisplayMenu();
                break;
            case true:
                HideMenu();
                break;
        }
    }

    public void SetGold(int newValue)
    {
        GoldDisplay.text = newValue.ToString().PadLeft(8, '0');
    }

    public void SetInventoryMesage(string message)
    {
        if(!CanDisplayInventoryMessage)
        {
            return;
        }

        CanDisplayInventoryMessage = false;
        InventoryMesage.SetActive(true);
        InventoryMesage.transform.Find("Text").GetComponent<Text>().text = message;

        StartCoroutine(InventoryMsgAppear());
    }

    public void SetLifeTo(float newLife)
    {
        DOTween.To(() => GaugeLife.value, x => GaugeLife.value = x, newLife, 0.35f);
    }

    public void SetManaTo(float newMana)
    {
        DOTween.To(() => GaugeMana.value, x => GaugeMana.value = x, newMana, 0.35f);
    }

    IEnumerator InventoryMsgAppear()
    {
        yield return new WaitForSeconds(3);

        CanDisplayInventoryMessage = true;
        InventoryMesage.SetActive(false);
    }

    public void DisplayDialogue(DialogItem dialogue)
    {
        GUIItemClicked = true;

        DialogueOpen = true;
        DlgAutoSpeech.SetActive(false);

        DialogWindow.transform.Find("TxtTitle").GetComponent<Text>().text = dialogue.Title;
        DialogWindow.transform.Find("TxtSpeech").GetComponent<Text>().text = dialogue.SpeechFull;
        DialogWindow.transform.Find("ImgAvatar").gameObject.SetActive(true);

        if (dialogue.Avatar != null)
        {
            DialogWindow.transform.Find("ImgAvatar").GetComponent<Image>().sprite = dialogue.Avatar;

            DialogWindow.transform.Find("TxtName").gameObject.SetActive(true);
            DialogWindow.transform.Find("TxtName").GetComponent<Text>().text = dialogue.FullName;

            DialogWindow.transform.Find("TxtInfo").gameObject.SetActive(true);
            DialogWindow.transform.Find("TxtInfo").GetComponent<Text>().text = dialogue.Info;
        } else
        {
            DialogWindow.transform.Find("ImgAvatar").GetComponent<Image>().sprite = dialogue.Manager.DefaultSprite;

            DialogWindow.transform.Find("TxtName").gameObject.SetActive(false);
            DialogWindow.transform.Find("TxtInfo").gameObject.SetActive(false);
        }

        switch(dialogue.Response)
        {
            case DialogItem.ResponseType.NextDialogue:
                int choiceCount = 0;
                int firstChild = -1;
                for (int i = 0; i < dialogue.gameObject.transform.childCount; i++)
                {
                    if (!dialogue.gameObject.transform.GetChild(i).gameObject.activeSelf)
                    {
                        continue;
                    }

                    if(firstChild == -1)
                    {
                        firstChild = i;
                    }

                    choiceCount += 1;
                }
                
                if (choiceCount != 1)
                {
                    GameObject Button;
                    Text textItem;

                    // Active responses
                    int _i = (6 - choiceCount);
                    for (int i = 0; i < dialogue.gameObject.transform.childCount; i++)
                    {
                        if (!dialogue.gameObject.transform.GetChild(i).gameObject.activeSelf)
                        {
                            continue;
                        }

                        Button = DialogWindow.transform.Find("BtnR" + (_i + 1)).gameObject;
                        Button.SetActive(true);
                        textItem = Button.transform.GetChild(0).GetComponent<Text>();

                        DialogResponses[_i] = dialogue.gameObject.transform.GetChild(i).gameObject.GetComponent<DialogItem>();
                        if (DialogResponses[_i].ItemType == DialogItem.DialogueItemType.Important)
                        {
                            textItem.color = new Color(textItem.color.r, textItem.color.g, textItem.color.b, 1.0f);
                        }
                        else
                        {
                            textItem.color = new Color(textItem.color.r, textItem.color.g, textItem.color.b, 0.15f);
                        }

                        textItem.text = DialogResponses[_i].SpeechSummary;
                        _i += 1;
                    }

                    // Inactive response slots
                    for (int i = 0; i < 6 - choiceCount; i++)
                    {
                        Button = DialogWindow.transform.Find("BtnR" + (i + 1)).gameObject;
                        Button.SetActive(false);
                        DialogResponses[i] = null;
                    }
                } else
                {
                    for (int i = 0; i < 6; i++)
                    {
                        DialogWindow.transform.Find("BtnR" + (i + 1)).gameObject.SetActive(false);
                    }

                    DlgAutoSpeech.SetActive(true);
                    var slider = DlgAutoSpeech.transform.Find("SldSpeechPrc").GetComponent<Slider>();
                    slider.value = 0;

                    try
                    {
                        AutoSpeechCountdown.Kill();
                    } catch
                    {

                    }

                    AutoSpeechCountdown = slider.DOValue(1, 7).SetEase(Ease.Linear);
                    AutoSpeechCoroutinePaused = false;

                    StartCoroutine(AutoSpeech(dialogue.gameObject.transform.GetChild(firstChild).gameObject.GetComponent<DialogItem>()));
                }

                DialogWindow.SetActive(true);
                break;
            case DialogItem.ResponseType.BackToPrevious:
                DisplayDialogue(dialogue.transform.parent.parent.parent.gameObject.GetComponent<DialogItem>());

                break;
            case DialogItem.ResponseType.QuitFromDialogue:
                HideDialogue();

                break;
            case DialogItem.ResponseType.ExecuteAction:
                dialogue.Manager.ExecuteCode(dialogue.Code, dialogue);

                break;
        }
    }

    IEnumerator AutoSpeech(DialogItem item)
    {
        NextAutoSpeechItem = item;
        int index = ++AutoSpeechIndex;
        for (int i = 0; i < 7; i++)
        {
            if(index != AutoSpeechIndex)
            {
                yield break;
            }

            while(AutoSpeechCoroutinePaused)
            {
                yield return null;
            }

            yield return new WaitForSeconds(1f);
        }

        if (index != AutoSpeechIndex)
        {
            yield break;
        }

        while (AutoSpeechCoroutinePaused)
        {
            yield return null;
        }

        DisplayDialogue(item);
    }

    public void PauseDialogue()
    {
        AutoSpeechCoroutinePaused = !AutoSpeechCoroutinePaused;

        switch(AutoSpeechCoroutinePaused)
        {
            case true:
                AutoSpeechCountdown.Pause();
                break;
            case false:
                AutoSpeechCountdown.Play();
                break;
        }
    }

    public void Choosedialogue(int index)
    {
        if(DialogResponses[index] == null)
        {
            return;
        } else
        {
            DisplayDialogue(DialogResponses[index]);
        }
    }

    public void HideDialogue()
    {
        DialogueOpen = false;

        DialogWindow.SetActive(false);
    }

    public bool IsQuestsOpen()
    {
        return QuestsOpen;
    }

    public bool IsDialogueOpen()
    {
        return DialogueOpen;
    }

    public void SetXP(float newLevel)
    {
        GaugeXP.DOValue(newLevel, 0.5f);
    }

    public void DisplayQuests()
    {
        HideSkills();

        var trans = QuestWindow.GetComponent<RectTransform>();
        trans.anchoredPosition = new Vector2(0f, 853f);
        QuestWindow.SetActive(true);

        DisplayQuestItems();

        DOTween.To(() => trans.anchoredPosition, x => trans.anchoredPosition = x, new Vector2(0f, 35f), 0.35f);
        QuestsOpen = true;
    }

    public void HideQuests()
    {
        QuestsOpen = false;
        StartCoroutine(HideQuestsNow());
    }

    public void ToggleMap()
    {
        MapOpen = !MapOpen;

        WinMap.SetActive(MapOpen);
    }

    private IEnumerator HideQuestsNow()
    {
        var trans = QuestWindow.GetComponent<RectTransform>();
        DOTween.To(() => trans.anchoredPosition, x => trans.anchoredPosition = x, new Vector2(0f, 853f), 0.35f);
        QuestsOpen = false;
        GUIItemClicked = true;

        yield return new WaitForSeconds(0.35f);

        QuestWindow.SetActive(false);
    }

    public void ShowLevelUpDisplay()
    {
        HideMenu();

        LevelUpDisplay.transform.DOScale(0f, 0.5f);
        StartCoroutine(ShowLevelDisplay());

        LevelUpDisplay.SetActive(true);
        LevelUpDisplay.GetComponent<AudioSource>().Play();
        StartCoroutine(HideLevelDisplay());
    }

    IEnumerator ShowLevelDisplay()
    {
        LevelUpDisplay.transform.DOScale(1f, 0.5f);

        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator HideLevelDisplay()
    {
        yield return new WaitForSeconds(5f);

        LevelUpDisplay.transform.DOScale(0f, 0.5f);

        yield return new WaitForSeconds(0.6f);

        LevelUpDisplay.SetActive(false);
        LevelUpDisplay.transform.DOScale(1f, 0.5f);
    }

    public CanvasScaler GetCanvasScaler()
    {
        return GetComponent<CanvasScaler>();
    }

    public void DisplayQuestItems()
    {
        // Clear previously created items
        for (int i = 1; i < QuestItemParent.transform.childCount; i++)
        {
            Destroy(QuestItemParent.transform.GetChild(1).gameObject);
        }

        // Add existing quest items
        GameObject newItem;
        int totalItems = 0;
        for (int i = 0; i < QuestMgr.Quests.Length; i++)
        {
            if (QuestMgr.Quests[i].Active && !QuestMgr.Quests[i].Completed)
            {
                // add item
                newItem = Instantiate(QuestItem, QuestItem.transform.position, QuestItem.transform.rotation, QuestItemParent.transform);
                newItem.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 65 * totalItems++);
                newItem.SetActive(true);

                int code = QuestMgr.Quests[i].Code;
                newItem.GetComponent<Button>().onClick.AddListener(delegate
                {
                    DisplayQuestDetail(code);
                });
            }
        }

        QuestNoItems.SetActive(totalItems == 0);

        // Handle right side
        QuestSelectToDisplayDetails.SetActive(true);
    }

    public void DisplayQuestDetail(int code)
    {
        // supply -1 as code argument to hide details
        QuestSelectToDisplayDetails.SetActive(code == -1);

        for (int i = 0; i < QuestDetails.transform.childCount; i++)
        {
            QuestDetails.transform.GetChild(i).gameObject.SetActive(code != -1);
        }

        if(code == -1)
        {
            return;
        }

        QuestItem quest = QuestMgr.GetQuestById(code);
        QuestDetails.transform.GetChild(0).GetComponent<Text>().text = quest.Title;
        QuestDetails.transform.GetChild(1).GetComponent<Text>().text = quest.Description;
        QuestDetails.transform.GetChild(2).transform.GetChild(0).GetComponent<Text>().text = quest.XP.ToString();
        QuestDetails.transform.GetChild(3).transform.GetChild(0).GetComponent<Text>().text = quest.Level.ToString();
        QuestDetails.transform.GetChild(4).transform.GetChild(0).GetComponent<Text>().text = "- yok -";

        GameObject node;
        for (int i = 0; i < quest.Conditions.Length; i++)
        {
            node = QuestDetails.transform.GetChild(5).transform.GetChild(i).gameObject;
            node.SetActive(true);

            node.transform.GetChild(0).gameObject.SetActive(quest.Conditions[i].Completed);
            node.transform.GetChild(1).GetComponent<Text>().text = quest.Conditions[i].Title;
            node.transform.GetChild(1).GetComponent<Text>().color = quest.Conditions[i].Completed ? new Color(0, 0, 0, 0.5f) : new Color(0, 0, 0, 1);
        }

        for (int i = quest.Conditions.Length; i < 5; i++)
        {
            node = QuestDetails.transform.GetChild(5).transform.GetChild(i).gameObject;
            node.SetActive(false);
        }
    }

    public void DropEquipment(InventoryItem item)
    {
        // drop from equipment to inventory
        for (int i = 0; i < Inventory.SlotWidth; i++)
        {
            for (int j = 0; j < Inventory.SlotHeight; j++)
            {
                if (InventoryItems[i][j] != null && RectTransformUtility.RectangleContainsScreenPoint(InventoryItems[i][j].GetComponent<RectTransform>(), Input.mousePosition))
                {
                    // Found at inventory pos i, j
                    if (Inventory.Slots[0][i][j] == null)
                    {
                        // place the item here
                        item.PutMeBack();
                        Inventory.AddItem(0, i, j, item);
                        //InventoryEquipment[item.ItemSourceParam1] = null;

                        switch(item.ItemSourceParam1)
                        {
                            case EQUIPMENT_HEAD:
                                Equipment.Head = null;
                                break;
                            case EQUIPMENT_ABDOMEN:
                                Equipment.Abdomen = null;
                                break;
                            case EQUIPMENT_CLOAK:
                                Equipment.Cloak = null;
                                break;
                            case EQUIPMENT_FEET:
                                Equipment.Feet = null;
                                break;
                            case EQUIPMENT_GLOVES:
                                Equipment.Gloves = null;
                                break;
                            case EQUIPMENT_LEFT_HAND:
                                Equipment.LeftHand = null;
                                break;
                            case EQUIPMENT_LEGS:
                                Equipment.Legs = null;
                                break;
                            case EQUIPMENT_NECK:
                                Equipment.Neck = null;
                                break;
                            case EQUIPMENT_RIGHT_HAND:
                                Equipment.RightHand = null;
                                break;
                            case EQUIPMENT_SHOULDER:
                                Equipment.Shoulder = null;
                                break;
                            case EQUIPMENT_TORSO:
                                Equipment.Torso = null;
                                break;
                            case EQUIPMENT_RING1:
                                Equipment.Ring1 = null;
                                break;
                            case EQUIPMENT_RING2:
                                Equipment.Ring2 = null;
                                break;
                            case EQUIPMENT_RING3:
                                Equipment.Ring3 = null;
                                break;
                            case EQUIPMENT_RING4:
                                Equipment.Ring4 = null;
                                break;
                        }

                        Inventory.ItWorked();
                        PrepareItems();
                        PrepareEquipment();

                        return;
                    }
                }
            }
        }

        item.PutMeBack();
        Inventory.ThatWontWork();
        PrepareEquipment();
    }

    public void DropInventoryItem(InventoryItem item)
    {
        // Dropped from inventory to inventory
        for (int i = 0; i < Inventory.SlotWidth; i++)
        {
            for (int j = 0; j < Inventory.SlotHeight; j++)
            {
                if(InventoryItems[i][j] != null && RectTransformUtility.RectangleContainsScreenPoint(InventoryItems[i][j].GetComponent<RectTransform>(), Input.mousePosition))
                {
                    // Found at inventory pos i, j
                    if(Inventory.Slots[0][i][j] == null)
                    {
                        // place the item here
                        item.PutMeBack();
                        Inventory.MoveItem(0, item.ItemSourceParam1, item.ItemSourceParam2, i, j);

                        Inventory.ItWorked();
                        PrepareItems();

                        return;
                    }
                }
            }
        }

        // drop from inventory to equipment
        for (int i = 0; i < InventoryEquipment.Length; i++)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(InventoryEquipment[i].GetComponent<RectTransform>(), Input.mousePosition))
            {
                switch (i)
                {
                    case EQUIPMENT_HEAD:
                        if (item.Type == InventoryItem.ItemType.Helm && Equipment.Head == null)
                        {
                            item.PutMeBack();
                            Equipment.Head = item;
                            Inventory.RemoveItem(0, item.ItemSourceParam1, item.ItemSourceParam2);

                            PrepareEquipment();
                            Inventory.ItWorked();
                        }

                        PrepareItems();
                        return;
                    case EQUIPMENT_ABDOMEN:
                        if (item.Type == InventoryItem.ItemType.Belt && Equipment.Abdomen == null)
                        {
                            item.PutMeBack();
                            Equipment.Abdomen = item;
                            Inventory.RemoveItem(0, item.ItemSourceParam1, item.ItemSourceParam2);

                            PrepareEquipment();
                            Inventory.ItWorked();
                        }

                        PrepareItems();
                        return;
                    case EQUIPMENT_GLOVES:
                        if (item.Type == InventoryItem.ItemType.Glove && Equipment.Gloves == null)
                        {
                            item.PutMeBack();
                            Equipment.Gloves = item;
                            Inventory.RemoveItem(0, item.ItemSourceParam1, item.ItemSourceParam2);

                            PrepareEquipment();
                            Inventory.ItWorked();
                        }

                        PrepareItems();
                        return;
                    case EQUIPMENT_FEET:
                        if (item.Type == InventoryItem.ItemType.Boots && Equipment.Feet == null)
                        {
                            item.PutMeBack();
                            Equipment.Feet = item;
                            Inventory.RemoveItem(0, item.ItemSourceParam1, item.ItemSourceParam2);

                            PrepareEquipment();
                            Inventory.ItWorked();
                        }

                        PrepareItems();
                        return;
                    case EQUIPMENT_SHOULDER:
                        if (item.Type == InventoryItem.ItemType.Shoulder && Equipment.Shoulder == null)
                        {
                            item.PutMeBack();
                            Equipment.Shoulder = item;
                            Inventory.RemoveItem(0, item.ItemSourceParam1, item.ItemSourceParam2);

                            PrepareEquipment();
                            Inventory.ItWorked();
                        }

                        PrepareItems();
                        return;
                    case EQUIPMENT_TORSO:
                        if (item.Type == InventoryItem.ItemType.Armor && Equipment.Torso == null)
                        {
                            item.PutMeBack();
                            Equipment.Torso = item;
                            Inventory.RemoveItem(0, item.ItemSourceParam1, item.ItemSourceParam2);

                            PrepareEquipment();
                            Inventory.ItWorked();
                        }

                        PrepareItems();
                        return;
                    case EQUIPMENT_LEFT_HAND:
                        if ((item.Type == InventoryItem.ItemType.Sword || item.Type == InventoryItem.ItemType.Bow || item.Type == InventoryItem.ItemType.Staff || item.Type == InventoryItem.ItemType.Shield) && Equipment.LeftHand == null)
                        {
                            item.PutMeBack();
                            Equipment.LeftHand = item;
                            Inventory.RemoveItem(0, item.ItemSourceParam1, item.ItemSourceParam2);

                            PrepareEquipment();
                            Inventory.ItWorked();
                        }

                        PrepareItems();
                        return;
                    case EQUIPMENT_NECK:
                        if (item.Type == InventoryItem.ItemType.Necklace && Equipment.Neck == null)
                        {
                            item.PutMeBack();
                            Equipment.Neck = item;
                            Inventory.RemoveItem(0, item.ItemSourceParam1, item.ItemSourceParam2);

                            PrepareEquipment();
                            Inventory.ItWorked();
                        }

                        PrepareItems();
                        return;
                }
            }
        }

        item.PutMeBack();
        Inventory.ThatWontWork();
    }
}
