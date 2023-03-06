using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

public class BrickManagerEditor : Singleton<BrickManagerEditor>
{
    public enum PaintType { One, X4, X9, Verticle, Horizon, X25
    }

    Vector2Int size = new Vector2Int(32, 32);
    public Transform tf;
    public BrickEditor prefab;

    public List<BrickEditor> bricks = new List<BrickEditor>();

    public ColorType colorType;

    public LevelData levelData;
    public BrickData brickData;

    public List<LevelData> levelDatas = new List<LevelData>();

    Dictionary<Vector2Int, BrickEditor> brickMap = new Dictionary<Vector2Int, BrickEditor>();

    string name = "";

    [Header("FILL CELL")]
    public Vector2Int cellMin;
    public Vector2Int cellMax;

    private PaintType paintType = PaintType.One;

    public GameObject paint;

    public bool isCanPaint => !paint.activeInHierarchy;

    public Transform[] suggests;
    int suggestBrick;

    private void Awake()
    {
        int i = 1;
        while (true)
        {
            levelData = Resources.Load<LevelData>("LevelData_" + i);

            if (levelData == null) break;
            levelDatas.Add(levelData);
            i++;
        }
        name = (levelDatas.Count + 1).ToString();
    }

    // Start is called before the first frame update
    void Start()
    {
        brickData = null;

        //khoi tao dau tien
        for (int i = 0; i < size.y; i++)
        {
            for (int j = 0; j < size.x; j++)
            {
                BrickEditor brickEditor = Instantiate(prefab, tf.position + Vector3.right * (j - size.x * 0.5f + 0.5f) + Vector3.forward * (i - size.y * 0.5f + 0.5f), Quaternion.identity, tf);
                brickEditor.id = new Vector2Int(j, i);
                bricks.Add(brickEditor);
                //map brick lai cho nhanh
                brickMap.Add(brickEditor.id, brickEditor);
            }
        }
        //colorData.SetupInit();

        SetPaintStyle(PaintType.One);
    }

    public ColorData colorData;

