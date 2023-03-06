using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CanvasLoading : UICanvas
{
    public static CanvasLoading ins;
    const string LOADING = "Loading";
    public Image img_ProccessLoading;
    public Image backGround;
    public Text processText;
    public float timeCounting = 0;
    public float realTimeCounting = 0;
    public bool isAdsLoaded = false;
    public bool loadingSceneDone = false;
    private float loadingTimeLimit;
    private void Start()
    {
        if (ins != null)
        {
            Destroy(this.gameObject);
            return;
        }
        timeCounting = 0;
        realTimeCounting = 0;
        DataManager.Instance.LoadData();
        Debug.Log("Start Loading Scene");
        loadingSceneDone = false;
        loadingTimeLimit = 7f;
#if UNITY_EDITOR
        loadingTimeLimit = 0.5f;
#endif
    }

    private void Update()
    {
        timeCounting += Time.deltaTime;
        realTimeCounting += Time.deltaTime;

        if ((int)(realTimeCounting * 2) % 4 == 0) processText.text = LOADING;
        if ((int)(realTimeCounting * 2) % 4 == 1) processText.text = LOADING+".";
        if ((int)(realTimeCounting * 2) % 4 == 2) processText.text = LOADING+"..";
        if ((int)(realTimeCounting * 2) % 4 == 3) processText.text = LOADING+"...";

        if (timeCounting<4) img_ProccessLoading.fillAmount = timeCounting / 10f; //Nếu chưa tải xong Ads thì chạy với tốc độ chậm
        else
        {
            timeCounting += Time.deltaTime;
            img_ProccessLoading.fillAmount = timeCounting / 10f;
        }
        if ((isAdsLoaded || realTimeCounting > loadingTimeLimit) && !loadingSceneDone) 
        {
            loadingSceneDone = true;
            LoadNextLevel();
        } 
    }

    void LoadNextLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(Constant.MainSceneStr);
    }
}
