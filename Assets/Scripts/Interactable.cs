using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
public class Interactable : MonoBehaviour
{
    [Header("---General Interactable Settings ---")]
    public float range;
    public int timeCost = 15;

    [Space, Header("Marker Settings")]
    public Vector3 markerOffset;
    public GameObject marker;

    [Space, Header("Basic Event Settings")]
    public UnityEvent generalEvents;
    [Space]
    public bool useNotification = false;
    public Notifcation notification;

    public virtual void Start()
    {
        //When our interactable spawns, initialize our marker object
        FindObjectOfType<InteractableHandler>().interactables.Add(this);
        marker = Instantiate(marker, transform.position + markerOffset, Quaternion.identity);
        marker.transform.eulerAngles = new Vector3(0, 45, 0);
        marker.SetActive(false);
    }

    //Methods for hiding and revealing our marker.
    public virtual void RevealMaker()
    {
        marker.SetActive(true);
    } 
    public void HideMarker()
    {
        marker.SetActive(false);
    }

    
    public virtual void Interact()
    {
        //If we're tagged as using a notification, we send the notification and resolve our events after it has been dismissed.
        //Otherwise, we simply invoke our events and increment the time instantly.
        if (useNotification)
        {
            NotificationPanel.SendNotification(notification.title, notification.content, OnInteractEnd);
        } else
        {
            if (generalEvents != null)
                generalEvents.Invoke();

            TimeManager.instance.IncrementTime(timeCost);
        }
    }

    public virtual void OnInteractEnd()
    {
        TimeManager.instance.IncrementTime(timeCost);

        if (generalEvents != null)
            generalEvents.Invoke();
    }

    void OnDestroy()
    {
        if(FindObjectOfType<InteractableHandler>()!= null)
            FindObjectOfType<InteractableHandler>().interactables.Remove(this);
        if(marker != null)
            Destroy(marker);
    }

    private void OnDisable()
    {
        if(marker != null)
            marker.SetActive(false);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position + markerOffset, .5f);
    }
}
[System.Serializable]
public struct Notifcation
{
    public string title;
    [TextArea(1,4)]public string content;
}