    internal void OnInit()
    {
         //bricks = new List<BrickEditor>();
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && isCanPaint)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100))
            {
                //Debug.DrawLine(ray.origin, hit.point);
                //Debug.Log(hit.point);
                BrickEditor brick = hit.collider.GetComponent<BrickEditor>();

                if (brick != null)
                {
                    CheckNewBrickData();

                    List<BrickEditor> bricks = GetBrickInPaint(brick);

                    for (int i = 0; i < bricks.Count; i++)
                    {
                        bricks[i].RemoveParent();
                        //paint
                        bricks[i].SetParent(brickData);
                    }
                }
            }
        }

        //suggest brick
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100))
            {
                //Debug.DrawLine(ray.origin, hit.point);
                //Debug.Log(hit.point);
                BrickEditor brick = hit.collider.GetComponent<BrickEditor>();

                if (brick != null)
                {
                    List<BrickEditor> bricks = GetBrickInPaint(brick);

                    for (int i = 0; i < bricks.Count; i++)
                    {
                        suggests[i].position = bricks[i].transform.position;
                    }
                }
            }
        }
    }

    public void SetPaintStyle(PaintType paintType)
    {
        this.paintType = paintType;

        switch (paintType)
        {
            case PaintType.One:
                suggestBrick = 1;
                break;
            case PaintType.X4:
                suggestBrick = 4;
                break;
            case PaintType.X9:
                suggestBrick = 9;
                break;
            case PaintType.Verticle:
                suggestBrick = 3;
                break;
            case PaintType.Horizon:
                suggestBrick = 3;
                break;
            case PaintType.X25:
                suggestBrick = 25;
                break;
            default:
                break;
        }


        for (int i = 0; i < suggests.Length; i++)
        {
            suggests[i].gameObject.SetActive(i < suggestBrick);
        }
    }

    private List<BrickEditor> GetBrickInPaint(BrickEditor brick)
    {
        List<BrickEditor> bricks = new List<BrickEditor>();

        bricks.Add(brick);

        switch (this.paintType)
        {
            case PaintType.One:
                break;
            case PaintType.X4:
                if (brickMap.ContainsKey(brick.id + Vector2Int.up)) bricks.Add(brickMap[brick.id + Vector2Int.up]);
                if (brickMap.ContainsKey(brick.id + Vector2Int.right)) bricks.Add(brickMap[brick.id + Vector2Int.right]);
                if (brickMap.ContainsKey(brick.id + Vector2Int.one)) bricks.Add(brickMap[brick.id + Vector2Int.one]);
                break;

            case PaintType.X9:
                if (brickMap.ContainsKey(brick.id + Vector2Int.up - Vector2Int.right)) bricks.Add(brickMap[brick.id + Vector2Int.up - Vector2Int.right]);
                if (brickMap.ContainsKey(brick.id + Vector2Int.up + Vector2Int.zero)) bricks.Add(brickMap[brick.id + Vector2Int.up + Vector2Int.zero]);
                if (brickMap.ContainsKey(brick.id + Vector2Int.up + Vector2Int.right)) bricks.Add(brickMap[brick.id + Vector2Int.up + Vector2Int.right]);

                if (brickMap.ContainsKey(brick.id + Vector2Int.zero - Vector2Int.right)) bricks.Add(brickMap[brick.id + Vector2Int.zero - Vector2Int.right]);
                if (brickMap.ContainsKey(brick.id + Vector2Int.zero + Vector2Int.right)) bricks.Add(brickMap[brick.id + Vector2Int.zero + Vector2Int.right]);

                if (brickMap.ContainsKey(brick.id - Vector2Int.up - Vector2Int.right)) bricks.Add(brickMap[brick.id - Vector2Int.up - Vector2Int.right]);
                if (brickMap.ContainsKey(brick.id - Vector2Int.up + Vector2Int.zero)) bricks.Add(brickMap[brick.id - Vector2Int.up + Vector2Int.zero]);
                if (brickMap.ContainsKey(brick.id - Vector2Int.up + Vector2Int.right)) bricks.Add(brickMap[brick.id - Vector2Int.up + Vector2Int.right]);
                break;

            case PaintType.Verticle:
                if (brickMap.ContainsKey(brick.id + Vector2Int.up)) bricks.Add(brickMap[brick.id + Vector2Int.up]);
                if (brickMap.ContainsKey(brick.id + Vector2Int.down)) bricks.Add(brickMap[brick.id + Vector2Int.down]);
                break;

            case PaintType.Horizon:
                if (brickMap.ContainsKey(brick.id + Vector2Int.left)) bricks.Add(brickMap[brick.id + Vector2Int.left]);
                if (brickMap.ContainsKey(brick.id + Vector2Int.right)) bricks.Add(brickMap[brick.id + Vector2Int.right]);
                break;

            case PaintType.X25:

                for (int i = -5; i <= 5; i++)
                {
                    for (int j = -5; j <= 5; j++)
                    {
                        if (i != 0 && j != 0)
                        {
                            if (brickMap.ContainsKey(brick.id + Vector2Int.up * i + Vector2Int.right * j)) bricks.Add(brickMap[brick.id + Vector2Int.up * i + Vector2Int.right * j]);
                        }
                    }
                }
                break;
            default:
                break;
        }

        return bricks;
    }

    public void CheckNewBrickData()
    {
        if (levelData == null)
        {
            levelData = ScriptableObjectUtility.CreateAsset<LevelData>(name);
        }

        //tim xem trong level data co mau nay chua
        //co r thi gan no ve
        //chua co thi phai tao mau moi
        for (int i = 0; i < levelData.brickDatas.Count; i++)
        {
            if (levelData.brickDatas[i].colorType == this.colorType)
            {
                brickData = levelData.brickDatas[i];
                return;
            }
        }

        //if (brickData == null)
        {
            brickData = new BrickData();
            brickData.colorType = this.colorType;
        }

        if (!levelData.brickDatas.Contains(brickData))
        {
            levelData.brickDatas.Add(brickData);
        }
    }

    public void Save()
    {
        SplitBrickGroup();
        DefaultHammer();

        EditorUtility.SetDirty(levelData);

        Debug.Log("Save Succsess Level " + name + " !!!");
    }

    public void Load(int level)
    {
        levelData = levelDatas[level];
        name = levelData.name;

        for (int i = 0; i < bricks.Count; i++)
        {
            bricks[i].OnInit();
        }

        //for (int i = 0; i < levelData.brickDatas.Count; i++)
        //{
        //    brickData = levelData.brickDatas[i];
        //    for (int j = 0; j < levelData.brickDatas[i].listBricksID.Count; j++)
        //    {
        //        brickMap[levelData.brickDatas[i].listBricksID[j]].SetParent(brickData);
        //    }
        //}

        MergeBrickGroup();

        Debug.Log("Load Succsess Level " + name + " !!!");
    }

    private void MergeBrickGroup()
    {
        Dictionary<ColorType, BrickData> map = new Dictionary<ColorType, BrickData>();

        for (int i = 0; i < levelData.brickDatas.Count; i++)
        {
            if (!map.ContainsKey(levelData.brickDatas[i].colorType))
            {
                map.Add(levelData.brickDatas[i].colorType, levelData.brickDatas[i]);
            }
            else
            {
                map[levelData.brickDatas[i].colorType].listBricksID.AddRange(levelData.brickDatas[i].listBricksID);
            }
        }

        levelData.brickDatas.Clear();

        foreach (var item in map)
        {
            if (item.Key != ColorType.Editor && item.Key != ColorType.Clear)
            {
                BrickData brickData = new BrickData();
                brickData.colorType = item.Key;
                brickData.listBricksID.AddRange(item.Value.listBricksID);
                levelData.brickDatas.Add(brickData);
            }
        }

        for (int i = 0; i < levelData.brickDatas.Count; i++)
        {
            this.brickData = levelData.brickDatas[i];
            for (int j = 0; j < brickData.listBricksID.Count; j++)
            {
                //Debug.Log(" _ " + brickData.listBricksID[j]);
                brickMap[brickData.listBricksID[j]].SetParent(brickData);
            }
        }

        EditorUtility.SetDirty(levelData);
    }

    private void SplitBrickGroup()
    {
        List<BrickData> removeList = new List<BrickData>();

        for (int i = 0; i < levelData.brickDatas.Count; i++)
        {
            if (levelData.brickDatas[i].colorType == ColorType.Editor || levelData.brickDatas[i].colorType == ColorType.Clear || levelData.brickDatas[i].listBricksID.Count <= 0)
            {
                removeList.Add(levelData.brickDatas[i]);
            }
        }

        for (int i = 0; i < removeList.Count; i++)
        {
            levelData.brickDatas.Remove(removeList[i]);
        }

        //split brick data by array
        List<BrickData> brickDatas = new List<BrickData>();

        //Debug.LogError("1 - " + levelData.brickDatas.Count);

        for (int i = 0; i < levelData.brickDatas.Count; i++)
        {
            brickDatas.AddRange(SplitBrickData(levelData.brickDatas[i]));
        }

        //Debug.LogError(brickDatas.Count);

        levelData.brickDatas.Clear();
        levelData.brickDatas.AddRange(brickDatas);
    }

    private List<BrickData> SplitBrickData(BrickData brickData)
    {
        List<BrickData> brickDatas = new List<BrickData>();


        while (brickData.listBricksID.Count > 0)
        {
            Dictionary<Vector2Int, bool> flag = new Dictionary<Vector2Int, bool>();

            //khoi tao vi tri dau tien
            VinesEditor vine = new VinesEditor();

            vine.colorType = brickData.colorType;

            vine.AddLeaf(brickMap[brickData.listBricksID[0]]);

            flag[brickData.listBricksID[0]] = true;

            brickData.listBricksID.RemoveAt(0);

            //

            while (vine.leaf.Count > 0)
            {
                vine.CollectLeaf();


                for (int i = 0; i < vine.branch.Count; i++)
                {
                    //Debug.Log("Branch " + vine.branch[i].id);

                    vine.AddLeaf(GetNeighborBricks(vine.colorType, vine.branch[i].id, ref flag));
                }

                //Debug.Log("leaf count " + vine.leaf.Count);

                for (int i = 0; i < vine.leaf.Count; i++)
                {
                    brickData.listBricksID.Remove(vine.leaf[i].id);
                }

                vine.CollectBranch();
            }

            //add new BrickData
            BrickData newBrickData = new BrickData();
            newBrickData.colorType = vine.colorType;
            for (int i = 0; i < vine.bricks.Count; i++)
            {
                newBrickData.listBricksID.Add(vine.bricks[i].id);
            }

            brickDatas.Add(newBrickData);
        }

        return brickDatas;
    }

    private List<BrickEditor> GetNeighborBricks(ColorType colorType, Vector2Int index, ref Dictionary<Vector2Int, bool> flag)
    {
        List<BrickEditor> brickEditors = new List<BrickEditor>();

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                Vector2Int point = new Vector2Int(index.x + j, index.y + i);

                if (!flag.ContainsKey(point))
                {
                    flag[point] = false;
                }

                if (flag[point])
                {
                    continue;
                }

                if ((point.x >= 0 && point.x < size.x) && (point.y >= 0 && point.y < size.y))
                {
                    if (brickMap[point].ColorType == colorType)
                    {
                        brickEditors.Add(brickMap[point]);
                    }
                }

                flag[point] = true;
            }
        }

        return brickEditors;
    }

    public class VinesEditor
    {
        public ColorType colorType;
        public List<BrickEditor> bricks = new List<BrickEditor>();
        public List<BrickEditor> branch = new List<BrickEditor>();
        public List<BrickEditor> leaf = new List<BrickEditor>();

        public void AddLeaf(BrickEditor brick)
        {
            leaf.Add(brick);
        }

        public void AddLeaf(List<BrickEditor> bricks)
        {
            leaf.AddRange(bricks);
        }

        public void AddBranch(BrickEditor brick)
        {
            branch.Add(brick);
        }

        public void CollectLeaf()
        {
            branch.AddRange(leaf);
            leaf.Clear();
        }

        public void CollectBranch()
        {
            bricks.AddRange(branch);
            branch.Clear();
        }

    }

    private void DefaultHammer()
    {
        if (levelData.hammerColor.Count == 0)
        {
            for (int i = 0; i < levelData.brickDatas.Count; i++)
            {
                if (!levelData.hammerColor.Contains(levelData.brickDatas[i].colorType))
                {
                    levelData.hammerColor.Add(levelData.brickDatas[i].colorType);
                }
            }
        }
    }

    #region Move

    public void MoveRight()
    {
        Vector2Int go = Vector2Int.zero;
        Vector2Int to = Vector2Int.zero;
        //di chuyen map
        for (int i = 0; i < size.y; i++)
        {
            for (int j = size.x - 1; j > 0 ; j--)
            {
                to.x = j;
                to.y = i;

                go.x = j - 1;
                go.y = i;

                brickMap[to].SetParent(brickMap[go].brickData);
            }
        }

        //clear right
        for (int i = 0; i < size.y; i++)
        {
            to.x = 0;
            to.y = i;

            brickMap[to].SetParent(null);
        }
    }


    public void MoveLeft()
    {
        Vector2Int go = Vector2Int.zero;
        Vector2Int to = Vector2Int.zero;
        //di chuyen map
        for (int i = 0; i < size.y; i++)
        {
            for (int j = 0; j < size.x - 1; j++)
            {
                to.x = j;
                to.y = i;

                go.x = j + 1;
                go.y = i;

                brickMap[to].SetParent(brickMap[go].brickData);
            }
        }

        //clear right
        for (int i = 0; i < size.y; i++)
        {
            to.x = size.x - 1;
            to.y = i;

            brickMap[to].SetParent(null);
        }
    }

    public void MoveUp()
    {
        Vector2Int go = Vector2Int.zero;
        Vector2Int to = Vector2Int.zero;

        //di chuyen map
        for (int i = size.y - 1; i > 0 ; i--)
        {
            for (int j = 0; j < size.x; j++)
            {
                to.x = j;
                to.y = i;

                go.x = j;
                go.y = i - 1;

                brickMap[to].SetParent(brickMap[go].brickData);
            }
        }

        //clear bot
        for (int j = 0; j < size.x; j++)
        {
            to.x = j;
            to.y = 0;

            brickMap[to].SetParent(null);
        }
    }

    public void MoveDown()
    {
        Vector2Int go = Vector2Int.zero;
        Vector2Int to = Vector2Int.zero;
        //di chuyen map
        for (int i = 0; i < size.y - 1; i++)
        {
            for (int j = 0; j < size.x; j++)
            {
                to.x = j;
                to.y = i;

                go.x = j;
                go.y = i + 1;

                brickMap[to].SetParent(brickMap[go].brickData);
            }
        }

        //clear top
        for (int j = 0; j < size.x; j++)
        {
            to.x = j;
            to.y = size.y - 1;

            brickMap[to].SetParent(null);
        }
    }

    #endregion

    #region FillCell

    public void FillCell()
    {
        CheckNewBrickData();

        cellMin -= Vector2Int.one;
        cellMin.x = Mathf.Clamp(cellMin.x, 0, size.x);
        cellMin.y = Mathf.Clamp(cellMin.y, 0, size.y);

        cellMax.x = Mathf.Clamp(cellMax.x, 0, size.x);
        cellMax.y = Mathf.Clamp(cellMax.y, 0, size.y);

        for (int i = cellMin.x; i < cellMax.x; i++)
        {
            for (int j = cellMin.y; j < cellMax.y; j++)
            {
                brickMap[new Vector2Int(i, j)].SetParent(brickData);
            }
        }
    }

    #endregion

    private void OnDisable()
    {
        Debug.Log("disable");
        Save();
    }

}



#endif