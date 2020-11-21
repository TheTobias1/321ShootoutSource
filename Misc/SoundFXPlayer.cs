using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFXPlayer : MonoBehaviour
{

    public static SoundFXPlayer SFX;
    public AudioSource source;

    private void Awake()
    {
        SoundFXPlayer.SFX = this;
    }

    public void PlaySound(AudioClip clip)
    {
        source.PlayOneShot(clip);
    }
    
}
