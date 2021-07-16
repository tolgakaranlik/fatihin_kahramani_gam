using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogItem : MonoBehaviour
{
    public DialogueManager Manager;

    public enum DialogueItemType { Important, Informative }
    public DialogueItemType ItemType = DialogueItemType.Important;

    public enum ResponseType { BackToPrevious, QuitFromDialogue, Attack, ExecuteAction, NextDialogue }
    public ResponseType Response = ResponseType.NextDialogue;

    public int Code;
    public string SpeechSummary;
    public string SpeechFull;

    public string Title;
    public string FullName;
    public Sprite Avatar;
    public string Info;
    public LocalizationManager Localization;

    // Start is called before the first frame update
    void Start()
    {
        if(Manager == null)
        {
            Manager = transform.Find("/DialogueManager").gameObject.GetComponent<DialogueManager>();
        }

        if(Localization == null)
        {
            Localization = transform.Find("/LocalizationManager").gameObject.GetComponent<LocalizationManager>();
        }

        SpeechSummary = Localization.Translate(SpeechSummary);
        SpeechFull = Localization.Translate(SpeechFull);
        Title = Localization.Translate(Title);
        FullName = Localization.Translate(FullName);
    }

    public DialogItem[] FindAnswers()
    {
        List<DialogItem> answers = new List<DialogItem>();

        DialogItem item;
        for (int i = 0; i < transform.childCount; i++)
        {
            if(transform.GetChild(i).gameObject.activeSelf)
            {
                item = transform.GetChild(i).gameObject.GetComponent<DialogItem>();

                if(item != null)
                {
                    answers.Add(item);
                }
            }
        }

        return answers.ToArray();
    }

    public void Select()
    {
        Manager.SelectItem(this, Code);
    }
}
