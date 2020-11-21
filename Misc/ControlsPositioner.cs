using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsPositioner : MonoBehaviour
{
    public string controlsKey;
    public RectTransform rectTransform;

    // Start is called before the first frame update
    void Start()
    {
        if(PlayerPrefs.HasKey(controlsKey + "X"))
        {
            Vector2 anchor = new Vector2(PlayerPrefs.GetFloat(controlsKey + "X"), PlayerPrefs.GetFloat(controlsKey + "Y"));
            PositionControls(anchor);
        }
    }

    public void PositionControls(Vector2 anchor)
    {
        rectTransform.anchorMin = anchor;
        rectTransform.anchorMax = anchor;
        rectTransform.anchoredPosition = Vector3.zero;
    }

    public void StorePosition(Vector2 anchor)
    {
        PlayerPrefs.SetFloat(controlsKey + "X", anchor.x);
        PlayerPrefs.SetFloat(controlsKey + "Y", anchor.y);

        PlayerPrefs.Save();
    }

    private void OnValidate()
    {
        if(rectTransform == null)
            rectTransform = GetComponent<RectTransform>();
    }
}
