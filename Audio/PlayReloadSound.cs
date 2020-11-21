using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayReloadSound : MonoBehaviour
{
    public AudioSource source;
    public AudioClip clipRemoval;
    public AudioClip clipInsertion;
    
    public void PlayClipRemoval()
    {
        source.PlayOneShot(clipRemoval);
    }

    public void PlayClipInsertion()
    {
        source.PlayOneShot(clipInsertion);
    }
}
