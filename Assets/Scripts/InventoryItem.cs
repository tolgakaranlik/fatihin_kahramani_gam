using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class InventoryItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IDragHandler, IDropHandler
{
    public enum ItemType { Useless, Gold, Sword, Staff, Bow, Shield, Armor, Helm, Shoulder, Necklace, Glove, Belt, Boots, PotionLife, PotionMana, PotionAntidote, PotionRejuvenation, Gem };
    public ItemType Type;

    public int Code;
    public int SellValue;
    public GameObject DescriptionWindow;

    bool DescWinOpen = false;

    public bool CanDrag = true;
    public int MinLevel = -1;
    public int MaxLevel = -1;
    public bool TwoHanded = false;
    public int MinDamage = 1;
    public int MaxDamage = 2;
    public int MinArmor = 0;
    public int MaxArmor = 0;

    public bool CanSell;
    public int AttrStrength;
    public int AttrStamina;
    public int AttrLife;
    public int AttrMagic;
    public int AttrSpeed;
    public int AttrArmor;
    public Sprite Glyph;
    public AudioClip ClipUse;

    public InventoryItem[] Sockets;
    public LocalizationManager Localization;

    public enum ItemSourceType { Inventory, Equipment, Other };
    public ItemSourceType ItemSource = ItemSourceType.Other;
    public int ItemSourceParam1 = 0;
    public int ItemSourceParam2 = 0;

    CharAttributes PlayerAttributes;
    InventoryManager Inventory;
    PlayerEquipment Equipment;
    GUIController GUI;
    CanvasScaler Scaler;
    Vector3 LastItemPos;
    bool Dragging;

    private void Awake()
    {
        CanSell = false;
        AttrStrength = 0;
        AttrStamina = 0;
        AttrLife = 0;
        AttrMagic = 0;
        AttrSpeed = 0;
        AttrArmor = 0;
    }

    private void Start()
    {
        Dragging = false;
        var PlayerCharactersNode = transform.Find("/PlayerCharacters");
        string playerSkin = PlayerCharactersNode.GetComponent<PlayerSkinSelector>().GetPlayerSkin();

        PlayerAttributes = PlayerCharactersNode.transform.Find(playerSkin).gameObject.GetComponent<CharAttributes>();
        Inventory = transform.Find("/InventoryManager").gameObject.GetComponent<InventoryManager>();
        Equipment = PlayerCharactersNode.transform.Find(playerSkin).gameObject.GetComponent<PlayerEquipment>();
        GUI = PlayerCharactersNode.transform.Find(playerSkin).gameObject.GetComponent<PlayerMovement>().GUI;
        Scaler = GUI.GetCanvasScaler();

        Localization = transform.Find("/LocalizationManager").GetComponent<LocalizationManager>();
    }

    public InventoryManager GetInventoryManager()
    {
        return Inventory;
    }

    public PlayerEquipment GetEquipment()
    {
        return Equipment;
    }

    public string GetTitle()
    {
        switch(Code)
        {
            case 100:
                return Localization.Translate("GOLD");
            case 101:
                return Localization.Translate("LIFE_POTION_MINI");
            case 102:
                return Localization.Translate("LIFE_POTION_SMALL");
            case 103:
                return Localization.Translate("LIFE_POTION_MEDIUM");
            case 104:
                return Localization.Translate("LIFE_POTION_LARGE");
            case 105:
                return Localization.Translate("LIFE_POTION_HUGE");
            case 106:
                return Localization.Translate("MANA_POTION_MINI");
            case 107:
                return Localization.Translate("MANA_POTION_SMALL");
            case 108:
                return Localization.Translate("MANA_POTION_MEDIUM");
            case 109:
                return Localization.Translate("MANA_POTION_LARGE");
            case 110:
                return Localization.Translate("MANA_POTION_HUGE");
            case 111:
                return Localization.Translate("ANTIDOTE_POTION_MINI");
            case 112:
                return Localization.Translate("ANTIDOTE_POTION_SMALL");
            case 113:
                return Localization.Translate("ANTIDOTE_POTION_MEDIUM");
            case 114:
                return Localization.Translate("ANTIDOTE_POTION_LARGE");
            case 115:
                return Localization.Translate("ANTIDOTE_POTION_HUGE");
            case 116:
                return Localization.Translate("REJUVENATION_POTION_MINI");
            case 117:
                return Localization.Translate("REJUVENATION_POTION_SMALL");
            case 118:
                return Localization.Translate("REJUVENATION_POTION_MEDIUM");
            case 119:
                return Localization.Translate("REJUVENATION_POTION_LARGE");
            case 120:
                return Localization.Translate("REJUVENATION_POTION_HUGE");
            case 801:
                return Localization.Translate("BRACER_WOODEN");
            case 901:
                return Localization.Translate("BOOTS_BEAR");
            case 1001:
                return Localization.Translate("USELESS_APPLE_FRESH");
            case 1002:
                return Localization.Translate("USELESS_SPIDER_TOOTH");
            case 1003:
                return Localization.Translate("USELESS_BREAD_LOAF");
            case 98001:
                return Localization.Translate("BOW_LONG");
            case 98002:
                return Localization.Translate("BOW_FLEXI");
            case 96001:
                return Localization.Translate("ARMOR_SILK_CLOTH");
            case 96002:
                return Localization.Translate("SHIELD_BROKEN");
            case 98101:
                return Localization.Translate("SWORD_RUSTY");
            case 98102:
                return Localization.Translate("SWORD_MYSTIQUE");
            case 98103:
                return Localization.Translate("STAFF_OLD");
            case 98104:
                return Localization.Translate("STAFF_MAGE");
        }

        return Localization.Translate("MAGIC_DETAILS");
    }

    public string GetDescription()
    {
        switch (Code)
        {
            case 101:
                return Localization.Translate("LIFE_POTION_MINI_DESC");
            case 102:
                return Localization.Translate("LIFE_POTION_SMALL_DESC");
            case 103:
                return Localization.Translate("LIFE_POTION_MEDIUM_DESC");
            case 104:
                return Localization.Translate("LIFE_POTION_LARGE_DESC");
            case 105:
                return Localization.Translate("LIFE_POTION_HUGE_DESC");
            case 106:
                return Localization.Translate("MANA_POTION_MINI_DESC");
            case 107:
                return Localization.Translate("MANA_POTION_SMALL_DESC");
            case 108:
                return Localization.Translate("MANA_POTION_MEDIUM_DESC");
            case 109:
                return Localization.Translate("MANA_POTION_LARGE_DESC");
            case 110:
                return Localization.Translate("MANA_POTION_HUGE_DESC");
            case 111:
                return Localization.Translate("ANTIDOTE_POTION_MINI_DESC");
            case 112:
                return Localization.Translate("ANTIDOTE_POTION_SMALL_DESC");
            case 113:
                return Localization.Translate("ANTIDOTE_POTION_MEDIUM_DESC");
            case 114:
                return Localization.Translate("ANTIDOTE_POTION_LARGE_DESC");
            case 115:
                return Localization.Translate("ANTIDOTE_POTION_HUGE_DESC");
            case 116:
                return Localization.Translate("REJUVENATION_POTION_MINI_DESC");
            case 117:
                return Localization.Translate("REJUVENATION_POTION_SMALL_DESC");
            case 118:
                return Localization.Translate("REJUVENATION_POTION_MEDIUM_DESC");
            case 119:
                return Localization.Translate("REJUVENATION_POTION_LARGE_DESC");
            case 120:
                return Localization.Translate("REJUVENATION_POTION_HUGE_DESC");
            case 801:
                return Localization.Translate("BRACER_WOODEN_DESC");
            case 901:
                return Localization.Translate("BOOTS_BEAR_DESC");
            case 1001:
                return Localization.Translate("USELESS_APPLE_FRESH_DESC");
            case 1002:
                return Localization.Translate("USELESS_SPIDER_TOOTH_DESC");
            case 1003:
                return Localization.Translate("USELESS_BREAD_LOAF_DESC");
            case 96001:
                return Localization.Translate("ARMOR_SILK_CLOTH_DESC");
            case 96002:
                return Localization.Translate("SHIELD_BROKEN_DESC");
            case 98001:
                return Localization.Translate("BOW_LONG_DESC");
            case 98002:
                return Localization.Translate("BOW_FLEXI_DESC");
            case 98101:
                return Localization.Translate("SWORD_RUSTY_DESC");
            case 98102:
                return Localization.Translate("SWORD_MYSTIQUE_DESC");
            case 98103:
                return Localization.Translate("STAFF_OLD_DESC");
            case 98104:
                return Localization.Translate("STAFF_MAGE_DESC");
        }

        return Localization.Translate("MAGIC_DETAILS");
    }

    private void Update()
    {
        if (DescWinOpen && !Dragging)
        {
            float heightBias = 0;
            if (Input.mousePosition.x < Screen.height * 0.5f)
            {
                heightBias = 0f;
            } else
            {
                heightBias = 1f;
            }

            if (Input.mousePosition.x < Screen.width * 0.3f)
            {
                DescriptionWindow.GetComponent<RectTransform>().pivot = new Vector2(0, heightBias);
                DescriptionWindow.transform.position = new Vector2(Input.mousePosition.x + 5f, Input.mousePosition.y - 2f);
            }
            else if (Input.mousePosition.x > Screen.width * 0.6f)
            {
                DescriptionWindow.GetComponent<RectTransform>().pivot = new Vector2(1, heightBias);
                DescriptionWindow.transform.position = new Vector2(Input.mousePosition.x - 5f, Input.mousePosition.y - 2f);
            } 
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(DescriptionWindow == null || Dragging)
        {
            return;
        }

        DescriptionWindow.SetActive(true);
        DescriptionWindow.transform.SetAsLastSibling();
        DescWinOpen = true;
        DescriptionWindow.transform.position =  new Vector2(Input.mousePosition.x - 48, Input.mousePosition.y - 145);

        DescriptionWindow.transform.Find("Caption/Text").GetComponent<Text>().text = GetTitle();
        DescriptionWindow.transform.Find("Text").GetComponent<Text>().text = GetDescription();
        DescriptionWindow.transform.Find("TxtPrice").GetComponent<Text>().text = SellValue.ToString();
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

    public void Copy(InventoryItem old)
    {
        Type = old.Type;
        Code = old.Code;
        SellValue = old.SellValue;
        CanSell = old.CanSell;

        MinLevel = old.MinLevel;
        MaxLevel = old.MaxLevel;
        TwoHanded = old.TwoHanded;
        MinDamage = old.MinDamage;
        MaxDamage = old.MaxDamage;
        MinArmor = old.MinArmor;
        MaxArmor = old.MaxArmor;

        AttrStrength = old.AttrStrength;
        AttrStamina = old.AttrStamina;
        AttrLife = old.AttrLife;
        AttrMagic = old.AttrMagic;
        AttrSpeed = old.AttrSpeed;
        AttrArmor = old.AttrArmor;
        Glyph = old.Glyph;
        ClipUse = old.ClipUse;

        Sockets = new InventoryItem[old.Sockets.Length];
        for (int i = 0; i < Sockets.Length; i++)
        {
            Sockets[i] = old.Sockets[i];
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            // consume
            if (Code >= 101 && Code <= 105 && PlayerAttributes.HPRemaining >= PlayerAttributes.HP)
            {
                GUI.SetInventoryMesage(Localization.Translate("FULL_HEALTH"));
                Inventory.ThatWontWork();

                return;
            }

            if (Code >= 106 && Code <= 110 && PlayerAttributes.ManaRemaining >= PlayerAttributes.Mana)
            {
                GUI.SetInventoryMesage(Localization.Translate("FULL_MANA"));
                Inventory.ThatWontWork();

                return;
            }

            string name = gameObject.name.Substring(9);
            int posX = int.Parse(name.Substring(0, 1)) - 1;
            int posY = int.Parse(name.Substring(2, 1)) - 1;

            AudioSource audio = null;

            if (ClipUse != null)
            {
                audio = gameObject.AddComponent<AudioSource>();
                audio.playOnAwake = false;
                audio.clip = ClipUse;

                Destroy(audio, 5);
            }
            
            Inventory.RemoveItem(0, posX, posY);
            GUI.PrepareItems();

            switch (Code)
            {
                // life potions
                case 101:
                    PlayerAttributes.HPRemaining = Mathf.Min(PlayerAttributes.HP, PlayerAttributes.HPRemaining + 20);
                    GUI.SetLifeTo(PlayerAttributes.HPRemaining / (float)PlayerAttributes.HP);
                    OnPointerExit(null);
                    audio?.Play();

                    break;
                case 102:
                    PlayerAttributes.HPRemaining = Mathf.Min(PlayerAttributes.HP, PlayerAttributes.HPRemaining + 30);
                    GUI.SetLifeTo(PlayerAttributes.HPRemaining / (float)PlayerAttributes.HP);
                    OnPointerExit(null);
                    audio?.Play();

                    break;
                case 103:
                    PlayerAttributes.HPRemaining = Mathf.Min(PlayerAttributes.HP, PlayerAttributes.HPRemaining + 40);
                    GUI.SetLifeTo(PlayerAttributes.HPRemaining / (float)PlayerAttributes.HP);
                    OnPointerExit(null);
                    audio?.Play();

                    break;
                case 104:
                    PlayerAttributes.HPRemaining = Mathf.Min(PlayerAttributes.HP, PlayerAttributes.HPRemaining + 60);
                    GUI.SetLifeTo(PlayerAttributes.HPRemaining / (float)PlayerAttributes.HP);
                    OnPointerExit(null);
                    audio?.Play();

                    break;
                case 105:
                    PlayerAttributes.HPRemaining = Mathf.Min(PlayerAttributes.HP, PlayerAttributes.HPRemaining + 90);
                    GUI.SetLifeTo(PlayerAttributes.HPRemaining / (float)PlayerAttributes.HP);
                    OnPointerExit(null);
                    audio?.Play();

                    break;

                // mana potions
                case 106:
                    PlayerAttributes.ManaRemaining = Mathf.Min(PlayerAttributes.Mana, PlayerAttributes.ManaRemaining + 20);
                    GUI.SetManaTo(PlayerAttributes.ManaRemaining / (float)PlayerAttributes.Mana);
                    OnPointerExit(null);
                    audio?.Play();

                    break;
                case 107:
                    PlayerAttributes.ManaRemaining = Mathf.Min(PlayerAttributes.Mana, PlayerAttributes.ManaRemaining + 30);
                    GUI.SetManaTo(PlayerAttributes.ManaRemaining / (float)PlayerAttributes.Mana);
                    OnPointerExit(null);
                    audio?.Play();

                    break;
                case 108:
                    PlayerAttributes.ManaRemaining = Mathf.Min(PlayerAttributes.Mana, PlayerAttributes.ManaRemaining + 40);
                    GUI.SetManaTo(PlayerAttributes.ManaRemaining / (float)PlayerAttributes.Mana);
                    OnPointerExit(null);
                    audio?.Play();

                    break;
                case 109:
                    PlayerAttributes.ManaRemaining = Mathf.Min(PlayerAttributes.Mana, PlayerAttributes.ManaRemaining + 60);
                    GUI.SetManaTo(PlayerAttributes.ManaRemaining / (float)PlayerAttributes.Mana);
                    OnPointerExit(null);
                    audio?.Play();

                    break;
                case 110:
                    PlayerAttributes.ManaRemaining = Mathf.Min(PlayerAttributes.Mana, PlayerAttributes.ManaRemaining + 90);
                    GUI.SetManaTo(PlayerAttributes.ManaRemaining / (float)PlayerAttributes.Mana);
                    OnPointerExit(null);
                    audio?.Play();

                    break;
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(!CanDrag)
        {
            return;
        }

        if(!Dragging)
        {
            LastItemPos = transform.GetChild(0).position;
        }

        DescWinOpen = false;
        if (DescriptionWindow != null)
        {
            DescriptionWindow.SetActive(false);
        }

        Dragging = true;
        GUI.GUIItemClicked = true;

        if (ItemSource == ItemSourceType.Inventory)
        {
            GUI.InventoryWindow.transform.SetAsLastSibling();
        } else if (ItemSource == ItemSourceType.Equipment)
        {
            GUI.EquipmentWindow.transform.SetAsLastSibling();
        }

        transform.SetAsLastSibling();
        transform.GetChild(0).position = Input.mousePosition;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if(!Dragging)
        {
            return;
        }

        Dragging = false;
        if (ItemSource == ItemSourceType.Inventory)
        {
            GUI.DropInventoryItem(this);
        } else if (ItemSource == ItemSourceType.Equipment)
        {
            GUI.DropEquipment(this);
        }
    }

    public void PutMeBack()
    {
        //transform.GetChild(0).position = LastItemPos;
        var rect = transform.GetChild(0).GetComponent<RectTransform>();
        rect.anchoredPosition = Vector2.zero;
    }
}
