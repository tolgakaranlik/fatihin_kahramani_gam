using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoLocalize : MonoBehaviour
{
    public LocalizationManager Manager;

    void Start()
    {
        var image = GetComponent<Image>();

        if (image == null)
        {
            Debug.Log("Auto localizer has an issue at " + gameObject.name);
        }
        else
        {
            image.sprite = Resources.Load<Sprite>(Manager.Localize(image.sprite.name));

        }
    }
}
