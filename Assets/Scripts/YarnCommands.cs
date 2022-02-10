using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Yarn.Unity;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Yarn;
using DG.Tweening;
public class YarnCommands : MonoBehaviour
{
    public GameObject TestPortrait;
    DialogueRunner runner;
    public Transform holder;
    Image i;
    ReturningFunction implementation;
    void Start()
    {
        implementation += visited;
        i = holder.GetComponent<Image>();
        runner = GetComponent<DialogueRunner>();
        runner.AddFunction("visited", 1, implementation);
        GetComponent<DialogueUI>().OnCharacterAppear += replacePortrait;
    }

    [YarnCommand("decreaseSanity")]
    public void DialogSanity(string sanityAmount)
    {
        GameManager.instance.DecreaseSanity(Int32.Parse(sanityAmount));
    }

    [YarnCommand("setTime")]
    public void DialogTime(string timeAmount)
    {
        TimeManager.instance.IncrementTime(Int32.Parse(timeAmount));
    }

    [YarnCommand("debug")]
    public void DialogDebug(string debug)
    {
        Debug.Log(debug);
    }

    [YarnCommand("displayPortrait")]
    public void setPortrait(string[] portraitName)
    {
        holder.gameObject.SetActive(true);

        Sprite s = NPCManager.instance.GetCharacterSprite(string.Join(" ", portraitName));
        i.sprite = s;

    }

    [YarnCommand("switchPortrait")]
    public void replacePortrait(string[] newPortrait)
    {
        setPortrait(newPortrait);
    }

    [YarnCommand("bellsSFX")]
    public void playBells()
    {
        SoundManager.instance.TriggerLoopedBells();
    }

    [YarnCommand("accuse")]
    public void accuse()
    {
        Sequence s = DOTween.Sequence();
        s.AppendInterval(.1f);
        s.AppendCallback(()=>GameManager.instance.StartAccusation());
    }
    public object visited(params Value[] s)
    {
        string nodeName = s[0].AsString;
        Debug.Log(s[0].AsString);
        if (runner.NodeExists(nodeName))
        {
            return true;
        }

        return false;
    }

    [YarnCommand("addEvidence")]
    public void AddEvidence(string[] evidenceText)
    {
        Evidence e = new Evidence();

        string s = string.Join(" ",evidenceText);

        string title = s.Split(new char[] { ':' })[0].Trim();
        
        string body = s.Split(new char[] { ':' })[1].Trim();
        
        bool admissible = s.Split(new char[] { ':' })[2].Trim() == "true" || s.Split(new char[] { ':' })[2].Trim() == "True";

        e.evidenceName = title;
        e.evidenceDescription = body;
        e.admissable = admissible;

        GameManager.instance.AddEvidence(e);
    }
}
