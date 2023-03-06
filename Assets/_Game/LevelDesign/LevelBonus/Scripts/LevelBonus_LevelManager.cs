using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBonus_LevelManager : LevelBase
{
    [Header("--------LevelManager---------")]
    public LevelBonus currentLevelBonus;

    [Header("------Levels--------")]
    public DataLevelBonus[] dataLevelsBonus;

    private Coroutine timer;
    private void Awake()
    {
        InitializeLevelBonus();
    }
    private void Start()
    {
        InitializeTimer(currentLevelBonus.TimeInGame);
        FirebaseManager.Instance.LogCheckPoint(EventName.g_checkpoint_start_ + ((DataManager.Instance.gameData.currentLevel > 9) ? DataManager.Instance.gameData.currentLevel.ToString() : ("0" + DataManager.Instance.gameData.currentLevel.ToString())), DataManager.Instance.gameData.currentLevel.ToString(), levelType.sort);
    }
    #region Timer
    private void InitializeTimer(int timeCount)
    {
        timer = StartCoroutine(Timer(timeCount));
    }

    IEnumerator Timer(int timeCount)//dvi second
    {
        for (int i = timeCount; i >=0; i-- )
        {
            if (i == 0)
            {
                Lose();
            }
            yield return new WaitForSeconds(1f);
        }
    }
    #endregion
    #region Initialize level
    private void InitializeLevelBonus()
    {
        if (currentLevelBonus != null)
        {
            DestroyImmediate(currentLevelBonus);
        }
        foreach (DataLevelBonus d in dataLevelsBonus)
        {
            Debug.Log(d.level);
            if (d.level == DataManager.Instance.gameData.currentSortLevel)
            {
                currentLevelBonus = Instantiate(d.prefabLevel);
                if (currentLevelBonus.isTutorial && !DataManager.Instance.gameData.isSortTutorialDone)
                {
                    currentLevelBonus.SetUpTutorial();
                    return;
                }
                currentLevelBonus.SignItemToBox();
                return;
            }
        }
        //khong co levelbonus day
        currentLevelBonus = Instantiate(dataLevelsBonus[dataLevelsBonus.Length - 1].prefabLevel);
        currentLevelBonus.SignItemToBox();
        /*if (DataManager.Instance.gameData.sortLevelInfomation[DataManager.Instance.gameData.currentSortLevel-1].isFirstTime)
        {
            DataManager.Instance.gameData.sortLevelInfomation[DataManager.Instance.gameData.currentSortLevel - 1].isFirstTime = false;
            //FirebaseManager.Instance.LogAnalyticsEvent(EventName.g_gameplay_sort_start_level_ + ((DataManager.Instance.gameData.currentSortLevel > 9) ? DataManager.Instance.gameData.currentSortLevel.ToString() : ("0" + DataManager.Instance.gameData.currentSortLevel).ToString()), Parameter.level, DataManager.Instance.gameData.currentSortLevel.ToString());
        }*/
    }
    #endregion
    
    public void Win()
    {
        StopCoroutine(timer);
        StartCoroutine(EndLevel(true));
    }
    public void Lose()
    {
        StartCoroutine(EndLevel(false));
    }
}
