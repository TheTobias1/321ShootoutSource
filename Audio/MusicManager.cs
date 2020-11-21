using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager music;

    public AudioSource musicSource;
    public AudioHighPassFilter highPass;
    public AudioLowPassFilter lowPass;

    public float highPassFrequency;
    public float lowPassFrequency;

    public int state;

    private void Start()
    {
        MusicManager.music = this;

        if(PlayerPrefs.HasKey("Music"))
        {
            musicSource.mute = PlayerPrefs.GetInt("Music") == 0;
        }
    }

    public static void FadeHighpass()
    {
        if(MusicManager.music != null)
        {
            MusicManager.music.state = 1;
            music.highPass.enabled = true;
            music.lowPass.enabled = false;
        }
    }

    public static void FadeToNormal()
    {
        if (MusicManager.music != null)
        {
            MusicManager.music.state = 0;
        }

    }

    public static void FadeLowpass()
    {
        if (MusicManager.music != null)
        {
            MusicManager.music.state = -1;
            music.highPass.enabled = false;
            music.lowPass.enabled = true;
        }

    }

    public static void Fadeout()
    {
        if (MusicManager.music != null)
        {
            MusicManager.music.state = -2;
        }

    }


    private void Update()
    {
        if(state == 1)
        {
            highPass.cutoffFrequency = Mathf.Lerp(highPass.cutoffFrequency, highPassFrequency, 6 * Time.deltaTime);
            lowPass.cutoffFrequency = Mathf.Lerp(lowPass.cutoffFrequency, 22000, 6 * Time.deltaTime);
        }
        else if(state == -1)
        {
            highPass.cutoffFrequency = Mathf.Lerp(highPass.cutoffFrequency, 0, 6 * Time.deltaTime);
            lowPass.cutoffFrequency = Mathf.Lerp(lowPass.cutoffFrequency, lowPassFrequency, 6 * Time.deltaTime);
        }
        else if(state == 0)
        {
            lowPass.cutoffFrequency = Mathf.Lerp(lowPass.cutoffFrequency, 22000, 6 * Time.deltaTime);
            highPass.cutoffFrequency = Mathf.Lerp(highPass.cutoffFrequency, 0, 6 * Time.deltaTime);

            if(lowPass.cutoffFrequency > 21900 && highPass.cutoffFrequency < 100)
            {
                highPass.cutoffFrequency = 0;
                lowPass.cutoffFrequency = 22000;
                music.highPass.enabled = false;
                music.lowPass.enabled = true;
                state = 2;
            }
        }
        else if(state == -2)
        {
            musicSource.volume = Mathf.Lerp(musicSource.volume, 0, 2 * Time.deltaTime);
        }
    }

    public static void PlayMusic()
    {
        if (MusicManager.music != null)
        {
            if(!MusicManager.music.musicSource.isPlaying)
            {
                MusicManager.music.musicSource.Play();
            }
        }
    }
}
