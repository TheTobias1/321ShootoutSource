using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchInputDisabler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        gameObject.SetActive(false);
#endif
    }
}
