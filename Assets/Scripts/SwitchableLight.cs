using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
public class SwitchableLight : MonoBehaviour
{
     Transform target;

    bool followTarget = false;

    Sequence s;
    Vector3 vel;

    void Start()
    {
         
    }

    public void TurnOn()
    {
        if (s != null)
            s.Kill();

        s = DOTween.Sequence();
        s.Append(GetComponent<Light>().DOIntensity(100, .25f).SetEase(Ease.Flash));
    }

    public void TurnOff()
    {
        if (s != null)
            s.Kill();

        s = DOTween.Sequence();
        s.Append(GetComponent<Light>().DOIntensity(0, .4f).SetEase(Ease.OutBack));
    }

    public void ToggleFollowState()
    {
        followTarget = !followTarget;
    }
    void Update()
    {
        target = FindObjectOfType<PlayerController>().transform;

        if (followTarget)
            transform.forward = Vector3.SmoothDamp(transform.forward, (target.transform.position - transform.position).normalized, ref vel, .2f);
    }
}
