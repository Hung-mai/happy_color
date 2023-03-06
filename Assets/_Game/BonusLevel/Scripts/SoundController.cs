using UnityEngine;
using DG.Tweening;

public class SoundController : MonoBehaviour
{
    public static SoundController Instance;
    public SoundData dataMusic;

    [Header("--------------- Quản lý một vài Sound đơn khác --------")]
    public SoundObject sound_Knock;
    public SoundObject sound_Bubble;
    public SoundObject sound_Star;
    public SoundObject sound_Ting;
    public SoundObject sound_Click;
    public SoundObject sound_Firework;
    public SoundObject sound_FlyingStar;
    public SoundObject sound_Sellecthammer;
    public SoundObject sound_TEP;
    public SoundObject sound_Victory;
    public SoundObject sound_Fail;
    public SoundObject sound_Sort_Explore;
    public SoundObject sound_Sort_Touch;

    private AudioSource audioSource;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
        audioSource = gameObject.GetComponent<AudioSource>();
    }


    private void OnEnable()
    {
        if (dataMusic != null && DataManager.Instance != null && DataManager.Instance.gameData != null && dataMusic.clip != null)
        {
            audioSource.clip = dataMusic.clip;
            audioSource.volume = 0;
            FadeIn();
            audioSource.Play();
        }
    }

    public void ReloadMusic()
    {
        if (dataMusic != null && DataManager.Instance != null && DataManager.Instance.gameData != null && dataMusic.clip != null)
        {
            audioSource.volume = dataMusic.volume;
        }
    }

    public void Enable()
    {
        audioSource.enabled = true;
    }

    public void Disable()
    {
        audioSource.enabled = false;
    }

    public void OnOffSound(bool on)
    {
        AudioListener.pause = !on;
    }

    public void OnOffMusic(bool IsOn)
    {
        if (IsOn)
        {
            dataMusic.volume = 0.25f;
        }
        else
        {
            dataMusic.volume = 0f;
        }
        ReloadMusic();
    }

    public void FadeIn()
    {
        if (DataManager.Instance.gameData.isMusic) DOTween.To(() => audioSource.volume, x => audioSource.volume = x, dataMusic.volume, 3f);
    }

    public void FadeOut()
    {
        DOTween.To(() => audioSource.volume, x => audioSource.volume = x, 0, 1f);
    }
}
