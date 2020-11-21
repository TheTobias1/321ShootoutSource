using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectSliderOnEnable : MonoBehaviour
{
    public Slider slider;

    private void OnEnable()
    {
        slider.Select();
    }
}
