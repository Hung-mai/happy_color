using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public enum SoundID
{
    BG_1,
}

public enum FxID
{
    Click,
    OpenPopup,
    Jump,
    Flat,
    Hurt,
    Cut,
    LevelComplete,
    CollectItem,
    BuyFuniture,
    NewDecor,
    BurgerCollect,
    CashCollect,
    Fork,
    Hammer,
    CashReceipt,
    CustomerSmile,

}


public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] UserData userData;

    private AudioSource soundSource;
    private AudioSource fxSource;

    [SerializeField] private AudioClip[] soundAus;
    [SerializeField] private AudioClip[] fxAus;

    private bool isLoaded = false;
    private int indexSound;

    public void Awake()
    {
        soundSource = gameObject.AddComponent<AudioSource>();
        soundSource.loop = true;
        fxSource = gameObject.AddComponent<AudioSource>();
        fxSource.loop = false;
    }

    private void Start()
    {
        StartCoroutine(IELoad());
    }

    private IEnumerator IELoad()
    {
        if (soundAus.Length > 0)
        {
            yield return Cache.GetWFS(1f);
            isLoaded = true;

            indexSound = Random.Range(0, soundAus.Length);
            PlaySound((SoundID)indexSound);
        }
    }


    public void PlaySound(SoundID ID)
    {
        soundSource.clip = soundAus[(int)ID];

        if (userData.musicIsOn && isLoaded)
        {
            soundSource.Play();
            //Debug.Log(ID);
        }
    }

    public void NextSound()
    {
        indexSound = indexSound >= soundAus.Length - 1 ? 0 : indexSound + 1;
        PlaySound((SoundID)indexSound);
    }

    public void PlayMusic(bool play)
    {
        if (play)
        {
            soundSource.Play();
        }
        else
        {
            soundSource.Stop();
        }
    }


    public void PlayFx(FxID ID)
    {
        if (userData.fxIsOn && isLoaded)
        {
            fxSource.PlayOneShot(fxAus[(int)ID]);

            //Debug.Log(ID);
        }
    }

    public void ChangeSound(SoundID ID, float time)
    {
        if (userData.musicIsOn && isLoaded)
        {
            float spacetime = time / 2;

            ChangeVol(.1f, spacetime,
                () =>
                {
                    PlaySound(ID);
                    ChangeVol(1, spacetime, null);
                });
        }
    }

    public void ChangeVol(float vol, float time, UnityAction callBack)
    {
        StartCoroutine(ChangeVolume(vol, time, callBack));
    }

    private IEnumerator ChangeVolume(float vol, float time, UnityAction callBack)
    {
        float stepVol = (vol - soundSource.volume) / 10;
        float stepTime = time / 10;

        for (int i = 0; i < 10; i++)
        {
            soundSource.volume += stepVol;
            yield return Cache.GetWFS(stepTime);
        }

        callBack?.Invoke();
    }


}
