using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectGem : MonoBehaviour {
    private int amountObj = 0;
    public GameObject gemBase;
    public List<Transform> poolArrayGem;

    public void ShowEffect(int _amount, Vector3 orgin, Transform target, Action close) {
        if(_amount == 0) {
            close();
            return;
        }
        if (_amount <= 50) {
            amountObj = 2;
        } else if (_amount <= 100) {
            amountObj = 3;
        } else if (_amount <= 200) {
            amountObj = 4;
        } else if (_amount <= 300) {
            amountObj = 5;
        } else if (_amount <= 400) {
            amountObj = 6;
        } else if (_amount <= 500) {
            amountObj = 7;
        } else if (_amount <= 1000) {
            amountObj = 8;
        } else {
            amountObj = 9;
        }
        StartCoroutine(CreaterGem(amountObj, orgin, target.position, close));
    }

    #region ------ Move Gem -------
    IEnumerator CreaterGem(int number, Vector2 from, Vector2 target, Action onClose) {
        yield return new WaitForSeconds(.2f);
        Transform[] gems = new Transform[number];
        int poolingGemCount = poolArrayGem.Count;
        for (int i = 0; i < number; i++) {
            if (i < poolingGemCount) {
                Transform g1 = poolArrayGem[i];
                if (!g1.gameObject.activeSelf) {
                    g1.localEulerAngles = new Vector3(0, 0, 0);
                    g1.position = from;
                    gems[i] = g1;
                    continue;
                }
            }
            GameObject g = Instantiate(gemBase, transform, false);
            Transform gTranform = g.transform;
            gTranform.localEulerAngles = new Vector3(0, 0, 0);
            gTranform.position = from;
            poolArrayGem.Add(gTranform);
            gems[i] = gTranform;
        }
        yield return new WaitForSeconds(.1f);
        for (int i = 0; i < number; i++) {
            gems[i].gameObject.SetActive(true);
            if (i == 0) {
                StartCoroutine(CorMoveGem(gems[i], target, onClose));
            } else {
                StartCoroutine(CorMoveGem(gems[i], target));
            }
            yield return new WaitForSeconds(.08f);
        }
    }

    //IEnumerator MoveGem_Linear(Transform tr_gem, Vector3 target, Action callBack = null)
    //{
    //    Vector3 startPoint = tr_gem.position;
    //    //Di chuyển các Obj 
    //    int frame = 80; //1 giây có 100 frame
    //    Vector3 vectorMove = (target - startPoint) / frame;
    //    for (int i = 0; i < frame; i++)
    //    {
    //        tr_gem.position = new Vector3(startPoint.x + vectorMove.x * i, startPoint.y + vectorMove.y * i, startPoint.z + vectorMove.z * i);
    //        yield return new WaitForSeconds(.01f);
    //    }
    //    //Khi đã đến nơi rồi thì gọi callBack và tắt obj này đi
    //    if (callBack != null)
    //        callBack();
    //    tr_gem.gameObject.SetActive(false);
    //}

    IEnumerator CorMoveGem(Transform tr_gem, Vector3 target, Action callBack = null) {
        Vector3 startPoint = tr_gem.position;
        Vector3 endPoint = target;
        Vector2 mid_point = new Vector2(
            (startPoint.x + endPoint.x) / UnityEngine.Random.Range(1.5f, 3.5f) + UnityEngine.Random.Range(-1.5f, 1.5f),
            (startPoint.x + endPoint.x) / UnityEngine.Random.Range(1.5f, 3.5f) + UnityEngine.Random.Range(-1.5f, 1.5f));
        float t = .025f;
        int count = (int)(1 / t);
        Vector2[] path = new Vector2[count + 1];
        bezier_level2(startPoint, mid_point, target, t, path, count);
        float[] angel = angle(path);
        int path_count = path.Length;
        int angle_count = angel.Length;
        for (int i = 0; i < path_count; i++) {
            tr_gem.position = path[i];
            yield return new WaitForSeconds(.01f);
        }
        int fxGemStoreCount = poolArrayGem.Count;
        tr_gem.gameObject.SetActive(false);
        if (callBack != null)
            callBack();
    }
    #endregion

    public void bezier_level2(Vector2 p0, Vector2 p1, Vector2 p2, float t, Vector2[] result, int sizeArray) {
        for (int i = 0; i < sizeArray; i++) {
            float time = t * i;
            float sub_t = 1.0f - time;

            float x = sub_t * sub_t * p0.x + 2 * (sub_t) * time * p1.x + time * time * p2.x;
            float y = sub_t * sub_t * p0.y + 2 * (sub_t) * time * p1.y + time * time * p2.y;
            result[i] = new Vector2(x, y);
        }
        result[sizeArray] = p2;
    }

    public float[] angle(Vector2[] bezier) {
        int count = bezier.Length;
        float[] angles = new float[count - 1];
        for (int i = 0; i < count - 1; i++) {
            Vector2 dir = new Vector2(bezier[i + 1].x - bezier[i].x, bezier[i + 1].y - bezier[i].y);
            angles[i] = Vector2.Angle(dir, Vector2.up);
        }
        return angles;
    }
}
