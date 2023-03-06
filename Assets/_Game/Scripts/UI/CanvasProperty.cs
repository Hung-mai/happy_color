using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasProperty : UICanvas
{
    [Header("------------- special hammer --------")]
    [SerializeField] private GameObject[] specialHammers;

    [Header("-------------------------------------")]
    //Page
    [SerializeField] private GameObject brickPage;
    [SerializeField] private GameObject hammerPage;

    //Image
    [SerializeField] private Image[] hammerImageList;
    [SerializeField] private Image[] brickImageList;

    //OutLine
    [SerializeField] private GameObject[] brickOutLineList;
    [SerializeField] private GameObject[] hammerOutLineList;

    //BackGround
    [SerializeField] private Sprite[] backGroundImage;
    [SerializeField] private Image[] hammerBGList;
    [SerializeField] private Image[] brickBGList;

    //Btn List
    [SerializeField] private Sprite[] btnHammerListImage;
    [SerializeField] private Sprite[] btnBrickListImage;
    [SerializeField] private Image btnHammerImage;
    [SerializeField] private Image btnBrickImage;

    //Sellectting 
    private int sellectingHammerID;
    private int sellectingBrickID;

    //Button
    [SerializeField] private GameObject btnEquipHammer;
    [SerializeField] private GameObject btnEquipBrick;
    [SerializeField] private GameObject btnEquipSpecialItem;

    //StickList
    [SerializeField] private GameObject[] hammerStickList;
    [SerializeField] private GameObject[] brickStickList;

    //Ads icon List
    [SerializeField] private GameObject[] hammerAdsList;
    [SerializeField] private GameObject[] brickAdsList;

    //Unlock Btn
    [SerializeField] private GameObject[] btnHammerUnlock;
    [SerializeField] private GameObject[] AdsHammerImageList;
    [SerializeField] private GameObject[] loadingHammerImageList;
    [SerializeField] private GameObject[] btnBrickUnlock;
    [SerializeField] private GameObject[] AdsBrickImageList;
    [SerializeField] private GameObject[] loadingBrickImageList;
    public override void Setup()
    {
        base.Setup();
        Init();
        if (DataManager.Instance.gameData.indexSpecialOffer == 1)
        {
            DataManager.Instance.gameData.specialItem[0].isDoneOffer = true;
        }
        else if (DataManager.Instance.gameData.indexSpecialOffer == 2)
        {
            DataManager.Instance.gameData.specialItem[1].isDoneOffer = true;
        }
        DataManager.Instance.SaveGame();
        SpecialItemInit();
    }

    public override void Open()
    {
        base.Open();
    }

    private void Init()
    {
        hammerPage.SetActive(true);
        brickPage.SetActive(false);
        btnHammerImage.sprite = btnHammerListImage[0];  //Chuyển nút hammer về màu orange
        btnBrickImage.sprite = btnBrickListImage[1];  //Chuyển nút brick về màu gray
        LevelManager.Instance?.UnEnableToClickOnBrick();

        for (int i = 0; i < DataManager.Instance.gameData.hammerTypeInfo.Count; i++)    //Init Hammers
        {
            if (DataManager.Instance.gameData.hammerTypeInfo[i].progressPercent >= 99 || GameManager.Instance.IsBuildMaketing)
            {
                if (DataManager.Instance.gameData.hammerTypeInfo[i].isUnlock || GameManager.Instance.IsBuildMaketing)
                {
                    hammerBGList[i].sprite = backGroundImage[0];    //Đặt BG Unlock
                    hammerImageList[i].sprite = DataManager.Instance.gameData.hammerTypeInfo[i].giftSprite; //Setup hình ảnh
                    btnHammerUnlock[i].SetActive(false);
                    hammerImageList[i].color = new Color32(255, 255, 255, 255);
                    hammerAdsList[i].SetActive(false);
                }
                else
                {
                    hammerBGList[i].sprite = backGroundImage[0];    //Đặt BG Unlock
                    hammerImageList[i].sprite = DataManager.Instance.gameData.hammerTypeInfo[i].giftSprite; //Setup hình ảnh
                    btnHammerUnlock[i].SetActive(true);
                    hammerImageList[i].color = new Color32(255, 255, 255, 255);
                    hammerAdsList[i].SetActive(true);
                }
            }
            else
            {
                hammerBGList[i].sprite = backGroundImage[1];    //Đặt BG Lock
                hammerImageList[i].sprite = DataManager.Instance.gameData.hammerTypeInfo[i].giftSprite;
                btnHammerUnlock[i].SetActive(false);
                hammerImageList[i].color = new Color32(50, 50, 50, 255);
                hammerAdsList[i].SetActive(false);
            }

            if (DataManager.Instance.gameData.currentHammerTypeID == i)
            {
                hammerOutLineList[i].SetActive(true);
                hammerStickList[i].SetActive(true);
            }
            else
            {
                hammerOutLineList[i].SetActive(false);
                hammerStickList[i].SetActive(false);
            }

        }
        for (int i = 0; i < DataManager.Instance.gameData.brickTypeInfo.Count; i++)     //Init Brick
        {
            if (DataManager.Instance.gameData.brickTypeInfo[i].progressPercent >= 99 || GameManager.Instance.IsBuildMaketing)
            {
                if (DataManager.Instance.gameData.brickTypeInfo[i].isUnlock || GameManager.Instance.IsBuildMaketing)
                {
                    brickBGList[i].sprite = backGroundImage[0];    //Đặt BG Unlock
                    brickImageList[i].sprite = DataManager.Instance.gameData.brickTypeInfo[i].giftSprite;
                    btnBrickUnlock[i].SetActive(false);
                    brickImageList[i].color = new Color32(255, 255, 255, 255);
                    brickAdsList[i].SetActive(false);
                }
                else
                {
                    brickBGList[i].sprite = backGroundImage[0];    //Đặt BG Unlock
                    brickImageList[i].sprite = DataManager.Instance.gameData.brickTypeInfo[i].giftSprite;
                    btnBrickUnlock[i].SetActive(true);
                    brickImageList[i].color = new Color32(255, 255, 255, 255);
                    brickAdsList[i].SetActive(true);
                }
            }
            else
            {
                brickBGList[i].sprite = backGroundImage[1];    //Đặt BG Lock
                brickImageList[i].sprite = DataManager.Instance.gameData.brickTypeInfo[i].giftSprite;
                btnBrickUnlock[i].SetActive(false);
                brickImageList[i].color = new Color32(50, 50, 50, 255);
                brickAdsList[i].SetActive(false);
            }

            if (DataManager.Instance.gameData.currentBrickTypeID == i)
            {
                brickOutLineList[i].SetActive(true);
                brickStickList[i].SetActive(true);
            }
            else
            {
                brickOutLineList[i].SetActive(false);
                brickStickList[i].SetActive(false);
            }
        }
    }

    public override void Close()
    {
        LevelManager.Instance?.EnableToClickOnBrick();
        base.Close();
    }

    public void BtnHammerList()     //Nút bấm để xem list hammer
    {
        SoundController.Instance.sound_Click.PlaySoundOneShot();
        btnHammerImage.sprite = btnHammerListImage[0];  //Chuyển nút hammer về màu orange
        btnBrickImage.sprite = btnBrickListImage[1];  //Chuyển nút brick về màu gray
        hammerPage.SetActive(true);
        brickPage.SetActive(false);
    }

    public void BtnBrickList()      //Nút bấm để xem list Brick
    {
        SoundController.Instance.sound_Click.PlaySoundOneShot();
        btnHammerImage.sprite = btnHammerListImage[1];  //Chuyển nút hammer về màu orange
        btnBrickImage.sprite = btnBrickListImage[0];  //Chuyển nút brick về màu gray
        hammerPage.SetActive(false);
        brickPage.SetActive(true);
    }

    public void BtnHammerSellect(int hammerIndex)   //Nút bấm chọn Hammer
    {
        if (!DataManager.Instance.gameData.hammerTypeInfo[hammerIndex].isUnlock) return;
        SoundController.Instance.sound_Click.PlaySoundOneShot();
        sellectingHammerID = hammerIndex;
        btnEquipHammer.SetActive(true);
        btnEquipBrick.SetActive(false);
        btnEquipSpecialItem.SetActive(false);
        for (int i = 0; i < hammerOutLineList.Length; i++)
        {
            if (i == hammerIndex) hammerOutLineList[i].SetActive(true);
            else hammerOutLineList[i].SetActive(false);
        }
        for (int i = 0; i < specialItemBorder.Length; i++)
        {
            specialItemBorder[i].SetActive(false);
        }
    }

    public void BtnBrickSellect(int brickIndex)     //Nút bấm chọn Brick
    {
        if (!DataManager.Instance.gameData.brickTypeInfo[brickIndex].isUnlock) return;
        SoundController.Instance.sound_Click.PlaySoundOneShot();
        sellectingBrickID = brickIndex;
        btnEquipHammer.SetActive(false);
        btnEquipBrick.SetActive(true);
        btnEquipSpecialItem.SetActive(false);
        for (int i = 0; i < brickOutLineList.Length; i++)
        {
            if (i == brickIndex) brickOutLineList[i].SetActive(true);
            else brickOutLineList[i].SetActive(false);
        }
    }

    public void BtnEquipHammer(bool isPlaySound)    //Nút trang bị Hammer
    {
        if (!DataManager.Instance.gameData.hammerTypeInfo[sellectingHammerID].isUnlock && !GameManager.Instance.IsBuildMaketing) return;
        if(isPlaySound) SoundController.Instance.sound_Click.PlaySoundOneShot();
        DataManager.Instance.gameData.currentHammerTypeID = sellectingHammerID;
        HammerControl.Instance.SetHammerType((HammerType)sellectingHammerID);
        for (int i = 0; i < hammerStickList.Length; i++)
        {
            if (i == sellectingHammerID) hammerStickList[i].SetActive(true);
            else hammerStickList[i].SetActive(false);
        }
        for (int i = 0; i < specialItemTick.Length; i++)
        {
            specialItemTick[i].SetActive(false);
        }
    }

    public void BtnEquipBrick(bool isPlaySound)     //Nút trang bị brick
    {
        if (!DataManager.Instance.gameData.brickTypeInfo[sellectingBrickID].isUnlock && !GameManager.Instance.IsBuildMaketing) return;
        if (isPlaySound) SoundController.Instance.sound_Click.PlaySoundOneShot();
        DataManager.Instance.gameData.currentBrickTypeID = sellectingBrickID;
        BrickManager.Instance.SetBrickType((BrickType)sellectingBrickID);
        for (int i = 0; i < brickStickList.Length; i++)
        {
            if (i == sellectingBrickID) brickStickList[i].SetActive(true);
            else brickStickList[i].SetActive(false);
        }
    }

    public void BtnReceiveHammerByAds(int hammerIndex)
    {
        AppLovinController.instance.SetRewardPlacement(placement.Receive_Explode_item);
        if (!MaxSdk.IsRewardedAdReady(AppLovinController.instance.AndroidRewardID))
        {
            AdsHammerImageList[hammerIndex].SetActive(false);
            loadingHammerImageList[hammerIndex].SetActive(true);
        }
        if (GameManager.Instance.IsBuildMaketing || GameManager.Instance.IsNoAds)
        {
            DataManager.Instance.gameData.hammerTypeInfo[hammerIndex].isUnlock = true;
            BtnHammerSellect(hammerIndex);
            BtnEquipHammer(false);
            return;
        }
        StartCoroutine(WaitToShowReward(true, hammerIndex));
    }

    public void BtnReceiveBrickByAds(int brickIndex)
    {
        AppLovinController.instance.SetRewardPlacement(placement.Receive_Explode_item);
        if (!MaxSdk.IsRewardedAdReady(AppLovinController.instance.AndroidRewardID))
        {
            AdsBrickImageList[brickIndex].SetActive(false);
            loadingBrickImageList[brickIndex].SetActive(true);
        }
        if (GameManager.Instance.IsBuildMaketing || GameManager.Instance.IsNoAds)
        {
            DataManager.Instance.gameData.brickTypeInfo[brickIndex].isUnlock = true;
            BtnBrickSellect(brickIndex);
            BtnEquipBrick(false);
            Close();
            return;
        }
        StartCoroutine(WaitToShowReward(false, brickIndex));
    }

    IEnumerator WaitToShowReward(bool hammerORbrick,int giftIndex)  //True: Hammer, False: Brick
    {
        yield return new WaitUntil(() => MaxSdk.IsRewardedAdReady(AppLovinController.instance.AndroidRewardID));
        AppLovinController.instance.ShowRewardedAd(() =>
        {
            if (hammerORbrick)
            {
                DataManager.Instance.gameData.hammerTypeInfo[giftIndex].isUnlock = true;
                BtnHammerSellect(giftIndex);
                BtnEquipHammer(false);
            }
            else
            {
                DataManager.Instance.gameData.brickTypeInfo[giftIndex].isUnlock = true;
                BtnBrickSellect(giftIndex);
                BtnEquipBrick(false);
            }
            Close();
        });
    }

    [Header("Special Item")]
    public GameObject[] specialItemBorder;
    public GameObject[] specialItemTick;
    public GameObject[] specialItemAdsIcon;
    public GameObject[] specialItemAdsButton;
    public Image[] specialItemBG;

    #region Special Item Init
    public void SpecialItemInit()
    {
        if (DataManager.Instance.gameData.specialItem[0].isDoneOffer)
        {
            specialHammers[0].SetActive(true);
            if (DataManager.Instance.gameData.specialItem[0].isUnlock) 
            {
                specialItemBorder[0].SetActive(false);
                specialItemTick[0].SetActive(false);
                specialItemAdsIcon[0].SetActive(false);
                specialItemAdsButton[0].SetActive(false);
                specialItemBG[0].sprite = backGroundImage[0];
                if ((HammerType)DataManager.Instance.gameData.currentHammerTypeID == HammerType.Firework)
                {
                    specialItemBorder[0].SetActive(true);
                    specialItemTick[0].SetActive(true);
                }
                else
                {
                    specialItemBorder[0].SetActive(false);
                    specialItemTick[0].SetActive(false);
                }
            }
            else
            {
                specialItemBorder[0].SetActive(false);
                specialItemTick[0].SetActive(false);
                specialItemAdsIcon[0].SetActive(true);
                specialItemAdsButton[0].SetActive(true);
                specialItemBG[0].sprite = backGroundImage[1];
            }
        }else specialHammers[0].SetActive(false);
        if (DataManager.Instance.gameData.specialItem[1].isDoneOffer)
        {
            specialHammers[1].SetActive(true);
            if (DataManager.Instance.gameData.specialItem[1].isUnlock)
            {
                specialItemBorder[1].SetActive(false);
                specialItemTick[1].SetActive(false);
                specialItemAdsIcon[1].SetActive(false);
                specialItemAdsButton[1].SetActive(false);
                specialItemBG[1].sprite = backGroundImage[0];
                if ((HammerType)DataManager.Instance.gameData.currentHammerTypeID == HammerType.Bomb)
                {
                    specialItemBorder[1].SetActive(true);
                    specialItemTick[1].SetActive(true);
                }
                else
                {
                    specialItemBorder[1].SetActive(false);
                    specialItemTick[1].SetActive(false);
                }
            }
            else
            {
                specialItemBorder[1].SetActive(false);
                specialItemTick[1].SetActive(false);
                specialItemAdsIcon[1].SetActive(true);
                specialItemAdsButton[1].SetActive(true);
                specialItemBG[1].sprite = backGroundImage[1];
            }
        }else specialHammers[1].SetActive(false);
    }
    #endregion

    public void BtnSpecialItemUnlock(int specialItemID)
    {
        StartCoroutine(UnlockSpecialItemByAds(specialItemID));
    }

    IEnumerator UnlockSpecialItemByAds(int specialItemID)
    {
        AppLovinController.instance.SetRewardPlacement(placement.Receive_Explode_item);
        yield return new WaitUntil(() => MaxSdk.IsRewardedAdReady(AppLovinController.instance.AndroidRewardID));
        AppLovinController.instance.ShowRewardedAd(() =>
        {
            DataManager.Instance.gameData.specialItem[specialItemID-6].isUnlock = true;
            specialItemAdsIcon[specialItemID - 6].SetActive(false);
            specialItemAdsButton[specialItemID - 6].SetActive(false);
            specialItemBG[specialItemID - 6].sprite = backGroundImage[0];
            SpecialItemSellect(specialItemID);
            BtnEquipSpecialItem();
        });
    }

    public void SpecialItemSellect(int specialItemID)
    {
        if (!DataManager.Instance.gameData.specialItem[specialItemID-6].isUnlock) return;
        SoundController.Instance.sound_Click.PlaySoundOneShot();
        if (specialItemID == 6)
        {
            specialItemBorder[0].SetActive(true);
            specialItemBorder[1].SetActive(false);
        }
        else if (specialItemID == 7)
        {
            specialItemBorder[0].SetActive(false);
            specialItemBorder[1].SetActive(true);
        }
        sellectingHammerID = specialItemID;
        btnEquipHammer.SetActive(false);
        btnEquipBrick.SetActive(false);
        btnEquipSpecialItem.SetActive(true);
        for (int i = 0; i < hammerOutLineList.Length; i++)
        {
            hammerOutLineList[i].SetActive(false);
        }
    }

    public void BtnEquipSpecialItem()
    {
        if (!DataManager.Instance.gameData.specialItem[sellectingHammerID - 6].isUnlock) return;
        SoundController.Instance.sound_Click.PlaySoundOneShot();
        DataManager.Instance.gameData.currentHammerTypeID = sellectingHammerID;
        HammerControl.Instance.SetHammerType((HammerType)DataManager.Instance.gameData.currentHammerTypeID);
        for (int i = 0; i < hammerStickList.Length; i++)
        {
            hammerStickList[i].SetActive(false);
        }
        for (int i = 0; i < specialItemTick.Length; i++)
        {
            if (i == (sellectingHammerID - 6)) specialItemTick[i].SetActive(true);
            else specialItemTick[i].SetActive(false);
        }
    }
}
