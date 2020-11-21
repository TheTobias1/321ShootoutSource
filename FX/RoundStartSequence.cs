using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundStartSequence : MonoBehaviour
{
    public AnimationEvent[] events;
    public static event ActionDelegate OnPlay;

    [System.Serializable]
    public struct AnimationEvent
    {
        public RoundStartAnimation eventAnimation;
        public float waitTime;
    }

    private void Start()
    {
        StartCoroutine(DoSequence());
    }

    public IEnumerator DoSequence()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(0.2f);

        for(int i = 0; i < events.Length; ++i)
        {
            AnimationEvent e = events[i];
            StartCoroutine(e.eventAnimation.RoundAnimation());

            if (i == events.Length - 1 && RoundStartSequence.OnPlay != null)
            {
                RoundStartSequence.OnPlay();
            }

            yield return new WaitForSeconds(e.waitTime);
        }
    }
}
