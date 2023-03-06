using DG.Tweening;
using UnityEngine.UI;
using System;
using TMPro;
public static class DOTweenEx
{
    public static Tweener DOTextInt(this TMP_Text text, int initialValue, int finalValue, float duration, Func<int, string> convertor)

    {

        return DOTween.To(

             () => initialValue,

             it => text.text = convertor(it),

             finalValue,

             duration

         );

    }
    public static Tweener DOTextInt(this TMP_Text text, int initialValue, int finalValue, float duration)

    {

        return DOTweenEx.DOTextInt(text, initialValue, finalValue, duration, it => it.ToString());

    }
}
