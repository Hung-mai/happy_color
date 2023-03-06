using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BonusBrickManager : Singleton<BonusBrickManager>
{
    public Transform tf;
    public BonusBrick bonusBrickPrefabs;
    public BonusBrick sampleBonusBrickPrefabs;  //Hình mẫu
    public ExplodeEffect explodePrefabs;
    private BonusBrick clickingBrick; //Brick đang được click
    public List<BonusBrick> totalBonusBricks = new List<BonusBrick>();
    public List<BonusBrick> totalSampleBonusBricks = new List<BonusBrick>();
    public MiniPool<BonusBrick> BrickPool = new MiniPool<BonusBrick>();
    public MiniPool<BonusBrick> sampleBrickPool = new MiniPool<BonusBrick>();
    public MiniPool<ExplodeEffect> endGameExplodePool = new MiniPool<ExplodeEffect>();
    public Vector2Int mapSize;
    public Transform sampleModelParent; //Model mẫu
    public int completeBrickAmount = 0; //Số lượng Brick đúng màu khi nhấn nút Done
    public float completeBrickRate = 0;   //% Brick đã hoàn thành được màu
    public bool endGameCompareIsDone = false;
    private void Awake()
    {
        //BrickPool.OnInit(bonusBrickPrefabs.gameObject, 50, this.gameObject.transform);
        //sampleBrickPool.OnInit(sampleBonusBrickPrefabs.gameObject, 50, sampleModelParent);
        //endGameExplodePool.OnInit(explodePrefabs.gameObject, 10);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.State!=GameState.GamePlay) return;
        if(Input.GetMouseButton(0)&& BonusGameManager.Instance.choosingMaterial != null)
        {
            CheckHitPoint();
        }
        if (Input.GetMouseButtonUp(0))
        {
            clickingBrick = null;
        }
    }

    private void CheckHitPoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Debug.DrawRay(ray.origin, ray.direction * 100, Color.red);
        if (Physics.Raycast(ray.origin, ray.direction, out hit) && hit.collider != null)
        {
            BonusBrick bonusBrick = hit.collider.gameObject.GetComponent<BonusBrick>();
            if (clickingBrick == bonusBrick) return;
            clickingBrick = bonusBrick;
            if (bonusBrick != null) bonusBrick.OnHit();
        }
    }

    public void CheckEndGame()  //Check trước khi kết thúc level
    {
        completeBrickAmount = 0;
        SampleBrickScript.Instance.OnFinish();
        StartCoroutine(StartCompare());
    }

    public void SpawnBrick() 
    {
        for (int z = 0; z < mapSize.y; z++)
        {
            for (int x = 0; x < mapSize.x; x++)
            {
                if (BonusLevelManager.Instance.listBrickIDColorID[new Vector2Int(x, z)] == new Vector3Int(255, 255, 255)) continue;
                BonusBrick brick = BrickPool.Spawn(tf.position + Vector3.right * (-x + mapSize.x * 0.5f - 0.5f) + Vector3.forward * (z - mapSize.y * 0.5f - 0.5f), Quaternion.identity);
                if (BonusLevelManager.Instance.marginTransform.position.x < (-x + mapSize.x * 0.5f + 0.5f)) BonusLevelManager.Instance.marginTransform.position = new Vector3(-x + mapSize.x * 0.5f + 0.5f,0,0);
                brick.BrickInit(new Vector2Int(x, z), BonusLevelManager.Instance.rgbID2Material[BonusLevelManager.Instance.listBrickIDColorID[new Vector2Int(x, z)]]);

                if (BonusLevelManager.Instance.listBrickIDColorID[new Vector2Int(x, z)] != new Vector3Int(0, 0, 0))  totalBonusBricks.Add(brick);   //Chỉ lấy những brick không phải tường
            }
        }
        CameraControl.Instance.ChangeCameraPosition();
        SpawnSampleBrick();
    }

    public void SpawnSampleBrick()
    {
        for (int z = 0; z < mapSize.y; z++)
        {
            for (int x = 0; x < mapSize.x; x++)
            {
                if (BonusLevelManager.Instance.listBrickIDColorID[new Vector2Int(x, z)] == new Vector3Int(255, 255, 255)) continue;
                BonusBrick brick = sampleBrickPool.Spawn(SampleBrickScript.Instance.tf.position + Vector3.right * (-x + mapSize.x * 0.5f - 0.5f) + Vector3.forward * (z - mapSize.y * 0.5f - 0.5f), Quaternion.identity);
                brick.SampleBrickInit(BonusLevelManager.Instance.rgbID2SampleMaterial[BonusLevelManager.Instance.listBrickIDColorID[new Vector2Int(x, z)]]);
                if (BonusLevelManager.Instance.listBrickIDColorID[new Vector2Int(x, z)] != new Vector3Int(0, 0, 0)) totalSampleBonusBricks.Add(brick);
            }
        }
        SampleBrickScript.Instance.OnInit();
        BonusGameManager.Instance.StartGame();
    }

    public void ReloadLevelBrick()
    {
        //UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(Constant.PaintLevelStr);
    }

    public void SkipButtonClick()
    {
        for (int i = 0; i < totalBonusBricks.Count; i++)
        {
            totalBonusBricks[i].SkipButton();
        }
    }

    public void HintButton()
    {
        for (int i = 0; i < totalBonusBricks.Count; i++)
        {
            if (!totalBonusBricks[i].isComplete)
            {
                Material tempMat = new Material(totalBonusBricks[i].exactMaterial);
                tempMat.color = new Color(tempMat.color.r, tempMat.color.g, tempMat.color.b, 0.5f);
                totalBonusBricks[i].ChangeMaterial(tempMat);
            }
        }
    }

    public IEnumerator StartCompare()
    {
        tf.DOScale(Vector3.one,1f);
        tf.DOMove(Vector3.zero,1f);
        CameraControl.Instance.tf.DOMove(BonusLevelManager.Instance.paintLevelLayout.paintLevelLayout[DataManager.Instance.gameData.currentPaintLevel - 1].cameraEndGamePos, 2f);
        yield return Cache.GetWFS(3f);
        for (int i = 0; i < totalBonusBricks.Count; i++)
        {
            if (totalBonusBricks[i].isComplete) 
            {
                if(DataManager.Instance.gameData.currentPaintLevel==1 
                    || DataManager.Instance.gameData.currentPaintLevel == 2 
                    || DataManager.Instance.gameData.currentPaintLevel == 4 
                    || DataManager.Instance.gameData.currentPaintLevel == 7) yield return Cache.GetWFS(0.02f);
                else yield return Cache.GetWFS(0.05f);
                totalBonusBricks[i].ExplodeCube();
                totalSampleBonusBricks[i].ExplodeCube();
                completeBrickAmount++;
            } 
        }
        endGameCompareIsDone = true;
    }

    public void Btn_Replay() //Nút Retry trong phần UI EndBonusGame
    {
        Application.LoadLevel(Application.loadedLevel);
    }
}
