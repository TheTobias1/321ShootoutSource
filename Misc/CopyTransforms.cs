using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CopyTransforms : MonoBehaviour
{
    public Transform[] startTransforms;
    public Transform[] targetTransforms;

    public bool place;

    // Update is called once per frame
    void Update()
    {
        if(place)
        {
            int count = Mathf.Min(startTransforms.Length, targetTransforms.Length);
            for (int i = 0; i < count; ++i)
            {
                targetTransforms[i].position = startTransforms[i].position;
                targetTransforms[i].rotation = startTransforms[i].rotation;
            }

            place = false;
        }
    }
}
