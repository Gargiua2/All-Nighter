using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimeDisplay : MonoBehaviour
{
    TextMeshProUGUI displayText;
    void Awake()
    {
        displayText = GetComponent<TextMeshProUGUI>();
    }


    void Update()
    {
        displayText.text = TimeManager.instance.GetTimeAsString();
    }
}
