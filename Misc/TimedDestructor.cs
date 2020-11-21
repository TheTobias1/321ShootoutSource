using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedDestructor : MonoBehaviour
{
    public float timeout;
    public GameObject deathPrefab;
    public bool resetRotation;

    public bool pooled;
    public bool deathPrefabPooled;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("Timeout", timeout);
    }


    void Timeout()
    {
        if(deathPrefab != null)
        {
            Quaternion rot = transform.rotation;
            if (resetRotation)
                rot = Quaternion.identity;

            if(!deathPrefabPooled)
                Instantiate(deathPrefab, transform.position, rot);
            else
                MF_AutoPool.Spawn(deathPrefab, transform.position, rot);
        }

        if (!pooled)
            Destroy(gameObject);
        else
            MF_AutoPool.Despawn(gameObject);
    }
}
