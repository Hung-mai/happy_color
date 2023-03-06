using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class LevelBonus_Controller : Singleton<LevelBonus_Controller>
{
    public Item itemHolding;
    public Camera cam;

    [Header("---------liftItem--------")]
    private Vector3 mousePos;
    private Vector3 camPos;
    [SerializeField]private Transform refTransformDistance;
    private Vector3 targetMoveItem;
    [Header("-------OldAttribute---------")]
    public Point pointOriginal;

    [Header("------------ref-------------")]
    public LevelBonus_LevelManager levelManager;

    private Tweener tweenMoveFollow;
    private Box chosingBox;
    private Item chosingItem;
    private Vector3 offset;

    public float timeCounting;
    private void Start()
    {
        timeCounting = 0;
    }
    private void Update()
    {
        timeCounting += Time.deltaTime;
        if (Input.GetMouseButtonDown(0))
        {
            if (levelManager.currentLevelBonus.isTutorial) levelManager.currentLevelBonus.handTut.SetActive(false);
            Debug.Log("Down");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("box")))
            {
                chosingBox = hit.collider.GetComponent<Box>();
            }
            if (Physics.Raycast(ray, out hit, Mathf.Infinity,LayerMask.GetMask("item")))//8 = layer so 8 = item
            {
                Debug.DrawLine(ray.origin, hit.point);
                chosingItem = hit.collider.GetComponent<Item>();
                if (hit.collider.gameObject != null && hit.collider.GetComponent<Item>())
                {
                    if (chosingBox.points[1].item == chosingItem && chosingBox.points[0].item != null || chosingBox.points[3].item == chosingItem && chosingBox.points[2].item != null) return;
                    itemHolding = hit.collider.GetComponent<Item>();
                    pointOriginal = CheckItemInPoint(itemHolding);
                    offset = hit.point - hit.transform.position;
                    for (int i = 0; i < chosingBox.points.Length; i++)
                    {
                        if (chosingBox.points[i].item == itemHolding) chosingBox.points[i].item = null;

                        SoundController.Instance.sound_Sort_Touch.PlaySoundOneShot();
                    }
                }
                else
                {
                    return;
                }
            }
        }
        if (Input.GetMouseButton(0))
        {
            LiftItem();
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (levelManager.currentLevelBonus.isTutorial) levelManager.currentLevelBonus.handTut.SetActive(true);
            tweenMoveFollow.Kill();
            Debug.Log("Up");
            DropItem();
        }
    }

    private void DropItem()
    {
        if (itemHolding == null) return;
        SoundController.Instance.sound_Sort_Touch.PlaySoundOneShot();
        Box boxOnDrop = null;
        Point pointNearest = null;

        Item currentItem = itemHolding;
        itemHolding = null;

        #region raycast check
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("box")))//9 = layer so 9 = box
        {
            //Debug.DrawLine(ray.origin, hit.point);
            if (hit.collider.gameObject != null)
            {
                boxOnDrop = hit.collider.GetComponent<Box>();
                //tim xem diem nao gan` nhat' trong cacpoint null
                //Logic mới: Nếu bị chắn bởi 1 bình ở ngoài thì sẽ không thể cất bình vào trong
                float minDis = 10000f;
                for (int i = 0; i < boxOnDrop.points.Length; i++)
                {
                    if (boxOnDrop.points[i].item == null)
                    {
                        if (i == 1 && boxOnDrop.points[0].item != null || i == 3 && boxOnDrop.points[2].item != null) continue;
                        if (Vector3.Distance(hit.point, boxOnDrop.points[i].transPoint.position) <= minDis)
                        {
                            minDis = Vector3.Distance(hit.point, boxOnDrop.points[i].transPoint.position);
                            pointNearest = boxOnDrop.points[i];
                        }
                    }
                }

                //nhet vao` diem gan` nhat do
                //nhet' vao`
                if (pointNearest != null)
                {
                    pointNearest.item = currentItem;
                    //pointOriginal.item = null;
                    //sau nay se ve~ path sau
                    currentItem.transform.DOMove(pointNearest.transPoint.position, 0.1f).SetEase(Ease.Linear).OnComplete(() => {
                        //check match three
                        LevelBonus.Instance.CheckBox(boxOnDrop);
                    });
                }
                else
                {
                    currentItem.transform.position = pointOriginal.transPoint.position;
                    pointOriginal.item = currentItem;
                    pointOriginal = null;
                }
            }
            else if(hit.collider == null)
            {
               
            }
        }
        else
        {
            //tra ve vi tri cu~
            currentItem.transform.position = pointOriginal.transPoint.position;
            pointOriginal.item = currentItem;
            pointOriginal = null;
        }
        #endregion
    }

    private void LiftItem()
    {
        if (itemHolding == null)
        {
            return;
        }
        camPos = Camera.main.transform.position;
        mousePos = Input.mousePosition;
        mousePos.z = Mathf.Abs(camPos.z - refTransformDistance.position.z);

        targetMoveItem = Camera.main.ScreenToWorldPoint(mousePos) - offset;
        tweenMoveFollow = itemHolding.transform.DOMove(targetMoveItem,Time.deltaTime).SetEase(Ease.Linear);
    }
    private Point CheckItemInPoint(Item item)
    {
        foreach (Box boxes in LevelBonus.Instance.boxs)
        {
            foreach (Point point in boxes.points)
            {
                if (item == point.item)
                {
                    return point;
                }
            }
        }
        Debug.LogError("Item k ton` tai trong bat ki box nao`!");
        return null;
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(targetMoveItem,1f);
    }
}
