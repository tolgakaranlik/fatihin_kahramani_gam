using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LootManager : MonoBehaviour
{
    public enum PotionSize { Tiny, Small, Medium, Large, Huge }
    public enum PotionColor { Red, Blue, Green, Purple, Yellow, LightBlue }
    public enum PotionType { Life, Mana, Anitdote, Rejuvenation }

    public GameObject GoldPile;
    public GameObject Potion;

    public void LootGoldPile(Vector3 position, int amount)
    {
        StartCoroutine(InitGoldPile(position, amount, 0));
    }

    public void LootGoldPile(Vector3 position, float delay, int amount)
    {
        StartCoroutine(InitGoldPile(position, amount, delay));
    }

    public void LootPotion(Vector3 position, float delay, PotionSize size, PotionType type)
    {
        StartCoroutine(InitPotion(position, size, type, delay));
    }

    IEnumerator InitGoldPile(Vector3 position, int amount, float delay)
    {
        yield return new WaitForSeconds(delay);

        var element = Instantiate(GoldPile, position, Quaternion.identity);
        var iv = element.GetComponent<InventoryItem>();
        var os = element.GetComponent<ObjectSpawn>();

        iv.Code = 100; // gold
        iv.SellValue = amount;
        iv.Type = InventoryItem.ItemType.Gold;

        element.SetActive(true);
    }

    IEnumerator InitPotion(Vector3 position, PotionSize size, PotionType type, float delay)
    {
        yield return new WaitForSeconds(delay);

        var element = Instantiate(Potion, position, Quaternion.identity);
        var iv = element.GetComponent<InventoryItem>();
        var os = element.GetComponent<ObjectSpawn>();
        var pointLight = element.transform.Find("Point Light").GetComponent<Light>();

        PotionColor color = PotionColor.Red;

        switch (type)
        {
            case PotionType.Life:
                iv.Code = 101;
                iv.Type = InventoryItem.ItemType.PotionLife;

                break;
            case PotionType.Mana:
                color = PotionColor.Blue;
                iv.Code = 106;
                iv.Type = InventoryItem.ItemType.PotionMana;

                break;
            case PotionType.Anitdote:
                color = PotionColor.Green;
                iv.Code = 111;
                iv.Type = InventoryItem.ItemType.PotionAntidote;

                break;
            case PotionType.Rejuvenation:
                color = PotionColor.Purple;
                iv.Code = 116;
                iv.Type = InventoryItem.ItemType.PotionRejuvenation;

                break;
        }

        GameObject potionRedSmall = element.transform.Find("Red_small").gameObject;
        GameObject potionBlueSmall = element.transform.Find("Blue_small").gameObject;
        Image imageRedSmall = element.transform.Find("GlyphRedSmall").gameObject.GetComponent<Image>();
        Image imageBlueSmall = element.transform.Find("GlyphBlueSmall").gameObject.GetComponent<Image>();

        switch (color)
        {
            case PotionColor.Red:
                os.ObjectToSpawn = potionRedSmall;
                pointLight.color = new Color(0.4150943f, 0.04053379f, 0.009789959f);
                iv.Glyph = imageRedSmall.sprite;

                break;
            case PotionColor.Blue:
                os.ObjectToSpawn = potionBlueSmall;
                pointLight.color = new Color(0.119749f, 0.2797931f, 0.6509434f);
                iv.Glyph = imageBlueSmall.sprite;

                break;
        }

        int sellValue = 30;
        switch(size)
        {
            case PotionSize.Small:
                sellValue = 50;
                iv.Code += 1;

                break;
            case PotionSize.Medium:
                sellValue = 90;
                iv.Code += 2;

                break;
            case PotionSize.Large:
                sellValue = 150;
                iv.Code += 3;

                break;
            case PotionSize.Huge:
                sellValue = 240;
                iv.Code += 4;

                break;
        }

        iv.SellValue = sellValue;
        iv.ClipUse = element.transform.Find("Sound").gameObject.GetComponent<AudioSource>().clip;

        element.SetActive(true);
    }
}
