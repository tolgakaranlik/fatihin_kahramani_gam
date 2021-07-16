using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
using DG;
using DG.Tweening;
using System;
using System.Collections.Generic;

public class GUILoading : MonoBehaviour
{
    public Text InfoTitle;
    public Text InfoDetail;
    public Slider Loader;
    public GameObject PnlInitial;
    public GameObject ImgLogo;
    public GameObject ImgLogoTop;
    public GameObject ImgEffect;
    public GameObject TxtPressAnyKey;
    public GameObject TxtDisclaimer;
    public GameObject WinMainMenu;
    public GameObject WinSettings;
    public GameObject WinConfirm;
    public GameObject WinLoadCharacter;
    public GameObject WinNewCharacter;
    public GameObject PnlDark;
    public GameObject Background;
    public AudioSource BGMusic;
    public Dropdown ResolutionList;
    public float EffectDuration = 5.75f;
    public float PressKeyDuration = 1.5f;

    public GameObject BadgeArcher;
    public GameObject BadgeMage;
    public GameObject BadgeThief;
    public GameObject BadgePaladin;

    public GameObject[] CharacterModels;
    public PostProcessLayer PostProcess;
    public GameObject ActionCamera;

    public Text SexDisplay;
    public Text ClassDisplayLeft;
    public GameObject[] ClassIconsLeft;
    public GameObject[] ClassIconsRight;
    public Text[] ClassDisplaysRight;
    public GameObject[] SpellSets;
    public GameObject[] Attributes;
    public Text ClassDescription;

    public Material[] SkinMaterialsMale;
    public Material[] SkinMaterialsFemale;
    public Material[] HairMaterialsMale;
    public Material[] HairMaterialsFemale;

    public Material[] SuitMaterialsArcherMale;
    public Material[] SuitMaterialsArcherFemale;
    public Material[] SuitMaterialsMageMale;
    public Material[] SuitMaterialsMageFemale;
    public Material[] SuitMaterialsRangerMale;
    public Material[] SuitMaterialsRangerFemale;
    public Material[] SuitMaterialsPaladinMale;
    public Material[] SuitMaterialsPaladinFemale;

    public LocalizationManager Localization;

    bool canTapNow = false;
    bool displayingDisclaimer = false;
    bool finishingDisclaimer = false;
    bool loadCharacterDisplayed = false;
    bool canProcessResolutionChange = true;
    int displayedCharacter = 0;
    int selectedBodyMaterial = 0;
    int selectedHairMaterial = 0;
    int selectedHairStyle = 0;
    int selectedSuitMaterial = 1;
    int selectedSex = SEX_MALE;
    int lastValidResolution = 0;

    const int SEX_MALE = 0;
    const int SEX_FEMALE = 1;

    public delegate void UserChoice();

    void Start()
    {
        //Screen.SetResolution(1920, 1080, true);

        InfoTitle.gameObject.SetActive(false);
        InfoDetail.gameObject.SetActive(false);
        Loader.gameObject.SetActive(false);
        PnlInitial.SetActive(true);
        ImgLogo.SetActive(false);
        ImgLogoTop.SetActive(false);
        ImgEffect.SetActive(false);
        TxtDisclaimer.SetActive(false);
        TxtPressAnyKey.SetActive(false);
        WinMainMenu.SetActive(false);
        WinSettings.SetActive(false);
        WinLoadCharacter.SetActive(false);
        WinNewCharacter.SetActive(false);
        WinConfirm.SetActive(false);
        Background.SetActive(false);
        PnlDark.SetActive(false);
        ActionCamera.SetActive(true);

        StartCoroutine(DoTheOpening());
    }

    public void LoadCharacter()
    {
        TurnDOF(true);

        HideSettings();

        WinLoadCharacter.SetActive(true);
        WinLoadCharacter.GetComponent<RectTransform>().DOAnchorPosX(-300, 0.35f);

        DisplayPreveiouslyPlayedCharacter(0);
    }

