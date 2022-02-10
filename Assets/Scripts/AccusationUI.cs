using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AccusationUI : MonoBehaviour
{
    public TextMeshProUGUI evidenceTitle;
    public TextMeshProUGUI evidenceContent;
    public TMP_Dropdown dropdown;
    public List<TextMeshProUGUI> chosenEvidence;
    public AccusationNote preview;

    List<Evidence> evidence = new List<Evidence>();
    string suspect;
    int evidenceIndex = 0;

    void Start()
    {
        suspect = dropdown.options[0].text;
    }

    public void Open()
    {
        gameObject.SetActive(true);
        evidenceTitle.text = GameManager.instance.discoveredEvidence[evidenceIndex].evidenceName;
        evidenceContent.text = GameManager.instance.discoveredEvidence[evidenceIndex].evidenceDescription;
    }

    public void SetSuspect(int _suspect)
    {
        suspect = dropdown.options[_suspect].text;
    }

    public void ScrollEvidenceForward()
    {
        evidenceIndex++;

        if (evidenceIndex >= GameManager.instance.discoveredEvidence.Count)
        {
            evidenceIndex = 0;
        } else if (evidenceIndex < 0)
        {
            evidenceIndex = GameManager.instance.discoveredEvidence.Count - 1;
        }

        evidenceTitle.text = GameManager.instance.discoveredEvidence[evidenceIndex].evidenceName;
        evidenceContent.text = GameManager.instance.discoveredEvidence[evidenceIndex].evidenceDescription;
    }

    public void ScrollEvidenceBack()
    {
        evidenceIndex--;

        if (evidenceIndex >= GameManager.instance.discoveredEvidence.Count)
        {
            evidenceIndex = 0;
        }
        else if (evidenceIndex < 0)
        {
            evidenceIndex = GameManager.instance.discoveredEvidence.Count - 1;
        }

        evidenceTitle.text = GameManager.instance.discoveredEvidence[evidenceIndex].evidenceName;
        evidenceContent.text = GameManager.instance.discoveredEvidence[evidenceIndex].evidenceDescription;
    }

    public void AddEvidence()
    {
        bool valid = true;

        foreach(Evidence e in evidence)
        {
            if(e.evidenceName == GameManager.instance.discoveredEvidence[evidenceIndex].evidenceName)
            {
                valid = false;
            }
        }

        if (!valid || evidence.Count == 3)
            return;

        evidence.Add(GameManager.instance.discoveredEvidence[evidenceIndex]);
        chosenEvidence[evidence.Count - 1].text = evidence[evidence.Count - 1].evidenceName;
    }

    public void Preview()
    {
        if (evidence.Count < 3)
            return;

        preview.DisplayAccusation(suspect, evidence[0], evidence[1], evidence[2]);
        evidence.Clear();
        foreach (TextMeshProUGUI tmp in chosenEvidence)
        {
            tmp.text = "";
        }

        evidenceIndex = 0;
        gameObject.SetActive(false);
    }

    public void Close()
    {
        evidence.Clear();
        foreach(TextMeshProUGUI tmp in chosenEvidence)
        {
            tmp.text = "";
        }

        evidenceIndex = 0;
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }
}
