using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoTranslator : MonoBehaviour
{
    public LocalizationManager Manager;
    public bool UpperCase = false;

    // Start is called before the first frame update
    void Start()
    {
        var text = GetComponent<Text>();

        if(text == null)
        {
            Debug.Log("Auto translator has an issue at " + gameObject.name);
        } else
        {
            text.text = Manager.Translate(text.text);

            if (UpperCase)
            {
                text.text = text.text.ToUpper();
            }
        }
    }
}