    private void TurnDOF(bool v)
    {
        List<PostProcessVolume> volList = new List<PostProcessVolume>();
        PostProcessManager.instance.GetActiveVolumes(PostProcess, volList, true, true);

        foreach (PostProcessVolume vol in volList)
        {
            PostProcessProfile ppp = vol.profile;
            if (ppp)
            {
                DepthOfField dph;
                if (ppp.TryGetSettings<DepthOfField>(out dph))
                {
                    //dph.active = v;
                    DOTween.To(() => dph.focalLength.value, x => dph.focalLength.value = x, v ? 65 : 50, 1.5f);
                }

                var b = ActionCamera.GetComponent<UnityStandardAssets.ImageEffects.Bloom>();
                DOTween.To(() => b.bloomThreshold, x => b.bloomThreshold = x, v ? 0.2f : 0.04f, 1.5f);
                //b.enabled = !v;
            }
        }
    }

    public void DisplayLoadCharacterNext()
    {
        displayedCharacter = (displayedCharacter + 1) % 4; //CharacterModels.Length;

        DisplayLoadCahacter(displayedCharacter);
    }

    public void DisplayLoadCharacterPrev()
    {
        displayedCharacter -= 1;
        if(displayedCharacter < 0)
        {
            displayedCharacter = 3; //CharacterModels.Length - 1;
        }

        DisplayLoadCahacter(displayedCharacter);
    }

    public void DisplayLoadSexNext()
    {
        if(selectedSex != SEX_FEMALE)
        {
            selectedSex = SEX_FEMALE;

            SexDisplay.text = Localization.Translate("SEX_FEMALE").ToUpper();
        } else
        {
            selectedSex = SEX_MALE;

            SexDisplay.text = Localization.Translate("SEX_MALE").ToUpper();
        }

        DisplayLoadCahacter(displayedCharacter);
        DisplayHairStyle();
        DisplayHairMaterial();
        DisplaySuitMaterial();
    }

    public void DisplayPreveiouslyPlayedCharacter(int index)
    {
        PlayerPrefs.SetInt("SelectedCharacter", index);

        BadgeArcher.SetActive(false);
        BadgeMage.SetActive(false);
        BadgePaladin.SetActive(false);
        BadgeThief.SetActive(false);

        var txtName = WinLoadCharacter.transform.Find("TxtTitle").GetComponent<Text>();
        var txtInfo = WinLoadCharacter.transform.Find("TxtInfo").GetComponent<Text>();
        var arrowLeft = WinLoadCharacter.transform.Find("ArrowLeft").GetComponent<Button>();
        var arrowRight = WinLoadCharacter.transform.Find("ArrowRight").GetComponent<Button>();

        switch (index)
        {
            case 0:
                arrowLeft.interactable = false;
                arrowRight.interactable = true;

                txtName.text = "Fuego";
                txtInfo.text = "Level 2\n"+ Localization.Translate("LAST_PLAYED") +": " + Localization.Translate("YESTERDAY");

                BadgeArcher.SetActive(true);

                selectedBodyMaterial = 0;
                DisplayNewCahacter(0, SEX_FEMALE, 0, 0, 0, 0);

                for (int i = 0; i < CharacterModels.Length; i++)
                {
                    CharacterModels[i].SetActive(i == 4);
                }

                break;
            case 1:
                arrowLeft.interactable = true;
                arrowRight.interactable = false;

                txtName.text = "Kalisto";
                txtInfo.text = "Level 2\n"+ Localization.Translate("LAST_PLAYED") + ": " + Localization.Translate("X_DAYS_AGO", new string[] { "2" });
                BadgeMage.SetActive(true);

                selectedBodyMaterial = 2;
                DisplayNewCahacter(1, SEX_MALE, 0, 0, 0, 1);

                for (int i = 0; i < CharacterModels.Length; i++)
                {
                    CharacterModels[i].SetActive(i == 1);
                }

                break;
        }
    }

