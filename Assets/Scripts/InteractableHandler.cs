using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableHandler : MonoBehaviour
{
    
    public List<Interactable> interactables;
    
    void Update()
    {
        if (Time.timeScale == 0 || GameManager.paused || GameManager.instance.sceneLoadedLength < 4f)
            return;

        Interactable nearestInRange = null;
        float closestDistance = float.MaxValue;
        foreach(Interactable i in interactables)
        {
            float d = Vector3.Distance(transform.position, i.transform.position);
            if (d < i.range)
            {
                if(d < closestDistance && i.enabled)
                {
                    nearestInRange = i;
                    closestDistance = d;
                }
            }
        }

        foreach(Interactable i in interactables)
        {
            if(i != nearestInRange)
            {
                i.HideMarker();
            }
            else
            {
                i.RevealMaker();
            }
        }

        if (Input.GetKeyDown(KeyCode.E) && nearestInRange != null)
        {
            nearestInRange.Interact();
        }
    }
}
