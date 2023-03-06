using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class Hammer : MonoBehaviour
{
    public const string HIT = "Hit";
    public const string IDLE = "Idle";
    public const string PREHIT = "PreHit";
    public const string INIT = "Init";
    public const string NOHIT = "NoHit";

    public const string FIREWORKREADY = "FireworkReady";

    public Transform tf;

    public ColorData colorData;

    public ColorType ColorType => colorType;

    public Animation animation;

    public GameObject Arrow;

    private Vector3 initPoint;

    private ColorType colorType;

    public SpriteRenderer arrowSpr;

    public MeshRenderer hammerMeshRenderer;

    public GameObject hammerGob;

    public MeshRenderer[] hammerMeshRendererList;

    public GameObject[] hammerGobList;

    //Firework + Bomb
    public MeshRenderer fireworkMeshRenderer;
    public MeshRenderer bombMeshRenderer;
    public GameObject fireworkGob;
    public GameObject bombGob;

    public Transform fireworkTf;
    public Transform hammerSkinTf;
    public Transform bombTf;

    //Firework Effect
    public GameObject fireworkEffect;
    public GameObject bombEffect;

    public ParticleSystem particleSystemEffect;
    private void UpdatelifeTimeValue(float lifeTimeValue)
    {
        var main = particleSystemEffect.main;
        main.startLifetime = lifeTimeValue;
    }
    public void OnInit()
    {
        animation.Play(INIT);
        initPoint = tf.localPosition;

        Arrow.SetActive(false);
        fireworkEffect.SetActive(false);
        bombEffect.SetActive(false);
        fireworkTf.localEulerAngles = new Vector3(0, 90f, 0f);
        fireworkTf.localPosition = new Vector3(0f, 0f, 0f);
        hammerSkinTf.eulerAngles = new Vector3(0, -90f, -77.74f);

        bombTf.localEulerAngles = new Vector3(0, 0, 0);
        bombTf.localPosition = new Vector3(0, 0, 0);
        UpdatelifeTimeValue(0.1f);
    }

    public void SetColor(ColorType colorType)
    {
        this.colorType = colorType;
        hammerMeshRenderer.material = colorData.GetColorMat(colorType);
        fireworkMeshRenderer.material = colorData.GetColorMat(colorType);
        bombMeshRenderer.material = colorData.GetColorMat(colorType);
        arrowSpr.color = colorData.GetColor(colorType);
    }

    public void OnClick()
    {
        initPoint = tf.localPosition;
        SoundController.Instance.sound_Sellecthammer.PlaySoundOneShot();
        HammerControl.Instance.SelectHammer(this);
        if ((HammerType)DataManager.Instance.gameData.currentHammerTypeID == HammerType.Bomb) 
        {
            for (int i = 0; i < HammerControl.Instance.hammers.Count; i++)
            {
                HammerControl.Instance.hammers[i].bombEffect.SetActive(false);
            }
            bombEffect.SetActive(true);
        }
        else if ((HammerType)DataManager.Instance.gameData.currentHammerTypeID == HammerType.Firework)
        {
            animation.Play(FIREWORKREADY);
            for (int i = 0; i < HammerControl.Instance.hammers.Count; i++)
            {
                UpdatelifeTimeValue(0.1f);
                HammerControl.Instance.hammers[i].fireworkEffect.SetActive(false);
            }
            UpdatelifeTimeValue(0.5f);
            fireworkEffect.SetActive(true);
        }
        
    }

    public void OnSelected()
    {
        animation.Play(IDLE);
    }

    public void OnDeselected()
    {
        animation.Play(PREHIT);
    }

    public void OnHit()
    {
        animation.Play(HIT);
    }

    public void OnNoHit()
    {
        animation.Play(NOHIT);
    }

    public void ShowArrow()
    {
        if (gameObject.activeInHierarchy)
        {
            Arrow.gameObject.SetActive(true);
        }
    }

    public void ActiveAction(Vector3 targetPoint, UnityAction hitAction, UnityAction doneAction)
    {
        StopAllCoroutines();
        StartCoroutine(IEHitAction( targetPoint, hitAction, doneAction));
    }

    private IEnumerator IEHitAction(Vector3 targetPoint, UnityAction hitAction, UnityAction doneAction)
    {
        UpdatelifeTimeValue(0.1f);
        if ((HammerType)DataManager.Instance.gameData.currentHammerTypeID == HammerType.Firework)
        {
            //targetPoint = targetPoint - new Vector3(0, 12f, 0);
            fireworkEffect.SetActive(true);
            Vector3[] randomPoint = new Vector3[7];
            Vector3 lastPos = Vector3.zero;
            Vector3 direction = Vector3.zero;
            randomPoint[0] = new Vector3(tf.position.x, Random.Range(16f, 17f), 15f);
            randomPoint[1] = new Vector3(-3f, Random.Range(5f, 12f), 20f);
            randomPoint[2] = new Vector3(3f, Random.Range(5f, 12f), -5f);
            randomPoint[3] = new Vector3(-3f, Random.Range(5f, 12f), -5f);
            randomPoint[4] = new Vector3(3f, Random.Range(5f, 12f), 20f);
            randomPoint[randomPoint.Length - 2] = targetPoint + new Vector3(0, 35f, 0);
            randomPoint[randomPoint.Length - 1] = targetPoint;

            fireworkTf.DOLocalRotate(new Vector3(30f, 0f, 0f), 0.5f);
            fireworkTf.DOLocalMove(new Vector3(1.71f, -0.57f, 0.97f), 0.5f);
            hammerSkinTf.eulerAngles = new Vector3(60, 0f, 0f);

            int targetIndex = 0;
            float rotSpeed = 6f;
            float timeCounting = 0;
            float timeCountingtemp = 0;
            do
            {
                timeCounting += Time.deltaTime;
                timeCountingtemp += Time.deltaTime;
                yield return null;
                tf.Translate(Vector3.forward * Time.deltaTime*100f);
                Vector3 missleDirection = randomPoint[targetIndex] - tf.position;
                missleDirection = missleDirection.normalized;
                Quaternion missleRot = Quaternion.LookRotation(missleDirection);
                tf.rotation = Quaternion.Slerp(tf.rotation, missleRot, rotSpeed * Time.deltaTime);
                if ((Vector3.Distance(tf.position, randomPoint[targetIndex]) < 12f && targetIndex < randomPoint.Length - 1) || timeCounting > 2f) 
                {
                    targetIndex++;
                    timeCounting = 0;
                } 
                if ((targetIndex == randomPoint.Length - 1)&& rotSpeed<20f) rotSpeed = 30f;
                if (targetIndex == randomPoint.Length) break;
            } while (Vector3.Distance(tf.position, targetPoint) > 1f);
            //delay cho anim hit action
            yield return null;

            hitAction?.Invoke();

            yield return Cache.GetWFS(0.01f);

            tf.position = initPoint;

            doneAction?.Invoke();
            Control.Instance.StartFireworkEffect();
        }
        else if ((HammerType)DataManager.Instance.gameData.currentHammerTypeID == HammerType.Bomb)
        {
            bombTf.DOLocalRotate(new Vector3(120f, 0f, 180f), 1f);
            bombTf.DOLocalMove(new Vector3(1.54f, 4.32f, 0.57f), 1f);
            hammerSkinTf.DORotate(new Vector3(60, 0f, 0f), 1f);
            targetPoint = targetPoint - new Vector3(0, 8, 0);
            Vector3[] randomPoint = new Vector3[3];
            //Vector3 lastPos = Vector3.zero;
            //Vector3 direction = Vector3.zero;

            randomPoint[0] = new Vector3(tf.position.x, 2f, tf.position.z);
            randomPoint[1] = new Vector3(targetPoint.x, 50f, targetPoint.z);
            randomPoint[2] = targetPoint;
            bombGob.transform.localEulerAngles = new Vector3(90f, -20f, 0f);
            tf.DOPath(randomPoint, 1.3f, PathType.CatmullRom)
                .SetEase(Ease.Linear)
                .OnUpdate(() =>
                {
                    /*if (lastPos != Vector3.zero)
                    {
                        direction = tf.position - lastPos;
                        Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
                        tf.rotation = rotation;
                        lastPos = tf.position;
                    }
                    lastPos = tf.position;*/
                });
            //delay cho anim hit action
            yield return Cache.GetWFS(1.3f);

            hitAction?.Invoke();

            yield return Cache.GetWFS(0.2f);

            tf.position = initPoint;

            doneAction?.Invoke();
        }
        else
        {
            targetPoint = targetPoint + Vector3.up * 2;

            while ((tf.position - targetPoint).sqrMagnitude > .1f)  //Di chuyển đến nơi gõ búa
            {
                tf.position = Vector3.Lerp(tf.position, targetPoint, Time.deltaTime * 5f);
                tf.localScale = Vector3.Lerp(tf.localScale, Vector3.one * 1.2f, Time.deltaTime * 5f);
                yield return null;
            }

            OnHit();

            //delay cho anim hit action
            yield return Cache.GetWFS(0.5f);

            hitAction?.Invoke();

            yield return Cache.GetWFS(0.2f);

            while ((tf.localPosition - initPoint).sqrMagnitude > 0.01f)
            {
                tf.localPosition = Vector3.Lerp(tf.localPosition, initPoint, Time.deltaTime * 5f);
                tf.localScale = Vector3.Lerp(tf.localScale, Vector3.one, Time.deltaTime * 5f);
                yield return null;
            }

            tf.localPosition = initPoint;

            doneAction?.Invoke();
        }
        
    }

    public void Move(float targetPoint)
    {
        StartCoroutine(IEMove(targetPoint));
    }

    private IEnumerator IEMove(float targetPoint)
    {
        /*while ((tf.position - targetPoint).sqrMagnitude > .001f)
        {
            tf.position = Vector3.Lerp(tf.position, targetPoint, Time.deltaTime * 5f);
            yield return null;
        }*/
        yield return null;
        tf.DOLocalMoveX(targetPoint, 0.5f);
        initPoint = tf.position;
    }

    public void ChangeHammerType(HammerType hammerType)
    {
        if (hammerType == HammerType.Firework)
        {
            ChangeToFirework();
        }
        else if (hammerType == HammerType.Bomb)
        {
            ChangeToBomb();
        }
        else
        {
            hammerGob.SetActive(false);
            bombGob.SetActive(false);
            fireworkGob.SetActive(false);
            hammerMeshRendererList[(int)hammerType].material = hammerMeshRenderer.material;
            hammerGob = hammerGobList[(int)hammerType];

            hammerGob.SetActive(true);
        }
    }

    public void ChangeToFirework()
    {
        hammerGob.SetActive(false);
        bombGob.SetActive(false);
        fireworkGob.SetActive(true);
    }
    public void ChangeToBomb()
    {
        hammerGob.SetActive(false);
        fireworkGob.SetActive(false);
        bombGob.SetActive(true);
    }
}
