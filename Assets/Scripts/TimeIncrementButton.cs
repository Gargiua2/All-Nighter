using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TimeIncrementButton : MonoBehaviour
{
    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(delegate { TimeManager.instance.IncrementTime(TimeManager.instance.minuteIncrememntSize); });    
    }
}
