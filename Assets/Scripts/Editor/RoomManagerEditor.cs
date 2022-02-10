using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(RoomManager))]
public class RoomManagerEditor : Editor
{

    private void OnSceneGUI()
    {
        RoomManager t = target as RoomManager;

        GUIStyle style = new GUIStyle();
        style.fontSize = 20;
        style.fontStyle = FontStyle.Bold;
        style.alignment = TextAnchor.MiddleCenter;
        

        for(int i = 0; i < t.NPCIdlePoints.Count; i++)
        {
            Handles.Label(t.NPCIdlePoints[i].position + Vector3.up * 10, "NPC Point " + (i).ToString(), style);
        }

        for (int i = 0; i < t.doorEntryPoints.Count; i++)
        {
            Handles.Label(t.doorEntryPoints[i].position + Vector3.up * 10, "Door Entry " + (i).ToString(), style);
        }
    }
}
