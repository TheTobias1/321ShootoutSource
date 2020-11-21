using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseSlider : MonoBehaviour
{
    public static event FloatDelegate OnSensitivityChange;
    public static event FloatDelegate OnAccelerationChange;

    public Slider slider;
    public SliderTypes sliderType;
    public string key;

    private float currentValue;

    public void SliderChanged()
    {
        if(sliderType == SliderTypes.Sensitivity)
        {
            if(OnSensitivityChange != null)
            {
                OnSensitivityChange(slider.value);
            }
        }
        else if (sliderType == SliderTypes.Acceleration)
        {
            if (OnAccelerationChange != null)
            {
                OnAccelerationChange(slider.value);
            }
        }
    }
}
public enum SliderTypes { Sensitivity, Acceleration };