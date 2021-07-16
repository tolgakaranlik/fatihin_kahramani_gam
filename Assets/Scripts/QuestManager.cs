using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using UnityEngine;

public class QuestCondition
{
    public bool Completed;
    public string Title;
    public string Description;

    public QuestCondition(string title, string description)
    {
        Title = title;
        Description = description;
        Completed = false;
    }
}

public class QuestItem {
    public bool Active;
    public int Code { get; }
    public int Level;
    public int XP;
    public bool Completed;
    public string Title;
    public string Place;
    public string Description;

    public QuestCondition[] Conditions;
    public InventoryItem[] Prizes;

    public QuestItem(int code)
    {
        Code = code;
        Active = false;
        Completed = false;
    }
}

public class QuestManager : MonoBehaviour
{
    public const int MAGICIANS_BOOK = 101;
    public LocalizationManager Localization;

    public QuestItem[] Quests { 
        get
        {
            return quests;
        }
    }

    QuestItem[] quests;

    QuestItem QstMagiciansBook;

    // Start is called before the first frame update
    void Start()
    {
        if(Localization == null)
        {
            Localization = transform.Find("/LocalizationManager").GetComponent<LocalizationManager>();
        }

        CreateQuests();

        quests = new QuestItem[] { QstMagiciansBook };
    }

    public QuestItem GetQuestById(int code)
    {
        for (int i = 0; i < Quests.Length; i++)
        {
            if(Quests[i].Code == code)
            {
                return Quests[i];
            }
        }

        return null;
    }

    private void CreateQuests()
    {
        // Magician's book
        QstMagiciansBook = new QuestItem(MAGICIANS_BOOK);
        QstMagiciansBook.XP = 1000;
        QstMagiciansBook.Level = 1;
        QstMagiciansBook.Title = Localization.Translate("QUEST_1_TITLE");
        QstMagiciansBook.Place = Localization.Translate("LEVEL_1_NAME");
        QstMagiciansBook.Description = Localization.Translate("QUEST_1_DESC");

        QuestCondition step1 = new QuestCondition(Localization.Translate("QUEST_1_ITEM_1"), Localization.Translate("QUEST_1_ITEM_1_DESC"));
        QuestCondition step2 = new QuestCondition(Localization.Translate("QUEST_1_ITEM_2"), Localization.Translate("QUEST_1_ITEM_2_DESC"));
        QuestCondition step3 = new QuestCondition(Localization.Translate("QUEST_1_ITEM_3"), Localization.Translate("QUEST_1_ITEM_3_DESC"));

        QstMagiciansBook.Conditions = new QuestCondition[] { step1, step2, step3 };
    }
}
