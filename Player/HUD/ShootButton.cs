using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class ShootButton : MonoBehaviour
{

    public RectTransform rectTransform;
    private Player player;
    private CustomController controller;

    public Rect buttonRect;
    public Vector2 mousePos;

    public bool isShooting = false;
    public bool stayPressed;
    public int foundID;

    public int rewiredButtonId = 3;

    private void Start()
    {
        player = ReInput.players.GetPlayer(0);
        ReInput.InputSourceUpdateEvent += OnInputUpdate; // subscribe to input update event
        controller = (CustomController)player.controllers.GetControllerWithTag(ControllerType.Custom, "Touch");

        buttonRect = TouchControlsManager.RectTransformToScreenSpace(rectTransform);
    }

    // Update is called once per frame
    void Update()
    {
        GetShootInput();
    }

    void GetShootInput()
    {
        //if (controller == null)
        //return;
        mousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        bool s = isShooting;
        if(!stayPressed)
        {
            s = false;
        }

        for(int i = 0; i < Input.touchCount; ++i)
        {
            Touch t = Input.touches[i];

            if(t.phase == TouchPhase.Ended)
            {
                if(isShooting && t.fingerId == foundID)
                {
                    s = false;
                }
                continue;
            }

            if(buttonRect.Contains(t.position) && (t.phase == TouchPhase.Began || t.phase == TouchPhase.Stationary))
            {
                s = true;
                foundID = t.fingerId;
                break;
            }
        }

        isShooting = s;

    }

    void OnInputUpdate()
    {
        controller.SetButtonValueById(rewiredButtonId, isShooting);
    }
}
