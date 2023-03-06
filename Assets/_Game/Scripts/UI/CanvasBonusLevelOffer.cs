using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasBonusLevelOffer : UICanvas
{
    public GameObject adsImage;
    public GameObject loadingAds;
    public Image bonusPaintIconImage;
    public override void Setup()
    {
        adsImage.SetActive(true);
        loadingAds.SetActive(false);
        bonusPaintIconImage.sprite = DataManager.Instance.bonusLevelImage[DataManager.Instance.gameData.currentPaintLevel % DataManager.Instance.bonusLevelImage.Length-1];
        bonusPaintIconImage.SetNativeSize();
        base.Setup();
        FirebaseManager.Instance.LogAnalyticsEvent(EventName.level_bonus_offer, Parameter.level, DataManager.Instance.gameData.currentPaintLevel.ToString());
        DataManager.Instance.gameData.sortLevelOfferTimes++;
        FirebaseManager.Instance.LogAnalyticsEvent(EventName.g_gameplay_sort_offer_+DataManager.Instance.gameData.sortLevelOfferTimes.ToString(), Parameter.level, DataManager.Instance.gameData.currentSortLevel.ToString());
    }
    public void Btn_PlayBonusLevel()
    {
        SoundController.Instance.sound_Click.PlaySoundOneShot();
        if (!MaxSdk.IsRewardedAdReady(AppLovinController.instance.AndroidRewardID))
        {
            adsImage.SetActive(false);
            loadingAds.SetActive(true);
        }
        if (GameManager.Instance.IsBuildMaketing || GameManager.Instance.IsNoAds)
        {
            LoadBonusLevel();
            return;
        }
        StartCoroutine(WaitToShowReward());
    }

    IEnumerator WaitToShowReward()
    {
        AppLovinController.instance.SetRewardPlacement(placement.Bonus_level_offer);
        yield return new WaitUntil(() => MaxSdk.IsRewardedAdReady(AppLovinController.instance.AndroidRewardID));
        AppLovinController.instance.ShowRewardedAd(() =>
        {
            LoadBonusLevel();
        });
        FirebaseManager.Instance.LogAnalyticsEvent(EventName.level_bonus_click, Parameter.level, DataManager.Instance.gameData.currentPaintLevel.ToString());
    }

    void LoadBonusLevel()
    {
        LevelManager.Instance.LoadPaintLevel();
        DataManager.Instance.gameData.sortLevelInfomation[DataManager.Instance.gameData.currentSortLevel - 1].isUnlock = true;
        DataManager.Instance.SaveGame();
    }

    public void Btn_LoseIt()    //Khi show Popup Bonus level offer
    {
        SoundController.Instance.sound_Click.PlaySoundOneShot();
        AppLovinController.instance.SetInterPlacement(placement.Bonus_level_offer);
        AppLovinController.instance.ShowInterstitial();
        DataManager.Instance.SaveGame();
        UI_Game.Instance.CloseUI(UIID.BlockRaycast);
        BrickManager.Instance.endEffectAction += () =>
        {
            UI_Game.Instance.CloseUI(UIID.GamePlay);
            UI_Game.Instance.OpenUI(UIID.MainMenu);
        };
        LevelManager.Instance.ChangeLevelEffect();
        DataManager.Instance.gameData.allLevelCounting++;
        DataManager.Instance.gameData.currentPaintLevel++;
        DataManager.Instance.SaveGame();
        UI_Game.Instance.GetUI<CanvasGamePlay>(UIID.GamePlay).Close();
        FirebaseManager.Instance.LogAnalyticsEvent(EventName.g_gameplay_sort_skip_ + DataManager.Instance.gameData.sortLevelOfferTimes.ToString(), Parameter.level, DataManager.Instance.gameData.currentSortLevel.ToString());
        Close();
    }
}
