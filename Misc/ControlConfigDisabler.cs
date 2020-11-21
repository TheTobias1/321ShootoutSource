using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlConfigDisabler : MonoBehaviour
{
    public GameObject[] defaultLayoutConfig;
    public GameObject[] customLayoutConfig;

    public bool triggerOnEnable;

    private void Awake()
    {
        Configure();
    }

    private void Configure()
    {
        bool defaultConfig = true;

        if (PlayerPrefs.HasKey("Controls"))
        {
            if (PlayerPrefs.GetInt("Controls") == 1)
            {
                defaultConfig = false;
            }
        }

        foreach (GameObject g in defaultLayoutConfig)
        {
            g.SetActive(defaultConfig);
        }

        foreach (GameObject g in customLayoutConfig)
        {
            g.SetActive(!defaultConfig);
        }
    }

    private void OnEnable()
    {
        if(triggerOnEnable)
        {
            Configure();
        }
    }
}
