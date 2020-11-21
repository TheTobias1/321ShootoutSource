using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Rewired;

public class CenterScrollOnSelect : MonoBehaviour, ISelectHandler
{
    public ScrollRect scroll;

    public void OnSelect(BaseEventData eventData)
    {
        Controller c = ReInput.players.GetPlayer(0).controllers.GetLastActiveController();

        if(c.type == ControllerType.Joystick || c.type == ControllerType.Keyboard)
        {
            float pos = (float)transform.GetSiblingIndex() / (float)transform.parent.childCount;
            scroll.verticalNormalizedPosition = 1 - pos;
        }
    }
}
