using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderPlayerPrefs : MonoBehaviour
{
    public Slider slider;
    public string key;
    private float currentValue;
    public bool inverse;
    // Start is called before the first frame update
    void Start()
    {
        //slider.onValueChanged.AddListener(delegate { SliderChanged(); });
        if(PlayerPrefs.HasKey(key))
        {
            slider.value = PlayerPrefs.GetFloat(key);
        }
    }

    public void SliderChanged()
    {
        PlayerPrefs.SetFloat(key, slider.value);
        PlayerPrefs.Save();
    }
}
