using UnityEngine;

[CreateAssetMenu(fileName = "Data_Sound", menuName = "ScriptableObjects/Data_Sound", order = 2)]
public class SoundData : ScriptableObject
{
    public AudioClip clip;
    public float volume = 1;
    public float delay = 0;
    public float pitch = 1;
    public bool isLoop = false;
}
