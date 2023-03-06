using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBase : MonoBehaviour
{
    [Header("------LevelAttribute--------")]
    public bool isPause = false;
    public bool isBlockControl = false;
    public bool isEndLevel = false;
    [SerializeField]private float timeDelayEndGame = 0.5f;
    public void PauseLevel()
    {
        Time.timeScale = 0;
        isPause = true;
    }
    public void ContinueLevel()
    {
        Time.timeScale = 1;
        isPause = false;
    }
    public IEnumerator EndLevel(bool isWin)
    {
        if (!isEndLevel)
        {
            isEndLevel = true;
            isBlockControl = true;
            yield return new WaitForSeconds(timeDelayEndGame);
        }
        
    }
    public virtual void HintFunction()
    {

    }
    public virtual void MixFunction()
    {

    }
    public virtual void ReviveLevel()
    {

    }
    
}
