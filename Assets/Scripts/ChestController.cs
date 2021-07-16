using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestController : MonoBehaviour
{
    public int Code;

    bool open;

    // Start is called before the first frame update
    void Start()
    {
        open = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenChest()
    {
        if(open)
        {
            return;
        }

        open = true;
        GetComponent<AudioSource>().Play();
        StartCoroutine(OpenChestNow());
    }

    IEnumerator OpenChestNow()
    {
        yield return new WaitForSeconds(1.0f);
            
        transform.Find("chest_close").gameObject.SetActive(false);
        transform.Find("Glow").gameObject.SetActive(false);
        transform.Find("chest_open").gameObject.SetActive(true);
        //transform.Find("Point").

        // items drop from inside
        switch (Code)
        {
            default:
                // a few mere coins depending on player level
                break;
        }
    }
}
