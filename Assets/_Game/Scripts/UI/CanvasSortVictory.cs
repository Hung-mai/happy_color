using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasSortVictory : UICanvas
{
    public Canvas canvas;
    public override void Open()
    {
        base.Open();
        StartCoroutine(DelayToPlayAds());
        FirebaseManager.Instance.LogLevelComplete(EventName.g_level_complete.ToString(), (DataManager.Instance.gameData.currentLevel-1).ToString(), LevelBonus_Controller.Instance.timeCounting);
        FirebaseManager.Instance.LogLevelComplete(EventName.g_sortgame_level_complete_ + ((DataManager.Instance.gameData.currentSortLevel > 9) ? DataManager.Instance.gameData.currentSortLevel.ToString() : ("0" + DataManager.Instance.gameData.currentSortLevel).ToString()), (DataManager.Instance.gameData.allLevelCounting + 1).ToString(), LevelBonus_Controller.Instance.timeCounting);
        LevelBonus_Controller.Instance.timeCounting = 0;
        DataManager.Instance.gameData.allLevelCounting++;
        DataManager.Instance.gameData.currentSortLevel++;
        DataManager.Instance.SaveGame();
    }
    IEnumerator DelayToPlayAds() //Chờ chạy hết anim show popup mới show ads
    {
        yield return Cache.GetWFS(0.5f);
        SoundController.Instance.sound_Victory.PlaySoundOneShot();
        SoundController.Instance.sound_Firework.PlaySoundOneShot();
        canvas.worldCamera = Camera.main;
    }

    public override void Close()
    {
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(Constant.MainSceneStr);
        base.Close();
    }
}