    public void DisplayLoadCahacter(int index)
    {
        Color c;
        for (int i = 0; i < ClassIconsLeft.Length; i++)
        {
            ClassIconsLeft[i].SetActive(i == (index % 4));

            c = ClassIconsRight[i].GetComponent<Image>().color;
            ClassIconsRight[i].GetComponent<Image>().color = new Color(c.r, c.g, c.b, i == (index % 4) ? 1 : 0.2f);
            ClassDisplaysRight[i].GetComponent<Text>().color = new Color(c.r, c.g, c.b, i == (index % 4) ? 1 : 0.2f);
            SpellSets[i].SetActive(i == (index % 4));
        }

        switch (index % 4)
        {
            case 0:
                DisplayAttributes(new string[] { "4", "4", "10", "40", "4", "10", "70", "10", "20" });

                ClassDisplayLeft.text = Localization.Translate("CLASS_ARCHER");
                ClassDescription.text = Localization.Translate("CLASS_ARCHER_DESC");
                break;
            case 1:
                DisplayAttributes(new string[] { "3", "2", "7", "35", "3", "24", "10", "10", "70" });

                ClassDisplayLeft.text = Localization.Translate("CLASS_MAGE");
                ClassDescription.text = Localization.Translate("CLASS_MAGE_DESC");
                break;
            case 2:
                DisplayAttributes(new string[] { "6", "3", "16", "50", "6", "10", "10", "70", "15" });

                ClassDisplayLeft.text = Localization.Translate("CLASS_PALADIN");
                ClassDescription.text = Localization.Translate("CLASS_PALADIN_DESC");
                break;
            case 3:
                DisplayAttributes(new string[] { "5", "5", "20", "40", "5", "14", "10", "40", "40" });

                ClassDisplayLeft.text = Localization.Translate("CLASS_THIEF");
                ClassDescription.text = Localization.Translate("CLASS_THIEF_DESC");
                break;
        }

        loadCharacterDisplayed = true;
        for (int i = 0; i < CharacterModels.Length; i++)
        {
            CharacterModels[i].SetActive(i == (index + (selectedSex == SEX_MALE ? 0 : 4)));
        }

        DisplayBodyMaterial();
        DisplayHairMaterial();
        DisplaySuitMaterial();
        DisplayHairStyle();
    }

    private void DisplayAttributes(string[] v)
    {
        for (int i = 0; i < Attributes.Length; i++)
        {
            Attributes[i].transform.Find("Value").GetComponent<Text>().text = v[i];
        }
    }

    public void HideLoadCharacter()
    {
        TurnDOF(false);
        loadCharacterDisplayed = false;

        StartCoroutine(HideLoadCharacterNow());
    }

    IEnumerator HideLoadCharacterNow()
    {
        WinLoadCharacter.GetComponent<RectTransform>().DOAnchorPosX(300, 0.35f);

        yield return new WaitForSeconds(0.35f);

        WinLoadCharacter.SetActive(true);

        for (int i = 0; i < CharacterModels.Length; i++)
        {
            CharacterModels[i].SetActive(false);
        }
    }

    public void LoadNext()
    {
        HideLoadCharacter();

        StartCoroutine(LoadScene("LanetliOrman"));
    }

    public void DeleteCharacter()
    {
        DisplayConfirm(Localization.Translate("PROMPT_DELETE_CHARACTER"), delegate
        {
            // TEMPORARY

        }, delegate
        {
        });
    }

