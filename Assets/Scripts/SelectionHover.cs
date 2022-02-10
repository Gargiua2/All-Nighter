using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
public class SelectionHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOScale(new Vector3(1.2f * Mathf.Sign(transform.localScale.x), 1.2f, 1.2f),.25f);
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOScale(new Vector3(1f * Mathf.Sign(transform.localScale.x), 1f, 1f), .15f);
    }

}
