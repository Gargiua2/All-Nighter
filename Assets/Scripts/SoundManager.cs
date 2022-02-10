using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class SoundManager : MonoBehaviour
{
    public AudioClip bells;
    public AudioClip tic;
    public AudioClip mainTheme;
    public AudioClip mainThemeRewind;
    public AudioClip mainThemeMildScrewy;
    public AudioClip mainThemeVeryScrewy;
    AudioSource[] sources;

    #region Singleton
    public static SoundManager instance;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    #endregion

    void Start()
    {
        sources = GetComponents<AudioSource>();
        Debug.Log(sources.Length);
    }

    public void SetMainMusic(int track)
    {
        switch (track)
        {
            case 0:
                sources[0].clip = mainTheme;
                sources[0].Play();
                break;
            case 1:
                sources[0].clip = mainThemeRewind;
                sources[0].Play();
                break;
            case 2:
                sources[0].clip = mainThemeMildScrewy;
                sources[0].Play();
                break;
            case 3:
                sources[0].clip = mainThemeVeryScrewy;
                sources[0].Play();
                break;

        }
    }

    public void StopAudioSource(int source)
    {
        sources[source].Stop();
    }

    public void HourChime()
    {
        sources[1].PlayOneShot(bells);
    }

    public void MinuteTick()
    {
        sources[1].PlayOneShot(tic);
    }

    public void TriggerLoopedBells()
    {
        Debug.Log("BELLS");
        sources[1].loop = true;
        sources[1].clip = bells;
        sources[1].Play();
    }
}