    IEnumerator LoadScene(string sceneName)
    {
        WinMainMenu.GetComponent<RectTransform>().DOAnchorPosX(-300, 0.5f);

        yield return new WaitForSeconds(1f);

        InfoTitle.gameObject.SetActive(true);
        InfoDetail.gameObject.SetActive(true);

        int selected;
        InfoTitle.text = SelectTitle(out selected);
        InfoDetail.text = SelectDescription(selected);

        Loader.gameObject.SetActive(true);
        Loader.value = 0;
        Loader.maxValue = 95;

        yield return new WaitForSeconds(1f);

        Loader.DOValue(5, 1f);

        yield return new WaitForSeconds(1f);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        asyncLoad.allowSceneActivation = true;

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            Loader.value = 5 + Mathf.Max(90, asyncLoad.progress * 100);

            yield return null;
        }
    }

    string SelectTitle(out int selected)
    {
        string[] titles = new string[] { Localization.Translate("PRETEXT_1"), Localization.Translate("PRETEXT_2"), Localization.Translate("PRETEXT_3") };
        selected = UnityEngine.Random.Range(0, titles.Length);

        return titles[selected];
    }

    string SelectDescription(int index)
    {
        string[] titles = new string[] { Localization.Translate("PRETEXT_1_DESC"), Localization.Translate("PRETEXT_2_DESC"), Localization.Translate("PRETEXT_3_DESC")};

        return titles[index];
    }

    IEnumerator DoTheOpening()
    {
        StartCoroutine(EffectSlide());

        yield return new WaitForSeconds(2f);

        PnlInitial.GetComponent<Image>().DOColor(Color.white, 1f);

        yield return new WaitForSeconds(2f);

        ImgLogo.transform.localScale = Vector3.zero;
        ImgLogo.SetActive(true);
        ImgLogo.transform.DOScale(Vector3.one, 1);

        ImgEffect.SetActive(true);
        ImgLogo.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        ImgLogo.GetComponent<Image>().DOColor(Color.white, 2f);

        canTapNow = true;
        StartCoroutine(BlinkPressAnyKey());
    }

    IEnumerator BlinkPressAnyKey()
    {
        while (true)
        {
            if (!canTapNow)
            {
                TxtPressAnyKey.SetActive(false);
                yield break;
            }

            TxtPressAnyKey.SetActive(false);

            yield return new WaitForSeconds(PressKeyDuration);

            if (canTapNow)
            {
                TxtPressAnyKey.SetActive(true);

                yield return new WaitForSeconds(PressKeyDuration);
            }
        }
    }

    IEnumerator EffectSlide()
    {
        var rect = ImgEffect.GetComponent<RectTransform>();
        while (true)
        {
            rect.anchoredPosition = new Vector2(0, 0);
            rect.DOAnchorPos(new Vector2(-1920f, -1080f), EffectDuration).SetEase(Ease.Linear);

            yield return new WaitForSeconds(EffectDuration);
        }
    }

    private void FixedUpdate()
    {
        if(canTapNow && Input.anyKey)
        {
            canTapNow = false;

            ImgLogo.SetActive(false);
            TxtPressAnyKey.SetActive(false);
            PnlInitial.GetComponent<AudioSource>().Play();

            ImgLogoTop.SetActive(true);
            StartCoroutine(DisplayDisclaimer());
        }

        if(displayingDisclaimer && Input.anyKey)
        {
            StartCoroutine(FinishDisclaimer());
        }
    }

    IEnumerator DisplayDisclaimer()
    {
        yield return new WaitForSeconds(1f);

        displayingDisclaimer = true;
        TxtDisclaimer.SetActive(true);
        TxtDisclaimer.transform.localScale = Vector3.zero;
        TxtDisclaimer.transform.DOScale(0.9f, 1.0f);

        yield return new WaitForSeconds(1.0f);

        TxtDisclaimer.transform.DOScale(1.0f, 12f).SetEase(Ease.Linear);

        yield return new WaitForSeconds(13f);

        StartCoroutine(FinishDisclaimer());
    }

    IEnumerator FinishDisclaimer()
    {
        if(finishingDisclaimer)
        {
            yield break;
        }

        finishingDisclaimer = true;
        Color c = TxtDisclaimer.GetComponent<Text>().color;

        TxtDisclaimer.GetComponent<Text>().DOColor(new Color(c.r, c.g, c.b, 0), 1f);

        yield return new WaitForSeconds(2f);

        displayingDisclaimer = false;

        ImgEffect.SetActive(false);
        //c = PnlInitial.GetComponent<Image>().color;
        PnlInitial.GetComponent<Image>().DOColor(new Color(0, 0, 0, 1), 2f);

        ImgLogoTop.GetComponent<RectTransform>().DOAnchorPosY(1200, 2f);

        yield return new WaitForSeconds(2f);
        PnlInitial.GetComponent<Image>().DOColor(new Color(0, 0, 0, 0), 2f);

        Background.gameObject.SetActive(true);
        BGMusic.Play();

        ImgLogoTop.SetActive(false);
        DisplayMenu();
    }

    private void DisplayMenu()
    {
        WinMainMenu.SetActive(true);
        WinMainMenu.GetComponent<RectTransform>().DOAnchorPosX(300, 0.5f);
    }

    public void PromptExit()
    {
        PnlDark.SetActive(true);
        PnlDark.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        PnlDark.GetComponent<Image>().DOColor(new Color(0, 0, 0, 0.9f), 0.5f);

        DisplayConfirm(Localization.Translate("QUIT_PROMPT"), () =>
        {
            Application.Quit();
        }, () =>
        {
        });
    }

    public void DisplayConfirm(string message, UserChoice clkYes, UserChoice clkNo, int defaultYesTime, int defaultNoTime)
    {
        PnlDark.SetActive(true);
        PnlDark.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        PnlDark.GetComponent<Image>().DOColor(new Color(0, 0, 0, 0.75f), 0.5f);

        Transform btnNo = WinConfirm.transform.Find("ButtonNo");
        Transform btnYes = WinConfirm.transform.Find("ButtonYes");

        WinConfirm.transform.Find("TxtDesc").GetComponent<Text>().text = message;

        btnYes.GetComponent<Button>().onClick.RemoveAllListeners();
        btnYes.GetComponent<Button>().onClick.AddListener(delegate
        {
            StartCoroutine(HideConfirm());
            StartCoroutine(HideDarkBG());

            clkYes?.Invoke();
        });

        btnNo.GetComponent<Button>().onClick.RemoveAllListeners();
        btnNo.GetComponent<Button>().onClick.AddListener(delegate
        {
            StartCoroutine(HideConfirm());
            StartCoroutine(HideDarkBG());

            clkNo?.Invoke();
        });

        WinConfirm.transform.localScale = Vector3.zero;
        WinConfirm.SetActive(true);
        WinConfirm.transform.DOScale(Vector3.one, 0.25f);

        if(defaultYesTime > 0)
        {
            StartCoroutine(CountDownForButton(btnYes, 10, clkYes));
        } else if(defaultNoTime > 0)
        {
            StartCoroutine(CountDownForButton(btnNo, 10, clkNo));
        }
    }

    private IEnumerator CountDownForButton(Transform button, int length, UserChoice handler)
    {
        string originalText = Localization.Translate(button.Find("Text").GetComponent<Text>().text);

        for (int i = 0; i < length; i++)
        {
            button.Find("Text").GetComponent<Text>().text = "(" + (length - i) + ") " + originalText;

            yield return new WaitForSeconds(1);

            if(!WinConfirm.activeSelf)
            {
                goto _cleanUp;
            }
        }

        StartCoroutine(HideConfirm());
        StartCoroutine(HideDarkBG());

        handler?.Invoke();

        _cleanUp:
        button.Find("Text").GetComponent<Text>().text = originalText;
    }

    public void DisplayConfirm(string message, UserChoice clkYes, UserChoice clkNo)
    {
        DisplayConfirm(message, clkYes, clkNo, 0, 0);
    }

    public void DisplaySettings()
    {
        HideMenu();
        HideLoadCharacter();

        // Fill resolutions list
        Resolution[] resolutions = Screen.resolutions;

        ResolutionList.options.Clear();
        foreach (Resolution resolution in resolutions)
        {
            if (resolution.width <= 1024)
            {
                continue;
            }

            ResolutionList.options.Add(new Dropdown.OptionData(resolution.width + "x" + resolution.height + "x" + resolution.refreshRate));
            if(Screen.currentResolution.width == resolution.width && Screen.currentResolution.height == resolution.height && Screen.currentResolution.refreshRate == resolution.refreshRate)
            {
                lastValidResolution = ResolutionList.options.Count - 1;
            }
        }

        ResolutionList.value = lastValidResolution;
        ResolutionList.onValueChanged.AddListener(delegate
        {
            //AltResolution = ResolutionList.value;
            if(!canProcessResolutionChange)
            {
                canProcessResolutionChange = true;
                return;
            }

            Screen.SetResolution(Screen.resolutions[ResolutionList.value].width, Screen.resolutions[ResolutionList.value].height, FullScreenMode.ExclusiveFullScreen);

            DisplayConfirm("Bu çözünürlükte devam etmek istiyor musunuz?", delegate
            {
                lastValidResolution = ResolutionList.value;
            }, delegate
            {
                Screen.SetResolution(Screen.resolutions[lastValidResolution].width, Screen.resolutions[lastValidResolution].height, FullScreenMode.ExclusiveFullScreen);
                canProcessResolutionChange = false;
                ResolutionList.value = lastValidResolution;
            }, 0, 10);
        });

        // Display window
        WinSettings.SetActive(true);
        WinSettings.GetComponent<RectTransform>().DOAnchorPosX(0, 0.35f);

        SetGraphicsQualitySettings();
    }

    void HideMenu()
    {
        StartCoroutine(HideMenuNow());
    }

    IEnumerator HideMenuNow()
    {
        WinMainMenu.GetComponent<RectTransform>().DOAnchorPosX(-300, 0.5f);

        yield return new WaitForSeconds(0.5f);

        WinMainMenu.SetActive(true);
    }

    void SetGraphicsQualitySettings()
    {

    }

    public void HideSettings()
    {
        DisplayMenu();

        StartCoroutine(HideSettingsNow());
    }

    IEnumerator HideSettingsNow()
    {
        WinSettings.GetComponent<RectTransform>().DOAnchorPosX(1820, 0.35f);

        yield return new WaitForSeconds(0.35f);

        WinSettings.SetActive(false);
    }

    IEnumerator HideConfirm()
    {
        WinConfirm.transform.DOScale(Vector3.zero, 0.25f);

        yield return new WaitForSeconds(0.5f);

        WinConfirm.SetActive(false);
    }

    IEnumerator HideDarkBG()
    {
        PnlDark.GetComponent<Image>().DOColor(new Color(0, 0, 0, 0f), 0.5f);

        yield return new WaitForSeconds(0.5f);

        PnlDark.SetActive(false);
    }

    public void DisplayCharacterSelection()
    {
        StartCoroutine(DisplayCharacterSelectionNow());
    }

    IEnumerator DisplayCharacterSelectionNow()
    {
        HideLoadCharacter();
        /*if (LoadCharacterDisplayed)
        {

            yield return new WaitForSeconds(0.65f);
        }*/
        HideMenu();

        yield return new WaitForSeconds(0.5f);

        TurnDOF(true);
        WinNewCharacter.SetActive(true);

        GameObject left = WinNewCharacter.transform.Find("ImgLeftMenu").gameObject;
        GameObject right = WinNewCharacter.transform.Find("ImgRightMenu").gameObject;
        GameObject bottom = WinNewCharacter.transform.Find("BottomElements").gameObject;

        var rectLeft = left.GetComponent<RectTransform>();
        var rectRight = right.GetComponent<RectTransform>();
        var rectBottom = bottom.GetComponent<RectTransform>();

        rectLeft.anchoredPosition = new Vector2(250, 505);
        rectRight.anchoredPosition = new Vector2(-300, 505);
        rectBottom.anchoredPosition = new Vector2(0, -150);

        rectLeft.DOAnchorPosY(-505, 0.5f);
        rectRight.DOAnchorPosY(-505, 0.5f);
        rectBottom.DOAnchorPosY(150, 0.5f);

        DisplayNewCahacter(0, 0, 0, 0, 0, 0);
    }

    public void DisplayNewCahacter(int playerClass, int sex, int skinColor, int hairMaterial, int hairStyle, int suitMaterial)
    {
        displayedCharacter = playerClass;
        selectedSex = sex;
        selectedBodyMaterial = skinColor;
        selectedHairMaterial = hairMaterial;
        selectedHairStyle = hairStyle;
        selectedSuitMaterial = suitMaterial;

        /*LoadCharacterDisplayed = true;
        PlayerPrefs.SetInt("SelectedCharacter", index);

        BadgeArcher.SetActive(false);
        BadgeMage.SetActive(false);
        BadgePaladin.SetActive(false);
        BadgeThief.SetActive(false);*/

        var arrowLeft = WinLoadCharacter.transform.Find("ArrowLeft").GetComponent<Button>();
        var arrowRight = WinLoadCharacter.transform.Find("ArrowRight").GetComponent<Button>();

        for (int i = 0; i < CharacterModels.Length; i++)
        {
            CharacterModels[i].SetActive(i == playerClass);
        }

        arrowLeft.interactable = true;
        arrowRight.interactable = true;

        DisplayBodyMaterial();
        DisplayHairMaterial();
        DisplaySuitMaterial();
        DisplayHairStyle();
    }

    public void DisplayNextBodyMaterial()
    {
        selectedBodyMaterial = (selectedBodyMaterial + 1) % SkinMaterialsMale.Length;

        DisplayBodyMaterial();
    }

    public void DisplayNextHairMaterial()
    {
        selectedHairMaterial = (selectedHairMaterial + 1) % HairMaterialsMale.Length;

        DisplayHairMaterial();
    }

    public void DisplayPreviousBodyMaterial()
    {
        selectedBodyMaterial = selectedBodyMaterial - 1;

        if(selectedBodyMaterial < 0)
        {
            selectedBodyMaterial = SkinMaterialsMale.Length - 1;
        }

        DisplayBodyMaterial();
    }

    public void DisplayPreviousHairMaterial()
    {
        selectedHairMaterial = selectedHairMaterial - 1;

        if (selectedHairMaterial < 0)
        {
            selectedHairMaterial = HairMaterialsMale.Length - 1;
        }

        DisplayHairMaterial();
    }

    public void DisplayBodyMaterial()
    {
        for (int i = 0; i < CharacterModels.Length; i++)
        {
            CharacterModels[i].transform.Find(i < 4 ? "Geometry/Base/Body" : "Female/Geometry/Base/Body").GetComponent<SkinnedMeshRenderer>().material = (i < 4 ? SkinMaterialsMale[selectedBodyMaterial] : SkinMaterialsFemale[selectedBodyMaterial]);
        }
    }

    public void DisplayHairMaterial()
    {
        for (int i = 0; i < CharacterModels.Length; i++)
        {
            for (int j = 1; j < 7; j++)
            {
                if(i < 4 && j == 6)
                {
                    continue;
                }

                CharacterModels[i].transform.Find(i < 4 ? ("Geometry/Hairs/Hair" + j) : ("Female/Geometry/Hairstyles/Hair_0" + j)).GetComponent<SkinnedMeshRenderer>().material = (i < 4 ? HairMaterialsMale[selectedHairMaterial] : HairMaterialsFemale[selectedHairMaterial]);

                // Female eyebrows
                if(i >= 4)
                {
                    CharacterModels[i].transform.Find("Female/Geometry/Base/Eyebrows").GetComponent<SkinnedMeshRenderer>().material = HairMaterialsFemale[selectedHairMaterial];
                }

                // Beard??
            }
        }
    }

    public void DisplayPreviousHairStyle()
    {
        selectedHairStyle -= 1;

        if (selectedHairStyle < 0)
        {
            selectedHairStyle = 5;
        }

        DisplayHairStyle();
    }

    public void DisplayNextHairStyle()
    {
        selectedHairStyle = (selectedHairStyle + 1) % 6;

        DisplayHairStyle();
    }

    public void DisplayHairStyle()
    {
        for (int i = 0; i < CharacterModels.Length; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                if(i < 4 && j == 5)
                {
                    // leave one gap for hairless male
                    continue;
                }

                CharacterModels[i].transform.Find(i < 4 ? ("Geometry/Hairs/Hair" + (j + 1)) : ("Female/Geometry/Hairstyles/Hair_0" + (j + 1))).GetComponent<SkinnedMeshRenderer>().enabled = false;
            }
        }

        if((selectedSex == 0 && selectedHairStyle == 5) ||
            displayedCharacter == 1 || displayedCharacter == 2 ||
            displayedCharacter == 5 || displayedCharacter == 6)
        {
            return;
        }

        int c = displayedCharacter + selectedSex * 4;
        CharacterModels[c].transform.Find(c < 4 ? ("Geometry/Hairs/Hair" + (selectedHairStyle + 1)) : ("Female/Geometry/Hairstyles/Hair_0" + (selectedHairStyle + 1))).GetComponent<SkinnedMeshRenderer>().enabled = true;
    }

    public void DisplayPreviousSuitMaterial()
    {
        selectedSuitMaterial = selectedSuitMaterial - 1;

        if (selectedSuitMaterial < 0)
        {
            selectedSuitMaterial = 3; // 4
        }

        DisplaySuitMaterial();
    }

    public void DisplayNextSuitMaterial()
    {
        selectedSuitMaterial = (selectedSuitMaterial + 1) % 3; // 4

        DisplaySuitMaterial();
    }

    public void DisplaySuitMaterial()
    {
        for (int i = 0; i < CharacterModels.Length; i++)
        {
            for (int j = 3; j < CharacterModels[i].transform.childCount; j++)
            {
                switch(i)
                {
                    case 0: // archer male
                        CharacterModels[i].transform.GetChild(j).GetComponent<SkinnedMeshRenderer>().material = SuitMaterialsArcherMale[(j - 3) + 7 * selectedSuitMaterial];
                        break;
                    case 1: // mage male
                        CharacterModels[i].transform.GetChild(j).GetComponent<SkinnedMeshRenderer>().material = SuitMaterialsMageMale[(j - 3) + 6 * selectedSuitMaterial];
                        break;
                    case 3: // raider male
                        CharacterModels[i].transform.GetChild(j).GetComponent<SkinnedMeshRenderer>().material = SuitMaterialsRangerMale[(j - 3) + 7 * selectedSuitMaterial];
                        break;
                    case 2: // paladin male
                        CharacterModels[i].transform.GetChild(j).GetComponent<SkinnedMeshRenderer>().material = SuitMaterialsPaladinMale[(j - 3) + 11 * selectedSuitMaterial];
                        break;
                    case 4: // archer female
                        CharacterModels[i].transform.GetChild(j).GetComponent<SkinnedMeshRenderer>().material = SuitMaterialsArcherFemale[(j - 3) + 7 * selectedSuitMaterial];
                        break;
                    case 5: // mage female
                        CharacterModels[i].transform.GetChild(j).GetComponent<SkinnedMeshRenderer>().material = SuitMaterialsMageFemale[(j - 3) + 6 * selectedSuitMaterial];
                        break;
                    case 7: // raider female
                        CharacterModels[i].transform.GetChild(j).GetComponent<SkinnedMeshRenderer>().material = SuitMaterialsRangerFemale[(j - 3) + 7 * selectedSuitMaterial];
                        break;
                    case 6: // paladin male
                        CharacterModels[i].transform.GetChild(j).GetComponent<SkinnedMeshRenderer>().material = SuitMaterialsPaladinFemale[(j - 3) + 11 * selectedSuitMaterial];
                        break;
                }
            }
        }
    }

    public void HideCharacterSelection()
    {
        StartCoroutine(HideCharacterSelectionNow());
    }

    IEnumerator HideCharacterSelectionNow()
    {
        for (int i = 0; i < CharacterModels.Length; i++)
        {
            CharacterModels[i].SetActive(false);
        }

        GameObject left = WinNewCharacter.transform.Find("ImgLeftMenu").gameObject;
        GameObject right = WinNewCharacter.transform.Find("ImgRightMenu").gameObject;
        GameObject bottom = WinNewCharacter.transform.Find("BottomElements").gameObject;

        var rectLeft = left.GetComponent<RectTransform>();
        var rectRight = right.GetComponent<RectTransform>();
        var rectBottom = bottom.GetComponent<RectTransform>();

        rectLeft.DOAnchorPosY(505, 0.5f);
        rectRight.DOAnchorPosY(505, 0.5f);
        rectBottom.DOAnchorPosY(-150, 0.5f);
        TurnDOF(false);

        yield return new WaitForSeconds(0.5f);

        WinNewCharacter.SetActive(false);

        DisplayMenu();
    }
}
