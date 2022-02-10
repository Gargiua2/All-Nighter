using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Yarn;

[CreateAssetMenu(fileName = "New NPC", menuName = "NPC")]
public class NPC : ScriptableObject
{
    public string characterName;
    public Color color;
    public Sprite overworldSprite;
    public Sprite dialoguePortrait;
    [Range(1, 2.5f)] public float height = 1;
    public List<LocationAppointment> locationSchedule;
    public List<DialogueAppointment> dialogueSchedule;

    public LocationAppointment GetLocationAppointment(int time)
    {
        for(int i = 0; i < locationSchedule.Count; i++)
        {
            int timeAtIndex = locationSchedule[i].time.GetMinuteTime();
            int timeAtNextIndex = (i + 1 < locationSchedule.Count) ? locationSchedule[i + 1].time.GetMinuteTime() : int.MaxValue;

            if(time >= timeAtIndex && time < timeAtNextIndex)
            {
                return locationSchedule[i];
            }
        }

        return locationSchedule[0];
    }

    public DialogueAppointment GetDialgoueAppointment(int time)
    {
        for (int i = 0; i < dialogueSchedule.Count; i++)
        {
            int timeAtIndex = dialogueSchedule[i].time.GetMinuteTime();
            int timeAtNextIndex = (i + 1 < dialogueSchedule.Count) ? dialogueSchedule[i + 1].time.GetMinuteTime() : int.MaxValue;

            if (time >= timeAtIndex && time < timeAtNextIndex)
            {
                return dialogueSchedule[i];
            }
        }

        return dialogueSchedule[0];
    }

    public void SortSchedules()
    {
        SortDialogueSchedule();
        SortLocationSchedule();
    }

    void SortLocationSchedule()
    {
        if (locationSchedule.Count == 0)
            return;

        List<LocationAppointment> sorted = new List<LocationAppointment>();
        while(locationSchedule.Count > 0)
        {
            int biggestValue = int.MinValue;
            int index = -1;

            for(int i = 0; i < locationSchedule.Count; i++)
            {
                if(locationSchedule[i].time.GetMinuteTime() > biggestValue)
                {
                    biggestValue = locationSchedule[i].time.GetMinuteTime();
                    index = i;
                }
            }

            sorted.Insert(0,locationSchedule[index]);
            locationSchedule.RemoveAt(index);
        }

        locationSchedule = sorted;
    }

    void SortDialogueSchedule()
    {
        if (dialogueSchedule.Count == 0)
            return;

        List<DialogueAppointment> sorted = new List<DialogueAppointment>();
        while (dialogueSchedule.Count > 0)
        {
            int biggestValue = int.MinValue;
            int index = -1;

            for (int i = 0; i < dialogueSchedule.Count; i++)
            {
                if (dialogueSchedule[i].time.GetMinuteTime() > biggestValue)
                {
                    biggestValue = dialogueSchedule[i].time.GetMinuteTime();
                    index = i;
                }
            }

            sorted.Insert(0, dialogueSchedule[index]);
            dialogueSchedule.RemoveAt(index);
        }

        dialogueSchedule = sorted;
    }

}

#if UNITY_EDITOR
[CustomEditor(typeof(NPC))]
public class NPCEditor : Editor
{
    bool catchChange = false;
    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();

        DrawDefaultInspector();

        if (EditorGUI.EndChangeCheck())
            catchChange = true;
        

        if(catchChange && !EditorGUIUtility.editingTextField)
        {
            NPC npc = target as NPC;
            npc.color = new Color(npc.color.r, npc.color.g, npc.color.b, 1);
            npc.SortSchedules();
            catchChange = false;
        }
    }
}
#endif
[System.Serializable]
public struct LocationAppointment
{
    public GameTime time;
    public Room room;
    public int location;
}

[System.Serializable]
public struct DialogueAppointment
{
    public GameTime time;
    public YarnProgram dialogueScript;
}