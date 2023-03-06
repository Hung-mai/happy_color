
using UnityEngine;
using UnityEngine.UI;

public class PlayFxButton : MonoBehaviour
{
    [SerializeField] FxID fx;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(PlayFx);
    }

    private void PlayFx()
    {
        SoundManager.Instance.PlayFx(fx);
    }
}