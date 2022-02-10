using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public void TriggerLoop()
    {
        GameManager.instance.Reset();
    }
}
