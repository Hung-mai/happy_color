using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SampleBrickScript : Singleton<SampleBrickScript>
{
    public Transform tf;
    public Transform cameraTf;
    public Camera camera;
    public Transform sampleBackGround;
    private float finishTime = 2f;
    public void OnInit()    //Setup hình ảnh mẫu
    {
        tf.position = new Vector3(0, 2f, -7.3f);
        tf.eulerAngles = new Vector3(30, 0, 0);
        tf.localScale = BonusLevelManager.Instance.paintLevelLayout.paintLevelLayout[(DataManager.Instance.gameData.currentPaintLevel-1)%BonusLevelManager.Instance.levelDataTextAssets.Count].sampleBrickScale;
        gameObject.layer = LayerMask.NameToLayer(Constant.UI);
        Vector3 temp = cameraTf.position - tf.position;
        temp.y -= (Vector3.Distance(cameraTf.position, tf.position));
        tf.eulerAngles = new Vector3(19.5f,0,0);
        BonusLevelManager.Instance.SetupLayout();
    }

    public void OnFinish()  //Khi hoàn thành thì cài lại LayerMask rồi di chuyển về vị trí so sánh
    {
        sampleBackGround.gameObject.SetActive(false);
        SetLayerMask();
        tf.DOMove(new Vector3(Mathf.Abs(BonusLevelManager.Instance.marginTransform.position.x*2), 0, 0), finishTime);
        tf.DORotate(Vector3.zero, finishTime);
        tf.DOScale(Vector3.one, finishTime);
        cameraTf.DOMove(new Vector3(BonusLevelManager.Instance.marginTransform.position.x, 65f, 22f), finishTime)
            .OnComplete(()=>
            {
                //Instantiate(BonusGameManager.Instance.fireWorkVfx, new Vector3(Mathf.Abs(BonusLevelManager.Instance.marginTransform.position.x), 0, 0), Quaternion.identity);
                Timer.Schedule(this,1.5f,() => { UI_Game.Instance.OpenUI(UIID.BonusVictory); });
            });
    }

    void SetLayerMask()
    {
        gameObject.layer = LayerMask.NameToLayer(Constant.DEFAULT);
        for (int i = 0; i < BonusBrickManager.Instance.totalSampleBonusBricks.Count; i++)
        {
            BonusBrickManager.Instance.totalSampleBonusBricks[i].ChangeLayerEndGame();
        }
    }
}
