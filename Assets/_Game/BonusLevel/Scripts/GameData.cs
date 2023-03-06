using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
[System.Serializable]
public class GameData
{
    //----- State -----
    public bool isFirstOpen;
    public bool isMusic;
    public bool isSound;
    public bool isVibrate;
    public bool isHideUI;
    public bool isBuildMKT;

    //----- Value -----
    public int currentLevel;            //Level hiện tại
    public int lastLevel;               //Sử dụng để bắn event firebase: checkpoint_start
    public int currentPaintLevel;       //Bonus Paint Level hiện tại
    public int bonusDesireLevel;        //Dùng để nhận biết người chơi muốn chơi level nào khi chọn trong list. Nếu =-1 nghĩa là người chơi chơi lần đầu.
    public int days_played = 1;             //Tổng số ngày player có chơi game
    public int reward_ads_watched;          //Tổng số ads reward mà người chơi đã xem
    public int levels_played_first_day = 0; //Số level đã chơi ở ngày đầu tiên
    public int levels_played = 0;           //Tổng số level đã chơi
    public int allLevelCounting = 0;        //Bộ đếm level. Đếm tất cả các level đã chơi qua kể cả mainlevel, paintlevel, sortlevel

    //Bonus Paint Level
    public bool isPaintTutorial;        //True: Đang để hiển thị Tutorial ở paint level; False: Đang tắt hiển thị tutorial ở màn hình paint

    //----- Time -----
    public int timeInstall;     //Thời điểm cài game
    public int timeLastOpen;    //Thời điểm cuối cùng mở game. Tính số ngày kể từ 1/1/1970

    //----- Bonus Level Info -----
    public PaintLevelInfo[] paintLevelInformation;

    //----- SortLevel -----
    public SortLevelInfo[] sortLevelInfomation;
    public bool isSortTutorialDone;
    public int currentSortLevel;

    //BrickType
    public int currentBrickTypeID; //0-Normal, 1-ConNhong, 2-Lego
    public List<GiftInfo> brickTypeInfo;    //Chứa thông tin của từng loại Brick
    //HammerType
    public int currentHammerTypeID;
    public List<GiftInfo> hammerTypeInfo; //Chứa thông tin của từng loại Hammer

    //Gift
    public int currentGiftID;   //ID của gift sẽ tăng từ 0 lên 7
    public string currentGiftName; //Dùng để bắn firebase
    public string currentSpecialItemName; //Dùng để bắn firebase

    //Theme
    public int themeID;
    public int currentThemeID;
    public int targetThemeID;

    public int lastTimeShowInter; //Level gần nhất show inter

    // special offer (pháo hoa và bomb)
    public int timeEndSpecialOffer = 0;    // ghi lại thời gian sẽ kết thúc special offer
    public int indexSpecialOffer = 0;    // lưu lại tiến độ của special offer
    public bool specialShowing = false;    // xem có đang show ra hay không

    public SpecialItemInfo[] specialItem;
    // public int levelGetFirework = 0;

    //Firebase
    public int sortLevelOfferTimes;
    public GameData()
    {
        isFirstOpen = true;
        isMusic = true;
        isSound = true;
        isVibrate = true;
        isHideUI = false;
        isBuildMKT = false;
        currentLevel = 1;
        currentPaintLevel = 1;
        lastLevel = 0;
        bonusDesireLevel = -1;
        allLevelCounting = 0;
        days_played = 1;             
        reward_ads_watched = 0;        
        levels_played_first_day = 0; 
        levels_played = 0;
        timeInstall = (int)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalDays;
        timeLastOpen = (int)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalDays;
        paintLevelInformation = new PaintLevelInfo[300];
        sortLevelInfomation = new SortLevelInfo[300];

        //Paint level
        isPaintTutorial = true;

        //----- ToyPicker -----
        isSortTutorialDone = false;
        currentSortLevel = 1;

        //Gift
        currentBrickTypeID = 0;
        currentHammerTypeID = 0;
        currentGiftID = 0;
        brickTypeInfo = new List<GiftInfo>();
        hammerTypeInfo = new List<GiftInfo>();

        //Theme
        themeID = 0;
        currentThemeID = 0;
        targetThemeID = 1;

        lastTimeShowInter = 0;

        sortLevelOfferTimes = 0;

        // special offer
        timeEndSpecialOffer = 0;
        indexSpecialOffer = 0;
        specialShowing = false;

        specialItem = new SpecialItemInfo[2];
        // levelGetFirework = 0;
    }
}

[System.Serializable]
public class PaintLevelInfo
{
    public bool isUnlock;
    public int numberOfStar;
    public bool isFirstTime;
    public bool isFirstTimeComplete;
    public PaintLevelInfo()
    {
        isUnlock = false;
        numberOfStar = 0;
        isFirstTime = true;
        isFirstTimeComplete = true;
    }
}

[System.Serializable]
public class SortLevelInfo
{
    public bool isUnlock;
    public bool isFirstTime;
    public bool isFirstTimeComplete;
    public SortLevelInfo()
    {
        isUnlock = false;
        isFirstTime = true;
        isFirstTimeComplete = true;
    }
}

[System.Serializable]
public class GiftInfo
{
    public string giftName;
    public bool isUnlock;
    public int progressPercent; //Đã đạt bao nhiêu % để được nhận rồi
    public int giftID;          //Dùng để sắp xếp thứ tự hiển thị ở EndGame. Còn phần hiển thị ở Property thì sẽ lấy theo thứ tự trong list Info => Phải sắp xếp trong list theo ID.
    public Sprite giftSprite;
    public Sprite giftEndgameSprite;
}

[System.Serializable] 
public class SpecialItemInfo
{
    public bool isDoneOffer;
    public bool isUnlock;
    public SpecialItemInfo()
    {
        isDoneOffer = false;
        isUnlock = false;
    }
}