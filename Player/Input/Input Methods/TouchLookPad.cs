using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class TouchLookPad : MonoBehaviour
{
    private TouchControlsManager manager;

    private Rect lookRect;
    public RectTransform lookRectTransform;

    public float lerp = 17.5f;
    public float curInput;

    public const float standardPPI = 440;
    float dpiMultiplier;

    public Vector4 framerateBuffer;
    float avgDelta;

    // Start is called before the first frame update
    void Start()
    {
        manager = TouchControlsManager.touchControls;
        lookRect = TouchControlsManager.RectTransformToScreenSpace(lookRectTransform);
        dpiMultiplier = Screen.dpi / standardPPI;

#if UNITY_IOS
        dpiMultiplier *= 50;
#endif
    }

    // Update is called once per frame
    void Update()
    {
        float totalInput = 0;
        for (int i = 0; i < Input.touchCount; ++i)
        {
            Touch t = Input.touches[i];

            if (lookRect.Contains(t.position))
            {
                float delta = t.deltaPosition.x / Screen.currentResolution.width;
                totalInput += delta * dpiMultiplier * Mathf.Min(t.deltaTime, avgDelta + 0.001f) * 60;
                break;
            }
        }
        curInput = Mathf.Lerp(curInput, totalInput, lerp * Time.deltaTime);
        manager.LookInput = curInput;

        framerateBuffer.w = framerateBuffer.z;
        framerateBuffer.z = framerateBuffer.y;
        framerateBuffer.y = framerateBuffer.x;
        framerateBuffer.x = Time.deltaTime;

        avgDelta = (framerateBuffer.x + framerateBuffer.y + framerateBuffer.z + framerateBuffer.w) / 4;

    }
}
