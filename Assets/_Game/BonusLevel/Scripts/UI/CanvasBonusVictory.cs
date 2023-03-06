using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CanvasBonusVictory : UICanvas
{
    private const string PERCENT = "%";
    public GameObject[] listStars;
    public GameObject perfectBtnEndgame;
    public GameObject notPerfectBtnEndgame;
    public Text processText;
    public Image processImage;
    public GameObject[] processStars;
    public Canvas canvas;
    public GameObject[] flyStars;
    public GameObject[] bloomStars;
    private int lastComleteBrickAmount=0;
    float myFloat;
    public override void Setup()
    {
        base.Setup();
        processImage.fillAmount = 0;
        if (DataManager.Instance.gameData.paintLevelInformation[BonusLevelManager.Instance.playingLevelID-1].isFirstTimeComplete) //Nếu là hoàn thành game lần đầu tiên thì bắn firebase checkpoint
        {
            FirebaseManager.Instance.LogAnalyticsEvent(EventName.checkpoint_bonus_end_ + BonusLevelManager.Instance.playingLevelID.ToString(), Parameter.level, BonusLevelManager.Instance.playingLevelID.ToString());
            FirebaseManager.Instance.LogCheckPoint(EventName.g_checkpoint_end_ + (((DataManager.Instance.gameData.currentLevel - 1) > 9) ? (DataManager.Instance.gameData.currentLevel-1).ToString() : ("0" + (DataManager.Instance.gameData.currentLevel-1).ToString())), (DataManager.Instance.gameData.currentLevel-1).ToString(), levelType.paint);

            DataManager.Instance.gameData.paintLevelInformation[BonusLevelManager.Instance.playingLevelID-1].isFirstTimeComplete = false;
            DataManager.Instance.SaveGame();
        }
        FirebaseManager.Instance.LogAnalyticsEvent(EventName.level_complete_bonus, Parameter.level, BonusLevelManager.Instance.playingLevelID.ToString());
        FirebaseManager.Instance.LogLevelComplete(EventName.g_level_complete.ToString(), (DataManager.Instance.gameData.currentLevel).ToString(), BonusGameManager.Instance.timeCounting);
        BonusGameManager.Instance.timeCounting = 0;
    }

    private void OnEnable()
    {
        canvas.worldCamera = Camera.main;
    }

    public void NextLevelButton()
    {
        SoundController.Instance.sound_Click.PlaySoundOneShot();
        StartCoroutine(ShowPopupNewTheme());
    }

    public void LoadNextLevel()
    {
        AppLovinController.instance.SetInterPlacement(placement.Bonus_level_victory);
        if (DataManager.Instance.gameData.currentLevel >= FirebaseRemoteConfigManager.m_Inter_from_level) AppLovinController.instance.ShowInterstitial();
        if (DataManager.Instance.gameData.bonusDesireLevel == -1)
        {
            DataManager.Instance.gameData.allLevelCounting++;
            DataManager.Instance.gameData.currentPaintLevel++;
        }
        DataManager.Instance.gameData.bonusDesireLevel = -1;
        DataManager.Instance.SaveGame();
        BonusLevelManager.Instance.LoadNextLevel();
    }

    public override void Open()
    {
        base.Open();
        StartCoroutine(EnableStars());
    }

    IEnumerator EnableStars()
    {
        int enableStarNum = (BonusBrickManager.Instance.completeBrickRate < 1 ? (BonusBrickManager.Instance.completeBrickRate < 0.7 ? 1 : 2) : 3);
        perfectBtnEndgame.SetActive(false);
        notPerfectBtnEndgame.SetActive(false);
        yield return Cache.GetWFS(3f);
    }

    public void Btn_Replay() //Nút Retry trong phần UI EndBonusGame
    {
        SoundController.Instance.sound_Click.PlaySoundOneShot();
        AppLovinController.instance.SetInterPlacement(placement.Bonus_level_victory);
        if (DataManager.Instance.gameData.currentLevel >= FirebaseRemoteConfigManager.m_Inter_from_level) AppLovinController.instance.ShowInterstitial();
        BonusBrickManager.Instance.Btn_Replay();
    }

    private void Update()
    {
        if (lastComleteBrickAmount != BonusBrickManager.Instance.completeBrickAmount)
        {
            SetUpProcessImage(BonusBrickManager.Instance.completeBrickAmount);
            lastComleteBrickAmount = BonusBrickManager.Instance.completeBrickAmount;
        }else if (BonusBrickManager.Instance.endGameCompareIsDone)
        {
            perfectBtnEndgame.SetActive(myFloat > 0.995);
            notPerfectBtnEndgame.SetActive(myFloat <= 0.995);
            BonusBrickManager.Instance.endGameCompareIsDone = false;
        }
    }

    private void SetUpProcessImage(int percent)
    {
        processText.text = (int)(((float)percent / (float)BonusBrickManager.Instance.totalBonusBricks.Count) * 100f) + PERCENT;
        DOTween.To(() => myFloat, x => myFloat = x, ((float)percent / (float)BonusBrickManager.Instance.totalBonusBricks.Count), 0.1f).OnUpdate(() => { processImage.fillAmount = myFloat; });
        if (myFloat > 0.295f && !processStars[0].activeSelf) 
        {
            DataManager.Instance.gameData.paintLevelInformation[BonusLevelManager.Instance.playingLevelID - 1].numberOfStar = 1;
            DataManager.Instance.SaveGame();
            processStars[0].SetActive(true);
            StartStarEffect(0);
        }
        if (myFloat > 0.695f && !processStars[1].activeSelf)
        {
            DataManager.Instance.gameData.paintLevelInformation[BonusLevelManager.Instance.playingLevelID - 1].numberOfStar = 2;
            DataManager.Instance.SaveGame();
            processStars[1].SetActive(true);
            StartStarEffect(1);
        }
        if (myFloat > 0.9 && !processStars[2].activeSelf&& percent== BonusBrickManager.Instance.totalBonusBricks.Count)
        {
            DataManager.Instance.gameData.paintLevelInformation[BonusLevelManager.Instance.playingLevelID - 1].numberOfStar = 3;
            DataManager.Instance.SaveGame();
            processStars[2].SetActive(true);
            StartStarEffect(2);
        }
    }

    private void StartStarEffect(int starID)
    {
        SoundController.Instance.sound_FlyingStar.PlaySoundOneShot();
        flyStars[starID].transform.DOMove(listStars[starID].transform.position, 1f).OnComplete(()=> 
        {
            bloomStars[starID].transform.position = listStars[starID].transform.position;
            bloomStars[starID].SetActive(true);
            Timer.Schedule(this, 0.3f, () => 
            {
                SoundController.Instance.sound_Firework.PlaySoundOneShot();
                listStars[starID].SetActive(true);
            });
        });
    }

    IEnumerator ShowPopupNewTheme()
    {
        yield return Cache.GetWFS(0.5f);
        if(DataManager.Instance.gameData.themeID < 2) UI_Game.Instance.OpenUI(UIID.CanvasNewTheme);
        else
        {
            SoundController.Instance.sound_Click.PlaySoundOneShot();
            DataManager.Instance.gameData.themeID++;

            if (DataManager.Instance.gameData.themeID < 3) DataManager.Instance.gameData.currentThemeID = DataManager.Instance.gameData.themeID;
            else DataManager.Instance.gameData.currentThemeID = DataManager.Instance.gameData.targetThemeID;

            //Tìm target Theme
            do
            {
                DataManager.Instance.gameData.targetThemeID = Random.Range(0, 3);
            } while (DataManager.Instance.gameData.currentThemeID == DataManager.Instance.gameData.targetThemeID);
            DataManager.Instance.SaveGame();
            LoadNextLevel();
            base.Close();
        }
    }
}
