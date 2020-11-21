using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayOnEnable : MonoBehaviour
{
    public AP_Reference poolManager;

    public ParticleSystem particle;
    public AudioSource audioSource;
    public RandomSound soundPlayer;

    private void OnEnable()
    {
        if(particle != null)
            particle.Play();

        if (audioSource != null)
        {
            if (audioSource.isPlaying)
                audioSource.Stop();
            audioSource.Play();
        }
            
        if(soundPlayer != null)
        {
            soundPlayer.PlaySound();
        }
    }
}
