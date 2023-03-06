using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;
    public Sprite[] bonusLevelImage;
    public GiftDatas giftDatas;
    public GameData gameData;

    private bool isLoaded = false; 
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    public void OnApplicationPause(bool pause){ SaveGame(); }
    public void OnApplicationQuit(){ SaveGame(); }
    public void LoadData()
    {
        if (isLoaded) return;
        //Load Data cũ
        if (PlayerPrefs.HasKey("gameData")) gameData = JsonUtility.FromJson<GameData>(PlayerPrefs.GetString("gameData"));
        //Tạo data mới nếu là lần đầu mở game
        if (gameData.isFirstOpen) 
        {
            gameData = new GameData();
            for (int i = 0; i < giftDatas.BrickTypeInfo.Count; i++) //Add info từ SO
            {
                GiftInfo tempValue = new GiftInfo();
                tempValue.giftName = giftDatas.BrickTypeInfo[i].giftName;
                tempValue.giftID = giftDatas.BrickTypeInfo[i].giftID;
                tempValue.isUnlock = giftDatas.BrickTypeInfo[i].isUnlock;
                tempValue.progressPercent = giftDatas.BrickTypeInfo[i].progressPercent;
                tempValue.giftSprite = giftDatas.BrickTypeInfo[i].giftSprite;
                tempValue.giftEndgameSprite = giftDatas.BrickTypeInfo[i].giftEndgameSprite;
                gameData.brickTypeInfo.Add(tempValue);
            }
            for (int i = 0; i < giftDatas.HammerTypeInfo.Count; i++)
            {
                GiftInfo tempValue = new GiftInfo();
                tempValue.giftName = giftDatas.HammerTypeInfo[i].giftName;
                tempValue.giftID = giftDatas.HammerTypeInfo[i].giftID;
                tempValue.isUnlock = giftDatas.HammerTypeInfo[i].isUnlock;
                tempValue.progressPercent = giftDatas.HammerTypeInfo[i].progressPercent;
                tempValue.giftSprite = giftDatas.HammerTypeInfo[i].giftSprite;
                tempValue.giftEndgameSprite = giftDatas.HammerTypeInfo[i].giftEndgameSprite;
                gameData.hammerTypeInfo.Add(tempValue);
            }
        } 
        else
        {
            if ((int)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalDays > gameData.timeLastOpen)//Nếu sang ngày mới thì ??
            {
                gameData.days_played++;
            }
            gameData.timeLastOpen = (int)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalDays;
        }
        gameData.isFirstOpen = false;
        isLoaded = true;
        SaveGame();
    }

    public void SaveGame()
    {
        if (!isLoaded) return;
        PlayerPrefs.SetString("gameData", JsonUtility.ToJson(gameData));
        PlayerPrefs.Save();
    }
}
