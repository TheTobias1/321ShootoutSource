using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchMovePad : MonoBehaviour
{

    private TouchControlsManager manager;

    private Rect moveRect;
    public RectTransform moveRectTransform;

    Vector2 origin;
    Vector2 pos;
    bool moving;

    public Texture circle;
    public Color originColour;
    public Color stickColour;
    public Vector2 joystickSize;

    // Start is called before the first frame update
    void Start()
    {
        manager = TouchControlsManager.touchControls;
        moveRect = TouchControlsManager.RectTransformToScreenSpace(moveRectTransform);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 movIn = Vector2.zero;
        bool m = false;

        for (int i = 0; i < Input.touchCount; ++i)
        {
            Touch t = Input.touches[i];

            if (moveRect.Contains(t.position))
            {
                m = true;
                if (t.phase == TouchPhase.Began)
                {
                    origin = t.position;
                    pos = t.position;
                }

                Vector2 delta = t.position - origin;
                if(delta.magnitude > 14)
                {
                    pos = t.position;
                    movIn = delta.normalized;
                    movIn.x = Mathf.Round(movIn.x);
                    movIn.y = Mathf.Round(movIn.y);

                    movIn.y *= 1.5f;

                    movIn = movIn.normalized;
                    
                }
            }
        }
        moving = m;
        manager.MoveInput = movIn.normalized;
    }

    private void OnGUI()
    {
        if(moving)
        {
            //origin pad
            Rect originPos = new Rect(origin.x - joystickSize.x / 2, origin.y + joystickSize.x / 2, joystickSize.x, joystickSize.x);
            originPos.y = Screen.currentResolution.height - originPos.y;
            GUI.DrawTexture(originPos, circle, ScaleMode.ScaleToFit, true, 0, originColour, 0, 0);

            Rect stickPos = new Rect(pos.x - joystickSize.y / 2, pos.y + joystickSize.y / 2, joystickSize.y, joystickSize.y);
            stickPos.y = Screen.currentResolution.height - stickPos.y;
            GUI.DrawTexture(stickPos, circle, ScaleMode.ScaleToFit, true, 0, stickColour, 0, 0);
        }
    }
}
