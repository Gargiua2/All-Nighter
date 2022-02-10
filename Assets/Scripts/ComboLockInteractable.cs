using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
public class ComboLockInteractable : Interactable
{
    [Space, Header("---Combo Lock Settings---")]
    [Range(0, 9999)] public int combo;

    [Space, Header("Combo Lock Events")]
    public UnityEvent onComboSuccess;
    public UnityEvent onComboFail;
    public Transform door;

    [Space]
    public bool containsEvidence = false;
    public Evidence storedEvidence;

    [Space]
    public bool useSuccessNotification = false;
    public Notifcation successNotification;
    
    [Space]
    public bool useFailureNotification = false;
    public Notifcation failureNotification;
    public override void Start()
    {
        base.Start();

        generalEvents.AddListener(delegate { FindObjectOfType<ComboLockUI>(true).Open(this); });
    }

    public override void Interact()
    {
        //If we're tagged as using a notification, we send the notification and resolve our events after it has been dismissed.
        //Otherwise, we simply invoke our events and increment the time instantly.
        if (useNotification)
        {
            NotificationPanel.SendNotification(notification.title, notification.content, OnInteractEnd);
        }
        else
        {
            if (generalEvents != null)
                generalEvents.Invoke();
        }
    }

    public override void OnInteractEnd()
    {
        if (generalEvents != null)
            generalEvents.Invoke();
    }

    public void OnComboAttempt(int enteredCombo)
    {
        if(combo == enteredCombo) 
        {
            onComboSuccess.Invoke();

            if(containsEvidence)
                GameManager.instance.AddEvidence(storedEvidence);
            
            TimeManager.instance.IncrementTime(timeCost);

            if (useSuccessNotification)
                NotificationPanel.SendNotification(successNotification.title, successNotification.content);

            if (door != null)
            {
                Sequence s = DOTween.Sequence();
                s.Append(door.DOLocalRotate(new Vector3(0, 95, 0), .3f))
                .AppendInterval(2.5f)
                .Append(door.DOLocalRotate(Vector3.zero, .2f));

            }

            Destroy(this);
        } else
        {
            onComboFail.Invoke();
            TimeManager.instance.IncrementTime(timeCost);

            if (useFailureNotification)
                NotificationPanel.SendNotification(failureNotification.title, failureNotification.content);
        }

    }
}
