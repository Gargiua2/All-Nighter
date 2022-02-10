using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
public class AccusationNote : MonoBehaviour
{
    public TextMeshProUGUI note;
    public TextMeshProUGUI evidenceAText;
    public TextMeshProUGUI evidenceBText;
    public TextMeshProUGUI evidenceCText;

    string suspect;
    Evidence evidenceA;
    Evidence evidenceB;
    Evidence evidenceC;

    public void DisplayAccusation(string suspectName, Evidence A, Evidence B, Evidence C)
    {
        gameObject.SetActive(true);
        suspect = suspectName;
        evidenceA = A;
        evidenceB = B;
        evidenceC = C;

        note.text = note.text.Replace("XXX", suspectName);
        evidenceAText.text = A.evidenceName;
        evidenceBText.text = B.evidenceName;
        evidenceCText.text = C.evidenceName;
    }

    public void Send()
    {
        GameManager.instance.SendAccusation(suspect, evidenceA, evidenceB, evidenceC);
        Close();
    }

    public void Scrap()
    {
        note.text = note.text.Replace(suspect, "XXX");
        Close();
    }

    void Close()
    {
        Sequence s = DOTween.Sequence();
        s.Append(transform.DOScale(0, .26f)).SetUpdate(true)
        .AppendCallback(()=>gameObject.SetActive(false)).SetUpdate(true);
        Time.timeScale = 1;
    }

}
