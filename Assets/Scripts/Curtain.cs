using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;
public class Curtain : MonoBehaviour
{
    Image img;
    public delegate void Callback();
    Callback callback;
    void Awake()
    {
        img = GetComponent<Image>();
    }

    public void SetCurtainState(bool closed)
    {
        if (closed)
        {
            img.color = new Color(0, 0, 0, 1);
        } else
        {
            img.color = new Color(0, 0, 0, 0);
        }
    }

    public void OpenCurtain(Callback c = null)
    {
        callback = c;

        Sequence s = DOTween.Sequence().SetUpdate(true);
        s.AppendInterval(.3f).SetUpdate(true);
        s.Append(img.DOFade(0, 1f)).SetUpdate(true)
        .AppendCallback(OnCurtainDrawn).SetUpdate(true);
    }

    public void CloseCurtain(Callback c = null)
    {
        callback = c;

        Sequence s = DOTween.Sequence().SetUpdate(true);
        s.Append(img.DOFade(1, .5f)).SetUpdate(true)
        .AppendCallback(OnCurtainDrawn).SetUpdate(true);
    }

    void OnCurtainDrawn()
    {
        callback?.Invoke();
    }
}
