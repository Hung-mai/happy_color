using System.Collections;
using UnityEngine;

public class SoundObject : MonoBehaviour
{
    public int id = 0;
    public bool playOnAwake = false;
    public SoundData[] listDataSound;

    private AudioSource audioSource;
    private void Awake()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        //Load thông tin sound
        if (listDataSound != null && id < listDataSound.Length && listDataSound[id].clip != null && DataManager.Instance != null && DataManager.Instance.gameData != null)
        {
            audioSource.clip = listDataSound[id].clip;
            audioSource.volume = listDataSound[id].volume;
            audioSource.pitch = listDataSound[id].pitch;
            audioSource.loop = listDataSound[id].isLoop;
        }
        if (playOnAwake) PlaySound();
    }

    public void PlaySound(int id = 0, float pitch = -1)
    {
        if (!DataManager.Instance.gameData.isSound) return;
        if (listDataSound != null && id < listDataSound.Length && listDataSound[id].clip != null && DataManager.Instance != null && DataManager.Instance.gameData != null)
        {
            audioSource.clip = listDataSound[id].clip;
            audioSource.volume = listDataSound[id].volume;
            audioSource.pitch = pitch >= 0 ? pitch : listDataSound[id].pitch;
            audioSource.loop = listDataSound[id].isLoop;
            audioSource.PlayDelayed(listDataSound[id].delay);
        }
    }

    public void PlaySoundOneShot(int id = 0, float pitch = -1, float delay = 0, float volumnScale = 1, bool followNewVolum = false)
    {
        if (!DataManager.Instance.gameData.isSound) return;
        if (listDataSound != null && id < listDataSound.Length && listDataSound[id].clip != null && DataManager.Instance != null && DataManager.Instance.gameData != null)
        {
            audioSource.clip = listDataSound[id].clip;
            audioSource.volume = listDataSound[id].volume;
            audioSource.pitch = pitch >= 0 ? pitch : listDataSound[id].pitch;
            audioSource.loop = listDataSound[id].isLoop;
            // audioSource.PlayDelayed(listDataSound[id].delay);
            if (followNewVolum == false)
            {
                volumnScale = listDataSound[id].volume;
            }
            this.id = id;
            StartCoroutine(delaySound(delay, volumnScale));
        }
    }
    IEnumerator delaySound(float delay, float volumnScale)
    {
        yield return Cache.GetWFS(delay);
        audioSource.PlayOneShot(listDataSound[id].clip, volumnScale);
    }

    public void Pause()
    {
        audioSource.Pause();
    }

    public void UnPause()
    {
        audioSource.UnPause();
    }

    public void Enable()
    {
        audioSource.enabled = true;
    }

    public void Disable()
    {
        audioSource.enabled = false;
    }
}
