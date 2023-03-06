using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CanvasMissionComplete : UICanvas
{
    public UserData userData;
    public GameObject levelCompletePanel;
    public Canvas canvas;

    //Base
    //public Sprite[] giftSpite;
    public GameObject[] giftTag;

    //Image
    public Image giftImage;
    public Image giftBGImage;
    public GameObject giftgob;
    public GameObject progressTxtgob;
    public GameObject victoryImage;

    //Progress
    public Text progressText;
    private int currentPercent;
    private int targetPercent;

    //Button
    public GameObject BtnNextLevel;
    public GameObject BtnClaim;

    //Ads Btn
    public GameObject adsImage;
    public GameObject loadingAds;

    public override void Setup()
    {
        base.Setup();
        SetupInitInfo();
        adsImage.SetActive(true);
        loadingAds.SetActive(false);
    }

    void SetupInitInfo()
    {
        if(DataManager.Instance.gameData.currentGiftID < (DataManager.Instance.gameData.brickTypeInfo.Count + DataManager.Instance.gameData.hammerTypeInfo.Count-2)) //Nếu Gift ID vượt quá số lượng gift thì bật mặt cười. -2 là số lượng gift free ban đầu
        {
            giftgob.SetActive(true);
            progressTxtgob.SetActive(true);
            victoryImage.SetActive(false);
            giftImage.SetNativeSize();
            giftBGImage.SetNativeSize();
            for (int i = 0; i < DataManager.Instance.gameData.brickTypeInfo.Count; i++)
            {
                if (DataManager.Instance.gameData.currentGiftID == DataManager.Instance.gameData.brickTypeInfo[i].giftID)
                {
                    giftImage.sprite = DataManager.Instance.gameData.brickTypeInfo[i].giftEndgameSprite;
                    giftBGImage.sprite = DataManager.Instance.gameData.brickTypeInfo[i].giftEndgameSprite;
                    currentPercent = DataManager.Instance.gameData.brickTypeInfo[i].progressPercent;
                    giftTag[0].SetActive(false);    
                    giftTag[1].SetActive(true);     //Đặt tag là  New Brick
                }
            }
            for (int i = 0; i < DataManager.Instance.gameData.hammerTypeInfo.Count; i++)
            {
                if (DataManager.Instance.gameData.currentGiftID == DataManager.Instance.gameData.hammerTypeInfo[i].giftID)
                {
                    giftImage.sprite = DataManager.Instance.gameData.hammerTypeInfo[i].giftEndgameSprite;
                    giftBGImage.sprite = DataManager.Instance.gameData.hammerTypeInfo[i].giftEndgameSprite;
                    currentPercent = DataManager.Instance.gameData.hammerTypeInfo[i].progressPercent;
                    giftTag[1].SetActive(false);
                    giftTag[0].SetActive(true);     //Đặt tag là  New Hammer
                }
            }
            giftImage.fillAmount = currentPercent * 0.01f;
            progressText.text = currentPercent + "%";
        }
        else
        {
            giftgob.SetActive(false);       //Tắt ảnh của Gift
            progressTxtgob.SetActive(false);//Tắt text %
            victoryImage.SetActive(true);   //Đặt hình mặt cười
            giftTag[1].SetActive(false);    //Tắt tag new Brick
            giftTag[0].SetActive(false);    //Tắt tag new Hammer
        }
        
        
    }

    IEnumerator UpdateProgress()
    {
        targetPercent = currentPercent + 33;
        if (targetPercent > 80) targetPercent = 100;
        for (int i = 0; i < DataManager.Instance.gameData.brickTypeInfo.Count; i++)
        {
            if (DataManager.Instance.gameData.currentGiftID == DataManager.Instance.gameData.brickTypeInfo[i].giftID)
            {
                DataManager.Instance.gameData.brickTypeInfo[i].progressPercent = targetPercent;
                DataManager.Instance.gameData.currentGiftName = DataManager.Instance.gameData.brickTypeInfo[i].giftName;
            }
        }
        for (int i = 0; i < DataManager.Instance.gameData.hammerTypeInfo.Count; i++)
        {
            if (DataManager.Instance.gameData.currentGiftID == DataManager.Instance.gameData.hammerTypeInfo[i].giftID)
            {
                DataManager.Instance.gameData.hammerTypeInfo[i].progressPercent = targetPercent;
                DataManager.Instance.gameData.currentGiftName = DataManager.Instance.gameData.hammerTypeInfo[i].giftName;
            }
        }
        if (targetPercent >= 99)
        {
            DataManager.Instance.gameData.currentGiftID++;
        }
        DataManager.Instance.SaveGame();
        if (targetPercent > 99)
        {
            BtnNextLevel.SetActive(false);
            BtnClaim.SetActive(true);
            FirebaseManager.Instance.LogAnalyticsEvent(EventName.g_rewarded_ads_offers, Parameter.item_ID, DataManager.Instance.gameData.currentGiftName);
        }
        else
        {
            BtnNextLevel.SetActive(true);
            BtnClaim.SetActive(false);
        }
        yield return Cache.GetWFS(1f);
        LevelManager.Instance.ChangeTheme();
        yield return Cache.GetWFS(1f);
        float currentPercentt = (float)currentPercent;
        DOTween.To(() => currentPercentt, x => currentPercentt = x, (float)targetPercent, 2)
            .OnUpdate(() =>
            {
                if (currentPercentt <= 100)
                {
                    progressText.text = (int)currentPercentt + "%";
                }
                else
                {
                    progressText.text = "100%";
                }
                giftImage.fillAmount = currentPercentt * 0.01f;
            });
    }

    public override void Open()
    {
        base.Open();
        StartCoroutine(DelayToPlayAds());
        if (DataManager.Instance.gameData.currentGiftID < (DataManager.Instance.gameData.brickTypeInfo.Count + DataManager.Instance.gameData.hammerTypeInfo.Count - 2))  StartCoroutine(UpdateProgress());
        else
        {
            BtnNextLevel.SetActive(true);
            BtnClaim.SetActive(false);
        }
    }

    IEnumerator DelayToPlayAds() //Chờ chạy hết anim show popup mới show ads
    {
        yield return Cache.GetWFS(0.5f);
        SoundController.Instance.sound_Victory.PlaySoundOneShot();
        SoundController.Instance.sound_Firework.PlaySoundOneShot();
        canvas.worldCamera = Camera.main;
        AppLovinController.instance.AppsflyerSentEventLevelAchieve();
        AppLovinController.instance.SetInterPlacement(placement.Level_Complete);
        if (DataManager.Instance.gameData.currentLevel == 1 && !FirebaseRemoteConfigManager.m_Inter_first_game) GameManager.Instance.ChangeState(GameState.Pause);
        else
        {
            if (DataManager.Instance.gameData.currentLevel >= FirebaseRemoteConfigManager.m_Inter_from_level && DataManager.Instance.gameData.currentLevel != 9) AppLovinController.instance.ShowInterstitial(); //Vì khi show rate
            GameManager.Instance.ChangeState(GameState.Pause);
        }
    }

    #region Next Level Button

    public void BtnReceiveGift()
    {
        SoundController.Instance.sound_Click.PlaySoundOneShot();
        if (!MaxSdk.IsRewardedAdReady(AppLovinController.instance.AndroidRewardID))
        {
            adsImage.SetActive(false);
            loadingAds.SetActive(true);
        }
        if (GameManager.Instance.IsBuildMaketing || GameManager.Instance.IsNoAds)
        {
            NextLevelButton();
            return;
        }
        StartCoroutine(WaitToShowReward());
    }
    IEnumerator WaitToShowReward()
    {
        yield return new WaitUntil(() => MaxSdk.IsRewardedAdReady(AppLovinController.instance.AndroidRewardID));
        AppLovinController.instance.SetRewardPlacement(placement.Receive_item_EndGame);
        AppLovinController.instance.ShowRewardedAd(() =>
        {
            FirebaseManager.Instance.LogAnalyticsEvent(EventName.g_rewarded_ads_completed, Parameter.item_ID, DataManager.Instance.gameData.currentGiftName);
            for (int i = 0; i < DataManager.Instance.gameData.brickTypeInfo.Count; i++)
            {
                if ((DataManager.Instance.gameData.currentGiftID-1) == (DataManager.Instance.gameData.brickTypeInfo[i].giftID))
                {
                    DataManager.Instance.gameData.brickTypeInfo[i].isUnlock = true;
                    DataManager.Instance.gameData.currentBrickTypeID = i;
                }
            }
            for (int i = 0; i < DataManager.Instance.gameData.hammerTypeInfo.Count; i++)
            {
                if ((DataManager.Instance.gameData.currentGiftID-1) == (DataManager.Instance.gameData.hammerTypeInfo[i].giftID))
                {
                    DataManager.Instance.gameData.hammerTypeInfo[i].isUnlock = true;
                    DataManager.Instance.gameData.currentHammerTypeID = i;
                }
            }
            NextLevelButton();
        });
        //FirebaseManager.Instance.LogAnalyticsEvent(EventName.level_bonus_click, Parameter.level, DataManager.Instance.gameData.currentPaintLevel.ToString());
    }
    public void NextLevelButton()
    {
        SoundController.Instance.sound_Click.PlaySoundOneShot();
        LoadNextLevel();
        AppLovinController.instance.SetInterPlacement(placement.Level_Complete);
        if (DataManager.Instance.gameData.currentLevel >= FirebaseRemoteConfigManager.m_Inter_from_level) AppLovinController.instance.ShowInterstitial();
    }
    #endregion

    #region Load Next Level
    public void LoadNextLevel()
    {
        UI_Game.Instance.OpenUI(UIID.BlockRaycast);
        Invoke(nameof(Close), 0.65f);
        HammerControl.Instance.MoveToPlayGame();
    }
    #endregion
    public override void Close()
    {
        base.Close();
        BrickManager.Instance.endEffectAction += () =>
         {
             UI_Game.Instance.OpenUI(UIID.GamePlay);
             HammerControl.Instance.MoveToPlayGame();
         };
        LevelManager.Instance.ChangeLevelEffect();
    }
}
