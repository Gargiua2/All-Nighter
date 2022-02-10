using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueSaver : MonoBehaviour
{
    public static DialogueSaver instance;
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
        }
        
    }

}
