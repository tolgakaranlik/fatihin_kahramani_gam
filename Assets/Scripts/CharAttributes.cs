using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharAttributes : MonoBehaviour
{
    // Inventory
    // Equipment
    // Spells

    public int HP;
    public int HPRemaining;
    public int Mana;
    public int ManaRemaining;
    public int Armor;
    public int AttackMin;
    public int AttackMax;
    public int Level = 1;
    public int Experience = 0;

    public int AttrStrength = 0;
    public int AttrStamina = 0;
    public int AttrLife = 0;
    public int AttrMagic = 0;
    public int AttrSpeed = 0;
    public int AttrArmor = 0;

    public int XPPerLevel = 500;

    Slider HealthBar = null;

    void Start()
    {
        HPRemaining = HP;
        ManaRemaining = Mana;

        try
        {
            HealthBar = transform.Find("Canvas").Find("HealthBar").GetComponent<Slider>();
            HealthBar.maxValue = HP;
        }
        catch
        {
        }
    }
    
    void Update()
    {
        if (HealthBar != null)
        {
            HealthBar.value = HPRemaining;
        }
    }

    public int XPLevelBase()
    {
        if(Level == 1)
        {
            return 0;
        } else
        {
            return (Level - 1) * XPPerLevel;
        }
    }

    public int XPNeededToLevelUp()
    {
        int totalXPNeeded = Level * XPPerLevel;

        return totalXPNeeded - Experience;
    }
}
