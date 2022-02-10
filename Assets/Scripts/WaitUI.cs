using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WaitUI : MonoBehaviour
{
    [HideInInspector] public GameTime timeToIncrement = new GameTime(Vector3Int.zero);

    public Button addHours;
    public Button subtractHours;
    public Button addMinutes;
    public Button subtractMinutes;
    public Button apply;
    public Button cancel;
    public TextMeshProUGUI timePreview;

    void Awake()
    {
        addHours.onClick.AddListener(incrementHour);
        subtractHours.onClick.AddListener(decrementHour);
        addMinutes.onClick.AddListener(incrementMinute);
        subtractMinutes.onClick.AddListener(decrementMinute);
        apply.onClick.AddListener(applyWait);
        cancel.onClick.AddListener(cancelWait);
    }

    void OnEnable()
    {
        Time.timeScale = 0;
        Update();
    }

    void Update()
    {
        if(timeToIncrement.clockTime.y == 0 && timeToIncrement.clockTime.x == 0)
        {
            subtractHours.gameObject.SetActive(false);
        } else
        {
            subtractHours.gameObject.SetActive(true);
        }

        if (timeToIncrement.clockTime.z == 0 && timeToIncrement.clockTime.y == 0 && timeToIncrement.clockTime.x == 0)
        {
            subtractMinutes.gameObject.SetActive(false);
        }
        else
        {
            subtractMinutes.gameObject.SetActive(true);
        }

        GameTime preview = TimeManager.instance.time;
        preview.AddTime(timeToIncrement);
        timePreview.text = preview.GetDisplayTime();
    }

    void incrementHour()
    {
        timeToIncrement.AddTime(60);
    }

    void decrementHour()
    {
        timeToIncrement.AddTime(-60);
    }

    void incrementMinute()
    {
        timeToIncrement.AddTime(TimeManager.instance.minuteIncrememntSize);
    }

    void decrementMinute()
    {
        timeToIncrement.AddTime(-TimeManager.instance.minuteIncrememntSize);
    }

    void applyWait()
    {
        TimeManager.instance.IncrementTime(timeToIncrement);
        closePanel();
    }

    void cancelWait()
    {
        closePanel();
    }

    void closePanel()
    {
        Time.timeScale = 1;
        timeToIncrement = new GameTime(Vector3Int.zero);
        gameObject.SetActive(false);
    }
}
