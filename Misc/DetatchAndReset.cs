using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetatchAndReset : MonoBehaviour
{
    public bool detatch;
    public bool resetTransform;

    void Awake()
    {
        if(detatch)
            transform.parent = null;

        if(resetTransform)
        {
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }
    }

}
