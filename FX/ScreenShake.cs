using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public static ScreenShake screenShaker;
    public Transform cameraToShake;

    public float shakeMultiplier;
    public float shakeSpeed;
    public float shakeLerp;
    private float currentShake = 0;
    private float randomOffset;

    // Start is called before the first frame update
    void Start()
    {
        ScreenShake.screenShaker = this;
        randomOffset = Random.Range(0, 200);
        //InvokeRepeating("Test", 3, 3);
    }

    // Update is called once per frame
    void Update()
    {
        if(currentShake > 0.13f)
        {
            float shake = (Mathf.PerlinNoise(Time.time * shakeSpeed, randomOffset) - 0.5f) * shakeMultiplier * currentShake;
            cameraToShake.eulerAngles = new Vector3(cameraToShake.eulerAngles.x, cameraToShake.eulerAngles.y, shake);
            currentShake = Mathf.Lerp(currentShake, 0, shakeLerp * Time.deltaTime);
        }
        else
        {
            currentShake = 0;
            cameraToShake.eulerAngles = new Vector3(cameraToShake.eulerAngles.x, cameraToShake.eulerAngles.y, 0);
        }
    }

    public void SetShake(float amount)
    {
        if(currentShake < amount)
        {
            currentShake = amount;
        }
    }

    public void Test()
    {
        SetShake(2);
    }

    public static void Shake(float amount)
    {
        ScreenShake shaker = ScreenShake.screenShaker;
        if (shaker != null)
        {
            shaker.SetShake(amount);
        }
    }
}
