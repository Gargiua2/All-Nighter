using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class NotificationPanel : MonoBehaviour
{
    public TextMeshProUGUI title;
    public TextMeshProUGUI content;

    public delegate void Callback();
    Callback onPanelClose = null;
    public static void SendNotification(string panelLabel, string panelText, Callback callback = null)
    {
        GameObject canvas = GameObject.Find("Screen Space Canvas");
        GameObject panel = Instantiate(Resources.Load("Notification", typeof(GameObject))) as GameObject;
        (panel.transform as RectTransform).SetParent(canvas.transform,false);
        panel.GetComponent<NotificationPanel>().InitializePanel(panelLabel, panelText, callback);
    }

    public void InitializePanel(string panelLabel, string panelText, Callback callback = null)
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(1, .3f).SetEase(Ease.OutBounce).SetUpdate(true);

        Time.timeScale = 0;

        title.text = panelLabel;
        content.text = panelText;
        onPanelClose = callback;
    }

    public void ClosePanel()
    {
        Time.timeScale = 1;

        Destroy(gameObject, .25f);
        transform.DOScale(0, .25f).SetEase(Ease.Flash).SetUpdate(true);
        onPanelClose?.Invoke();
    }

}
