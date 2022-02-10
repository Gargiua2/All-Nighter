using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedStateChange : MonoBehaviour
{
    [Header("Time")]
    public GameTime stateChangeTime;

    [Space, Header("Component State Changes")]
    public List<Behaviour> toEnable;
    public List<Behaviour> toDisable;

    [Space,Header("Transform State Changes")]
    public bool changePosition;
    public Vector3 newPositionState;
    [Space]
    public bool changeRotation;
    public Vector3 newAngleState;
    [Space]
    public bool changeScale;
    public Vector3 newScaleState;

    bool stateChanged = false;

    void Start()
    {
        ValidateState();
        TimeManager.instance.OnTimeChanged += ValidateState;    
    }

    void ValidateState()
    {
        if(TimeManager.instance.time.GetMinuteTime() >= stateChangeTime.GetMinuteTime() && !stateChanged)
        {
            for(int i = 0; i < toEnable.Count; i++)
            {
                if(toEnable[i] != null)
                    toEnable[i].enabled = true;
            }

            for (int i = 0; i < toDisable.Count; i++)
            {
                if (toDisable[i] != null)
                    toDisable[i].enabled = false;
            }

            if (changePosition)
                transform.localPosition = newPositionState;

            if (changeRotation)
                transform.localEulerAngles = newAngleState;

            if (changeScale)
                transform.localScale = newScaleState;

            stateChanged = true;
        }
    }

    void OnDestroy()
    {
        TimeManager.instance.OnTimeChanged -= ValidateState;
    }

    void OnDisable()
    {
        TimeManager.instance.OnTimeChanged -= ValidateState;
    }


}
