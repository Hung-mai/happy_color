using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CanvasBonusGamePlay : UICanvas
{
    public GameObject[] buttonList;
    public Image[] listButtonImage;
    public Sprite buttonSellect;
    public Sprite buttonUnsellect;
    public List<Image> listbuttonSprites;
    public Sprite[] iconImage;
    public Transform colorList;
    public Animator anim;
    public GameObject btnHideUI;
    public RectTransform BtnSetting;
    public RectTransform BtnReplay;
    public bool isTutorial;
    //Build MKT
    public GameObject btnReplay;
    public GameObject btnSetting;
    public GameObject btnDone;
    public GameObject btnSkip;
    public GameObject btnHintAds;
    public GameObject btnHintAdsIcon;


    private bool hintIsDone = false;

    //Tutorial
    public GameObject tutHandgob;
    public RectTransform tutHandTf;
    public RectTransform firstColorTf;
    private void Awake()
    {
        SetUpLayout();
        if (DataManager.Instance.gameData.currentPaintLevel == 1) isTutorial = true;
        else isTutorial = false;
    }
    public override void Setup()
    {
        base.Setup();
        listButtonImage = new Image[colorList.childCount];
        for (int i = 0; i < colorList.childCount; i++)
        {
            listButtonImage[i] = colorList.GetChild(i).GetChild(0).gameObject.GetComponent<Image>();
        }
        for (int i = 0; i < buttonList.Length; i++)
        {
            listbuttonSprites.Add(buttonList[i].GetComponent<Image>());
        }
        SetupColorButton();
        btnHideUI.SetActive(GameManager.Instance.IsBuildMaketing);
        FirebaseManager.Instance.OnSetUserProperty();
        hintIsDone = false;
        btnHintAdsIcon.SetActive(true);
    }

    public override void Open()
    {
        base.Open();
        if (DataManager.Instance.gameData.isPaintTutorial)
        {
            //DataManager.Instance.gameData.isPaintTutorial = false;
            for (int i = 0; i < buttonList.Length; i++)
            {
                if (buttonList[i].activeSelf)
                {
                    firstColorTf = buttonList[i].GetComponent<RectTransform>();
                    break;
                }
            }
            StartCoroutine(StartPaintTutorial());
        }
        
    }

    public void SetUpLayout()   //Điều chỉnh layout để tránh màn nốt ruồi
    {
        Vector3 temp = BtnSetting.position;
        temp.y -= (Screen.height-Screen.safeArea.height);
        BtnSetting.position = temp;

        temp = BtnReplay.position;
        temp.y -= (Screen.height - Screen.safeArea.height);
        BtnReplay.position = temp;
    }
    
    public void OnLoadColorButton()
    {
        for (int i = 0; i < BonusLevelManager.Instance.colorList.Count; i++)
        {
            colorList.GetChild(i).gameObject.SetActive(true);
        }
    }

    public void Btn_ChooseColor(int colorID)
    {
        SoundController.Instance.sound_Click.PlaySoundOneShot();
        Vibration.Vibrate(3);
        BonusGameManager.Instance.choosingMaterial = BonusLevelManager.Instance.materialsData.listMaterials[colorID];
        for (int i = 0; i < buttonList.Length; i++)
        {
            if (i == colorID)
            {
                listbuttonSprites[i].sprite = buttonSellect;
            }
            else
            {
                listbuttonSprites[i].sprite = buttonUnsellect;
            }
        }
    }

    public void Btn_FirstColorChoose()
    {
        if (DataManager.Instance.gameData.isPaintTutorial) return; //Nếu đang hiển thị tutorial thì bỏ qua ko chọn màu
        for (int i = 0; i < buttonList.Length; i++)
        {
            if (buttonList[i].activeSelf)
            {
                BonusGameManager.Instance.choosingMaterial = BonusLevelManager.Instance.materialsData.listMaterials[i];
                for (int j = 0; j < buttonList.Length; j++)
                {
                    if (i == j)
                    {
                        listbuttonSprites[j].sprite = buttonSellect;
                    }
                    else
                    {
                        listbuttonSprites[j].sprite = buttonUnsellect;
                    }
                }
                break;
            }
        }
    }

    private void SetupColorButton()
    {
        for (int i = 0; i < buttonList.Length; i++)  //Disable all Color button
        {
            buttonList[i].SetActive(false);
        }
        for (int i = 0; i < BonusLevelManager.Instance.colorList.Count; i++)
        {
            if (BonusLevelManager.Instance.materialsData.listMaterials[i].color != new Color32(255, 255, 255, 255) && BonusLevelManager.Instance.materialsData.listMaterials[i].color != new Color32(0, 0, 0, 255)) //Hiển thị nút ở UI
            {
                buttonList[i].SetActive(true);
                listButtonImage[i].color = BonusLevelManager.Instance.materialsData.listMaterials[i].color;
            }
        }
        Btn_FirstColorChoose();//Tự động chọn màu đầu tiên
    }

    public void SettingButton()
    {
        SoundController.Instance.sound_Click.PlaySoundOneShot();
        if (!GameManager.Instance.IsState(GameState.BlockAction) && !GameManager.Instance.IsState(GameState.Pause))
        {
            UI_Game.Instance.OpenUI(UIID.Setting);
        }
    }

    public void RetryButton()
    {
        AppLovinController.instance.SetInterPlacement(placement.Btn_replay_ingame);
        AppLovinController.instance.ShowInterstitial();
        BonusLevelManager.Instance.ResetLevel();
    }

    public void BtnDone()
    {
        SoundController.Instance.sound_Click.PlaySoundOneShot();
        BonusBrickManager.Instance.CheckEndGame();
        GameManager.Instance.ChangeState(GameState.Win);
        EndGame();
    }

    public void StartGame()
    {
        GameManager.Instance.ChangeState(GameState.GamePlay);
    }

    public void EndGame()
    {
        anim.SetTrigger(Constant.ANIM_DEACTIVE);
        Timer.Schedule(this, 1f, ()=> 
        {
            Close();
        });
    }

    public void BtnSkip()
    {
        SoundController.Instance.sound_Click.PlaySoundOneShot();
        AppLovinController.instance.SetRewardPlacement(placement.Bonus_level_skip);
        FirebaseManager.Instance.LogAnalyticsEvent(EventName.skip_bonus_level_offer, Parameter.level, BonusLevelManager.Instance.playingLevelID.ToString());
        AppLovinController.instance.ShowRewardedAd(()=> 
        {
            BonusBrickManager.Instance.SkipButtonClick();
            Timer.Schedule(this, 1f, () =>
            {
                BonusBrickManager.Instance.CheckEndGame();
                GameManager.Instance.ChangeState(GameState.Win);
                EndGame();
            });
        });
    }

    public void BtnHint()
    {
        SoundController.Instance.sound_Click.PlaySoundOneShot();
        if (!hintIsDone)
        {
            AppLovinController.instance.SetRewardPlacement(placement.Bonus_level_hint);
            FirebaseManager.Instance.LogAnalyticsEvent(EventName.hint_bonus_level_offer, Parameter.level, BonusLevelManager.Instance.playingLevelID.ToString());
            AppLovinController.instance.ShowRewardedAd(() =>
            {
                BonusBrickManager.Instance.HintButton();
                btnHintAdsIcon.SetActive(false);
                hintIsDone = true;
            });
        }
        else
        {
            BonusBrickManager.Instance.HintButton();
        }
    }
    public void BtnHideUI()
    {
        btnReplay.SetActive(!btnReplay.activeSelf);
        btnSetting.SetActive(!btnSetting.activeSelf);
        btnSkip.SetActive(!btnSkip.activeSelf);
        btnDone.SetActive(!btnDone.activeSelf);
        btnHintAds.SetActive(!btnHintAds.activeSelf);
    }

    #region Tutorial
    IEnumerator StartPaintTutorial()
    {
        yield return null;
        if (!tutHandgob.activeSelf)
        {
            yield return Cache.GetWFS(1f);
            tutHandgob.SetActive(true);
            tutHandTf.position = new Vector3(firstColorTf.position.x,480f,0);
        }
        yield return new WaitUntil(() => BonusGameManager.Instance.choosingMaterial != null);
        tutHandgob.SetActive(false);
        //StartCoroutine(StartPaintTutorial());
        int clickCount=0;
        while (true)
        {
            for (int i = 0; i < BonusBrickManager.Instance.totalBonusBricks.Count; i++)
            {
                if (BonusBrickManager.Instance.totalBonusBricks[i].exactMaterial == BonusGameManager.Instance.choosingMaterial&& !BonusBrickManager.Instance.totalBonusBricks[i].isComplete)
                {
                    tutHandTf.DOMove(Camera.main.WorldToScreenPoint(BonusBrickManager.Instance.totalBonusBricks[i].tf.position),0.5f);
                    Debug.Log("tutHandTf.position" + tutHandTf.position);
                    tutHandgob.SetActive(true);
                    clickCount++;
                    break;
                }
                if(i == BonusBrickManager.Instance.totalBonusBricks.Count-1)
                {
                    tutHandgob.SetActive(false);
                }
            }
            yield return null;
            if (clickCount >= 5) break;
        }
        
    }
    #endregion
}
