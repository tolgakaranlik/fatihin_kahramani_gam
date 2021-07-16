using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    public string NPCName;
    public DialogItem DialogStarter;
    public DialogueManager Manager;

    private void Start()
    {
        Manager.RegisterInventory(NPCName, GetComponent<NPCInventory>());
    }
}
