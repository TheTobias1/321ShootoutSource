using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectButtonOnEnable : MonoBehaviour
{
    public Button selectableButton;
    public bool skipStart = false;

    private void OnEnable()
    {
        if(skipStart)
        {
            skipStart = false;
            return;
        }
        
        selectableButton.Select();
    }
}
