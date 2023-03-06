using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HammerControl : Singleton<HammerControl>
{
    public Transform tf;

    public float space = 3f;

    public Hammer currentHammer;

    public Hammer CurrentHammer => currentHammer;

    public bool IsHaveHammer => currentHammer != null;

    public Transform content;

    public Transform banktf; 

    public Hammer hammerPrefab;

    public LayerMask hammerMask;

    public bool IsEmptyHammer => hammers.Count == 0;

    public float raycastDistance = Mathf.Infinity;

    private float initPoint = -1;

    private MiniPool<Hammer> hammerPool = new MiniPool<Hammer>();

    public List<Hammer> hammers = new List<Hammer>();
    public float TargetZPosition = -22f;

    public bool allowToMoveHammers = false;

    //Touch
    Vector3 tempPosition = Vector3.zero;
    Vector3 tempHammerPos = Vector3.zero;
    private bool isUpdateHammerDone = true;

    private void Awake()
    {
        hammerPool.OnInit(hammerPrefab.gameObject, 10, content);
        allowToMoveHammers = false;
    }

    private void Update()
    {
        if (GameManager.Instance.IsEditor) return;
        HammerSellect();
        /*if (Input.GetKey(KeyCode.Q))
        {
            SetHammerType(HammerType.NormalHammer);
        }
        if (Input.GetKey(KeyCode.W))
        {
            SetHammerType(HammerType.Hammer1);
        }
        if (Input.GetKey(KeyCode.E))
        {
            SetHammerType(HammerType.Hammer2);
        }
        if (Input.GetKey(KeyCode.R))
        {
            SetHammerType(HammerType.Hammer3);
        }
        if (Input.GetKey(KeyCode.T))
        {
            SetHammerType(HammerType.Hammer4);
        }
        if (Input.GetKey(KeyCode.Y))
        {
            SetHammerType(HammerType.Hammer5);
        }
        if (Input.GetKey(KeyCode.U))
        {
            SetHammerType(HammerType.Firework);
        }
        if (Input.GetKey(KeyCode.I))
        {
            SetHammerType(HammerType.Bomb);
        }*/
    }

    public void MoveTheHammers(bool rightleft) //0: Right, 1: Left
    {
        if(rightleft) tf.DOMoveX(tf.position.x + 5f, 0.5f);
        else tf.DOMoveX(tf.position.x - 5f, 0.5f);
        banktf.position = new Vector3(0, banktf.position.y, banktf.position.z);
        Timer.Schedule(this, 0.5f, () => { UI_Game.Instance.GetUI<CanvasGamePlay>(UIID.GamePlay).UpdateBtnMoveHammers(); });
        
    }

    private void HammerSellect()
    {
        if (Input.GetMouseButtonDown(0) && GameManager.Instance.IsState(GameState.GamePlay))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, raycastDistance, hammerMask))
            {
                Hammer hammer = Cache.GetHammer(hit.collider);

                if (hammer != null)
                {
                    hammer.OnClick();
                }
                else if (currentHammer == null)
                {
                    ShowArrow();
                }
            }
        }
    }

    public void OnInit(LevelData levelData)
    {
        initPoint = initPoint < 0 ? TargetZPosition : initPoint; //Nếu đề ntn initPoint = initPoint < 0 ? tf.position.z : initPoint; thì sẽ lỗi khi Replay game 2 lần liên tục

        tf.position = Vector3.forward * -45f;

        InitHammer(levelData.hammerColor);
    }

    public void InitHammer(List<ColorType> colorTypes)
    {
        hammers.Clear();
        hammerPool.DespawnAll();

        Vector3 startPoint = (colorTypes.Count / 2 - ((colorTypes.Count + 1) % 2) * 0.5f) * this.space * Vector3.left + Vector3.right + tf.position;
        Vector3 space = this.space * Vector3.right;
        List<int> randomNum = new List<int>();
        while (true)
        {
            int index = Random.Range(0, colorTypes.Count);
            if (!randomNum.Contains(index))
            {
                randomNum.Add(index);
            }
            if (randomNum.Count == colorTypes.Count) break;
        }
        for (int i = 0; i < colorTypes.Count; i++)
        {
            Hammer hammer = hammerPool.Spawn(startPoint + space * i, Quaternion.identity);
            if( DataManager.Instance.gameData.currentLevel<=2|| 
                DataManager.Instance.gameData.currentLevel==4|| 
                DataManager.Instance.gameData.currentLevel==5|| 
                DataManager.Instance.gameData.currentLevel==7) hammer.SetColor(colorTypes[i]);  //Nếu là level 4, 5,7 thì sẽ ko random vị trí của Hammer
            else hammer.SetColor(colorTypes[randomNum[i]]);
            hammer.OnInit();
            hammer.ChangeHammerType((HammerType)DataManager.Instance.gameData.currentHammerTypeID);
            hammers.Add(hammer);
        }
        currentHammer = null;
    }
    public void SelectHammer(Hammer hammer)
    {
        if (hammer != currentHammer)
        {
            DeselectHammer();
            currentHammer = hammer;
            currentHammer.OnSelected();
        }
    }

    public void DeselectHammer()
    {
        currentHammer?.OnDeselected();
        currentHammer = null;
    }

    public void CollectCurrentHammer()
    {
        hammers.Remove(currentHammer);
        hammerPool.Despawn(currentHammer.gameObject);
        currentHammer = null;

        UpdateHammerPoint();
    }

    public void ShowArrow()
    {
        for (int i = 0; i < hammers.Count; i++)
        {
            hammers[i].ShowArrow();
        }
    }

    public void MoveToPlayGame()
    {
        StartCoroutine(IEMoveToPlayGame(Vector3.forward * initPoint));
        //Bắn Firebase: checkpoint_start
        if (DataManager.Instance.gameData.currentLevel != DataManager.Instance.gameData.lastLevel)   //Vì là checkPoint nên chỉ bắn 1 lần duy nhất là lần chơi đầu tiên
        {
            FirebaseManager.Instance.LogAnalyticsEvent(EventName.checkpoint_start_+ DataManager.Instance.gameData.currentLevel.ToString(), Parameter.level, DataManager.Instance.gameData.currentLevel.ToString());
            FirebaseManager.Instance.LogCheckPoint(EventName.g_checkpoint_start_ + ((DataManager.Instance.gameData.currentLevel > 9) ? DataManager.Instance.gameData.currentLevel.ToString() : ("0" + DataManager.Instance.gameData.currentLevel.ToString())), DataManager.Instance.gameData.currentLevel.ToString(), levelType.normal);
            FirebaseManager.Instance.LogAnalyticsEvent(EventName.g_level_start, Parameter.level, (DataManager.Instance.gameData.currentLevel).ToString());
            DataManager.Instance.gameData.lastLevel = DataManager.Instance.gameData.currentLevel;
            DataManager.Instance.gameData.levels_played++;
            DataManager.Instance.SaveGame();
        }
    }

    public void MoveToHideHammer()
    {
        StartCoroutine(IEMoveToPlayGame(new Vector3(0,0,-45f)));
    }

    private IEnumerator IEMoveToPlayGame(Vector3 target)
    {
        if (GameManager.Instance.IsEditor) target = new Vector3(0, 0, -22f);
        //Vector3 target = Vector3.forward * initPoint;

        while ((tf.position - target).sqrMagnitude > 0.01f)
        {
            tf.position = Vector3.MoveTowards(tf.position, target, Time.deltaTime * 25);
            yield return null;
        }

        for (int i = 0; i < hammers.Count; i++)
        {
            hammers[i].OnInit();
        }
        GameManager.Instance.gameState = GameState.GamePlay;
        UI_Game.Instance.GetUI<CanvasGamePlay>(UIID.GamePlay).SetBtnMoveHammers();
    }

    public void UpdateHammerPoint()
    {
        float spacee = 4f;
        int count = hammers.Count;
        //Vector3 startPoint = (count / 2 - ((count + 1) % 2) * 0.5f) * this.space * Vector3.left + Vector3.right + tf.position;
        //Vector3 space = this.space * Vector3.right;
        float startPoint = ((float)count - 1) / 2 * -1 * spacee + 1;
        for (int i = 0; i < count; i++)
        {
            hammers[i].Move(startPoint + spacee * i);
        }
        if(count == 8)
        {
            tf.DOMoveX(0, 0.5f);
            UI_Game.Instance.GetUI<CanvasGamePlay>(UIID.GamePlay).btnMoveLeft.SetActive(false);
            UI_Game.Instance.GetUI<CanvasGamePlay>(UIID.GamePlay).btnMoveRight.SetActive(false);
        }
    }
    public void SetHammerType(HammerType hammerType)
    {
        for (int i = 0; i < hammers.Count; i++)
        {
            hammers[i].ChangeHammerType(hammerType);
        }
        DataManager.Instance.gameData.currentHammerTypeID = (int)hammerType;
    }
}
