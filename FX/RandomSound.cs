using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSound : MonoBehaviour
{
    public AudioSource source;
    public AudioClip[] clips;

    public bool playOnStart;
    public bool loop;
    public float delay = 0;

    // Start is called before the first frame update
    void Start()
    {
        if(playOnStart)
        {
            if(delay > 0)
            {
                Invoke("PlaySound", delay);
            }
            else
                PlaySound();
        }
    }


    public void PlaySound()
    {
        if(!loop)
            source.PlayOneShot(clips[Random.Range(0, clips.Length)]);
        else
        {
            source.clip = clips[Random.Range(0, clips.Length)];
            source.loop = true;
            source.Play();
        }
    }
}
