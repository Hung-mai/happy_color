using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class BrickManager : Singleton<BrickManager>
{
    [Header("----- change level effect 10 ------")]
    public AnimationCurve animationCurve1;
    public AnimationCurve animationCurve2;

    [Space(5)]
    Vector2 size = new Vector2(32, 32);
    public Transform tf;
    public Brick prefab;

    //total brick
    public List<Brick> totalBricks = new List<Brick>();

    //convert id position to brick
    private Dictionary<Vector2Int, Brick> brickMap = new Dictionary<Vector2Int, Brick>();

    //searcj closest support
    KdTree<Brick> kdTree = new KdTree<Brick>();

    //list data for game
    public List<BrickData> brickDatas = new List<BrickData>();

    internal BrickData currentBrickData;

    public ColorType ColorType => currentBrickData.colorType;

    public List<BrickData> BrickDatas => brickDatas;

    public ColorData colorData;

    public UnityAction endEffectAction = null;

    private float timeCounting = 0;
    // Start is called before the first frame update

    // tạo ra mảng 2 chiều chứa các cột và hàng
    // private Brick[,] brickArray = new Brick[32,32];
    public List<List<Brick>> brickList = new List<List<Brick>>();
    public List<List<Brick>> brickListRow = new List<List<Brick>>();
    GameObject[] newGameObject = new GameObject[32];

    void Awake()
    {
        for (int i = 0; i < size.y; i++)
        {
            for (int j = 0; j < size.x; j++)
            {
                Brick brick = Instantiate(prefab, tf.position + Vector3.right * (j - size.x * 0.5f + 0.5f) + Vector3.forward * (i - size.y * 0.5f + 0.5f), Quaternion.identity, tf);
                brick.SetID(new Vector2Int(j, i));
                brickMap.Add(brick.ID, brick);
                totalBricks.Add(brick);   
            }
        }
        kdTree.AddAll(totalBricks);
        SetBrickType((BrickType)DataManager.Instance.gameData.currentBrickTypeID);


        // gán vào list 2 chiều
        for (int i = 0; i < size.x; i++)
        {
            List<Brick> column = new List<Brick>();
            for (int j = 0; j < size.y; j++)
            {
                column.Add(totalBricks[j * 32 + i]);
            }
            brickList.Add(column);
        }
        // gán vào list 2 chiều
        for (int i = 0; i < size.x; i++)
        {
            List<Brick> row = new List<Brick>();
            for (int j = 0; j < size.y; j++)
            {
                row.Add(totalBricks[i * 32 + j]);
            }
            brickListRow.Add(row);
        }
        for (int i = 0; i < 32; i++)
        {
            newGameObject[i] = new GameObject();
        }
    }
    private void Update()
    {
        timeCounting += Time.deltaTime;
        /*if (Input.GetKeyDown(KeyCode.A))
        {
            SetBrickType(BrickType.NormalCube);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            SetBrickType(BrickType.LegoCube);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            SetBrickType(BrickType.ConNhong);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            SetBrickType(BrickType.LegoLoi);
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            SetBrickType(BrickType.LegoLom);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(PlayEffect(Random.Range(1, 11)));
        }*/
    }
    public void OnInit()
    {
        
    }

    public void OnReset()
    {
        for (int i = 0; i < totalBricks.Count; i++)
        {
            totalBricks[i].OnInit();
        }
        brickDatas.Clear();
    }

    //remove
    public void NewColor(ColorType colorType)
    {
        currentBrickData = new BrickData();
        currentBrickData.colorType = colorType;
        brickDatas.Add(currentBrickData);
    }
    
    public void ChangeBrickData(BrickData brickData, ColorType colorType)
    {
        currentBrickData = brickData;
        currentBrickData.colorType = colorType;
    }

    public Brick SearchBrickClosest(Vector3 position)
    {
        if (Vector3.Distance(kdTree.FindClosest(position).transform.position, position) < 1f) return kdTree.FindClosest(position);
        else return null;
    }  
    
    public Brick GetRandomBrick()
    {
        return GetBrickMap(brickDatas[0].listBricksID[Random.Range(0, brickDatas[0].listBricksID.Count)]);
    }

    public Brick GetBrickMap(Vector2Int id)
    {
        return brickMap[id];
    }

    public void SetData(LevelData levelData)
    {
        for (int i = 0; i < levelData.brickDatas.Count; i++)
        {
            //chia list
            BrickData brickData = new BrickData();

            switch (levelData.gameMode)
            {
                case GameMode.Normal:
                    brickData.colorType = levelData.brickDatas[i].colorType;
                    break;

                case GameMode.Paint:
                    brickData.colorType = colorData.GetGreyColorType(levelData.brickDatas[i].colorType);
                    break;
                default:
                    break;
            }


            brickData.listBricksID.AddRange(levelData.brickDatas[i].listBricksID);

            //change mau
            for (int j = 0; j < brickData.listBricksID.Count; j++)
            {
                GetBrickMap(brickData.listBricksID[j]).SetParent(brickData);
                GetBrickMap(brickData.listBricksID[j]).ChangeColor(brickData.colorType);
            }

            brickDatas.Add(brickData);
        }
    }

    public void SetBrickType(BrickType brickType)
    {
        for (int i = 0; i < totalBricks.Count; i++)
        {
            totalBricks[i].ChangeBrickType(brickType);
        }
        DataManager.Instance.gameData.currentBrickTypeID = (int)brickType;
        //LevelManager.Instance.OnReplay();
    }

    public bool IsFinishGame()
    {
        bool isFinishGame = true;

        for (int i = 0; i < brickDatas.Count - 1; i++)
        {
            if (brickDatas[i].colorType != brickDatas[i + 1].colorType)
            {
                isFinishGame = false;
                break;
            }
        }
        return isFinishGame;
    }

    #region Check Merge

    public void MergeBrickGroup()
    {
        HashSet<Vector2Int> checkFlag = new HashSet<Vector2Int>();

        //init flag
        for (int i = 0; i < currentBrickData.listBricksID.Count; i++)
        {
            checkFlag.Add(currentBrickData.listBricksID[i]);
        }

        List<BrickData> brickDatas = new List<BrickData>();

        for (int i = 0; i < currentBrickData.listBricksID.Count; i++)
        {
            List<Brick> bricksOutline = GetNeighborBricks(currentBrickData.listBricksID[i], ref checkFlag);

            for (int j = 0; j < bricksOutline.Count; j++)
            {
                if (bricksOutline[j].ColorType == ColorType && !brickDatas.Contains(bricksOutline[j].brickData))
                {
                    //flag group
                    brickDatas.Add(bricksOutline[j].brickData);
                }
            }
        }

        brickDatas.Add(currentBrickData);

        //merge group
        BrickData newBrickData = new BrickData();
        newBrickData.colorType = ColorType;

        for (int i = 0; i < brickDatas.Count; i++)
        {
            this.brickDatas.Remove(brickDatas[i]);
            newBrickData.listBricksID.AddRange(brickDatas[i].listBricksID);
        }

        for (int i = 0; i < newBrickData.listBricksID.Count; i++)
        {
            brickMap[newBrickData.listBricksID[i]].SetParent(newBrickData);
        }

        //clear list empty
        //for (int i = this.brickDatas.Count - 1; i >= 0; i--)
        //{
        //    if (this.brickDatas[i].listBricksID.Count == 0)
        //    {
        //        this.brickDatas.RemoveAt(i);
        //    }
        //}

        this.brickDatas.Remove(currentBrickData);
        this.brickDatas.Add(newBrickData);
    }


    private List<Brick> GetNeighborBricks(Vector2Int index, ref HashSet<Vector2Int> flag)
    {
        List<Brick> bricks = new List<Brick>();

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (Mathf.Abs(i) + Mathf.Abs(j) == 2) continue;
                Vector2Int point = new Vector2Int(index.x + j, index.y + i);

                if (flag.Contains(point))
                {
                    continue;
                }
                else
                {
                    flag.Add(point);
                    if (brickMap.ContainsKey(point))
                    {
                        bricks.Add(brickMap[point]);
                    }
                }
            }
        }

        return bricks;
    }

    #endregion
    #region Change Level Effect
    public void ChangeLevelEffect() 
    {
        tf.DOMoveY(-1.3f, 0.5f).OnComplete(() => 
        {
            for (int i = 0; i < totalBricks.Count; i++)
            {
                totalBricks[i].stage.SetActive(false);
            }
            StartCoroutine(PlayEffect(DataManager.Instance.gameData.currentLevel==2?10:Random.Range(1, 11)));   //Effect 11 bị lỗi 
        });
    }

    IEnumerator PlayEffect(int index)
    {
        Time.timeScale = 1.20f;
        Debug.LogError("run " + index);
        switch (index)
        {
            case 1: yield return RunEffect1();
                break;
            case 2: yield return RunEffect10();
                break;
            case 3: yield return RunEffect3();
                break;
            case 4: yield return RunEffect4();
                break;
            case 5: yield return RunEffect5();
                break;
            case 6: yield return RunEffect6();
                break;
            case 7: yield return RunEffect7();
                break;
            case 8: yield return RunEffect10();
                break;
            case 9: yield return RunEffect9();
                break;
            case 10: yield return RunEffect10();
                break;
            case 11: yield return RunEffect11();
                break;
            default:
                Debug.LogError("lỗi index chỗ chọn  run Effect");
                break;
        }

        int indexEnd;
        if(index == 3)
        {
            indexEnd = 3;
        }
        else
        {
            do
            {
                indexEnd = DataManager.Instance.gameData.currentLevel == 2 ? 10 : Random.Range(1, 11);
            }
            while (indexEnd == 3);
        }
        Debug.LogError("end " + indexEnd);
        switch (indexEnd)
        {
            case 1: yield return EndEffect1();
                break;
            case 2: yield return EndEffect10();
                break;
            case 3: yield return EndEffect3();
                break;
            case 4: yield return EndEffect4();
                break;
            case 5: yield return EndEffect5();
                break;
            case 6: yield return EndEffect6();
                break;
            case 7: yield return EndEffect7();
                break;
            case 8: yield return EndEffect10();
                break;
            case 9: yield return EndEffect9();
                break;
            case 10: yield return EndEffect10();
                break;
            case 11: yield return EndEffect11();
                break;
            default:
                Debug.LogError("lỗi index chỗ chọn end Effect");
                break;
        }
        Time.timeScale = 1;
        EndChangeLevelEffect();
    }

    
    #region Funtion effect 1
    private IEnumerator RunEffect1()
    {
        for (int i = 0; i < totalBricks.Count; i++)
        {
            float a = Vector3.Distance(totalBricks[i].child.position, new Vector3(30f, 0, 30f));
            Vector3 tempPos = new Vector3(30f, 0, 30f);
            if (a > Vector3.Distance(totalBricks[i].child.position, new Vector3(-30f, 0, 30f)))
            {
                a = Vector3.Distance(totalBricks[i].child.position, new Vector3(-30f, 0, 30f));
                tempPos = new Vector3(-30f, 0, 30f);
            }
            if (a > Vector3.Distance(totalBricks[i].child.position, new Vector3(30f, 0, -30f)))
            {
                a = Vector3.Distance(totalBricks[i].child.position, new Vector3(30f, 0, -30f));
                tempPos = new Vector3(30f, 0, -30f);
            }
            if (a > Vector3.Distance(totalBricks[i].child.position, new Vector3(-30f, 0, -30f)))
            {
                a = Vector3.Distance(totalBricks[i].child.position, new Vector3(-30f, 0, -30f));
                tempPos = new Vector3(-30f, 0, -30f);
            }

            if (a > Vector3.Distance(totalBricks[i].child.position, new Vector3(43f, 0, 0f)))
            {
                a = Vector3.Distance(totalBricks[i].child.position, new Vector3(43f, 0, 0f));
                tempPos = new Vector3(43f, 0, 0f);
            }
            if (a > Vector3.Distance(totalBricks[i].child.position, new Vector3(-43f, 0, 0f)))
            {
                a = Vector3.Distance(totalBricks[i].child.position, new Vector3(-43f, 0, 0f));
                tempPos = new Vector3(-43f, 0, 0f);
            }
            if (a > Vector3.Distance(totalBricks[i].child.position, new Vector3(0f, 0, -43f)))
            {
                a = Vector3.Distance(totalBricks[i].child.position, new Vector3(0f, 0, -43f));
                tempPos = new Vector3(0f, 0, -43f);
            }
            totalBricks[i].child.DOJump(tempPos, Random.Range(2f, 10f), Random.Range(3, 6), Random.Range(1.5f, 3f));
            totalBricks[i].child.DORotate(new Vector3(0, Random.Range(-180f, 180f), 0), Random.Range(1.5f, 3f));
        }

        yield return Cache.GetWFS(2f);
        for (int i = 0; i < totalBricks.Count; i++)
        {
            totalBricks[i].child.localEulerAngles = Vector3.zero;
        }
    }

    private IEnumerator EndEffect1()
    {
        DOTween.KillAll();

        LevelManager.Instance.OnLoadNextLevelEffect();
        tf.position = new Vector3(tf.position.x, -1.5f, tf.position.z);
        for (int i = 0; i < totalBricks.Count; i++)
        {
            totalBricks[i].child.DOLocalJump(Vector3.zero, Random.Range(2f, 10f), Random.Range(3, 6), Random.Range(1.5f, 3f));
            totalBricks[i].child.DORotate(new Vector3(0, 0, 0), Random.Range(1f, 3f));
        }
        yield return Cache.GetWFS(3f);
    }
    
    #endregion

    #region Funtion effect 2
    private IEnumerator RunEffect2()
    {
        for (int i = 0; i < totalBricks.Count; i++)
        {
            float a = Vector3.Distance(totalBricks[i].child.position, new Vector3(30f, 0, 30f));    //Biến a dùng làm mốc để so sánh độ dài. Mục tiêu là tìm được điểm gần nhất trong các điểm dưới đây
            Vector3 tempPos = new Vector3(30f, 0, 30f);
            if (a > Vector3.Distance(totalBricks[i].child.position, new Vector3(-30f, 0, 30f)))
            {
                a = Vector3.Distance(totalBricks[i].child.position, new Vector3(-30f, 0, 30f));
                tempPos = new Vector3(-30f, 0, 30f);
            }
            if (a > Vector3.Distance(totalBricks[i].child.position, new Vector3(30f, 0, -30f)))
            {
                a = Vector3.Distance(totalBricks[i].child.position, new Vector3(30f, 0, -30f));
                tempPos = new Vector3(30f, 0, -30f);
            }
            if (a > Vector3.Distance(totalBricks[i].child.position, new Vector3(-30f, 0, -30f)))
            {
                a = Vector3.Distance(totalBricks[i].child.position, new Vector3(-30f, 0, -30f));
                tempPos = new Vector3(-30f, 0, -30f);
            }

            if (a > Vector3.Distance(totalBricks[i].child.position, new Vector3(43f, 0, 0f)))
            {
                a = Vector3.Distance(totalBricks[i].child.position, new Vector3(43f, 0, 0f));
                tempPos = new Vector3(43f, 0, 0f);
            }
            if (a > Vector3.Distance(totalBricks[i].child.position, new Vector3(-43f, 0, 0f)))
            {
                a = Vector3.Distance(totalBricks[i].child.position, new Vector3(-43f, 0, 0f));
                tempPos = new Vector3(-43f, 0, 0f);
            }
            if (a > Vector3.Distance(totalBricks[i].child.position, new Vector3(0f, 0, -43f)))
            {
                a = Vector3.Distance(totalBricks[i].child.position, new Vector3(0f, 0, -43f));
                tempPos = new Vector3(0f, 0, -43f);
            }
            totalBricks[i].child.DOMove(tempPos, Random.Range(1f, 2f));
            totalBricks[i].child.DORotate(new Vector3(0, Random.Range(-180f, 180f), 0), Random.Range(1f, 2f));
        }
        yield return Cache.GetWFS(1.5f);
        for (int i = 0; i < totalBricks.Count; i++)
        {
            totalBricks[i].child.localEulerAngles = Vector3.zero;
        }
    }

    private IEnumerator EndEffect2()
    {
        DOTween.KillAll();

        LevelManager.Instance.OnLoadNextLevelEffect();
        tf.position = new Vector3(tf.position.x, -1.5f, tf.position.z);
        for (int i = 0; i < totalBricks.Count; i++)
        {
            totalBricks[i].child.DOLocalMove(Vector3.zero, Random.Range(2f, 3f));
            totalBricks[i].child.DORotate(new Vector3(0, 0, 0), Random.Range(1f, 3f));
        }
        yield return Cache.GetWFS(3f);
    }
    
    #endregion

    #region Funtion effect 3
    private IEnumerator RunEffect3()
    {
        for (int i = 0; i < totalBricks.Count; i++)
        {
            totalBricks[i].mainCube.DOLocalJump(new Vector3(totalBricks[i].tf.localPosition.x + Random.Range(-5f, 5f), Random.Range(45f,60f), totalBricks[i].tf.localPosition.z + Random.Range(-5f, 5f)), Random.Range(2f, 10f), 1, Random.Range(1f, 2f));
            totalBricks[i].SetPhysics(true);
        }
        yield return Cache.GetWFS(1f);
    }

    private IEnumerator EndEffect3()
    {
        // DOTween.KillAll();
        LevelManager.Instance.OnLoadNextLevelEffect();
        for (int i = 0; i < totalBricks.Count; i++)
        {
            totalBricks[i].mainCube.position = new Vector3(totalBricks[i].tf.localPosition.x + Random.Range(-5f, 5f), Random.Range(45f, 60f), totalBricks[i].tf.localPosition.z + Random.Range(-5f, 5f));
            totalBricks[i].SetPhysics(true);
        }
        tf.position = new Vector3(tf.position.x, -1.5f, tf.position.z);
        for (int i = 0; i < totalBricks.Count; i++)
        {
            totalBricks[i].stage.SetActive(false);
            totalBricks[i].rigidbody.velocity = new Vector3(0f,-60f,0f);
        }
        
        yield return Cache.GetWFS(4f);
        for (int i = 0; i < totalBricks.Count; i++)
        {
            totalBricks[i].SetPhysics(false);
            totalBricks[i].rigidbody.velocity = Vector3.zero;
            totalBricks[i].rigidbody.angularVelocity = Vector3.zero;
        }
        for (int i = 0; i < totalBricks.Count; i++)
        {
            totalBricks[i].mainCube.DOLocalJump(Vector3.zero, Random.Range(2f,5f), Random.Range(1, 4), Random.Range(1f, 2f));
            if( DataManager.Instance.gameData.currentBrickTypeID != 0 
                && DataManager.Instance.gameData.currentBrickTypeID != 2
                && DataManager.Instance.gameData.currentBrickTypeID != 4) totalBricks[i].mainCube.DOLocalRotate(new Vector3(0,0,0), Random.Range(1f, 2f));
            else totalBricks[i].mainCube.DOLocalRotate(new Vector3(-90f, 0, 0), Random.Range(1f, 2f));
        }
        yield return Cache.GetWFS(1f);

        yield return Cache.GetWFS(1f);
        for (int i = 0; i < totalBricks.Count; i++)
        {
            if (totalBricks[i].ColorType != ColorType.Clear) totalBricks[i].stage.SetActive(true);
        }
        yield return Cache.GetWFS(0.5f);
    }
    
    #endregion

    #region Funtion effect 4
    private IEnumerator RunEffect4()
    {
        StartCoroutine(RunEffect4_1());
        StartCoroutine(RunEffect4_2());

        yield return new WaitUntil(() => isEffect5Done == true);
    }

    private bool isEffect5Done = false;
    private IEnumerator RunEffect4_1()
    {
        isEffect5Done = false;
        for (int i = 0; i < GetLevelData(2).mapSize.x / 2; i++)  // 27 thì 0 đến 12  13 đến hết
        {
            for (int j = 0; j < 32; j++)
            {
                Transform brickTrans = brickList[i][j].child;
                brickTrans.DOLocalMove(Vector3.left * 20, 2).SetEase(Ease.OutQuad);
            }
            yield return Cache.GetWFS(0.05f);
        }
        yield return Cache.GetWFS(1.5f);
        isEffect5Done = true;
    }
    private IEnumerator RunEffect4_2()
    {
        for (int i = 31; i >= GetLevelData(2).mapSize.x / 2; i--)  // 27 thì 0 đến 12  13 đến hết
        {
            for (int j = 0; j < 32; j++)
            {
                Transform brickTrans = brickList[i][j].child;
                brickTrans.DOLocalMove(Vector3.right * 20, 2).SetEase(Ease.OutQuad);
            }

            if(i < GetLevelData(2).mapSize.x)
            {
                yield return Cache.GetWFS(0.05f);
            }   
        }
    }

    private IEnumerator EndEffect4()
    {
        DOTween.KillAll();

        LevelManager.Instance.OnLoadNextLevelEffect(); // chuyển màu vào đây
        tf.position = new Vector3(tf.position.x, -1.5f, tf.position.z);


        isEffect5Done = false;
        for (int i = 0; i < GetLevelData(1).mapSize.x / 2; i++)  // 27 thì 0 đến 12  13 đến hết
        {
            for (int j = 0; j < 32; j++)
            {
                Transform brickTrans = brickList[i][j].child;
                brickTrans.localPosition = Vector3.left * 20;
            }
        }

        for (int i = 31; i >= GetLevelData(1).mapSize.x / 2; i--)  // 27 thì 0 đến 12  13 đến hết
        {
            for (int j = 0; j < 32; j++)
            {
                Transform brickTrans = brickList[i][j].child;
                brickTrans.localPosition = Vector3.right * 20;
            } 
        }

        StartCoroutine(EndEffect4_2());
        yield return EndEffect4_1();

        yield return Cache.GetWFS(2.1f);
        DOTween.KillAll();
    }

    private IEnumerator EndEffect4_1()
    {
        for (int i = GetLevelData(1).mapSize.x / 2 - 1; i >= 0; i--)
        {
            for (int j = 0; j < 32; j++)
            {
                Transform brickTrans = brickList[i][j].child;
                brickTrans.DOLocalMove(Vector3.zero, 2).SetEase(Ease.InQuad);
            }
            yield return Cache.GetWFS(0.05f);
        }
        isEffect5Done = true;
    }
    private IEnumerator EndEffect4_2()
    {
        for (int i = GetLevelData(1).mapSize.x / 2; i < 32; i++)
        {
            for (int j = 0; j < 32; j++)
            {
                Transform brickTrans = brickList[i][j].child;
                brickTrans.DOLocalMove(Vector3.zero, 2).SetEase(Ease.InQuad);
            }

            if(i < GetLevelData(1).mapSize.x)
            {
                yield return Cache.GetWFS(0.05f);
            }   
        }
    }

    #endregion

    #region Funtion effect 5

    private IEnumerator RunEffect5()
    {
        StartCoroutine(RunEffect5_1());
        StartCoroutine(RunEffect5_2());

        yield return Cache.GetWFS(2);
    }

    private IEnumerator RunEffect5_1()
    {
        int dem = GetLevelData(2).mapSize.x / 2;
        isEffect5Done = false;
        for (int i = 0; i < GetLevelData(2).mapSize.x / 2; i++)  // 27 thì 0 đến 12  13 đến hết
        {
            for (int j = 0; j < 32; j++)
            {
                Transform brickTrans = brickList[i][j].child;
                brickTrans.DOLocalMoveX(-20, 2).SetEase(Ease.OutQuad);
                brickTrans.DOLocalMoveY(10 + dem, 2).SetEase(Ease.OutQuad);
            }
            yield return Cache.GetWFS(0.05f);
            dem--;
        }
        yield return Cache.GetWFS(1.5f);
        isEffect5Done = true;
    }
    private IEnumerator RunEffect5_2()
    {
        int dem = GetLevelData(2).mapSize.x / 2;
        for (int i = 31; i >= GetLevelData(2).mapSize.x / 2; i--)  // 27 thì 0 đến 12  13 đến hết
        {
            for (int j = 0; j < 32; j++)
            {
                Transform brickTrans = brickList[i][j].child;
                brickTrans.DOLocalMoveX(20, 2).SetEase(Ease.OutQuad);
                brickTrans.DOLocalMoveY(10 + dem, 2).SetEase(Ease.OutQuad);
            }

            if(i < GetLevelData(2).mapSize.x)
            {
                yield return Cache.GetWFS(0.05f);
                dem--;
            }   
        }
    }

    private IEnumerator EndEffect5()
    {
        DOTween.KillAll();
        LevelManager.Instance.OnLoadNextLevelEffect(); // chuyển màu vào đây
        tf.position = new Vector3(tf.position.x, -1.5f, tf.position.z);


        int dem = GetLevelData(1).mapSize.x / 2;
        isEffect5Done = false;
        for (int i = 0; i < GetLevelData(1).mapSize.x / 2; i++)  // 27 thì 0 đến 12  13 đến hết
        {
            for (int j = 0; j < 32; j++)
            {
                Transform brickTrans = brickList[i][j].child;
                brickTrans.localPosition = Vector3.left * 20 + Vector3.up * (10 + dem);
            }
            dem--;
        }

        dem = GetLevelData(1).mapSize.x / 2;
        int sizeX = GetLevelData(1).mapSize.x;
        for (int i = sizeX - 1; i >= sizeX / 2; i--)  // 27 thì 0 đến 12  13 đến hết
        {
            for (int j = 0; j < 32; j++)
            {
                Transform brickTrans = brickList[i][j].child;
                brickTrans.localPosition = Vector3.right * 20 + Vector3.up * (10 + dem);
            }
            dem--;
        }

        StartCoroutine(EndEffect5_1());
        StartCoroutine(EndEffect5_2());
        yield return Cache.GetWFS(2.5f);
    }

    private IEnumerator EndEffect5_1()
    {
        for (int i = GetLevelData(1).mapSize.x / 2 - 1; i >= 0; i--)
        {
            for (int j = 0; j < 32; j++)
            {
                Transform brickTrans = brickList[i][j].child;
                brickTrans.DOLocalMove(Vector3.zero, 2).SetEase(Ease.InQuad);
            }
            yield return Cache.GetWFS(0.05f);
        }
        yield return Cache.GetWFS(2f);
        isEffect5Done = true;
    }
    private IEnumerator EndEffect5_2()
    {
        for (int i = GetLevelData(1).mapSize.x / 2; i < 32; i++)
        {
            for (int j = 0; j < 32; j++)
            {
                Transform brickTrans = brickList[i][j].child;
                brickTrans.DOLocalMove(Vector3.zero, 2).SetEase(Ease.InQuad);
            }

            if(i < GetLevelData(1).mapSize.x)
            {
                yield return Cache.GetWFS(0.05f);
            }   
        }
    }
    #endregion

    #region Funtion Effect 6

    private IEnumerator RunEffect6()
    {
        ChangeTransformParent(true);
        for (int i = 0; i < 32; i++)
        {
            newGameObject[i].transform.DOJump(new Vector3(-40, 0, newGameObject[i].transform.position.z), 8, 6, 4);

            yield return Cache.GetWFS(0.01f);
        }
        yield return Cache.GetWFS(2f);
        BackTransformParent();
        DOTween.KillAll();
    }
    private IEnumerator EndEffect6()
    {
        DOTween.KillAll();
        LevelManager.Instance.OnLoadNextLevelEffect(); // chuyển màu vào đây
        tf.position = new Vector3(tf.position.x, -1.5f, tf.position.z);

        for (int i = 0; i < 32; i++)
        {
            for (int j = 0; j < 32; j++)
            {
                brickList[i][j].child.localPosition =  new Vector3(35, 0, 0);
            }
        }

        ChangeTransformParent(true);
        for (int i = 0; i < 32; i++)
        {
            newGameObject[i].transform.DOLocalJump(new Vector3(brickList[i][0].tf.position.x, 0, 0), 8, 5, 3).SetEase(Ease.InOutSine);

            yield return Cache.GetWFS(0.05f);
        }
        yield return Cache.GetWFS(2.95f);
        BackTransformParent();
        DOTween.KillAll();
    }
    #endregion
    
    #region Funtion Effect 7

    private IEnumerator RunEffect7()
    {
        ChangeTransformParent(true);
        for (int i = 0; i < 32; i++)
        {
            newGameObject[i].transform.DOLocalMoveY(6, 0.2f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
            newGameObject[i].transform.DOLocalMoveX(-50, 3.5f).SetEase(Ease.Linear);

            yield return Cache.GetWFS(0.02f);
        }
        yield return Cache.GetWFS(2f);
        BackTransformParent();
    }
    private IEnumerator EndEffect7()
    {
        DOTween.KillAll();
        LevelManager.Instance.OnLoadNextLevelEffect(); // chuyển màu vào đây
        tf.position = new Vector3(tf.position.x, -1.5f, tf.position.z);

        for (int i = 0; i < 32; i++)
        {
            for (int j = 0; j < 32; j++)
            {
                brickList[i][j].child.localPosition =  new Vector3(40, 0, 0);
            }
        }

        ChangeTransformParent(true);  // theo chiều dọc
        for (int i = 0; i < 32; i++)
        {
            newGameObject[i].transform.DOLocalMoveY(6, 0.2f).SetLoops(20, LoopType.Yoyo).SetEase(Ease.InOutSine);
            newGameObject[i].transform.DOLocalMoveX(brickList[i][0].tf.position.x, 4).SetEase(Ease.Linear);

            yield return Cache.GetWFS(0.02f);
        }
        yield return Cache.GetWFS(3.98f);
        BackTransformParent();
        DOTween.KillAll();
    }
    #endregion
    
    #region Funtion Effect 8

    private IEnumerator RunEffect8()
    {
        ChangeTransformParent(false);  // theo chiều ngang
        for (int i = 0; i < 32; i++)
        {
            newGameObject[i].transform.DOLocalMoveY(6, 0.2f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
            newGameObject[i].transform.DOLocalMoveX(-50, 4).SetEase(Ease.Linear);
            yield return Cache.GetWFS(0.02f);
        }
        yield return Cache.GetWFS(3f);
        BackTransformParent();
    }
    private IEnumerator EndEffect8()
    {
        DOTween.KillAll();
        LevelManager.Instance.OnLoadNextLevelEffect(); // chuyển màu vào đây
        tf.position = new Vector3(tf.position.x, -1.5f, tf.position.z);

        for (int i = 0; i < 32; i++)
        {
            for (int j = 0; j < 32; j++)
            {
                brickList[i][j].child.localPosition =  new Vector3(40, 0, 0);
            }
        }

        ChangeTransformParent(false);  // theo chiều ngang
        for (int i = 0; i < 32; i++)
        {
            newGameObject[i].transform.DOLocalMoveY(6, 0.2f).SetLoops(16, LoopType.Yoyo).SetEase(Ease.InOutSine);
            newGameObject[i].transform.DOLocalMoveX(-40 * GetLevelData(1).mapScale.x, 3.2f).SetEase(Ease.Linear);

            yield return Cache.GetWFS(0.02f);
        }
        yield return Cache.GetWFS(3.18f);
        BackTransformParent();
        DOTween.KillAll();
    }
    #endregion
    
    #region Funtion Effect 9
    private IEnumerator RunEffect9()
    {
        ChangeTransformParent(true);
        StartCoroutine(RunEffect9_1());
        StartCoroutine(RunEffect9_2());
        yield return Cache.GetWFS(3f);
        BackTransformParent();
    }
    private IEnumerator RunEffect9_1()
    {
        Vector3[] path = {
            new Vector3(0, 1, 0),
            new Vector3(11, 11, 0),
            new Vector3(0, 22, 0),
            new Vector3(-11, 11, 0),
            new Vector3(0, 2, 0),
            new Vector3(30, 0, 0)
        };
        for (int i = GetLevelData(2).mapSize.x / 2 - 1; i >= 0; i--)  // 27 thì 0 đến 12  13 đến hết
        {
            newGameObject[i].transform.DOPath(path, 3, PathType.CatmullRom);
            yield return Cache.GetWFS(0.02f);
        }
    }
    private IEnumerator RunEffect9_2()
    {
        Vector3[] path = {
            new Vector3(0, 1, 0),
            new Vector3(-11, 11, 0),
            new Vector3(0, 22, 0),
            new Vector3(11, 11, 0),
            new Vector3(0, 2, 0),
            new Vector3(-30, 0, 0)
        };
        for (int i = GetLevelData(2).mapSize.x / 2; i < GetLevelData(2).mapSize.x; i++)  // 27 thì 0 đến 12  13 đến hết
        {
            newGameObject[i].transform.DOPath(path, 3, PathType.CatmullRom);
            if(i < GetLevelData(2).mapSize.x)
            {
                yield return Cache.GetWFS(0.02f);
            }  
        }
    }
    private IEnumerator EndEffect9()
    {
        DOTween.KillAll();
        LevelManager.Instance.OnLoadNextLevelEffect(); // chuyển màu vào đây
        tf.position = new Vector3(tf.position.x, -1.5f, tf.position.z);

        for (int i = 0; i < 32; i++)
        {
            for (int j = 0; j < 32; j++)
            {
                brickList[i][j].child.localPosition =  new Vector3(40, 0, 0);
            }
        }

        ChangeTransformParent(true);
        for (int i = 0; i < 32; i++)
        {
            newGameObject[i].transform.DOLocalMoveY(10, 2f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.InOutSine);
            newGameObject[i].transform.DOLocalMoveX(brickList[i][0].tf.position.x, 4).SetEase(animationCurve2);

            yield return Cache.GetWFS(0.02f);
        }
        yield return Cache.GetWFS(3.98f);
        BackTransformParent();
        DOTween.KillAll();
    }
    #endregion
    
    #region Funtion Effect 10
    float range = 3;
    private IEnumerator RunEffect10()
    {
        range = 3;
        int xx = GetLevelData(2).mapSize.x / 2;
        int yy = GetLevelData(2).mapSize.y / 2;
        newGameObject[0].transform.position = BrickManager.Instance.GetBrickMap(new Vector2Int(xx, yy)).tf.position;
        newGameObject[0].transform.DOMoveY(260, 5);
        newGameObject[0].transform.DORotate(Vector3.up * 360*4, 4, RotateMode.FastBeyond360);

        List<Vector2Int> waves = new List<Vector2Int>();
        for (int i = 0; i < totalBricks.Count; i++)
        {
            waves.Add(totalBricks[i].id);
        }

        while (waves.Count > 0)
        {
            for (int i = 0; i < waves.Count; i++)
            {
                if ((BrickManager.Instance.GetBrickMap(waves[i]).tf.position - BrickManager.Instance.GetBrickMap(new Vector2Int(xx, yy)).tf.position).sqrMagnitude <= range)
                {
                    Brick brick2 = BrickManager.Instance.GetBrickMap(waves[i]);
                    if(brick2.IsClear == false)
                    {
                        brick2.child.parent =  newGameObject[0].transform;
                        Vector2 newPos = new Vector2(brick2.child.localPosition.x, brick2.child.localPosition.z).normalized*2;
                        brick2.child.DOLocalMove(new Vector3(newPos.x , brick2.child.localPosition.y, newPos.y), 0.5f);
                    }
                    waves.RemoveAt(i);
                    i--;
                }
            }
            range += 8f;
            yield return null;
        }

        yield return Cache.GetWFS(0f);
        BackTransformParent();
        for (int i = 0; i < totalBricks.Count; i++)
        {
            totalBricks[i].child.localPosition = Vector3.up * 100;
            totalBricks[i].child.localEulerAngles = Vector3.zero;
        }
        yield return Cache.GetWFS(0.1f);
    }

    private IEnumerator EndEffect10()
    {
        DOTween.KillAll();
        LevelManager.Instance.OnLoadNextLevelEffect(); // chuyển màu vào đây
        tf.position = new Vector3(tf.position.x, -1.5f, tf.position.z);

        for (int i = 0; i < totalBricks.Count; i++)
        {
            totalBricks[i].child.localPosition = Vector3.up * 100;
            totalBricks[i].child.localEulerAngles = Vector3.zero;
        }

        range = 500;
        int xx = GetLevelData(1).mapSize.x / 2;
        int yy = GetLevelData(1).mapSize.y / 2;
        newGameObject[0].transform.position = BrickManager.Instance.GetBrickMap(new Vector2Int(xx, yy)).tf.position;
        
        newGameObject[0].transform.eulerAngles = Vector3.zero;
        newGameObject[0].transform.DORotate(Vector3.up * 360 * 4, 4, RotateMode.FastBeyond360);

        List<Vector2Int> waves = new List<Vector2Int>();
        for (int i = 0; i < totalBricks.Count; i++)
        {
            if(totalBricks[i].IsClear == false)
            {
                waves.Add(totalBricks[i].id);
            }
            else
            {
                totalBricks[i].child.localPosition = Vector3.zero;
            }
        }

        // gán cho thg cha để nó quay thôi
        for (int i = 0; i < waves.Count; i++)
        {
            Brick brick2 = BrickManager.Instance.GetBrickMap(waves[i]);
            brick2.child.parent =  newGameObject[0].transform;
        }

        while (waves.Count > 0)
        {
            for (int i = 0; i < waves.Count; i++)
            {
                if ((BrickManager.Instance.GetBrickMap(waves[i]).tf.position - BrickManager.Instance.GetBrickMap(new Vector2Int(xx, yy)).tf.position).sqrMagnitude >= range)
                {
                    Brick brick2 = BrickManager.Instance.GetBrickMap(waves[i]);
                    Vector2 newPos = new Vector2(brick2.child.localPosition.x, brick2.child.localPosition.z).normalized*2;
                    brick2.child.localPosition = new Vector3(newPos.x , brick2.child.localPosition.y, newPos.y);
                    waves.RemoveAt(i);
                    i--;
                    Tween tt = brick2.child.DOMoveY(brick2.tf.position.y + 10, 1)
                    .OnComplete(() => {
                        brick2.child.parent =  brick2.tf;
                        brick2.child.DOLocalRotate(Vector3.zero, 1);
                        brick2.child.DOLocalMove(Vector3.zero, 1);
                    });

                }
            }
            range -= 5f;
            yield return null;
        }

        yield return Cache.GetWFS(2.8f);
        DOTween.KillAll();
    }

    #endregion

    #region Funtion Effect 11
    private IEnumerator RunEffect11()
    {
        range = 3;
        int xx = GetLevelData(2).mapSize.x / 2;
        int yy = GetLevelData(2).mapSize.y / 2;
        newGameObject[0].transform.position = BrickManager.Instance.GetBrickMap(new Vector2Int(xx, yy)).tf.position + Vector3.up * 25;
       
        newGameObject[0].transform.DORotate(Vector3.up * 360 * 5, 5, RotateMode.FastBeyond360);
        // newGameObject[0].transform.DORotate(Vector3.up * 360 * 5, 5, RotateMode.FastBeyond360);
        
        Brick brick = LevelManager.Instance.brickManager.SearchBrickClosest(BrickManager.Instance.GetBrickMap(new Vector2Int(xx, yy)).tf.position);  
        List<Vector2Int> waves = new List<Vector2Int>(brick.brickData.listBricksID);

        while (waves.Count > 0)
        {
            for (int i = 0; i < waves.Count; i++)
            {
                if ((BrickManager.Instance.GetBrickMap(waves[i]).tf.position - BrickManager.Instance.GetBrickMap(new Vector2Int(xx, yy)).tf.position).sqrMagnitude <= range)
                {
                    Brick brick2 = BrickManager.Instance.GetBrickMap(waves[i]);
                    waves.RemoveAt(i);
                    i--;
                    brick2.child.parent =  newGameObject[0].transform;
                    brick2.child.DOLocalMove(Vector3.zero, 2).SetEase(Ease.OutQuart);
                    brick2.meshRenderers.transform.DOScale(Vector3.zero * 0.001f, 1.5f);
                }
            }
            range += 8f;
            yield return null;
        }

        yield return Cache.GetWFS(2f);
        BackTransformParent();
        for (int i = 0; i < totalBricks.Count; i++)
        {
            totalBricks[i].child.localPosition = Vector3.up * 100;
            totalBricks[i].child.localEulerAngles = Vector3.zero;
            totalBricks[i].meshRenderers.transform.localScale = Vector3.one;
        }
        yield return Cache.GetWFS(0.1f);
    }
    private IEnumerator EndEffect11()
    {
        DOTween.KillAll();
        LevelManager.Instance.OnLoadNextLevelEffect(); // chuyển màu vào đây
        tf.position = new Vector3(tf.position.x, -1.5f, tf.position.z);

        // lấy vị trí tọa độ điểm chính giữa của level
        int xx = GetLevelData(1).mapSize.x / 2;
        int yy = GetLevelData(1).mapSize.y / 2;
        newGameObject[0].transform.position = BrickManager.Instance.GetBrickMap(new Vector2Int(xx, yy)).tf.position + Vector3.up * 25;


        // thiết lập trước vị trí ban đầu
        for (int i = 0; i < totalBricks.Count; i++)
        {
            totalBricks[i].meshRenderers.transform.localScale = Vector3.zero * 0.001f;
        }

        // bắt đầu cho quay và chạy về đúng vị trí
        List<Vector2Int> waves = new List<Vector2Int>();
        for (int i = 0; i < totalBricks.Count; i++)
        {
            waves.Add(totalBricks[i].id);
        }
        
        range = 600;
        // lấy ra từ ngoài vào trong
        while (waves.Count > 0)
        {
            for (int i = 0; i < waves.Count; i++)
            {
                if ((BrickManager.Instance.GetBrickMap(waves[i]).tf.position - BrickManager.Instance.GetBrickMap(new Vector2Int(xx, yy)).tf.position).sqrMagnitude >= range)
                {
                    Brick brick2 = BrickManager.Instance.GetBrickMap(waves[i]);
                    
                    if(brick2.IsClear == false)
                    {
                        brick2.child.position = newGameObject[0].transform.position;
                        brick2.meshRenderers.transform.DOScale(1, 1.5f);
                        brick2.child.DOLocalMove(Vector3.zero, 2);
                    }
                    else
                    {
                        brick2.child.parent =  brick2.tf;
                        brick2.meshRenderers.transform.localScale = Vector3.one;
                        brick2.child.localPosition = Vector3.zero;
                    }

                    waves.RemoveAt(i);
                    i--;
                }
            }
            range -= 5f;
            yield return null;
        }

        yield return Cache.GetWFS(2);
    }
    #endregion

    #region Chỉnh sửa transform parent
    private void ChangeTransformParent(bool theoChieuDoc)
    {
        if(theoChieuDoc)
        {
            for (int i = 0; i < 32; i++)
            {
                newGameObject[i].transform.position = new Vector3(brickList[i][0].child.position.x, 0, 0);
                for (int j = 0; j < 32; j++)
                {
                    brickList[i][j].child.transform.parent = newGameObject[i].transform;
                }
            }
        }
        else
        {
            for (int i = 0; i < 32; i++)
            {
                newGameObject[i].transform.position = new Vector3(0, 0, brickList[0][i].child.position.z);
                for (int j = 0; j < 32; j++)
                {
                    brickList[j][i].child.transform.parent = newGameObject[i].transform;
                }
            }
        }
    }
    private void BackTransformParent()
    {
        for (int i = 31; i >= 0; i--)
        {
            for (int j = 0; j < 32; j++)
            {
                brickList[i][j].child.transform.parent = brickList[i][j].tf;
            }
        }
    }

    private LevelData GetLevelData(int sub)
    {
        return LevelManager.Instance.levelDatas[(DataManager.Instance.gameData.currentLevel - sub) % LevelManager.Instance.levelDatas.Count];
    }

    #endregion
    
    private void EndChangeLevelEffect()
    {
        endEffectAction?.Invoke();
        endEffectAction = null;
        tf.DOMoveY(0f, 0.5f);
    }
    #endregion
}
