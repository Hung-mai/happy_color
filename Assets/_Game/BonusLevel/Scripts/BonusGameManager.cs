using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusGameManager : Singleton<BonusGameManager>
{
    public Material choosingMaterial;
    public GameObject fireWorkVfx;
    public float timeCounting = 0;

    private void Awake()
    {
        GameManager.Instance.ChangeState(GameState.MainMenu);
        Application.targetFrameRate = 60;
        //Time.captureDeltaTime = 0.02f; 
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        GameManager.Instance.gameMode = GameMode.Paint;
        timeCounting = 0;
    }

    public void StartGame()
    {
        UI_Game.Instance.GetUI<CanvasBonusGamePlay>(UIID.BonusGamePlay); //UI_Game.Instance.GetUI(UIID.BonusGamePlay).gameObject.GetComponent<CanvasBonusGamePlay>().StartGame();
    }
    private void Update()
    {
        timeCounting +=Time.deltaTime;
    }
}
