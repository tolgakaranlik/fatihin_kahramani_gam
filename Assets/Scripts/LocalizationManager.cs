using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
    public enum LanguageIdentifier { Türkçe, English };
    public LanguageIdentifier DefaultLanguage = LanguageIdentifier.English;

    Dictionary<string, string> LangDatabaseTR = new Dictionary<string, string>();
    Dictionary<string, string> LangDatabaseEN = new Dictionary<string, string>();

    Dictionary<string, string> LangResourceTR = new Dictionary<string, string>();
    Dictionary<string, string> LangResourceEN = new Dictionary<string, string>();

    void Awake()
    {
        if(Environment.GetCommandLineArgs().Contains("-lang=tr"))
        {
            DefaultLanguage = LanguageIdentifier.Türkçe;
        }

        Init();
    }

    public string Localize(string sprite_name)
    {
        switch (DefaultLanguage)
        {
            case LanguageIdentifier.English:
                if (LangResourceEN.ContainsKey(sprite_name))
                {
                    return LangResourceEN[sprite_name];
                }

                return sprite_name;
            case LanguageIdentifier.Türkçe:
                if (LangResourceTR.ContainsKey(sprite_name))
                {
                    return LangResourceTR[sprite_name];
                }

                return sprite_name;
        }

        return sprite_name;
    }

    public string Translate(string key, string[] args)
    {
        string result = Translate(key);

        for (int i = 0; i < args.Length; i++)
        {
            result = result.Replace("%" + (i + 1), args[i]);
        }

        return result;
    }

    public string Translate(string key)
    {
        switch (DefaultLanguage)
        {
            case LanguageIdentifier.English:
                if (LangDatabaseEN.ContainsKey(key))
                {
                    return LangDatabaseEN[key];
                }

                return key;
            case LanguageIdentifier.Türkçe:
                if (LangDatabaseTR.ContainsKey(key))
                {
                    return LangDatabaseTR[key];
                }

                return key;
        }

        return key;
    }

    public void Init()
    {
        // Dictionary items
        AddToDictionaries("N/A", new string[] { "yok", "n/a" });
        AddToDictionaries("MAGIC_FIREBALL", new string[] { "Alev Topu", "Fire Ball" });
        AddToDictionaries("MAGIC_FIREBALL_DESC_L1", new string[] { "Düşmana alev topu fırlatır. Seviye 1'de normal vuruşa ek olarak <color=#f94>%1</color> arası ilave güç götürür.", "Throws a fire ball towards the enemy. In addition to normal damage, it causes additional <color=#f94>%1</color> fire damage at level 1" });
        AddToDictionaries("MAGIC_ICYFIRE", new string[] { "Buz Ateşi", "Icy Fire" });
        AddToDictionaries("MAGIC_ICYFIRE_DESC_L1", new string[] { "Düşmana buz topu fırlatır. Seviye 1'de normal vuruşa ek olarak <color=#f94>%1</color> arası ilave güç götürür. Düşmanı <color=#f94>%2</color> saniye boyunca yavaşlatır.", "Throws ice balls towards the enemy. Causes additional <color=#f94>%1</color> cold damage in addition to normal damage on level 1. Slows down the enemy for <color=#f94>%2</color> seconds." });
        AddToDictionaries("MAGIC_FREEZER", new string[] { "Dondurucu", "Freezer" });
        AddToDictionaries("MAGIC_FREEZER_DESC_L1", new string[] { "Düşmanı <color=#f94>%1</color> saniye boyunca dondurur. Dondurulan düşman saldırılardan ve büyülerden etkilenmez.", "Freezes the enemy for <color=#f94>%1</color> seconds. Frozen enemy cannot be hit by shots or spells." });
        AddToDictionaries("MAGIC_TOMBRAIDER", new string[] { "Mezar Hırsızı", "Tomb Raider" });
        AddToDictionaries("MAGIC_TOMBRAIDER_DESC_L1", new string[] { "Ölen düşmanlardan altın veya değerli eşya çıkarmaya yarar. Düşmanın ve sizin seviyenize göre çıkacak altın veya eşyanın kalitesi değişir.", "Extracts gold or valuables from enemy corpses. Quality of items or amount of gold extracted can be varying depending on your and dead enemy's level." });
        AddToDictionaries("MAGIC_LIGHTNINGSHIELD", new string[] { "Şimşek Kalkanı", "Lightning Shield" });
        AddToDictionaries("MAGIC_LIGHTNINGSHIELD_DESC_L1", new string[] { "Yakın dövüşle size vuran bir miktar hasar alır. Şimşek kalkanı size ilave kalkan sağlamaz. Uzaktan vuruşlara ve büyülere karşı etkisizdir.", "Melee attacks takes damage from their own attacks. Lightning shield does not provide additional armor. Ineffective agains ranged shots and spells." });
        AddToDictionaries("MAGIC_CURSEDARMOR", new string[] { "Lanet Zırhı", "Cursed Armor" });
        AddToDictionaries("MAGIC_CURSEDARMOR_DESC_L1", new string[] { "Yakın dövüşle size vuranlar <color=#f94>%1</color> saniye boyunca lanetlenir ve mümkün olan en düşük güçle vuruş yaparlar. Uzaktan vuruşlara ve büyülere karşı etkisizdir.", "Melee attackers who attacked against you will be cusred for <color=#f94>%1</color> seconds and their damage becomes the lowest possible during that time. Ineffective against spells and ranged attacks." });
        AddToDictionaries("MAGIC_FIRESHIELD", new string[] { "Ateş Kalkanı", "Fire Shield" });
        AddToDictionaries("MAGIC_FIRESHIELD_DESC_L1", new string[] { "Yakın dövüşle size vuran bir miktar hasar alır ve <color=#f94>%1</color> saniye boyunca yanmaya ve hasar almaya devam eder. Aktif olduğu süre boyunca size ateş işlemez. Uzaktan vuruşlara ve büyülere karşı etkisizdir.", "Melee attackers who attacked against you take some damage back and they continue burning and being damaged for <color=#f94>%1</color> seconds. Ineffective against spells and ranged attacks." });
        AddToDictionaries("MAGIC_KISSOFTHEDRAGON", new string[] { "Ejderha Öpücüğü", "Kiss of the Dragon" });
        AddToDictionaries("MAGIC_KISSOFTHEDRAGON_DESC_L1", new string[] { "Kılıcınız ve kalkanınız ateşle kaplanır. Normal vuruşunuza ilave olarak <color=#f94>%1</color> ateş hasarı verir.", "Your sword and shield are covered with fire. It makes additional <color=#f94>%1</color> fire damage in addition to the original damage. Ineffective against spells and ranged attacks." });
        AddToDictionaries("MAGIC_CHAINSHIELD", new string[] { "Zincir Kalkanı", "Chain Shield" });
        AddToDictionaries("MAGIC_CHAINSHIELD_DESC_L1", new string[] { "Sihirli bir zincir etrafınızı kaplayarak vuruşlara engel olur. Yakın dövüş veya uzaktan yapılan tüm vuruşlardan hasarı azaltır. Büyülere karşı etkisi azdır.", "A magical shield covers you and prevents you to take damage. Reduces all melee and ranged damage. Its efficiency is low against spells." });
        AddToDictionaries("MAGIC_FIREARROW", new string[] { "Alev Oku", "Fire Arrow" });
        AddToDictionaries("MAGIC_FIREARROW_DESC_L1", new string[] { "Okunuz ve yayınız alevle kaplanır. Normal vuruşunuza ilave olarak <color=#f94>%1</color> ateş hasarı verir. Aktif olduğu süre boyunca her atışınızda mana tüketir.", "Your sword and shield are covered with fire. It makes additional <color=#f94>%1</color> fire damage in addition to the original damage. Consumes mana as long as it is active." });
        AddToDictionaries("MAGIC_ICYDREAMS", new string[] { "Buzdan Hayaller", "Icy Dreams" });
        AddToDictionaries("MAGIC_ICYDREAMS_DESC_L1", new string[] { "Okunuz geçtiği yollar boyunca yol üzerindeki herkese soğuk hasarı verir ve hepsi <color=#f94>%1</color> saniye boyunca donarlar. Donmuş düşmanlar hasar alamaz, hareket edemez, vuruş yapamaz ve büyü yapamazlar.", "Your arrow gives cold damage to everyone on its way and freezes them for <color=#f94>%1</color> seconds. Frozen enemies cannot take damage, cannot move, cannot strike and cannot cast spells." });
        AddToDictionaries("MAGIC_LIGHTNINGHARPOON", new string[] { "Şimşek Zıpkını", "Lightning Harpoon" });
        AddToDictionaries("MAGIC_LIGHTNINGHARPOON_DESC_L1", new string[] { "Normal vuruşunuza ilave olarak <color=#f94>%1</color> şimşek hasarı verir. Hasar yakınlardaki düşmanlara yarılanarak iletilir. Aktif olduğu süre boyunca her atışınızda mana tüketir.", "Gives additional <color=#f94>%1</color> lightning damage. Chain wave is transfered to nearby enemies by losing its half magnitude. Consumes mana as long as it is active." });

        AddToDictionaries("MAGIC_DETAILS", new string[] { "Detaylar", "Details" });
        AddToDictionaries("GAME_NAME", new string[] { "FATİH'İN KAHRAMANI", "HERO OF THE CONQUEST" });
        AddToDictionaries("PRESS_ANY_KEY", new string[] { "Bir Tuşa Basın", "Press Any Key" });
        AddToDictionaries("INITIAL_SPELLS", new string[] { "Başlangıç Büyüleri", "Initial Spells" });
        AddToDictionaries("ANA_MENU", new string[] { "Ana Menü", "Main Menu" });

        AddToDictionaries("CLASS", new string[] { "Sınıf", "Class" });
        AddToDictionaries("CLASS_ARCHER", new string[] { "Okçu", "Archer" });
        AddToDictionaries("CLASS_MAGE", new string[] { "Büyücü", "Mage" });
        AddToDictionaries("CLASS_THIEF", new string[] { "Hırsız", "Thief" });
        AddToDictionaries("CLASS_PALADIN", new string[] { "Şövalye", "Paladin" });

        AddToDictionaries("QUIT", new string[] { "Çıkış", "Quit" });
        AddToDictionaries("SETTINGS", new string[] { "Ayarlar", "Settings" });
        AddToDictionaries("STORE", new string[] { "Mağaza", "Store" });
        AddToDictionaries("GUILD_ENTRY", new string[] { "Zindana Giriş", "Enter the Guild" });
        AddToDictionaries("START_NEW_GAME", new string[] { "Yeni Oyuna Başlama", "Start a New Game" });
        AddToDictionaries("CONTINUE_GAME", new string[] { "Önceki Oyundan Devam", "Continue to Last Game" });

        AddToDictionaries("APPLY", new string[] { "Uygula", "Apply" });
        AddToDictionaries("CANCEL", new string[] { "İptal", "Cancel" });
        AddToDictionaries("DEFAULTS", new string[] { "Varsayılanlar...", "Defaults..." });

        AddToDictionaries("DISCLAIMER", new string[] { "TARİHİ OLAYLARDAN ve GERÇEK KİŞİLERDEN ESİNLENİLMİŞ OLSA DA, BU OYUNDAKİ TÜM OLAYLAR ve TÜM OYUN KARAKTERLERİ TAMAMEN HAYAL ÜRÜNÜDÜR. OYUNUN HER BİR PARÇASI FARKLI KÜLTÜRLERE, FARKLI DİLLERE ve DİNLERE MENSUP KİŞİLER TARAFINDAN TASARLAMIŞTIR. HİÇBİR KİŞİ, KURUM veya KÜLTÜRE KARŞI OLUMSUZ BİR YAKLAŞIM GÜDÜLMEMEKTEDİR.", 
                                                       "ALTHOUGH INSPIRED BY HISTORICAL EVENTS and REAL PEOPLE, ALL EVENTS and GAME CHARACTERS ARE FICTIONAL. ALL PARTS OF THE GAME is DESIGNED, CODED and IMPLEMENTED BY PEOPLE WHO ARE FROM DIFFERENT ORIGINS, RELIGIONAL BELIEFS and DIFFERENT GENDER IDENTITIES. WE DO NOT INTEND ANY OFFENCE FOR ANYONE, ANY CULTURE or ANY ORGANIZATION." });

        AddToDictionaries("MENU_RETURN", new string[] { "Ana Ekrana Dön", "Back to Main Menu" });
        AddToDictionaries("BACK_TO_GAME", new string[] { "Oyuna Dön", "Back To Game" });
        AddToDictionaries("QUIT_PROMPT", new string[] { "Çıkmak istediğinizden emin misiniz?", "Are you sure you want to quit?" });
        AddToDictionaries("YES", new string[] { "Evet", "Yes" });
        AddToDictionaries("NO", new string[] { "Hayır", "No" });

        AddToDictionaries("STRENGTH", new string[] { "Kuvvet", "Strength" });
        AddToDictionaries("SPEED", new string[] { "Hız", "Speed" });
        AddToDictionaries("STAMINA", new string[] { "Dayanıklılık", "Stamina" });
        AddToDictionaries("ARMOR", new string[] { "Zırh", "Armor" });
        AddToDictionaries("LIFE", new string[] { "Can", "Life" });
        AddToDictionaries("MAGIC", new string[] { "Büyü Gücü", "Magic" });
        AddToDictionaries("GAME_SETTINGS", new string[] { "Oyun Ayarları", "Game Settings" });
        AddToDictionaries("GRAPHICS_SETTINGS", new string[] { "Görüntü Ayarları", "Display Settings" });
        AddToDictionaries("RESOLUTION", new string[] { "Ekran Çözünürlüğü", "Screen Resolution" });
        AddToDictionaries("KEY_CONFIG", new string[] { "Tuş Ayarları", "Key Configuration" });
        AddToDictionaries("GAME_LANGUAGE", new string[] { "Oyun Dili", "Game Language" });
        AddToDictionaries("SOUND_VOLUME", new string[] { "Ses Seviyesi", "Sound Volume" });
        AddToDictionaries("MUSIC_VOLUME", new string[] { "Müzik Seviyesi", "Music Volume" });
        AddToDictionaries("LICENSE_EXPIRATION", new string[] { "Lisansınızın son geçerlilik tarihi", "Expiration date of your license" });
        AddToDictionaries("EXPAND", new string[] { "Uzat", "Expand" });
        AddToDictionaries("YANSIMA", new string[] { "Yansıma", "Reflections" });
        AddToDictionaries("MODEL_QUALITY", new string[] { "Model Kalitesi", "World Quality" });
        AddToDictionaries("TEXTURE_QUALITY", new string[] { "Kaplama Kalitesi", "Texture Quality" });
        AddToDictionaries("EFFECT_QUALITY", new string[] { "Efekt Kalitesi", "Effects Quality" });
        AddToDictionaries("AA_QUALITY", new string[] { "Köşe Yumuşatma", "Anti Alias" });
        AddToDictionaries("HIGH", new string[] { "Yüksek", "High" });
        AddToDictionaries("NO_QUESTS", new string[] { "~ GÖREV YOK ~", "~ NO QUESTS ~" });
        AddToDictionaries("CHOOSE_TASK", new string[] { "~ YAN TARAFTAN BİR GÖREV SEÇİN ~", "~ CHOOSE A QUEST FROM THE SIDE ~" });
        AddToDictionaries("PRIZES", new string[] { "Ödüller", "Prizes" });
        AddToDictionaries("DURATION_COLON", new string[] { "Süre:", "Duration:" });
        AddToDictionaries("SKILL_SWORD", new string[] { "Kılıç Becerisi", "Sword Skill" });
        AddToDictionaries("SKILL_BOW", new string[] { "Ok Becerisi", "Bow Skill" });
        AddToDictionaries("SKILL_MAGIC", new string[] { "Büyü Becerisi", "Magic Skill" });

        AddToDictionaries("CONFIRM", new string[] { "ONAYLAYINIZ", "CONFIRM" });
        AddToDictionaries("START_GAME", new string[] { "Oyuna Başla", "Start Game" });
        AddToDictionaries("CHAR_DELETE", new string[] { "Karakteri Sil", "Delete Character" });
        AddToDictionaries("CHAR_LOAD", new string[] { "Karakteri Yükle", "Load Character" });
        AddToDictionaries("SKIN_COLOR", new string[] { "Cilt Rengi", "Skin Color" });
        AddToDictionaries("HAIR_COLOR", new string[] { "Saç Rengi", "Hair Color" });
        AddToDictionaries("HAIR_STYLE", new string[] { "Saç Stili", "Hair Style" });
        AddToDictionaries("SUIT_COLOR", new string[] { "Kostüm Rengi", "Suit Color" });
        AddToDictionaries("SEX_MALE", new string[] { "Cinsiyet: Erkek", "Sex: Male" });
        AddToDictionaries("SEX_FEMALE", new string[] { "Cinsiyet: Kadın", "Sex: Female" });
        AddToDictionaries("LAST_PLAYED", new string[] { "Son Oyun", "Last Played" });
        AddToDictionaries("YESTERDAY", new string[] { "Dün", "Yesterday" });
        AddToDictionaries("X_DAYS_AGO", new string[] { "%1 Gün Önce", "%1 Days Ago" });
        AddToDictionaries("PROMPT_DELETE_CHARACTER", new string[] { "Bu karakteri silmek istediğinizden emin misiniz?", "Are you sure you wish to delete this character?" });
        AddToDictionaries("MENU_EQU", new string[] { "Ekipman (<color=#888>F1</color>)", "Equipment (<color=#888>F1</color>)" });
        AddToDictionaries("TITLE_EQU", new string[] { "Ekipman", "Equipment" });
        AddToDictionaries("MENU_MENU", new string[] { "Menü (<color=#888>F10</color>)", "Menu (<color=#888>F10</color>)" });
        AddToDictionaries("MENU_INV", new string[] { "Envanter (<color=#888>F2</color>)", "Inventory (<color=#888>F2</color>)" });
        AddToDictionaries("MENU_TASKS", new string[] { "Görevler (<color=#888>F3</color>)", "Quests (<color=#888>F3</color>)" });
        AddToDictionaries("MENU_SKILLS", new string[] { "Yetenekler (<color=#888>F4</color>)", "Skills (<color=#888>F4</color>)" });
        AddToDictionaries("SHOP", new string[] { "Dükkan", "Shop" });
        AddToDictionaries("QUESTS_TITLE", new string[] { "Görevler", "Quests" });
        AddToDictionaries("LEVEL_1_NAME", new string[] { "Lanetli Orman", "Cursed Forest" });
        AddToDictionaries("INV_TITLE", new string[] { "Envanter", "Inventory" });
        AddToDictionaries("FULL_HEALTH", new string[] { "Sağlığınız zaten dolu", "Already inm full health" });
        AddToDictionaries("FULL_MANA", new string[] { "Büyü gücünüz zaten dolu", "Already inm full mana" });
        AddToDictionaries("LEVEL_UP", new string[] { "YENİ SEVİYE", "LEVEL UP" });

        AddToDictionaries("GOLD", new string[] { "Altın", "Gold" });
        AddToDictionaries("LIFE_POTION_MINI", new string[] { "Mini Hayat İksiri", "Tiny Life Potion" });
        AddToDictionaries("LIFE_POTION_MINI_DESC", new string[] { "Sağlığınızı arttırmaya yarar. Her türlü yaralanmadan sonra içebilirsiniz.\n\n<color=lime>Sağlık +20</color>", "Increases your health. Feel free to drink after any damage.\n\n<color=lime>Health +20</color>" });
        AddToDictionaries("LIFE_POTION_SMALL", new string[] { "Küçük Hayat İksiri", "Small Life Potion" });
        AddToDictionaries("LIFE_POTION_SMALL_DESC", new string[] { "Sağlığınızı arttırmaya yarar. Her türlü yaralanmadan sonra içebilirsiniz.\n\n<color=lime>Sağlık +30</color>", "Increases your health. Feel free to drink after any damage.\n\n<color=lime>Health +30</color>" });
        AddToDictionaries("LIFE_POTION_MEDIUM", new string[] { "Orta Hayat İksiri", "Medium Life Potion" });
        AddToDictionaries("LIFE_POTION_MEDIUM_DESC", new string[] { "Sağlığınızı arttırmaya yarar. Her türlü yaralanmadan sonra içebilirsiniz.\n\n<color=lime>Sağlık +40</color>", "Increases your health. Feel free to drink after any damage.\n\n<color=lime>Health +40</color>" });
        AddToDictionaries("LIFE_POTION_LARGE", new string[] { "Büyük Hayat İksiri", "Large Life Potion" });
        AddToDictionaries("LIFE_POTION_LARGE_DESC", new string[] { "Sağlığınızı arttırmaya yarar. Her türlü yaralanmadan sonra içebilirsiniz.\n\n<color=lime>Sağlık +60</color>", "Increases your health. Feel free to drink after any damage.\n\n<color=lime>Health +60</color>" });
        AddToDictionaries("LIFE_POTION_HUGE", new string[] { "Dev Hayat İksiri", "Huge Life Potion" });
        AddToDictionaries("LIFE_POTION_HUGE_DESC", new string[] { "Sağlığınızı arttırmaya yarar. Her türlü yaralanmadan sonra içebilirsiniz.\n\n<color=lime>Sağlık +90</color>", "Increases your health. Feel free to drink after any damage.\n\n<color=lime>Health +90</color>" });
        AddToDictionaries("MANA_POTION_MINI", new string[] { "Mini Mana İksiri", "Tiny Mana Potion" });
        AddToDictionaries("MANA_POTION_MINI_DESC", new string[] { "Büyücülük kapasitenizi arttırmaya yarar. Büyü yaptıktan sonra içebilirsiniz.\n\n<color=lime>Mana +20</color>", "Increases your mana. Feel free to drink after casting spells.\n\n<color=lime>Health +20</color>" });
        AddToDictionaries("MANA_POTION_SMALL", new string[] { "Küçük Mana İksiri", "Small Mana Potion" });
        AddToDictionaries("MANA_POTION_SMALL_DESC", new string[] { "Büyücülük kapasitenizi arttırmaya yarar. Büyü yaptıktan sonra içebilirsiniz.\n\n<color=lime>Mana +30</color>", "Increases your mana. Feel free to drink after casting spells.\n\n<color=lime>Health +30</color>" });
        AddToDictionaries("MANA_POTION_MEDIUM", new string[] { "Orta Mana İksiri", "Medium Mana Potion" });
        AddToDictionaries("MANA_POTION_MEDIUM_DESC", new string[] { "Büyücülük kapasitenizi arttırmaya yarar. Büyü yaptıktan sonra içebilirsiniz.\n\n<color=lime>Mana +40</color>", "Increases your mana. Feel free to drink after casting spells.\n\n<color=lime>Health +40</color>" });
        AddToDictionaries("MANA_POTION_LARGE", new string[] { "Büyük Mana İksiri", "Large Mana Potion" });
        AddToDictionaries("MANA_POTION_LARGE_DESC", new string[] { "Büyücülük kapasitenizi arttırmaya yarar. Büyü yaptıktan sonra içebilirsiniz.\n\n<color=lime>Mana +60</color>", "Increases your mana. Feel free to drink after casting spells.\n\n<color=lime>Health +60</color>" });
        AddToDictionaries("MANA_POTION_HUGE", new string[] { "Dev Mana İksiri", "Huge Mana Potion" });
        AddToDictionaries("MANA_POTION_HUGE_DESC", new string[] { "Büyücülük kapasitenizi arttırmaya yarar. Büyü yaptıktan sonra içebilirsiniz.\n\n<color=lime>Mana +90</color>", "Increases your mana. Feel free to drink after casting spells.\n\n<color=lime>Health +90</color>" });
        AddToDictionaries("ANTIDOTE_POTION_MINI", new string[] { "Mini Panzehir İksiri", "Tiny Antidote Potion" });
        AddToDictionaries("ANTIDOTE_POTION_MINI_DESC", new string[] { "Vücudunuzdan zehir temizler. Zehirli bir silah ya da büyüye maruz kaldıktan sonra içebilirsiniz.\n\n<color=lime>Zehir -20</color>", "Clears out poison from your body. You can drink after struck by a poisonous weapon or a spell.\n\n<color=lime>Poison -20</color>" });
        AddToDictionaries("ANTIDOTE_POTION_SMALL", new string[] { "Küçük Panzehir İksiri", "Small Antidote Potion" });
        AddToDictionaries("ANTIDOTE_POTION_SMALL_DESC", new string[] { "Vücudunuzdan zehir temizler. Zehirli bir silah ya da büyüye maruz kaldıktan sonra içebilirsiniz.\n\n<color=lime>Zehir -30</color>", "Clears out poison from your body. You can drink after struck by a poisonous weapon or a spell.\n\n<color=lime>Poison -30</color>" });
        AddToDictionaries("ANTIDOTE_POTION_MEDIUM", new string[] { "Orta Panzehir İksiri", "Medium Antidote Potion" });
        AddToDictionaries("ANTIDOTE_POTION_MEDIUM_DESC", new string[] { "Vücudunuzdan zehir temizler. Zehirli bir silah ya da büyüye maruz kaldıktan sonra içebilirsiniz.\n\n<color=lime>Zehir -40</color>", "Clears out poison from your body. You can drink after struck by a poisonous weapon or a spell.\n\n<color=lime>Poison -40</color>" });
        AddToDictionaries("ANTIDOTE_POTION_LARGE", new string[] { "Büyük Panzehir İksiri", "Large Antidote Potion" });
        AddToDictionaries("ANTIDOTE_POTION_LARGE_DESC", new string[] { "Vücudunuzdan zehir temizler. Zehirli bir silah ya da büyüye maruz kaldıktan sonra içebilirsiniz.\n\n<color=lime>Zehir -60</color>", "Clears out poison from your body. You can drink after struck by a poisonous weapon or a spell.\n\n<color=lime>Poison -60</color>" });
        AddToDictionaries("ANTIDOTE_POTION_HUGE", new string[] { "Dev Panzehir İksiri", "Huge Antidote Potion" });
        AddToDictionaries("ANTIDOTE_POTION_HUGE_DESC", new string[] { "Vücudunuzdan zehir temizler. Zehirli bir silah ya da büyüye maruz kaldıktan sonra içebilirsiniz.\n\n<color=lime>Zehir -90</color>", "Clears out poison from your body. You can drink after struck by a poisonous weapon or a spell.\n\n<color=lime>Poison -90</color>" });
        AddToDictionaries("REJUVENATION_POTION_MINI", new string[] { "Mini Gençlik İksiri", "Tiny Rejuvenation Potion" });
        AddToDictionaries("REJUVENATION_POTION_MINI_DESC", new string[] { "Sağlığınızı ve mananızı aynı anda arttırır.\n\n<color=lime>Sağlık +10, Mana +10</color>", "Increases health and mana at the same time.\n\n<color=lime>Health +10, Mana +10</color>" });
        AddToDictionaries("REJUVENATION_POTION_SMALL", new string[] { "Küçük Gençlik İksiri", "Small Rejuvenation Potion" });
        AddToDictionaries("REJUVENATION_POTION_SMALL_DESC", new string[] { "Sağlığınızı ve mananızı aynı anda arttırır.\n\n<color=lime>Sağlık +20, Mana +20</color>", "Increases health and mana at the same time.\n\n<color=lime>Health +20, Mana +20</color>" });
        AddToDictionaries("REJUVENATION_POTION_MEDIUM", new string[] { "Orta Gençlik İksiri", "Medium Rejuvenation Potion" });
        AddToDictionaries("REJUVENATION_POTION_MEDIUM_DESC", new string[] { "Sağlığınızı ve mananızı aynı anda arttırır.\n\n<color=lime>Sağlık +30, Mana +30</color>", "Increases health and mana at the same time.\n\n<color=lime>Health +30, Mana +30</color>" });
        AddToDictionaries("REJUVENATION_POTION_LARGE", new string[] { "Büyük Gençlik İksiri", "Large Rejuvenation Potion" });
        AddToDictionaries("REJUVENATION_POTION_LARGE_DESC", new string[] { "Sağlığınızı ve mananızı aynı anda arttırır.\n\n<color=lime>Sağlık +40, Mana +40</color>", "Increases health and mana at the same time.\n\n<color=lime>Health +40, Mana +40</color>" });
        AddToDictionaries("REJUVENATION_POTION_HUGE", new string[] { "Dev Gençlik İksiri", "Huge Rejuvenation Potion" });
        AddToDictionaries("REJUVENATION_POTION_HUGE_DESC", new string[] { "Sağlığınızı ve mananızı aynı anda arttırır.\n\n<color=lime>Sağlık +50, Mana +50</color>", "Increases health and mana at the same time.\n\n<color=lime>Health +50, Mana +50</color>" });
        AddToDictionaries("BRACER_WOODEN", new string[] { "Tahta Bileklik", "Wooden Bracer" });
        AddToDictionaries("BRACER_WOODEN_DESC", new string[] { "Kollarınızı saldırılara karşı daha dayanıklı yapar.\n\n<color=yellow>zırh: </color><color=lime>2-3</color>", "Makes your arms lass vulnerable in battle.\n\n<color=yellow>armor: </color><color=lime>2-3</color>" });
        AddToDictionaries("BOOTS_BEAR", new string[] { "Ayı Botları", "Boots of Bear" });
        AddToDictionaries("BOOTS_BEAR_DESC", new string[] { "Ayı kürkünden yapısı ayağınızı rahat ettirir ve saldırılardan daha hızlı kaçmanızı sağlar.\n\n<color=yellow>zırh: </color><color=lime>3-4</color>", "Your feet will be relaxed inside those boots made of bear fur and you will be able to avoid attacks faster.\n\n<color=yellow>armor: </color><color=lime>3-4</color>" });
        AddToDictionaries("BOW_LONG", new string[] { "Uzun Yay", "Long Bow" });
        AddToDictionaries("BOW_LONG_DESC", new string[] { "Uzaktaki ve düşük seviyeli düşmanlara karşı etkilidir.\n\n<color=yellow>hasar: </color><color=lime>13-19</color>\n<color=yellow>hız: </color><color=olive>orta</color>", "Effective against distant low level enemies.\n\n<color=yellow>damage: </color><color=lime>13-19</color>\n<color=yellow>speed: </color><color=olive>medium</color>" });
        AddToDictionaries("BOW_FLEXI", new string[] { "Esnek Yay", "Flexi Bow" });
        AddToDictionaries("BOW_FLEXI_DESC", new string[] { "Uzaktaki düşük ve orta seviyeden düşmanlara karşı etkilidir\n\n<color=yellow>hasar: </color><color=lime>17-22</color>\n<color=yellow>hız: </color><color=olive>orta</color>", "Effective against distant low and medium level enemies.\n\n<color=yellow>damage: </color><color=lime>17-22</color>\n<color=yellow>speed: </color><color=olive>medium</color>" });
        AddToDictionaries("ARMOR_SILK_CLOTH", new string[] { "İpek Giyisi", "Silk Cloth" });
        AddToDictionaries("ARMOR_SILK_CLOTH_DESC", new string[] { "Kalın ipek dokuması silahların vereceği hasarı azaltır\n\n<color=yellow>zırh: </color><color=lime>6-8</color>", "Its thick silk weaving reduces the damage caused by enemy weapons\n\n<color=yellow>armor: </color><color=lime>6-8</color>" });
        AddToDictionaries("SHIELD_BROKEN", new string[] { "Kırık Kalkan", "Broken Shield" });
        AddToDictionaries("SHIELD_BROKEN_DESC", new string[] { "Hiç yoktan iyidir, kırık olabilir fakat bu işe yaramayacağı anlamına gelmez!\n\n<color=yellow>zırh: </color><color=lime>4-11</color>", "Better than nothing, being broken does not mean that it is useless!\n\n<color=yellow>armor: </color><color=lime>4-11</color>" });
        AddToDictionaries("SWORD_RUSTY", new string[] { "Paslı Kılıç", "Rusty Sword" });
        AddToDictionaries("SWORD_RUSTY_DESC", new string[] { "İyi görünmüyor olabilir fakat keskin olmayan kılıçlar daha çok can acıtır. Ayrıca düşmanların tetanoz olmasını sağlayabilir.\n\n<color=yellow>hasar: </color><color=lime>18-24</color>", "It may not look well but non-sharp swords may hurt more. Also can cause enemies to become tetanus\n\n<color=yellow>damage: </color><color=lime>18-24</color>" });
        AddToDictionaries("SWORD_MYSTIQUE", new string[] { "Mistik Kılıç", "Mystiuqe Sword" });
        AddToDictionaries("SWORD_MYSTIQUE_DESC", new string[] { "Büyük ve ağır bir kılıç, vurduğu kişi yerinde olmak istemezsin.\n\n<color=yellow>hasar: </color><color=lime>20-29</color>\n<color=yellow>hız: </color><color=red>-1</color>", "A heavy and big sword, you wouldn't like to be the one it hits.\n\n<color=yellow>damage: </color><color=lime>20-29</color>\n<color=yellow>speed: </color><color=red>-1</color>" });
        AddToDictionaries("STAFF_OLD", new string[] { "Eski Asa", "Old Staff" });
        AddToDictionaries("STAFF_OLD_DESC", new string[] { "Eski, çürük ve kirli bir odun parçasına benziyor fakat epey can acıtıyor\n\n<color=yellow>hasar: </color><color=lime>11-15</color>", "Looking like an old, dirty and useless piece of wood but it hurts bad\n\n<color=yellow>damage: </color><color=lime>11-15</color>" });
        AddToDictionaries("STAFF_MAGE", new string[] { "Büyücü Asası", "Mage's Staff" });
        AddToDictionaries("STAFF_MAGE_DESC", new string[] { "Topuzundan ışıklar çıkaran koca bir odun parçası, bunu gördüğün yerde kaçsan iyi edersin.\n\n<color=yellow>hasar: </color><color=lime>9-16</color>\n<color=yellow>hız: </color>\n<color=yellow>mana: </color><color=lime>+2</color>", "You better run when you see this big piece of wood which has a blinking gem on its top.\n\n<color=yellow>damage: </color><color=lime>9-16</color>\n<color=yellow>speed: </color>\n<color=yellow>mana: </color><color=lime>+2</color>" });
        AddToDictionaries("USELESS_APPLE_FRESH", new string[] { "Taze Elma", "Fresh Apple" });
        AddToDictionaries("USELESS_APPLE_FRESH_DESC", new string[] { "Cildinizi güzelleştirir, ağzınızı tatlandırır. Düzenli tüketilmesinde fayda var.", "Beautifies your skin, tastes good. Frequent usage will be good for you." });
        AddToDictionaries("USELESS_SPIDER_TOOTH", new string[] { "Örümcek Dişi", "Spider Tooth" });
        AddToDictionaries("USELESS_SPIDER_TOOTH_DESC", new string[] { "Büyücülük malzemesi olarak sihirli iksirlerde kullanılır. Bazı yaşlı büyücüler buna birkaç sefil kuruş ödeyebilir", "Used in potions as a magical receipe ingredient. Old magicians may pay a few mere coins for this." });
        AddToDictionaries("USELESS_BREAD_LOAF", new string[] { "Ekmek Somunu", "Bread Loaf" });
        AddToDictionaries("USELESS_BREAD_LOAF_DESC", new string[] { "Karnınızı doyurmasa da pazarda birkaç sefil kuruş edebilir. Etmeyedebilir.", "Although it would not end your hunger, may be it would cost a few mere coins in the market. Or may be not." });
        AddToDictionaries("ITEMS_FOR_SALE", new string[] { "Satılık Eşyalar", "Items for Sale", });
        AddToDictionaries("BUY", new string[] { "Satın Al", "Buy", });
        AddToDictionaries("ITEMS_FOR_BUY", new string[] { "Satabileceğin Eşyalar", "Items you could Sell", });
        AddToDictionaries("SELL", new string[] { "Sat", "Sell", });

        AddToDictionaries("PRETEXT_1", new string[] { "YEDİNCİ PADİŞAH", "SEVENTH EMPEROR" });
        AddToDictionaries("PRETEXT_2", new string[] { "BİZANS LANETİ", "A BYZANTIAN CURSE" });
        AddToDictionaries("PRETEXT_3", new string[] { "AYASOFYA", "HAGIA SOPHIA" });
        AddToDictionaries("PRETEXT_1_DESC", new string[] { "FATİH SULTAN MEHMET, OSMANLI İMPARATORLUĞU'NUN YEDİNCİ PADİŞAHIDIR", "MEHMED THE CONQUEROR WAS THE SEVENTH EMPEROR OF OTTOMAN EMPIRE" });
        AddToDictionaries("PRETEXT_2_DESC", new string[] { "BİZANSLILAR KUŞATMANIN BU KEZ BAŞARILI OLACAĞINI ANLAYINCA FATİH'E BİR KARA BÜYÜ YAPTIRDILAR. FATİH'İN BU KARA BÜYÜDEN KURTULMAK İÇİN TEK ÜMİDİ SENSİN!", "BYZANTINIANS HAVE CASTED A DARK SPELL OF MEHMED THE CONQUEROR WHEN THEY FORESAW THE NEXT INVASION ATTEMPT OF HIM WILL BE SUCCESSFUL. YOU ARE THE ONLY HOPE OF THE EMPEROR MEHMED NOW!" });
        AddToDictionaries("PRETEXT_3_DESC", new string[] { "KONSTANTİNİYYE FETHEDİLDİĞİ DÖNEMDE DÜNYADAKİ EN BÜYÜK KAPALI ALANA SAHİP YAPI AYASOFYA İDİ", "HAGIA SOPHIA WAS THE STRUCTURE WHICH HAS THE BIGGEST INDOOR AREA IN THE WORLD WHEN CONSTANTINOPLE WAS INVADED" });

        AddToDictionaries("CLASS_ARCHER_DESC", new string[] { "Okçular düşmanlarla uzaktan savaşmak konusunda çok başarılıdır. Bu nedenle hayatta kalma olasılıkları daha yüksektir ancak yakınlarındaki düşmanlara karşı her zaman çok etkili olamayabilirler.",
                                                            "Archers are successfull at dealing with enemies from a distance. For this reason, their possibility of survival is higher but they may not be very effective against enemies when they are close." });
        AddToDictionaries("CLASS_MAGE_DESC", new string[] { "Büyücüler düşmanlarla toplu halde başa çıkmak ve onlarla uzaktan savaşmak konusunda çok başarılıdır. Fakat özellikle düşük seviyelerde çok kırılgan ve yavaş olabilirler.",
                                                            "Mages are very successfull at dealing with mass enemy groups from a distance. On the other hand they may be fragile and vulnerable especially in lower levels." });
        AddToDictionaries("CLASS_PALADIN_DESC", new string[] { "Şövalyeler güçlü, dayanıklı ve özellikle yakın dövüşlerde çok etkilidirler. Dayanıklılıkları yüksektir, bununla birlikte uzaktan etkisizdirler ve bazan bir parça yavaş kalabilirler.",
                                                            "Paladins are strong, durable and especially very good at melee combat. Their stamina are high, however they are not very effective from a distance." });
        AddToDictionaries("CLASS_THIEF_DESC", new string[] { "Hızlı ve çevik yapılarıyla düşman grupları tarafından sıkıştırılmaları zordur. Sandıklardan, düşman ölülerinden ve görevlerden daha fazla altın çıkarabilirler. Fakat pek de güçlü değillerdir.",
                                                            "Their high speed and dexterity makes it difficult for enemies to capture thieves. They can extract higher amount of gold from chests, enemy coprses and quests. However they are not very strong." });

        // speech
        AddToDictionaries("BYE_FOR_NOW", new string[] { "Şimdilik bu kadar", "It's all for now" });
        AddToDictionaries("PREVIOUS_SPEECH", new string[] { "Önceki konuşma...", "Previous speech..." });
        AddToDictionaries("YOU", new string[] { "Siz", "You" });
        AddToDictionaries("HI", new string[] { "Merhaba!", "Hi!" });
        AddToDictionaries("NEVER_MIND", new string[] { "Boşver", "Never Mind" });
        AddToDictionaries("IHAVETOGONOW", new string[] { "Şimdi gitmem gerek, daha sonra belki yine gelirim", "I have to go now, may be I come back later" });
        AddToDictionaries("START_CONVERSATION", new string[] { "KONUŞMA BAŞLAT", "START A CONVERSATION" });
        AddToDictionaries("START_CONVERSATION_FULL", new string[] { "Konuşmayı başlatmak için aşağıdaki sözlerden birini seçin", "Select one of the sentences below to start a conversation" });
        AddToDictionaries("IWANTTOSELL", new string[] { "Elimdekileri satmak istiyorum", "I want to sell from my inventory" });
        AddToDictionaries("IWANTTOBUY", new string[] { "Satılık nelerin var?", "What do you have for sale?" });
        AddToDictionaries("MYSTERIOUSMAGE", new string[] { "Gizemli Büyücü", "Mysterious Mage" });
        AddToDictionaries("MERCANTHEMAGE", new string[] { "Büyücü Mercan", "Mercan the Mage" });
        AddToDictionaries("PORTALKEEPER", new string[] { "Portal Bekçisi", "Portal Keeper" });
        AddToDictionaries("MYSTERIOUSKNIGHT", new string[] { "Gizemli Şövalye", "Mysterious Knight" });
        AddToDictionaries("BLACKSMITH", new string[] { "Demirci Ustası", "Blacksmith" });
        AddToDictionaries("WELCOMESTRANGER", new string[] { "Hoş geldin yabancı", "Welcome stranger" });
        AddToDictionaries("MERCANWELCOME", new string[] { "Merhaba büyücü. Osmanlı İmparatorluğu'nun kudretli hükümdarı Sultan Mehmed Han hazretleri adına özel bir görev için burada bulunuyorum. Bana yardım et veya kendine yardım edecek birisini ara...", "Greetings wise mage! I am here on a special misson on behalf of the mighty emperor Mehmet the Second of Ottoman Empire. Help me or look for someone to help yourself..." });
        AddToDictionaries("MAERCANRESPONSE", new string[] { "Köyümüze hoş geldin yabancı. Buralarda nadiren yabancı birilerini görürüz. Buraya kadar tek parça gelebildiğine göre yaman bir savaşçısın. Sana yardım edebilirim, ama önce benim için küçük bir iş yapabilir misin? Karşılığında seni fazlasıyla ödüllendiririm.", "Welcome to our village stranger. We rarely see new faces here. Because you could make it this far in one piece, you should be a tough warrior. I can help you, but before that can you make a favor? I can reward you handsomely." });
        AddToDictionaries("RESPONSE", new string[] { "Cevap", "Response" });
        AddToDictionaries("YESSURE", new string[] { "Evet, elbette", "Yes, sure" });
        AddToDictionaries("MERCAN_YES", new string[] { "Eğer görevim için bana yardım edeceksen neden olmasın? Peki senin için ne yapmamı istiyorsun bilge büyücü?", "If you will help me on my mission, why shouldn't I? What do you want me to do for you?" });
        AddToDictionaries("MERCAN_MISSION_0", new string[] { "Verdiğin görev hakkında", "About that mission..." });
        AddToDictionaries("MERCAN_MISSION_0_DESC", new string[] { "Bana verdiğin görevi tekrar anlatabilir misin?", "Can you tell me once more about that mission you told about?" });
        AddToDictionaries("MERCAN_MISSION_1", new string[] { "Görev 1", "Mission 1" });
        AddToDictionaries("MERCAN_MISSION_1_DESC", new string[] { "Ormanda dikili taşlar göreceksin. Dikili taşların ortasında Portal Bekçisi bulunur. Ona git ve seni benim gönderdiğimi söyle. Sonrasında portaldan geçerek canavarların benden çaldığı kitabı bul ve bana geri getir. Fakat kitabı kolay teslim etmeyeceklerdir, kendini sıkı bir saşava hazırla!", "You will see obelisks in the forest. There will be a portal keeper next to them. Go to him and tell that I sent you. Then pass through the portal that he will open for you, find my book in the world it leads to and bring it back to me. But be ware, these creatures won't give up so easily! Prepare yourself for a tough battle" });
        AddToDictionaries("MERCAN_ABOUT_BOOK", new string[] { "Kitap neden bu kadar önemli?", "Why is that book so important?" });
        AddToDictionaries("MERCAN_ABOUT_BOOK_DESC", new string[] { "Senden çalmak için bu kadar uğraşıp sonra da gizemli bir boyuta ışınlandıklarına göre çok önemli bir kitap olmalı bu. Neden peki?", "Considering the effort of them to steal the book from you and run, it should be an important thing. Why?" });
        AddToDictionaries("MERCAN_CANT_TELL_NOW", new string[] { "Bunu şimdilik sana söyleyemem yabancı, fakat kitabım geri geldiğinde bunun hem sana hem de bana faydası olacağından emin olabilirsin. O kitabın yanlış ellere geçmemesi gerekiyor!", "I can't tell it to you now stranger, but you can be sure that when it comes back it will be good for everyone, including you. That book must not be in the wrong hands!" });
        AddToDictionaries("MERCAN_WHY_NOT_YOURSELF", new string[] { "Neden kendin almıyorsun?", "Why are you not taking it yourself?" });
        AddToDictionaries("MERCAN_WHY_NOT_YOURSELF_DESC", new string[] { "Portal Bekçisi'ni tanıyorsun, canavarların nerede saklandığını da biliyorsun. Üstelik kitap da senin için bu kadar önemliyken neden kitabı kendin almaya çalışmıyorsun?", "You know the Portal Keeper, you know where these creatures took it, so why don't you go and get it by yourself?" });
        AddToDictionaries("MERCAN_I_DONT_BECAUSE", new string[] { "Yüzlerce yıldır ruhumu taşıyan bu gövde artık göründüğü kadar güçlü değil. Ayrıca kaderlerimiz belki başka bir gün yine kesişebilir, o gün geldiğinde görevine destek olabilmek için tek parça halinde kalmalıyım.", "That body, which holds my sould for centuries is not as powerful as it seems. Also our paths may cross again some day, I need to stay in one piece in order to help you when that day comes." });
        AddToDictionaries("MERCAN_WHERE_FROM", new string[] { "Canavarlar nereden geliyor?", "Where do these monsters come from?" });
        AddToDictionaries("MERCAN_WHERE_FROM_DESC", new string[] { "Bu canavarların geldiği yere açılan portal, tam olarak nereye gidiyor?", "The portal which leads to the dimension of monsters, where exactly it is opening to?" });
        AddToDictionaries("MERCON_THEY_COME_FROM", new string[] { "Bunu anlaman zor olabilir, fakat aslında tam olarak hiçbir yerden gelmiyorlar. Kendilerini var eden hiçlikten çıkıp sonra da hiçliğe geri dönüyorlar. Şimdi bunu anlamadığını biliyorum, fakat bir gün anlayacaksın.", "It may be difficult for you to understand this, but they come out of nowhere. They appear from the nonsense which it creates them, then they go back into it. I know you do not understand it now, but you will some day." });
        AddToDictionaries("MERCAN_BYE_FOR_NOW", new string[] { "Bu kadarı şimdilik yeterli. Görevimi mümkün olduğunca çabuk bir şekilde bitirip kitabı sana getirmeye çalışacağım", "That's enough for now. I will do best to accomplish my mission and bring that book back to you as soon as possible" });
        AddToDictionaries("PK_GREET", new string[] { "Selam olsun sana güçlü savaşçı. Osmanlı İmparatorluğu'nun kudretli hükümdarı Sultan Mehmed Han hazretleri adına özel bir görev için burada bulunuyorum. Bana yardım et veya kendine yardım edecek birisini ara...", "Greetings to you mighty warrior. I am here on a special misson on behalf of the mighty emperor Mehmet the Second of Ottoman Empire. Help me or look for someone to help yourself..." });
        AddToDictionaries("PK_KEEPOUT", new string[] { "Buradan uzak dur", "Keep your distance" });
        AddToDictionaries("PK_KEEPOUT_DESC", new string[] { "Sana da selam olsun Yiğit savaşçı. Ben Portal Bekçisi'yim, bu taşların arasından çeşitli boyutlara açılan kapının koruyucusuyum. Benim görevim bu portaldan kimsenin geçmediğinden emin olmak. Buna sen de dahil.", "Greetings to you powerful one. I am the Keeper of Portal, which leads to the various dimensions from between these obelisks. My mission is to make sure that no one travels through the portal. Including you." });
        AddToDictionaries("PK_BYE", new string[] { "Sana kolay gelsin", "Good luck then" });
        AddToDictionaries("PK_BYE_DESC", new string[] { "Benim de bir görevim var, fakat bahsettiğin portal ile bir ilgim yok. Sana görevinde kolay gelsin", "I have a mission too, but I do not have any intend to cross through that portal. Good luck with your mission." });
        AddToDictionaries("PK_OPEN_PORTAL", new string[] { "Portalı aç", "Open the portal" });
        AddToDictionaries("PK_OPEN_PORTAL_DESC", new string[] { "Beni Büyücü Mercan gönderdi, gizli bir görev için senden portalı açmanı talep ediyorum.", "Mercan the Mage has sent me, I ask you to open the portal for a secret mission." });
        AddToDictionaries("PK_OPEN_ACCEPTED", new string[] { "Mercan'ın isteği emirdir", "Mercan's request is an order for me" });
        AddToDictionaries("PK_OPEN_ACCEPTED_DESC", new string[] { "Büyücü Mercan istiyorsa başım üstüne. Peki portalın neresi için açılmasını istersin yiğit savaşçı?", "If Mercan is asking, I do it gladly. So, where do you want the portal to lead to?" });
        AddToDictionaries("PK_PORTAL_DEST1", new string[] { "Yaratıkların geldiği hiçlik dünyasına", "To the nonsense dimension, which mysterious creatures come from" });
        AddToDictionaries("PK_PORTAL_DEST1_DESC", new string[] { "Mercan'ın kitabını çalan yaratıkların geldiği hiçlik dünyasına açtırmak istiyorum", "To the nonsense dimension, which mysterious creatures that stole Mercan's book come from" });
        AddToDictionaries("PK_PORTAL_DEST2", new string[] { "Yeraltı tünellerine", "To the underground tunnels" });
        AddToDictionaries("PK_PORTAL_DEST2_DESC", new string[] { "Lekath'ın izini sürebileceğim yeraltı tünellerine", "To the underground tunnels, where I can follow Lekath's trace" });
        AddToDictionaries("CHANGED_MY_MIND", new string[] { "Fikrimi değiştirdim", "I changed my mind" });
        AddToDictionaries("CHANGED_MY_MIND_DESC", new string[] { "Tekrar düşündüm de, şimdi gitmem lazım. Bir ara yine uğrarım.", "On second thought, I have to go now. I may come back some other time." });
        AddToDictionaries("HI_PARS", new string[] { "Merhaba usta demirci. Devletim ve hükümdarım adına önemli bir görev için buraya geldim. Bana yardımcı olabilecek bir şeylerin var mı?", "Hello strong blacksmith. I came here for a secret mission for my emperor and my country. Do you have anything that can help my mission?" });
        AddToDictionaries("WELCOMESTRANGER_DESC", new string[] { "Selam olsun güçlü savaşçı! Allah seni de, devletini de, hükümdarını da muvaffak kılsın. Eğer işine lazım olursa satılık eşyalarım var, veya elindeki eşyaları da uygun fiyattan satın alabilirim. Gelip geçtikçe uğra, arada ilginç şeyler denk gelebiliyor.", "Hello to you mighty warrior! May God help you on your mission. Feel free to check out the items that I sell, you may find something useful, or I could make interesting offers if you wish to sell anything. Feel free to drop by anytime, interesting items may come some days." });
        AddToDictionaries("SKIP_TEXT", new string[] { "Geçmek için <color=yellow>[Boşluk]</color> tuşuna basın", "Press <color=yellow>[Space]</color> to skip" });
        AddToDictionaries("SKILLS_MAGE", new string[] { "Yetenekler: Büyücü", "Skills: Mage" });
        AddToDictionaries("SKILLS_ARCHER", new string[] { "Yetenekler: Okçu", "Skills: Archer" });

        // quests
        AddToDictionaries("QUEST_1_TITLE", new string[] { "Büyücünün Kitabı", "Magician's Book" });
        AddToDictionaries("QUEST_1_DESC", new string[] { "Büyücü Mercan’ın kitabını çalıp bilinmeyen bir boyuta götüren canavarları bul ve kitabı onlardan geri al. Sihirli taşların yanında duran Portal Bekçisi'ne seni Mercan'ın gönderdiğini söylersen canavarların boyutuna bir portal açtırabilirsin.", "Find the mysterious creatures which took Mercan's book into a non existing dimension and take it back from them. Proceed to the Portal Keeper, who stands next to the obelisks in the forest and tell him Mercan sent you, so he will open the portal." });
        AddToDictionaries("QUEST_1_ITEM_1", new string[] { "Portal geçidini açtır", "Open the portal gate" });
        AddToDictionaries("QUEST_1_ITEM_2", new string[] { "Canavarların liderini öldür", "Kill the leader of monsters" });
        AddToDictionaries("QUEST_1_ITEM_3", new string[] { "Kitabı Mercan'a geri getir", "Bring the book back to Mercan" });
        AddToDictionaries("QUEST_1_ITEM_1_DESC", new string[] { "", "" });
        AddToDictionaries("QUEST_1_ITEM_2_DESC", new string[] { "", "" });
        AddToDictionaries("QUEST_1_ITEM_3_DESC", new string[] { "", "" });

        // resource items
        AddToResources("ui_logo", new string[] { "ui_logo", "ui_logo_en" });
        AddToResources("ui_logo_top", new string[] { "ui_logo_top", "ui_logo_top_en" });
    }

    public void AddToDictionaries(string key, string[] items)
    {
        LangDatabaseEN.Add(key, items[1]);
        LangDatabaseTR.Add(key, items[0]);
    }

    public void AddToResources(string key, string[] items)
    {
        LangResourceEN.Add(key, items[1]);
        LangResourceTR.Add(key, items[0]);
    }
}
