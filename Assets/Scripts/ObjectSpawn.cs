using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectSpawn : MonoBehaviour
{
    public AudioSource SoundToDisplay;
    public GameObject ObjectToSpawn;
    public GameObject LightToTurnOnAtTheEnd;
    public float Delay;
    public float HeightToJump;
    public float DurationToJump;
    public float DurationToRotate;
    public float TimesRotate;
    public float CaptionDisplayDelay = 1.0f;
    public InventoryManager Inventory;

    GameObject lightToTurnOffAtTheEnd;

    // Start is called before the first frame update
    void Init()
    {
        var newObject = Instantiate(ObjectToSpawn, ObjectToSpawn.transform.position, ObjectToSpawn.transform.rotation, transform);
        lightToTurnOffAtTheEnd = newObject.transform.Find("Point Light").gameObject;

        StartCoroutine(InitNow(newObject));
        StartCoroutine(RotateNow(newObject));
        StartCoroutine(DisplayCaption());
    }

    IEnumerator DisplayCaption()
    {
        yield return new WaitForSeconds(CaptionDisplayDelay);

        var iv = GetComponent<InventoryItem>();

        string itemType = iv.GetTitle();        

        var canvas = transform.Find("Canvas").gameObject;
        canvas.SetActive(true);
        var btnItem = canvas.transform.Find("BtnItem").gameObject;

        btnItem.transform.Find("TextItem").gameObject.GetComponent<Text>().text = (iv.Code == 100 ? (iv.SellValue.ToString() + " ") : "") + itemType;

        if (Inventory != null)
        {
            btnItem.GetComponent<Button>().onClick.AddListener(delegate
            {
                if(Inventory.AutoAddItem(iv))
                {
                    gameObject.SetActive(false);
                } else
                {
                    // Display "I can't carry anymore"
                }
            });
        }
    }

    IEnumerator InitNow(GameObject newObject)
    {
        yield return new WaitForSeconds(Delay);

        if(SoundToDisplay != null)
        {
            var snd = Instantiate(SoundToDisplay);
            snd.Play();

            Destroy(snd.gameObject, 5);
        }

        newObject.SetActive(true);
        if (LightToTurnOnAtTheEnd != null)
        {
            LightToTurnOnAtTheEnd.SetActive(true);
        }

        newObject.transform.DOLocalMoveY(newObject.transform.localPosition.y + HeightToJump, DurationToJump).SetEase(Ease.OutQuad);

        yield return new WaitForSeconds(DurationToJump);

        newObject.transform.DOLocalMoveY(newObject.transform.localPosition.y - HeightToJump, DurationToJump).SetEase(Ease.InQuad);

        yield return new WaitForSeconds(DurationToJump);

        if (lightToTurnOffAtTheEnd != null)
        {
            lightToTurnOffAtTheEnd.SetActive(false);
            Destroy(lightToTurnOffAtTheEnd);
        }
    }

    IEnumerator RotateNow(GameObject newObject)
    {
        yield return new WaitForSeconds(Delay);

        for (int i = 0; i < TimesRotate; i++)
        {
            newObject.transform.DORotate(new Vector3(360, 0, 0), DurationToRotate, RotateMode.WorldAxisAdd).SetEase(Ease.Linear);

            yield return new WaitForSeconds(DurationToRotate);
        }
    }

    public void Start()
    {
        Init();
    }
}
