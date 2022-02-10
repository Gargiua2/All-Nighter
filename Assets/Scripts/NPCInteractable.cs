using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Yarn.Unity;

public class NPCInteractable : Interactable
{
    [Space, Header("---NPC Settings---")]
    public NPC character;
    public LocationAppointment attendedAppointment;
    Transform player;
    SpriteRenderer render;
    Vector3 offset;
    public override void Start()
    {
        base.Start();
        
        transform.DOScale(Vector3.one, .5f).SetEase(Ease.OutBounce);
        player = FindObjectOfType<PlayerController>().transform;
        render = GetComponent<SpriteRenderer>();
        
        BoxCollider c = gameObject.AddComponent<BoxCollider>();
        Vector2 spriteSize = render.sprite.bounds.size;
        c.size = new Vector3(spriteSize.x, spriteSize.y, .25f);
        c.center = new Vector3(c.center.x, c.size.y / 2, c.center.z);


        offset = Camera.main.gameObject.GetComponent<CameraController>().offset;
    }

    void Update()
    {
        if(Vector3.Distance(transform.position, player.position) < range * 1.5f)
        {
            Vector3 offsetPosition = transform.position + offset;
            Plane p = new Plane(transform.position, new Vector3(offsetPosition.x, transform.position.y, offsetPosition.z), offsetPosition);

            if (p.GetSide(player.position))
            {
                render.flipX = false;
            }
            else
            {
                render.flipX = true;
            }
        }
        
    }
    public bool TryUnload()
    {
        int time = TimeManager.instance.time.GetMinuteTime();

        if (character.GetLocationAppointment(time).time.GetMinuteTime() != attendedAppointment.time.GetMinuteTime())
        {
            Unload();
            return true;
        }

        return false;
    }

    public override void Interact()
    {

        YarnProgram p = character.GetDialgoueAppointment(TimeManager.instance.time.GetMinuteTime()).dialogueScript;
        if (p != null)
        {
            DialogueRunner runner = FindObjectOfType<DialogueRunner>();

            if (!runner.NodeExists(GetStartNodeName(p))){
                runner.startNode = GetStartNodeName(p);
                runner.Add(p);
                runner.StartDialogue();

                runner.onDialogueComplete.AddListener(OnInteractEnd);

                HideMarker();
            }
            
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

    public override void OnInteractEnd()
    {
        TimeManager.instance.IncrementTime(timeCost);
        FindObjectOfType<DialogueRunner>().onDialogueComplete.RemoveListener(OnInteractEnd);
    }

    public override void RevealMaker()
    {
        YarnProgram current = character.GetDialgoueAppointment(TimeManager.instance.time.GetMinuteTime()).dialogueScript;

        //Only show the marker if the NPC has something to say currently.
        if (current != null)
        {
            DialogueRunner runner = FindObjectOfType<DialogueRunner>();

            //Only show the marker if the NPC has something to say and hasn't already said it this loop.
            
            if (!runner.NodeExists(GetStartNodeName(current)))
            {
                base.RevealMaker();
                marker.GetComponent<SpriteRenderer>().color = character.color;
            }
        }
        
    }

    void Unload()
    {
        Destroy(gameObject, .25f);
        transform.DOScale(Vector3.zero, .24f).SetEase(Ease.OutExpo);
    }
}
