using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasMissionFailed : UICanvas
{
    public Animator animator;

    public override void Setup()
    {
        base.Setup();
        SoundController.Instance.sound_Fail.PlaySoundOneShot();
        FirebaseManager.Instance.LogAnalyticsEvent(EventName.ads_reward_offer, Parameter.placement, placement.Endgame_Level_Skip);
        GameManager.Instance.ChangeState(GameState.Pause);
        AppLovinController.instance.SetInterPlacement(placement.Level_Failed);
        if (DataManager.Instance.gameData.currentLevel >= FirebaseRemoteConfigManager.m_Inter_from_level) AppLovinController.instance.ShowInterstitial();
    }

    public void RetryButton()
    {
        SoundController.Instance.sound_Click.PlaySoundOneShot();
        UI_Game.Instance.OpenUI(UIID.BlockRaycast);
        animator.SetTrigger(Constant.ANIM_CLOSE);
        AppLovinController.instance.SetInterPlacement(placement.Level_Failed);
        if (DataManager.Instance.gameData.currentLevel >= FirebaseRemoteConfigManager.m_Inter_from_level) AppLovinController.instance.ShowInterstitial();
        Invoke(nameof(Close), 0.65f);
    }

    public override void Close()
    {
        base.Close();
        UI_Game.Instance.CloseUI(UIID.BlockRaycast);
        LevelManager.Instance.LoadLevel();
        UI_Game.Instance.OpenUI(UIID.GamePlay);
        HammerControl.Instance.MoveToPlayGame();
    }

    public void Btn_SkipLevel()
    {
        SoundController.Instance.sound_Click.PlaySoundOneShot();
        AppLovinController.instance.SetRewardPlacement(placement.Endgame_Level_Skip);
        AppLovinController.instance.ShowRewardedAd(()=> 
        {
            FirebaseManager.Instance.LogAnalyticsEvent(EventName.checkpoint_end_ + DataManager.Instance.gameData.currentLevel.ToString(), Parameter.level, DataManager.Instance.gameData.currentLevel.ToString());
            FirebaseManager.Instance.LogCheckPoint(EventName.g_checkpoint_end_ + (((DataManager.Instance.gameData.currentLevel) > 9) ? (DataManager.Instance.gameData.currentLevel).ToString() : ("0" + (DataManager.Instance.gameData.currentLevel).ToString())), (DataManager.Instance.gameData.currentLevel).ToString(), levelType.paint);
            FirebaseManager.Instance.LogLevelComplete(EventName.g_level_complete.ToString(), (DataManager.Instance.gameData.currentLevel).ToString(), Control.Instance.timeCounting);
            Control.Instance.timeCounting = 0;
            DataManager.Instance.gameData.currentLevel++;
            DataManager.Instance.gameData.allLevelCounting++;
            DataManager.Instance.SaveGame();
            LoadNextLevel();
        });
    }

    public void LoadNextLevel()
    {
        LevelManager.Instance.LoadLevel();
        base.Close();
        UI_Game.Instance.OpenUI(UIID.GamePlay);
        HammerControl.Instance.MoveToPlayGame();
        GameManager.Instance.gameState = GameState.GamePlay;
    }
}
