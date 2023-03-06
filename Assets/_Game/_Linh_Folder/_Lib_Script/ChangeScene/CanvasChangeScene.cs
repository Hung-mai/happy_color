using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CanvasChangeScene : UICanvas
{
    public Animator animator;

    private UnityAction OnHideDoneAction;
    private UnityAction OnShowDoneAction;

    private CounterTime counterTime = new CounterTime();

    public void OnInit(UnityAction onHideAction, UnityAction onShowAction)
    {
        this.OnHideDoneAction = onHideAction;
        this.OnShowDoneAction = onShowAction;
    }

    private void Update()
    {
        counterTime.CounterExecute();
    }

    public override void Setup()
    {
        base.Setup();

        counterTime.CounterStart(null, Appear, 1.2f);
    }

    private void Appear()
    {
        animator.SetTrigger("appear");
        OnHideDoneAction?.Invoke();
        OnHideDoneAction = null;
        counterTime.CounterStart(null, Close, 1.2f);
    }

    public override void Close()
    {
        OnShowDoneAction?.Invoke();
        OnShowDoneAction = null;
        base.Close();
    }
}
