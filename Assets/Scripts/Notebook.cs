using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class Notebook : MonoBehaviour
{
    public TMP_InputField field;
    public TextMeshProUGUI pageCount;
    public GameObject backButton;
    public GameObject forwardButton;
    public GameObject notesPage;
    public Image notesTab;
    public GameObject evidencePage;
    public Image evidenceTab;
    public GameObject evidencePrefab;
    public GameObject evidencePopup;
    TextMeshProUGUI popupTitle;
    TextMeshProUGUI popupContent;


    public Color selectedTab;
    public Color unselectedTab;


    public float offScreenXPosition;
    public float onScreenXPosition;

    List<string> pages = new List<string>();
    int currentPage = 0;
    bool open = false;
    bool notes = true;

    Sequence s;

    RectTransform rTransform;

    public static Notebook instance;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(transform.parent.parent.gameObject);
        } else
        {
            Destroy(transform.parent.parent.gameObject);
        }

        pages.Add("");
        rTransform = transform.parent as RectTransform;
        rTransform.anchoredPosition = new Vector2(offScreenXPosition, rTransform.anchoredPosition.y);
        backButton.SetActive(false);
        forwardButton.SetActive(false);

        popupTitle = evidencePopup.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        popupContent = evidencePopup.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
    }

    public void SavePageContent()
    {
        pages[currentPage] = field.text;
    }

    public void turnPageForward()
    {
        currentPage++;
        if (notes)
        {
            field.text = pages[currentPage];
        } else
        {
            PopulateEvidence();
        }
        

        ValidateButtons();
    }

    public void turnPageBack()
    {
        currentPage--;

        if (notes)
        {
            field.text = pages[currentPage];
        } else
        {
            PopulateEvidence();
        }
        

        ValidateButtons();
    }

    public void AddPage()
    {
        pages.Add("");
        currentPage = pages.Count - 1;
        field.text = pages[currentPage];

        ValidateButtons();
    }

    public void PopupEvidence(int index)
    {
        evidencePopup.SetActive(true);

        Evidence e = GameManager.instance.discoveredEvidence[index + currentPage * 9];

        popupTitle.text = e.evidenceName;
        popupContent.text = e.evidenceDescription;
    }

    public void TabToNotes()
    {
        if (notes)
            return;
        
        notes = true;
        evidenceTab.color = unselectedTab;
        notesTab.color = selectedTab;
        evidencePage.SetActive(false);
        notesPage.SetActive(true);

        currentPage = 0;

        field.text = pages[currentPage];

        DepopulateEvidence();

        evidencePopup.SetActive(false);

        ValidateButtons();
    }


    public void TabToEvidence()
    {
        if (!notes)
            return;

        notes = false;
        evidenceTab.color = selectedTab;
        notesTab.color = unselectedTab;
        evidencePage.SetActive(true);
        notesPage.SetActive(false);
        
        currentPage = 0;

        PopulateEvidence();

        ValidateButtons();
    }

    public void PopulateEvidence()
    {
        DepopulateEvidence();
        Instantiate(evidencePrefab).transform.SetParent(evidencePage.transform);
        evidencePage.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "<u><b>Evidence</b></u>";

        List<Evidence> evidence = GameManager.instance.discoveredEvidence;

        for(int i = 0; i < 9; i++)
        {
            if(i + (currentPage * 9)< evidence.Count)
            {
                GameObject evidenceObject = Instantiate(evidencePrefab);
                evidenceObject.transform.SetParent(evidencePage.transform);
                TextMeshProUGUI evidenceText = evidenceObject.GetComponent<TextMeshProUGUI>();
                evidenceText.text = evidence[i + (currentPage*9)].evidenceName;
                evidenceObject.GetComponent<Button>().onClick.AddListener(delegate { PopupEvidence(evidenceObject.transform.GetSiblingIndex() - 1); });
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(evidencePage.transform as RectTransform);
    }

    public void DepopulateEvidence()
    {
        for(int i = evidencePage.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(evidencePage.transform.GetChild(i).gameObject);
        }
    }

    public void ValidateButtons()
    {
        pageCount.text = currentPage + 1 + "";

        if (notes)
        {
            if (currentPage + 1 >= pages.Count)
            {
                forwardButton.SetActive(false);
            }
            else
            {
                forwardButton.SetActive(true);
            }
            if (currentPage - 1 < 0)
            {
                backButton.SetActive(false);
            }
            else
            {
                backButton.SetActive(true);
            }
        } else if (!notes)
        {
            if ((currentPage + 1) * 9 >= GameManager.instance.discoveredEvidence.Count)
            {
                forwardButton.SetActive(false);
            }
            else
            {
                forwardButton.SetActive(true);
            }

            if (currentPage - 1 < 0)
            {
                backButton.SetActive(false);
            }
            else
            {
                backButton.SetActive(true);
            }
        }
        
    }

    public void OpenNotebook()
    {
        if(s.IsActive())
            s.Kill();

        Time.timeScale = 0;

        open = true;

        currentPage = 0;

        if (!notes)
            PopulateEvidence();

        ValidateButtons();

        s = DOTween.Sequence();
        s.Append(rTransform.DOAnchorPosX(onScreenXPosition, .3f)).SetUpdate(true)
        .AppendCallback(delegate { field.ActivateInputField(); }).SetUpdate(true);   
    }

    public void CloseNotebook()
    {
        if (s.IsActive())
            s.Kill();

        evidencePopup.SetActive(false);
        currentPage = 0;
        open = false;
        field.DeactivateInputField();   

        s = DOTween.Sequence();
        s.Append(rTransform.DOAnchorPosX(offScreenXPosition, .3f)).SetUpdate(true)
        .AppendCallback(delegate { Time.timeScale = 1; }).SetUpdate(true);
    }

    public void ToggleNotebook()
    {
        if (open)
        {
            CloseNotebook();
        }
        else
        {
            OpenNotebook();
        }
    }
}
