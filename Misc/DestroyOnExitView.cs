using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnExitView : MonoBehaviour
{
    public float MinimumTime;
    float destroyTime;

    public GameObject target;

    private void Start()
    {
        destroyTime = Time.time + MinimumTime;
    }

    private void OnBecameInvisible()
    {
        if(Time.time > destroyTime)
        {
            Destroy(target == null? gameObject : target);
        }
    }
}
