using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MagicShortcutManager : MonoBehaviour
{
    public GameObject[] BottomItemsList;
    public MagicShortcut[] Shortcuts;

    void Start()
    {
        Shortcuts = new MagicShortcut[10];

        for (int i = 0; i < BottomItemsList.Length; i++)
        {
            BottomItemsList[i].SetActive(false);
        }

        // Load from prefs may be?
    }

    void Update()
    {
        
    }

    public void SetMagicToShortcutKey(int code, int key, Sprite icon, bool passive, string shortcutKey)
    {
        // Is it previously assigned?
        for (int i = 0; i < Shortcuts.Length; i++)
        {
            if(i == key - 1)
            {
                continue;
            }

            if(Shortcuts[i] != null && Shortcuts[i].Code == code)
            {
                BottomItemsList[i].SetActive(false);
            }
        }

        // Make the new assignment
        Shortcuts[key - 1] = new MagicShortcut(key, code, icon);

        BottomItemsList[key - 1].SetActive(true);
        BottomItemsList[key - 1].GetComponent<Image>().sprite = icon;
        BottomItemsList[key - 1].GetComponent<SkillItem>().Code = code;
        BottomItemsList[key - 1].GetComponent<SkillItem>().Passive = passive;
        BottomItemsList[key - 1].GetComponent<SkillItem>().ShortcutKey = shortcutKey;
    }
}
