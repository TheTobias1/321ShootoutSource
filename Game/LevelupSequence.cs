using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelupSequence : MonoBehaviour
{
    public Slider xpSlider;
    public AudioSource barAudio;

    public ActionDelegate OnDone;
    public IntegerDelegate OnLevelUp;

    public GameObject levelupSpawn;
    public GameObject rootObject;

    public float levelupPause;
    public float speed = 30;

    public void InitSlider(int starting, int boundaries)
    {
        xpSlider.value = (float)(starting % boundaries) / (float)boundaries * 100;
    }

    public void DoLevelUp(int starting, int ending, int boundaries, int maxXP)
    {
        StartCoroutine(LevelUp(starting, ending, boundaries, maxXP));
    }

    IEnumerator LevelUp(int starting, int ending, int boundaries, int maxXP)
    {
        rootObject.transform.localPosition = new Vector3(2000, rootObject.transform.localPosition.y, rootObject.transform.localPosition.z);

        LeanTween.moveLocal(rootObject, Vector3.zero, 0.5f);
        yield return new WaitForSeconds(0.6f);

        yield return null;

        int endLevel = XPManager.CalculateLevel(ending, boundaries);
        int maxLevel = XPManager.CalculateLevel(maxXP, boundaries);

        if(endLevel >= maxLevel)
            endLevel = maxLevel - 1;
            
        int curXP = starting;


        int level;

        do
        {
            level = XPManager.CalculateLevel(curXP, boundaries);

            int b = level * boundaries + boundaries - 1;
            int target = Mathf.Min(b, ending);

            float sliderTarget = (float)(target % boundaries) / (float)boundaries * 100;

            barAudio.Play();
            float waitTime = Mathf.Max(0.2f, (sliderTarget - xpSlider.value) / speed);
            LeanTween.value(gameObject, SetSliderValue, xpSlider.value, sliderTarget, waitTime / 2f);
            yield return new WaitForSeconds(waitTime / 2);
            barAudio.Pause();

            if (target != ending)
            {
                //LEVEL UP
                if(levelupSpawn != null)
                {
                    Instantiate(levelupSpawn);
                }

                yield return new WaitForSeconds(levelupPause);

                if (OnLevelUp != null)
                {
                    OnLevelUp(level + 1);
                }
                ++target;
            }

            if (ending > target)
            {
                xpSlider.value = 0;
            }

            curXP = target;

        } while (level != endLevel);


        yield return new WaitForSeconds(1f);
        Vector3 targetPos = new Vector3(-2500, rootObject.transform.localPosition.y, rootObject.transform.localPosition.z);
        LeanTween.moveLocal(rootObject, targetPos, 0.5f).setEaseInBack();

        yield return new WaitForSeconds(0.5f);


        if (OnDone != null)
        {
            OnDone();
        }


        rootObject.SetActive(false);
    }

    public void SetSliderValue(float s)
    {
        xpSlider.value = s;
    }

    public void OnDestroy()
    {
        OnDone = null;
        OnLevelUp = null;
    }
}
