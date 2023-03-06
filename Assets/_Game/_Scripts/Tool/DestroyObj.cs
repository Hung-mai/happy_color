using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DestroyObj : MonoBehaviour {
    public float lifeTime;
    public bool isDisable = false;
    public bool isFadeOut = false;
    public CanvasGroup canvasGroup;
    public DOTweenAnimation twn_Fade;

    private void OnEnable()
    {
        if(isFadeOut && canvasGroup != null && twn_Fade != null) {
            canvasGroup.alpha = 1;
            twn_Fade.DORestart();
        } else if(isDisable) {
            Timer.Schedule(this, lifeTime, () => { gameObject.SetActive(false); }) ;
        } else {
            Destroy(gameObject, lifeTime);
        }
    }

    public void DisableObj() {
        gameObject.SetActive(false);
    }
}
