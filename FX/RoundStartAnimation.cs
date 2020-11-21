using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoundStartAnimation : MonoBehaviour
{
    public Text roundText;
    public Image animImage;
    
    public float largeSize = 1.4f;
    public float smallSize = 0.6f;
    public float scaleLerpSpeed = 7;
    public float fadeTime = 0.5f;
    public float holdTime = 0.2f;
    public string text;
    public bool isRoundText;

    public bool playOnStart;

    public AudioSource sfx;
    public AudioClip soundClip;
    // Start is called before the first frame update
    void Start()
    {
        MaskableGraphic g = (roundText == null) ? (MaskableGraphic)animImage : (MaskableGraphic)roundText;
        g.CrossFadeAlpha(0, 0, true);
        if (playOnStart)
            StartCoroutine(RoundAnimation());
    }

    public IEnumerator RoundAnimation()
    {
        if(isRoundText)
            roundText.text = "Round " + SessionManager.currentRound;
        else if(roundText != null)
            roundText.text = text;

        MaskableGraphic g = (roundText == null) ? (MaskableGraphic)animImage: (MaskableGraphic)roundText;

        g.transform.localScale = new Vector3(largeSize, largeSize, largeSize);

        g.CrossFadeAlpha(1, fadeTime, false);
        float waitUntil = Time.time + fadeTime + holdTime;
        //Invoke("PlaySound", fadeTime - 0.15f);
        PlaySound();

        while(Time.time < waitUntil)
        {
            g.transform.localScale = Vector3.Lerp(g.transform.localScale, new Vector3(1, 1, 1), scaleLerpSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        g.CrossFadeAlpha(0, fadeTime, false);
        waitUntil = Time.time + fadeTime;

        while (Time.time < waitUntil)
        {
            g.transform.localScale = Vector3.Lerp(g.transform.localScale, new Vector3(smallSize, smallSize, smallSize), scaleLerpSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }

    public void PlaySound()
    {
        if(sfx != null && soundClip != null)
            sfx.PlayOneShot(soundClip);
    }
}
