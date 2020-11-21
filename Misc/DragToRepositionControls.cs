using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragToRepositionControls : MonoBehaviour
{
    public ControlsPositioner positioner;
    public Rect buttonRect;
    public RectTransform rectTransform;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        buttonRect = TouchControlsManager.RectTransformToScreenSpace(rectTransform);

        for (int i = 0; i < Input.touchCount; ++i)
        {
            Touch t = Input.touches[i];

            if (buttonRect.Contains(t.position))
            {
                Vector2 anchor = new Vector2(t.position.x / Screen.width, t.position.y / Screen.height);
                positioner.PositionControls(anchor);

                if(t.phase == TouchPhase.Ended)
                {
                    positioner.StorePosition(anchor);
                }
                
                break;
            }
        }

#if UNITY_EDITOR
        Vector2 m = Input.mousePosition;
        if (buttonRect.Contains(m) && Input.GetButton("Fire1"))
        {
            Vector2 anchor = new Vector2(m.x / (float)Screen.width, m.y / (float)Screen.height);
            Debug.Log(anchor);
            positioner.PositionControls(anchor);
        }
#endif
    }

    private void OnValidate()
    {
        if(rectTransform == null)
            rectTransform = GetComponent<RectTransform>();

        if(positioner == null)
            positioner = GetComponent<ControlsPositioner>();
    }
}
