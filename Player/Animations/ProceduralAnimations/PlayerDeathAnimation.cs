using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerDeathAnimation : MonoBehaviour
{
    public Volume volume;
    Bloom bloom;
    ColorAdjustments colorFilter;

    public float bloomPeakAmount;
    public Vector2 bloomLerp;
    public int bloomStage = 0;

    public float retractSpeed = 50;
    public float lerpAmount = 2;
    public Camera cam;

    private void Start()
    {
        if(volume != null)
        {
            volume.profile.TryGet<Bloom>(out bloom);
            volume.profile.TryGet<ColorAdjustments>(out colorFilter);
        }
        else
        {
            bloomStage = -1;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position += transform.forward * -retractSpeed * Time.fixedDeltaTime;
        retractSpeed = Mathf.Lerp(retractSpeed, 0.1f, lerpAmount * Time.fixedDeltaTime);

        cam.fieldOfView += 3 * Time.fixedDeltaTime;

        if(bloomStage == 0)
        {
            bloom.intensity.value = Mathf.Lerp(bloom.intensity.value, bloomPeakAmount, bloomLerp.x * Time.fixedDeltaTime);
            if (bloom.intensity.value > bloomPeakAmount - 5)
                bloomStage = 1;
        }
        else
        {
            bloom.intensity.value = Mathf.Lerp(bloom.intensity.value, 2, bloomLerp.y * Time.fixedDeltaTime);
        }

        colorFilter.saturation.value = Mathf.Lerp(colorFilter.saturation.value, -100, 5 * Time.fixedDeltaTime);
        colorFilter.postExposure.value -= Time.fixedDeltaTime * 4;
    }
}
