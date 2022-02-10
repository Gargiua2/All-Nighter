using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NPCManager : MonoBehaviour
{
    public List<NPC> characters;
    public Material spriteMaterial;
    public GameObject speechBubble;
    List<NPCInteractable> activeCharacters = new List<NPCInteractable>();

    #region Singleton
    public static NPCManager instance;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    #endregion


    //Called when the time increments. Does the hard work of deloading NPC instances which need to move, and loading in new instances.
    public void OnTimeIncrement(bool initialLoad = false)
    {
        //Get the minute time of our new time, to use for comparsions.
        int time = TimeManager.instance.time.GetMinuteTime();

        //Create a list to store any NPC instances that may need to be unloaded this timestep.
        List<NPCInteractable> deactivating = new List<NPCInteractable>();
        foreach (NPCInteractable active in activeCharacters)
        {
            //Query each NPC instance to see if they need to unload.
            if (active.TryUnload())
            {
                //If they do need to unload, add them to our list of unloading NPC instances.
                deactivating.Add(active);
            }
        }

        //Remove each unloading NPC from our list of active NPC instances.
        foreach (NPCInteractable c in deactivating)
        {
            activeCharacters.Remove(c);
        }

        //Iterate over all NPCs in the game.
        foreach (NPC c in characters)
        {
            //Check that the NPC is even in this room.
            if ((int)c.GetLocationAppointment(time).room == SceneManager.GetActiveScene().buildIndex)
            {
                //If they are, check to see if we just deactived an NPC instance of that character.
                bool recentlyDeactivated = false;
                foreach (NPCInteractable i in deactivating)
                {
                    if (c == i.character)
                    {
                        recentlyDeactivated = true;
                        break;
                    }
                }

                //Also check to see if this NPC currently lacks a loaded instance
                bool noInstanceLoaded = true;
                foreach(NPCInteractable active in activeCharacters)
                {
                    if (active.character == c)
                    {
                        noInstanceLoaded = false;
                    }
                }

                //If we did deactive an instance of this NPC, or have don't have a loaded copy of this NPC, and they are in this room, it reload them in their new location!
                if (recentlyDeactivated || initialLoad || noInstanceLoaded)
                {
                    LoadNPC(c);
                }
            }
        }

    }

    public Sprite GetCharacterSprite(string characterName)
    {
        foreach(NPC c in characters)
        {
            if(c.characterName.ToLower() == characterName.ToLower())
            {
                return c.dialoguePortrait;
            }
        }

        if(characterName == "Masked Thief")
        {
            if (GameManager.instance.selectedCharacter == 0)
            {
                return GameManager.instance.catPortratit;
            } else
            {
                return GameManager.instance.birdPortrait;
            }
        }

        Debug.LogError("No portrait found associated with name " + characterName);
        return null;
    }

    public void UnloadAllNPCs()
    {
        for(int i = 0; i < activeCharacters.Count; i++)
        {
            Destroy(activeCharacters[i].gameObject);
        }

        activeCharacters = new List<NPCInteractable>();
    }

    //Called when an NPC needs to be loaded.
    void LoadNPC(NPC character)
    {
        LocationAppointment currentAppointment = character.GetLocationAppointment(TimeManager.instance.time.GetMinuteTime());

        GameObject instance = new GameObject(character.characterName);

        SpriteRenderer renderer = instance.AddComponent<SpriteRenderer>();
        renderer.sprite = character.overworldSprite;
        renderer.material = spriteMaterial;
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.TwoSided;
        renderer.receiveShadows = true;

        NPCInteractable npc = instance.AddComponent<NPCInteractable>();
        npc.character = character;
        npc.attendedAppointment = currentAppointment;
        npc.range = 10f;
        npc.timeCost = TimeManager.instance.minuteIncrememntSize;
        npc.marker = speechBubble;
        npc.markerOffset = Vector2.up * 5f * character.height;
        activeCharacters.Add(npc);

        instance.transform.position = GameObject.Find("RoomManager").GetComponent<RoomManager>().NPCIdlePoints[currentAppointment.location].position;
        instance.transform.eulerAngles = new Vector3(0, 35, 0);
        instance.transform.localScale = Vector3.zero;
        //transform.parent = transform;
    }
}
