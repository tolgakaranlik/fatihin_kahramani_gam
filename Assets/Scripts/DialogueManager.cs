using Knife.Portal;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public GUIController GUI;
    public DialogItem DlgQuit;
    public GameObject MercanWelcome;
    public GameObject MercanBuy;
    public GameObject MercanSell;
    public GameObject MercanQuest1Again;
    public GameObject ParsWelcome;
    public GameObject ParsBuy;
    public GameObject ParsSell;
    public GameObject PortalKeeperOpen;
    public GameObject PortalKeeper;
    public Sprite DefaultSprite;
    public QuestManager QuestMgr;
    public PortalTransition Portal;
    public LocalizationManager Localization;

    Dictionary<string, NPCInventory> npcInventories;

    // Start is called before the first frame update
    void Start()
    {
        if(Localization == null)
        {
            Localization = transform.Find("/LocalizationManager").GetComponent<LocalizationManager>();
        }

        npcInventories = new Dictionary<string, NPCInventory>();
    }

    public void SelectItem(DialogItem item, int code)
    {
        switch(code)
        {
            case 100:
                break;
        }
    }

    public void StartDialogue(DialogItem item)
    {
        GUI.HideInventory();
        GUI.HideEquipment();

        GUI.DisplayDialogue(item);
    }

    public DialogItem QuitItem()
    {
        return DlgQuit;
    }

    public void ExecuteCode(int code, DialogItem item)
    {
        switch (item.Code)
        {
            case -100:
                // quit
                GUI.HideDialogue();
                break;
            case 100:
                // Enable shop items
                MercanWelcome.SetActive(false);
                MercanQuest1Again.SetActive(true);
                MercanBuy.SetActive(true);
                MercanSell.SetActive(true);

                // Enable portal request to portal keeper
                PortalKeeperOpen.SetActive(true);

                QuestMgr.GetQuestById(QuestManager.MAGICIANS_BOOK).Active = true;

                GUI.DisplayDialogue(item.transform.GetChild(0).gameObject.GetComponent<DialogItem>());
                break;
            case 101:
                // Buy, Pars
                OpenInventoryForBuy("Pars");
                break;
            case 102:
                // Sell, Pars
                OpenInventoryForSell("Pars");
                break;
            case 103:
                // Buy, Mercan
                OpenInventoryForBuy("Mercan");
                break;
            case 104:
                // Sell, Mercan
                OpenInventoryForSell("Mercan");
                break;
            case 109:
                // Disable Pars welcome dialog
                ParsWelcome.SetActive(false);

                // Enable Pars' buy and sell dialogs
                ParsSell.SetActive(true);
                ParsBuy.SetActive(true);

                item.Response = DialogItem.ResponseType.NextDialogue;
                GUI.DisplayDialogue(item.gameObject.GetComponent<DialogItem>());

                break;
            case 110:
                StartCoroutine(OpenPortal());
                break;
        }

    }

    IEnumerator OpenPortal()
    {
        GUI.HideDialogue();
        Portal.GetComponent<AudioSource>().Play();
        PortalKeeper.GetComponent<Animator>().CrossFade("arthur_active_01", 0.01f);

        yield return new WaitForSeconds(1f);

        Portal.OpenPortal();
    }

    private void OpenInventoryForSell(string npcName)
    {
        var inventoryManager = transform.Find("/InventoryManager").GetComponent<InventoryManager>();
        List<InventoryItem> items = new List<InventoryItem>();

        bool found;
        for (int i = 0; i < inventoryManager.Slots[0].Length; i++)
        {
            for (int j = 0; j < inventoryManager.Slots[0][i].Length; j++)
            {
                found = false;
                for (int k = 0; k < npcInventories[npcName].ItemTypesToBuy.Length; k++)
                {
                    if (inventoryManager.Slots[0][i][j] != null && inventoryManager.Slots[0][i][j].Type == npcInventories[npcName].ItemTypesToBuy[k])
                    {
                        found = true;
                        break;
                    }
                }

                if(found)
                {
                    // put item into the ones to sell
                    items.Add(inventoryManager.Slots[0][i][j]);
                }
            }
        }

        var itemList = items.ToArray();
        GUI.DisplayShop(Localization.Translate("ITEMS_FOR_BUY"), itemList, Localization.Translate("SELL").ToUpper(), false, (soldItem) => {
            var manager = soldItem.GetInventoryManager();
            var price = (int)Mathf.Ceil(soldItem.SellValue);

            manager.Gold += price;
            manager.RemoveItemFromInventory(soldItem);
            manager.ItWorked();

            GUI.SetGold(manager.Gold);
            OpenInventoryForSell(npcName);

            return true;
        });
    }

    private void OpenInventoryForBuy(string npcName)
    {
        GUI.DisplayShop(Localization.Translate("ITEMS_FOR_SALE"), npcInventories[npcName].ItemsToSell, Localization.Translate("BUY").ToUpper(), true, (boughtItem) => {
            var manager = boughtItem.GetInventoryManager();
            var equipment = boughtItem.GetEquipment();
            var price = (int)Mathf.Ceil(boughtItem.SellValue * 1.5f);

            if (price < manager.Gold)
            {
                // add to player's inventory
                manager.Gold -= price;

                // remove from seller's inventroy
                manager.RemoveItemFromInventory(boughtItem, ref npcInventories[npcName].ItemsToSell);
                manager.ItWorked();

                bool placedInEquipment = false;

                // put to empty equipment slots automatically
                switch(boughtItem.Type)
                {
                    case InventoryItem.ItemType.Armor:
                        if(equipment.Torso == null)
                        {
                            placedInEquipment = true;
                            equipment.Torso = boughtItem;
                        }

                        break;
                    case InventoryItem.ItemType.Belt:
                        if (equipment.Abdomen == null)
                        {
                            placedInEquipment = true;
                            equipment.Abdomen = boughtItem;
                        }

                        break;
                    case InventoryItem.ItemType.Boots:
                        if (equipment.Feet == null)
                        {
                            placedInEquipment = true;
                            equipment.Feet = boughtItem;
                        }

                        break;
                    case InventoryItem.ItemType.Bow:
                        if (equipment.LeftHand == null && equipment.RightHand == null)
                        {
                            placedInEquipment = true;
                            equipment.LeftHand = boughtItem;
                        }

                        break;
                    case InventoryItem.ItemType.Glove:
                        if (equipment.Gloves == null)
                        {
                            placedInEquipment = true;
                            equipment.Gloves = boughtItem;
                        }

                        break;
                    case InventoryItem.ItemType.Helm:
                        if (equipment.Head == null)
                        {
                            placedInEquipment = true;
                            equipment.Head = boughtItem;
                        }

                        break;
                    case InventoryItem.ItemType.Necklace:
                        if (equipment.Neck == null)
                        {
                            placedInEquipment = true;
                            equipment.Neck = boughtItem;
                        }

                        break;
                    case InventoryItem.ItemType.Shoulder:
                        if (equipment.Shoulder == null)
                        {
                            placedInEquipment = true;
                            equipment.Shoulder = boughtItem;
                        }

                        break;
                    case InventoryItem.ItemType.Shield:
                        if (equipment.LeftHand == null && equipment.RightHand == null)
                        {
                            placedInEquipment = true;
                            equipment.RightHand = boughtItem;
                        }

                        break;
                    case InventoryItem.ItemType.Staff:
                        if (equipment.LeftHand == null && equipment.RightHand == null)
                        {
                            placedInEquipment = true;
                            equipment.LeftHand = boughtItem;
                        }

                        break;
                    case InventoryItem.ItemType.Sword:
                        if ((boughtItem.TwoHanded && equipment.LeftHand == null && equipment.RightHand == null) || (!boughtItem.TwoHanded && equipment.LeftHand == null))
                        {
                            placedInEquipment = true;
                            equipment.LeftHand = boughtItem;
                        }

                        break;
                }

                if(!placedInEquipment)
                {
                    manager.AutoAddItem(boughtItem);
                }

                GUI.SetGold(manager.Gold);
                OpenInventoryForBuy(npcName);
                return true;
            }
            else
            {
                manager.ThatWontWork();
            }

            return false;
        });
    }

    public bool RegisterInventory(string name, NPCInventory inventory)
    {
        if(npcInventories.ContainsKey(name))
        {
            return false;
        }

        npcInventories.Add(name, inventory);
        return true;
    }
}
