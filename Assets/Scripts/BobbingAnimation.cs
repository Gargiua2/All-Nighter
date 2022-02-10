using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class BobbingAnimation : MonoBehaviour
{
    public float speed = .1f;
    public float height = .75f;
    public Ease easing;

    // Start is called before the first frame update
    void Start()
    {
        Sequence s = DOTween.Sequence();
        s.Append(transform.DOMoveY(transform.position.y + height, speed)).SetEase(easing);
        s.SetLoops(-1,LoopType.Yoyo).SetSpeedBased();
        s.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
