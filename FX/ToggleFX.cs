using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ToggleFX : MonoBehaviour
{
    public Volume postProcess;
    // Start is called before the first frame update
    void Awake()
    {
        if(PlayerPrefs.HasKey("FX"))
        {
            postProcess.enabled = PlayerPrefs.GetInt("FX") == 1;
        }
    }
}
