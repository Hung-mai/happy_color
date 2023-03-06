using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Control : Singleton<Control>
{
    public const float SPEED = 20;

    public GameObject[] hitEffect;
    public Transform hitEffectTranform;
    public Transform bombEffectTranform;

    private List<Vector2Int> waves;

    private UnityAction OnExecuteAction;

    private Vector3 indexPoint;

    public bool isCheckWave = false;

    private float range;

    public ColorType colorType => HammerControl.Instance.CurrentHammer.ColorType;

    public bool isWinWave = false;

    public float raycastDistance = Mathf.Infinity;

    private bool allowToStartWave = false;

    public float timeCounting;

    private void Start()
    {
        hitEffect[0].SetActive(false);
        hitEffect[1].SetActive(false);
        CameraControl.Instance.ChangeCameraPosition();
        allowToStartWave = false;
        timeCounting = 0;
    }

    private void Update()
    {
        if (GameManager.Instance.IsEditor) return;
        if (Input.GetMouseButtonDown(0) && !isCheckWave && OnExecuteAction == null && GameManager.Instance.IsState(GameState.GamePlay))
        {
            allowToStartWave = true;
        }
        timeCounting += Time.deltaTime;
    }

    private void FixedUpdate()
    {
        OnExecuteAction?.Invoke();
        CheckWave();
        if (allowToStartWave)
        {
            allowToStartWave = false;
            CheckHitPoint();
        }
    }

    public void OnInit()
    {

    }

    public void OnReset()
    {

    }

    private void CheckHitPoint()
    {
        if (HammerControl.Instance.IsHaveHammer)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, raycastDistance, 16))
            {
                StartWave(hit.point);
            }
        }
    }

    public void StartWave(Vector3 indexPoint)
    {
        if (LevelManager.Instance.brickManager.SearchBrickClosest(indexPoint) == null) return;
        if (!LevelManager.Instance.brickManager.SearchBrickClosest(indexPoint).IsClear)
        {
            if(LevelManager.Instance.brickManager.SearchBrickClosest(indexPoint).ColorType == colorType)
            {
                HammerControl.Instance.CurrentHammer.OnNoHit();
                UI_Game.Instance.GetUI<CanvasGamePlay>(UIID.GamePlay).StartWarning();
                if (DataManager.Instance.gameData.isVibrate) Vibration.Vibrate(50);
                return;
            }
            //fix by hammer color
            GameManager.Instance.ChangeState(GameState.BlockAction);
            UI_Game.Instance.GetUI<CanvasGamePlay>(UIID.GamePlay).MoveBtn(true);
            UI_Game.Instance.GetUI<CanvasGamePlay>(UIID.GamePlay).isAllowToReplay=true;
            hitEffectTranform.position = new Vector3(indexPoint.x, 2f, indexPoint.z);
            bombEffectTranform.position = new Vector3(indexPoint.x, 2f, indexPoint.z);
            if ((HammerType)DataManager.Instance.gameData.currentHammerTypeID != HammerType.Firework)
            {
                Timer.Schedule(this, 0.9f, () =>
                {
                    SoundController.Instance.sound_Knock.PlaySoundOneShot(0, 1, 0.2f);
                    Timer.Schedule(this, 0.4f, () =>
                    {
                        if ((HammerType)DataManager.Instance.gameData.currentHammerTypeID == HammerType.Bomb || (HammerType)DataManager.Instance.gameData.currentHammerTypeID == HammerType.Firework) hitEffect[1].SetActive(true);
                        else hitEffect[0].SetActive(true);
                        Timer.Schedule(this, 1f, () =>
                        {
                            hitEffect[0].SetActive(false);
                            hitEffect[1].SetActive(false);
                        });
                    });
                });
            }
            this.indexPoint = indexPoint;
            Brick brick = LevelManager.Instance.brickManager.SearchBrickClosest(indexPoint);
            BrickManager.Instance.ChangeBrickData(brick.brickData, colorType);
            waves = new List<Vector2Int>(brick.brickData.listBricksID);
            Hammer hammer = HammerControl.Instance.CurrentHammer;
            hammer.ActiveAction(brick.tf.position, 
                ()=> 
                {
                    range = 0;
                    isCheckWave = true;
                },
                
                () =>
                {
                    HammerControl.Instance.CollectCurrentHammer();
                    GameManager.Instance.ChangeState(GameState.GamePlay);
                }
            );

            OnExecuteAction = null;
        }
    }

    public void StartFireworkEffect()
    {
        SoundController.Instance.sound_Knock.PlaySoundOneShot(0, 1, 0.2f);
        if ((HammerType)DataManager.Instance.gameData.currentHammerTypeID == HammerType.Bomb || (HammerType)DataManager.Instance.gameData.currentHammerTypeID == HammerType.Firework) hitEffect[1].SetActive(true);
        else hitEffect[0].SetActive(true);
        Timer.Schedule(this, 1f, () =>
        {
            hitEffect[0].SetActive(false);
            hitEffect[1].SetActive(false);
        });
        Timer.Schedule(this, 0.4f, () =>
        {
            
        });
    }

    public void CheckWave()
    {
        if (isCheckWave || OnExecuteAction != null)
        {
            range += SPEED;

            if (isWinWave)
            {
                CheckWinWave(indexPoint, range * Time.smoothDeltaTime);
            }
            else
            {
                CheckWave(indexPoint, range * Time.smoothDeltaTime);
            }

            Debug.Log("checkWave");
        }
    }

    private void CheckWave(Vector3 indexPoint, float range)
    {
        range = range * range;

        if (waves.Count > 0)
        {
            for (int i = 0; i < waves.Count; i++)
            {
                if ((BrickManager.Instance.GetBrickMap(waves[i]).tf.position - indexPoint).sqrMagnitude <= range)
                {
                    Brick brick = BrickManager.Instance.GetBrickMap(waves[i]);
                    brick.OnEnter(brick.AddWave, brick.ChangeColor, brick.RemoveWave);
                    waves.RemoveAt(i);
                    i--;
                }
            }

        }

        isCheckWave = waves.Count > 0;
        if (!isCheckWave) 
        {
                StartCoroutine(AllowReplay());
                Timer.Schedule(this, 0.5f, () => { UI_Game.Instance.GetUI<CanvasGamePlay>(UIID.GamePlay).MoveBtn(false); });
        }
    }

    IEnumerator AllowReplay()
    {
        yield return Cache.GetWFS(0.5f);
        UI_Game.Instance.GetUI<CanvasGamePlay>(UIID.GamePlay).isAllowToReplay = false;
    }

    public void AddWave(Brick brick)
    {
        OnExecuteAction += brick.OnExecute;
    }

    public void RemoveWave(Brick brick)
    {
        OnExecuteAction -= brick.OnExecute;

        //finish wave
        //Check State
        if (OnExecuteAction == null)
        {
            BrickManager.Instance.MergeBrickGroup();
            //UI_Game.Instance.GetUI<CanvasGamePlay>(UIID.GamePlay).UpdateBtnMoveHammers();
            if (BrickManager.Instance.IsFinishGame())
            {
                WinWave();
            }
            else
            {
                Timer.Schedule(this, 1f, () =>
                {
                    if (HammerControl.Instance.IsEmptyHammer)
                    {
                        UI_Game.Instance.CloseUI(UIID.GamePlay);
                        UI_Game.Instance.OpenUI(UIID.MissionFail);
                        FirebaseManager.Instance.LogAnalyticsEvent(EventName.level_fail, Parameter.level, DataManager.Instance.gameData.currentLevel.ToString());
                        FirebaseManager.Instance.LogAnalyticsEvent(EventName.g_level_fail, Parameter.level, (DataManager.Instance.gameData.currentLevel).ToString());
                    }
                });
            }
        }
    }


    #region WinWave

    private void WinWave()
    {
        GameManager.Instance.ChangeState(GameState.Pause);
        UI_Game.Instance.OpenUI(UIID.BlockRaycast);

        Brick brick = LevelManager.Instance.brickManager.GetRandomBrick();
        waves = new List<Vector2Int>(brick.brickData.listBricksID);

        this.indexPoint = brick.tf.position;
        range = 0;
        isWinWave = true;
        OnExecuteAction = null;
        isCheckWave = true;
    }

    private void CheckWinWave(Vector3 indexPoint, float range)
    {
        range = range * range;

        if (waves.Count > 0)
        {
            for (int i = 0; i < waves.Count; i++)
            {
                if ((BrickManager.Instance.GetBrickMap(waves[i]).tf.position - indexPoint).sqrMagnitude <= range)
                {
                    Brick brick = BrickManager.Instance.GetBrickMap(waves[i]);
                    brick.OnEnter(brick.AddWinWave, null, brick.RemoveWinWave);
                    waves.RemoveAt(i);
                    i--;
                }
            }

        }

        isCheckWave = waves.Count > 0;
        if(!isCheckWave) StartCoroutine(AllowReplay());
    }

    public void RemoveWinWave(Brick brick)
    {
        OnExecuteAction -= brick.OnExecute;

        //finish wave
        //Check State
        if (OnExecuteAction == null)
        {
            //FirebaseManager.Instance.LogAnalyticsEvent(EventName.checkpoint_ + DataManager.Instance.gameData.countDownToBonusLevel.ToString());
            UI_Game.Instance.CloseUI(UIID.BlockRaycast);
            UI_Game.Instance.CloseUI(UIID.GamePlay);
            DataManager.Instance.gameData.currentLevel++;    //Tăng 1 Level
            DataManager.Instance.gameData.allLevelCounting++;    //Tăng 1 Level
            if(DataManager.Instance.gameData.currentLevel==12&& FirebaseRemoteConfigManager.m_Enable_PopupRate) UI_Game.Instance.OpenUI(UIID.Rate);
            else UI_Game.Instance.OpenUI(UIID.MissionComplete);
            isWinWave = false;
            DataManager.Instance.SaveGame();
        }
    }

    #endregion
}


