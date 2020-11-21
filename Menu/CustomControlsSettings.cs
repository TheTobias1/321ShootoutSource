using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomControlsSettings : MonoBehaviour
{
    public Toggle controlsToggle;
    public string key = "Controls";

    public bool ToggleControls
    {
        get { return customControls; }
        set
        {
            customControls = value;
            StoreControlSettings();
        }
    }
    public bool customControls;

    private void Start()
    {
        if (PlayerPrefs.HasKey(key))
        {
             controlsToggle.isOn = PlayerPrefs.GetInt(key) == 1;
        }
    }

    public void StoreControlSettings()
    {
        PlayerPrefs.SetInt(key, customControls? 1 : 0);
        PlayerPrefs.Save();
    }
}
