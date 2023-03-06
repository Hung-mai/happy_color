using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using DG.Tweening;

public class CanvasSpecialOffer : UICanvas
{
    [SerializeField] private GameObject[] contentImage;
    public TextMeshProUGUI txt_time;
    public Transform contentPopup;
    private Vector3 targetMove; 
    public Image panel;

    public override void Setup()
    {
        base.Setup();
        LevelManager.Instance?.UnEnableToClickOnBrick();
        OnInit();
    }

    private void OnInit()
    {
        contentImage[0].SetActive(false);
        contentImage[1].SetActive(false);
        if(DataManager.Instance.gameData.indexSpecialOffer < 2)
        {
            contentImage[DataManager.Instance.gameData.indexSpecialOffer].SetActive(true);
        }
        if (DataManager.Instance.gameData.indexSpecialOffer == 0) DataManager.Instance.gameData.currentSpecialItemName = Constant.FIREWORK_STR;
        else DataManager.Instance.gameData.currentSpecialItemName = Constant.BOMB_STR;
        FirebaseManager.Instance.LogAnalyticsEvent(EventName.g_special_offers_show, Parameter.item_ID, DataManager.Instance.gameData.currentSpecialItemName);
    }

    public void Btn_GetIt()
    {
        StartCoroutine(WaitToShowReward());
    }

    IEnumerator WaitToShowReward()
    {
        AppLovinController.instance.SetRewardPlacement(placement.Receive_Special_item);
        yield return new WaitUntil(() => MaxSdk.IsRewardedAdReady(AppLovinController.instance.AndroidRewardID));
        AppLovinController.instance.ShowRewardedAd(() =>
        {
            // logic nhận offer chỗ này
            if (DataManager.Instance.gameData.indexSpecialOffer == 0)
            {
                DataManager.Instance.gameData.currentHammerTypeID = (int)(HammerType.Firework);
                // TODO: đổi hình dạng chỗ này
                HammerControl.Instance.SetHammerType(HammerType.Firework);
                DataManager.Instance.gameData.specialItem[0].isUnlock = true;
            }
            else if (DataManager.Instance.gameData.indexSpecialOffer == 1)
            {
                DataManager.Instance.gameData.currentHammerTypeID = (int)(HammerType.Bomb);
                // TODO: đổi hình dạng chỗ này
                HammerControl.Instance.SetHammerType(HammerType.Bomb);
                DataManager.Instance.gameData.specialItem[1].isUnlock = true;
            }


            // logix tắt hết UI
            DataManager.Instance.gameData.timeEndSpecialOffer = (int)DateTime.Now.Subtract(Constant.TIME_MARKER).TotalSeconds;
            DataManager.Instance.gameData.specialShowing = false;
            // if(DataManager.Instance.gameData.indexSpecialOffer == 0)
            // {
            //     DataManager.Instance.gameData.levelGetFirework = DataManager.Instance.gameData.allLevelCounting;
            // }
            DataManager.Instance.gameData.indexSpecialOffer++;

            // hết thời gian thì tự tắt 
            if (UI_Game.Instance.IsOpenedUI(UIID.MainMenu))
            {
                UI_Game.Instance.GetUI<CanvasMainMenu>(UIID.MainMenu).btn_specialOffer.SetActive(false);
            }
            if (UI_Game.Instance.IsOpenedUI(UIID.GamePlay))
            {
                UI_Game.Instance.GetUI<CanvasGamePlay>(UIID.GamePlay).btn_specialOffer.SetActive(false);
            }
            StopCoroutine(GameManager.Instance.couroutine_countDown);
            GameManager.Instance.couroutine_countDown = null;
            Close();
        });
    }

    public override void Open()
    {
        panel.color = new Color(panel.color.r, panel.color.g, panel.color.b, 200f / 255f);
        contentPopup.localScale = Vector3.one;
        contentPopup.localPosition = Vector3.zero;
        base.Open();
        DataManager.Instance.gameData.specialShowing = true;

        int timeCount = GameManager.Instance.timeCountSpecial;

        string hours = ((timeCount / 60) / 60).ToString();
        string minutes = timeCount / 60 < 10 ? "0" + (timeCount / 60).ToString() : (timeCount / 60).ToString();
        string seconds = timeCount % 60 < 10 ? "0" + (timeCount % 60).ToString() : (timeCount % 60).ToString();
        string time = hours + ":" + minutes + ":" + seconds;

        txt_time.text = time;
        GameManager.Instance.showSpecialOpenApp = true;
    }

    public override void Close()
    {
        if(UI_Game.Instance != null)
        {
            if(UI_Game.Instance.IsOpenedUI(UIID.MainMenu))
            {
                targetMove = UI_Game.Instance.GetUI<CanvasMainMenu>(UIID.MainMenu).btn_specialOffer.transform.position;
            }
            else if(UI_Game.Instance.IsOpenedUI(UIID.GamePlay))
            {
                targetMove = UI_Game.Instance.GetUI<CanvasGamePlay>(UIID.GamePlay).btn_specialOffer.transform.position;
            }
        }
        contentPopup.DOMove(targetMove, 0.55f).SetEase(Ease.InQuad);
        contentPopup.DOScale(Vector3.zero, 0.55f).SetEase(Ease.InQuad);
        panel.DOFade(0, 0.55f).SetEase(Ease.InQuad);

        LevelManager.Instance?.EnableToClickOnBrick();
        Timer.Schedule(this, 0.7f, () => {
            base.Close();
        });
    }
}
