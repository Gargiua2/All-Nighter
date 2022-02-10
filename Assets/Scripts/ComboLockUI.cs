using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ComboLockUI : MonoBehaviour
{
    int firstDigit = 0;
    int secondDigit = 0;
    int thirdDigit = 0;
    int fourthDigit = 0;

    public TextMeshProUGUI comboDisplay;

    ComboLockInteractable targetLock;
    void Start()
    {
        UpdateTextDisplay();    
    }

    public void Open(ComboLockInteractable i)
    {
        targetLock = i;
        gameObject.SetActive(true);
        Time.timeScale = 0;
    }

    public void UpdateTextDisplay()
    {
        comboDisplay.text = fourthDigit + " " + thirdDigit + " " + secondDigit + " " + firstDigit;
    }

    public void AddToDigit(int digitPlace)
    {
        switch (digitPlace)
        {
            case 0:
                firstDigit++;
                if (firstDigit > 9)
                    firstDigit = 0;
                break;
            case 1:
                secondDigit++;
                if (secondDigit > 9)
                    secondDigit = 0;
                break;
            case 2:
                thirdDigit++;
                if (thirdDigit > 9)
                    thirdDigit = 0;
                break;
            case 3:
                fourthDigit++;
                if (fourthDigit > 9)
                    fourthDigit = 0;
                break;
        }

        UpdateTextDisplay();

    }

    public void SubtractFromDigit(int digitPlace)
    {
        switch (digitPlace)
        {
            case 0:
                firstDigit--;
                if (firstDigit < 0)
                    firstDigit = 9;
                break;
            case 1:
                secondDigit--;
                if (secondDigit < 0)
                    secondDigit = 9;
                break;
            case 2:
                thirdDigit--;
                if (thirdDigit < 0)
                    thirdDigit = 9;
                break;
            case 3:
                fourthDigit--;
                if (fourthDigit < 0)
                    fourthDigit = 9;
                break;
        }

        UpdateTextDisplay();

    }

    public void Cancel()
    {
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }

    public void AttemptUnlock()
    {
        Time.timeScale = 1;
        gameObject.SetActive(false);
        targetLock.OnComboAttempt(int.Parse(comboDisplay.text.Replace(" ", string.Empty)));
    }

}
