using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LevelBonus : Singleton<LevelBonus>
{
    public Box[] boxs;
    public Transform itemContainer;
    public bool isTutorial = false;
    public int TimeInGame;
    #region tool check and initilatize level
    
    public Item[] items ;

    [Header("-------TriggerSignItem--------")]
    public bool isSign;
    private void OnValidate()
    {
        for (int i = 0; i < boxs.Length; i++)
        {
            for (int j = 0; j < boxs[i].points.Length; j++)
            {
                boxs[i].points[j].item = null;
            }
        }
        if (!isSign) return;
        SignItemToBox();
        isSign = false;
        
    }
    public void SignItemToBox()
    {
        if (handTut != null && DataManager.Instance.gameData.isSortTutorialDone)
        {
            isTutorial = false;
            handTut.SetActive(false);
        }
        List<Item> lstItem = new List<Item>();
        foreach (Item i in items)
        {
            lstItem.Add(i);
        }
        
        //sign item vào box.point.item
        foreach (Box b in boxs)
        {
            if (lstItem.Count == 0)
            {
                ChooseRandomItem(b);
                continue;
            }
            b.points[0].item = lstItem[Random.Range(0, lstItem.Count)];//item0
            b.points[0].item.transform.position = b.points[0].transPoint.position;
            lstItem.Remove(b.points[0].item);
            if (lstItem.Count == 0)
            {
                ChooseRandomItem(b);
                //b.points[1].item = null;
                //b.points[2].item = null;
                continue;
            }
            for (int y = 0; y < lstItem.Count; y++)
            {
                if (lstItem[y].type != b.points[0].item.type)
                {
                    b.points[1].item = lstItem[y];//item1
                    b.points[1].item.transform.position = b.points[1].transPoint.position;
                    lstItem.RemoveAt(y);
                    
                    break;
                }
            }
            if (lstItem.Count == 0)
            {
                ChooseRandomItem(b);
                //b.points[2].item = null;
                continue;
            }
            b.points[2].item = lstItem[Random.Range(0, lstItem.Count)];//item2
            b.points[2].item.transform.position = b.points[2].transPoint.position;
            lstItem.Remove(b.points[2].item);
            if (lstItem.Count == 0)
            {
                ChooseRandomItem(b);
                //b.points[2].item = null;
                continue;
            }
            b.points[3].item = lstItem[Random.Range(0, lstItem.Count)];//item3
            b.points[3].item.transform.position = b.points[3].transPoint.position;
            lstItem.Remove(b.points[3].item);
            //sap xep lai position
        }
        
    }

    void ChooseRandomItem(Box b)
    {
        //random o doan nay`
        if (Random.Range(0, 3) == 0&&false) b.points[0].item = null;
        else
        {
            int boxsI = Random.Range(0, boxs.Length);
            int pointI = Random.Range(0, 3);
            Item i = boxs[boxsI].points[pointI].item;
            if (i != null)
            {
                boxs[boxsI].points[pointI].item = null;
                b.points[0].item = i;
                b.points[0].item.transform.position = b.points[0].transPoint.position;
            }
            else b.points[0].item = null;
        }

        if (Random.Range(0, 3) == 0 && false) b.points[1].item = null;
        else
        {
            int boxsI = Random.Range(0, boxs.Length);
            int pointI = Random.Range(0, 3);
            Item i = boxs[boxsI].points[pointI].item;
            if (i != null)
            {
                boxs[boxsI].points[pointI].item = null;
                b.points[1].item = i;
                b.points[1].item.transform.position = b.points[1].transPoint.position;
            }
            else b.points[1].item = null;
        }
        if (Random.Range(0, 3) == 0 && false) b.points[2].item = null;
        else
        {
            int boxsI = Random.Range(0, boxs.Length);
            int pointI = Random.Range(0, 3);
            Item i = boxs[boxsI].points[pointI].item;
            if (i != null)
            {
                boxs[boxsI].points[pointI].item = null;
                b.points[2].item = i;
                b.points[2].item.transform.position = b.points[2].transPoint.position;
            }
            else b.points[2].item = null;
        }
        if (Random.Range(0, 3) == 0 && false) b.points[3].item = null;
        else
        {
            int boxsI = Random.Range(0, boxs.Length);
            int pointI = Random.Range(0, 3);
            Item i = boxs[boxsI].points[pointI].item;
            if (i != null)
            {
                boxs[boxsI].points[pointI].item = null;
                b.points[3].item = i;
                b.points[3].item.transform.position = b.points[3].transPoint.position;
            }
            else b.points[3].item = null;
        }
    }

    [ContextMenu("CheckAmout")]
    public void CheckAmountItem()
    {
        List<Item> templstItems = new List<Item>();
        foreach (Item i in items)
        {
            templstItems.Add(i);
        }
        Level.CheckAmount(templstItems);
    }
    #endregion
    
    public bool CheckWin()
    {
        foreach (Box b in boxs)
        {
            foreach (Point p in b.points)
            {
                if (p.item != null)
                {
                    return false;
                }
            }
        }
        return true;
    }
    public void CheckBox(Box b)
    {
        if (b.points[0].item == null || b.points[1].item == null || b.points[2].item == null || b.points[3].item == null) return;

        if (b.points[0].item.type == b.points[1].item.type && b.points[0].item.type == b.points[2].item.type && b.points[0].item.type == b.points[3].item.type)
        {
            Item item = b.points[0].item;
            Item item1 = b.points[1].item;
            Item item2 = b.points[2].item;
            Item item3 = b.points[3].item;
            int LayerIgnoreRaycast = LayerMask.NameToLayer("Ignore Raycast");
            
            foreach (Point p in b.points)
            {
                p.item.gameObject.layer = LayerIgnoreRaycast;
                p.item = null;
            }
            Vector3 dessapearDestination = (item.transform.position + item3.transform.position) / 2f;
            item1.transform.DOMove(dessapearDestination, 0.3f).SetEase(Ease.InBack);
            item.transform.DOMove(dessapearDestination, 0.3f).SetEase(Ease.InBack);
            item3.transform.DOMove(dessapearDestination, 0.3f).SetEase(Ease.InBack);
            item2.transform.DOMove(dessapearDestination, 0.3f).SetEase(Ease.InBack).OnComplete(() => {
                Vector3 posEffect = item1.transform.position + Vector3.up * 0.05f + (Camera.main.transform.position - item1.transform.position) * 0.35f;
                SoundController.Instance.sound_Sort_Explore.PlaySoundOneShot();
                ObjectPooler.Instance.SpawnFromPool("Effect", posEffect, Camera.main.transform.eulerAngles);
                item.gameObject.SetActive(false);
                item1.gameObject.SetActive(false);
                item2.gameObject.SetActive(false);
                item3.gameObject.SetActive(false);
                if (isTutorial && !DataManager.Instance.gameData.isSortTutorialDone)
                {
                    DataManager.Instance.gameData.isSortTutorialDone = true;
                    isTutorial = false;
                    handTut.SetActive(false);
                    Timer.Schedule(this,timeDelayTutorial,()=> {
                        foreach (Item i in items)
                        {
                            i.gameObject.SetActive(true);
                        }
                        SignItemToBox();
                    });
                    
                    return;
                }
                if (DataManager.Instance.gameData.isSortTutorialDone)
                {
                    handTut.SetActive(false) ;
                }
                //check Win
                if (CheckWin())
                {
                    StartCoroutine(ShowWinPopup());
                    AppLovinController.instance.SetInterPlacement(placement.Sort_level_complete);
                    AppLovinController.instance.ShowInterstitial();
                }
            });
            
        }
        
    }

    IEnumerator ShowWinPopup()
    {
        yield return Cache.GetWFS(1f);
        FirebaseManager.Instance.LogCheckPoint(EventName.g_checkpoint_end_ + (((DataManager.Instance.gameData.currentLevel - 1) > 9) ? (DataManager.Instance.gameData.currentLevel - 1).ToString() : ("0" + (DataManager.Instance.gameData.currentLevel-1).ToString())), (DataManager.Instance.gameData.currentLevel-1).ToString(), levelType.sort);
        //FirebaseManager.Instance.LogAnalyticsEvent(EventName.g_gameplay_sort_end_level_ + ((DataManager.Instance.gameData.currentSortLevel > 9) ? DataManager.Instance.gameData.currentSortLevel.ToString() : ("0" + DataManager.Instance.gameData.currentSortLevel).ToString()), Parameter.level, DataManager.Instance.gameData.currentSortLevel.ToString());
        UI_Game.Instance.OpenUI(UIID.LevelSortVictory);

    }

    #region Tutorial
    [Header("--------Tutorial--------")]
    public Item[] itemsTutorial;
    public Box[] boxsTutorial;
    public GameObject handTut;
    [SerializeField]private float timeDelayTutorial = 1.5f;

    public void SetUpTutorial()
    {
        //box 10- 21
        //cho toàn bộ item setactive false
        //gỡ toàn bộ item ra khỏi box

        foreach (Item i in items)
        {
            i.gameObject.SetActive(false);
        }

        foreach (Box b in boxs)
        {
            b.points[0].item = null;
            b.points[1].item = null;
            b.points[2].item = null;
            b.points[3].item = null;
        }
        foreach (Item i in itemsTutorial)
        {
            i.gameObject.SetActive(true);
        }

        //set vi tri item tutorial vao` box tutorial
        //box 9
        boxsTutorial[1].points[0].item = itemsTutorial[0];
        itemsTutorial[0].transform.position = boxsTutorial[1].points[0].transPoint.position;

        boxsTutorial[1].points[1].item = itemsTutorial[1];
        itemsTutorial[1].transform.position = boxsTutorial[1].points[1].transPoint.position;

        
        boxsTutorial[1].points[3].item = itemsTutorial[2];
        itemsTutorial[2].transform.position = boxsTutorial[1].points[3].transPoint.position;
        //box 6
        boxsTutorial[0].points[2].item = itemsTutorial[3];
        itemsTutorial[3].transform.position = boxsTutorial[0].points[2].transPoint.position;
    }
    #endregion
    
}
