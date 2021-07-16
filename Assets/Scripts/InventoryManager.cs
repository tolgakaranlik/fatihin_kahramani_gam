using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public int SlotHeight = 6;
    public int SlotWidth = 7;
    public int NumSlots = 1;
    public int Gold = 0;

    public InventoryItem[][][] Slots;
    public GUIController GUI;
    public InventoryItem[] FirstInventoryItems;

    int[] slotUsages;
    AudioSource pickSound;
    AudioSource wontWorkSound;

    void Start()
    {
        Slots = new InventoryItem[NumSlots][][];
        slotUsages = new int[NumSlots];
        for (int i = 0; i < NumSlots; i++)
        {
            Slots[i] = new InventoryItem[SlotWidth][];
            slotUsages[i] = 0;

            for (int j = 0; j < SlotWidth; j++)
            {
                Slots[i][j] = new InventoryItem[SlotHeight];
            }
        }

        GUI.SetGold(Gold);
        pickSound = GetComponents<AudioSource>()[0];
        wontWorkSound = GetComponents<AudioSource>()[1];

        CreateFirstInventoryItems();
    }

    public bool AddItem(int slotIndex, int posX, int posY, InventoryItem item)
    {
        if(item.Type == InventoryItem.ItemType.Gold)
        {
            Gold += item.SellValue;
            GUI.SetGold(Gold);

            return true;
        }

        if(Slots[slotIndex][posX][posY] == null)
        {
            Slots[slotIndex][posX][posY] = item;
            return true;
        }

        return false;
    }

    public void MoveItem(int slotIndex, int fromX, int fromY, int toX, int toY)
    {
        Slots[slotIndex][toX][toY] = Slots[slotIndex][fromX][fromY];
        Slots[slotIndex][fromX][fromY] = null;
    }

    public bool AutoAddItem(InventoryItem item)
    {
        return AutoAddItem(item, true, true);
    }

    public bool AutoAddItem(InventoryItem item, bool playSound, bool prepareItems)
    {
        if (item.Type == InventoryItem.ItemType.Gold)
        {
            Gold += item.SellValue;
            GUI.SetGold(Gold);

            if (playSound)
            {
                pickSound.Play();
            }

            if (prepareItems)
            {
                GUI.PrepareItems();
            }

            return true;
        }

        for (int slot = 0; slot < NumSlots; slot++)
        {
            for (int j = 0; j < SlotHeight; j++)
            {
                for (int i = 0; i < SlotWidth; i++)
                {
                    if (Slots[slot][i][j] == null)
                    {
                        // there is an available position here
                        Slots[slot][i][j] = item;

                        if (playSound)
                        {
                            pickSound.Play();
                        }

                        if (prepareItems)
                        {
                            GUI.PrepareItems();
                        }

                        return true;
                    }
                }
            }

        }

        return false;
    }

    public void RemoveItem(int slot, int x, int y)
    {
        Slots[slot][x][y] = null;
    }

    public void ThatWontWork()
    {
        wontWorkSound.Play();
    }

    public void ItWorked()
    {
        pickSound.Play();
    }

    public void RemoveItemFromInventory(InventoryItem item, ref InventoryItem[] inventory)
    {
        List<InventoryItem> tempInv = inventory.ToList();
        for (int i = 0; i < tempInv.Count; i++)
        {
            if(tempInv[i].Code == item.Code && tempInv[i].Type == item.Type)
            {
                tempInv.RemoveAt(i);
                break;
            }
        }

        inventory = tempInv.ToArray();
    }

    public void RemoveItemFromInventory(InventoryItem item)
    {
        for (int i = 0; i < Slots[0].Length; i++)
        {
            for (int j = 0; j < Slots[0][i].Length; j++)
            {
                if (Slots[0][i][j] != null && Slots[0][i][j].Type == item.Type && Slots[0][i][j].Code == item.Code)
                {
                    Slots[0][i][j] = null;
                    break;
                }
            }
        }

    }

    private void CreateFirstInventoryItems()
    {
        for (int i = 0; i < FirstInventoryItems.Length; i++)
        {
            AutoAddItem(FirstInventoryItems[i], false, false);
        }
    }
}
