using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteractable : Interactable
{
    public Room connectedRoom;
    public int connectedDoorIndex;

    public override void Interact()
    {
        Time.timeScale = 0;
        FindObjectOfType<Curtain>().CloseCurtain(OnInteractEnd);
    }

    public override void OnInteractEnd()
    {
        GameManager.instance.LoadRoom(connectedRoom, connectedDoorIndex);
    }

}
