using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelControl : MonoBehaviour
{
    //Starts the game
    public void playGame()
    {
        SceneManager.LoadScene("Foyer");
    }
    //option on the start sceene
    public void quitGame()
    {
        Debug.Log("game quit!");
        Application.Quit();
    }
}
