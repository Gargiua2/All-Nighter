using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn;
using Yarn.Unity;
using UnityEngine.UI;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    public GameObject dialogue;
    [Range(0,100)]public float sanity = 100;
    [Range(0, 100)] public float loopSanityCost = 10;
    public Room startingRoom;
    public int startingDoorIndex = 0;

    public List<Evidence> discoveredEvidence;
    GameTime startingTime;
    int entryDoorIndex = 0;
    [HideInInspector] public float sceneLoadedLength = 0;
    VariableStorage yarnVariableStorage;

    public static bool paused = false;
    public List<YarnProgram> badEndings;
    #region Singleton
    public static GameManager instance;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    #endregion

    void Start()
    {
        discoveredEvidence = new List<Evidence>();
        startingTime = TimeManager.instance.time;
        paused = false;
    }
    [HideInInspector] public AccusationUI accuse;
    void Update()
    {
        sceneLoadedLength += Time.deltaTime;
    }

    public void StartAccusation()
    {
        if(sanity == 100)
        {
            NotificationPanel.SendNotification("No Reason!", "This party is just getting started, there's no reason to send a telegram now!");
            return;
        }

        if(discoveredEvidence.Count < 3)
        {
            NotificationPanel.SendNotification("Evidence?", "If you want to contact the Agency, you'll need to find more evidence first!");
            return;
        }

        Time.timeScale = 0;
        accuse.Open();
    }

    public static void Pause()
    {
        Debug.Log("Pausing");
        paused = true;
    }

    [Header("---Accusation Stuff---")]
    public NPC culprit;
    public YarnProgram SuccessfulAccusation;
    public void SendAccusation(string culpritName, Evidence A, Evidence B, Evidence C)
    {
        Debug.Log(culpritName + " " + A.admissable + " " + B.admissable + " " + C.admissable);
        if(culprit.characterName.Trim().ToLower() == culpritName.Trim().ToLower() && A.admissable && B.admissable && C.admissable)
        {
            TriggerAccusationSuccess();
        } else
        {
            TriggerAccusationFail();
        }
    }

    void TriggerAccusationFail()
    {
        Sequence s = DOTween.Sequence();
        s.AppendInterval(.3f);
        s.AppendCallback(() =>
        {
            NotificationPanel.SendNotification("No Luck", "You send the accusation off. You wait for the Agency's appearance. They don't show.", delegate { TriggerBadEnding(); });
        });
    }

    void TriggerAccusationSuccess()
    {
        Sequence s = DOTween.Sequence();
        s.AppendInterval(.3f);
        s.AppendCallback(() => 
        {
            NotificationPanel.SendNotification("Waiting...", "You send the accusation off. You wait for midnight with anticpation", delegate { TriggerYarn(SuccessfulAccusation); });
        });
    }

    public void TriggerYarn(YarnProgram p)
    {
        DialogueRunner runner = FindObjectOfType<DialogueRunner>();
        FindObjectOfType<DialogueUI>().onDialogueEnd.AddListener(WinGame);
        SoundManager.instance.SetMainMusic(0);

        runner.startNode = GetStartNodeName(p);
        runner.Add(p);
        runner.StartDialogue();
    }

    void WinGame()
    {
        FindObjectOfType<Curtain>().CloseCurtain();
    }

    public static void Unpause()
    {
        Debug.Log("Unpausing");
        paused = false;
    }
    bool freshReset = false;
    public void Reset()
    {
        TimeManager.instance.time = startingTime;
        DecreaseSanity(loopSanityCost);
        Time.timeScale = 0;
        DestroyImmediate(GameObject.Find("Dialogue"));
        DialogueSaver.instance = null;
        Instantiate(dialogue).name = "Dialogue";
        FindObjectOfType<Curtain>().CloseCurtain(delegate { LoadRoom(startingRoom, 0); });
        freshReset = true;
    }

    public void AddEvidence(Evidence e)
    {
        bool isNew = true;
        foreach(Evidence discovered in discoveredEvidence)
        {
            if(e.evidenceName == discovered.evidenceName)
            {
                isNew = false;
                break;
            }
        }

        if (isNew)
            discoveredEvidence.Add(e);
    }

    public void DecreaseSanity(float amount)
    {
        sanity -= amount;

        if(yarnVariableStorage != null)
        {
            Value yarnSanity = new Value(sanity);
            yarnVariableStorage.SetValue("$sanity", yarnSanity);
        } else
        {
            Debug.LogWarning("Trying to edit sanity in variable storage, but can't find variable storage!");
        }
    }

    public void LoadRoom(Room room, int entryIndex)
    {
        NPCManager.instance.UnloadAllNPCs();
        FindObjectOfType<DialogueUI>().onDialogueStart.RemoveListener(Pause);
        FindObjectOfType<DialogueUI>().onDialogueEnd.RemoveListener(Unpause);
        entryDoorIndex = entryIndex;
        SceneManager.LoadScene((int)room, LoadSceneMode.Single);
    }

    [Space, Header("--- Character Select Garbage ---")]
    bool selectedAvatar = false;
    [HideInInspector]public int selectedCharacter;
    public Sprite birdAvatar;
    public Sprite birdPortrait;
    public Sprite catAvatar;
    public Sprite catPortratit;
    public Image charDialoguePortraitHolder;
    public GameObject selectionPanel;
    public void SelectAvatar(int character)
    {
        if (!selectedAvatar)
            FindObjectOfType<Curtain>().SetCurtainState(true);

        selectedCharacter = character;
        selectedAvatar = true;

        if(character == 0)
        {
            FindObjectOfType<PlayerController>().transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = birdAvatar;
            charDialoguePortraitHolder.sprite = birdPortrait;
        } else
        {
            FindObjectOfType<PlayerController>().transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = catAvatar;
            charDialoguePortraitHolder.sprite = catPortratit;
        }

        FindObjectOfType<Curtain>().OpenCurtain(delegate { Welcome(); });
        GameObject.Find("ChooseYourCharacter")?.SetActive(false);
        GameObject.Find("Screen Space Canvas").GetComponent<Canvas>().sortingOrder = 0;
    }

    public YarnProgram welcomeScript;
    bool welcomed = false;
    void Welcome()
    {
        if (!welcomed)
        {
            DialogueRunner runner = FindObjectOfType<DialogueRunner>();
            runner.startNode = GetStartNodeName(welcomeScript);
            runner.Add(welcomeScript);
            runner.StartDialogue();
            welcomed = true;
        }
    }

    public void TriggerBadEnding()
    {
        DialogueRunner runner = FindObjectOfType<DialogueRunner>();
        FindObjectOfType<DialogueUI>().onDialogueEnd.AddListener(Reset);
        SoundManager.instance.SetMainMusic(1);

        if(sanity > 91)
        {
            runner.startNode = GetStartNodeName(badEndings[0]);
            runner.Add(badEndings[0]);
            runner.StartDialogue();
        } else if (sanity > 80)
        {
            runner.startNode = GetStartNodeName(badEndings[1]);
            runner.Add(badEndings[1]);
            runner.StartDialogue();
        } else if (sanity > 70)
        {
            runner.startNode = GetStartNodeName(badEndings[2]);
            runner.Add(badEndings[2]);
            runner.StartDialogue();
        } else if (sanity <= 70)
        {
            runner.startNode = GetStartNodeName(badEndings[3]);
            runner.Add(badEndings[3]);
            runner.StartDialogue();
        }
        
    }

    public string GetStartNodeName(YarnProgram p)
    {
        List<string> nodes = new List<string>();

        foreach (string s in p.GetProgram().Nodes.Keys)
        {
            nodes.Add(s);
        }

        return nodes[0];
    }


    void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
    {
        if(selectedAvatar == true)
        {
            GameObject.Find("ChooseYourCharacter").SetActive(false);
        }

        sceneLoadedLength = 0;

        Debug.Log("Scene " + scene.buildIndex + " loading...");

        FindObjectOfType<DialogueUI>().onDialogueStart.AddListener(Pause);
        FindObjectOfType<DialogueUI>().onDialogueEnd.AddListener(Unpause);

        //Grab the RoomManager;
        RoomManager roomManager = FindObjectOfType<RoomManager>();

        //Load in our player. If a player exists in the scene already, simply use that but log a warning.
        PlayerController pController;
        if(FindObjectOfType<PlayerController>() == null)
        {
            pController = Instantiate(player).GetComponent<PlayerController>();
            pController.transform.position = roomManager.doorEntryPoints[entryDoorIndex].position + Vector3.up * 3f;
        } else
        {
            pController = FindObjectOfType<PlayerController>();
            Debug.LogWarning("Loaded room already contains a Player object! You may not want this.");
        }

        //Find our camera. If it doesn't already have a follow script, log a warning.
        //Move the camera into place.
        CameraController cam = Camera.main.gameObject.GetComponent<CameraController>();

        if (cam == null)
            Debug.LogError("CAMERA IN LOADED ROOM IS MISSING A CAMERA CONTROLLER SCRIPT");

        cam.target = pController.transform;
        cam.transform.position = pController.transform.position + cam.offset;

        charDialoguePortraitHolder = FindObjectOfType<Setter>().holder;
        accuse = FindObjectOfType<AccuseSetter>().accuse;

        if (selectedAvatar)
            SelectAvatar(selectedCharacter);

        FindObjectOfType<Curtain>().SetCurtainState(true);
        FindObjectOfType<Curtain>().OpenCurtain(OnCurtainDrawn);
    }

    public YarnProgram resetPrompt;
    void OnCurtainDrawn()
    {
        Time.timeScale = 1;
        NPCManager.instance.OnTimeIncrement(true);

        if (freshReset)
        {
            if (sanity == 90)
            {
                DialogueRunner runner = FindObjectOfType<DialogueRunner>();
                runner.startNode = GetStartNodeName(resetPrompt);
                runner.Add(resetPrompt);
                runner.StartDialogue();
            }

            if(sanity > 70)
            {
                SoundManager.instance.SetMainMusic(0);
                SoundManager.instance.StopAudioSource(1);
            } else if (sanity > 40)
            {
                SoundManager.instance.SetMainMusic(2);
                SoundManager.instance.StopAudioSource(1);
            } else
            {
                SoundManager.instance.SetMainMusic(3);
                SoundManager.instance.StopAudioSource(1);
            }
            
        }
        

        freshReset = false;
    }
}

[System.Serializable]
public struct Evidence
{
    public string evidenceName;
    [TextArea(1, 3)] public string evidenceDescription;
    public bool admissable;
}