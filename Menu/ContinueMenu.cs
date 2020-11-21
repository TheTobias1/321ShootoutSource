using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContinueMenu : MonoBehaviour
{
    public static event ActionDelegate Continue;
    public Button continueButton;

    private void Start()
    {
        GameObject eventSystem = GameObject.Find("EventSystem");
        if (eventSystem != null)
            eventSystem.SetActive(false);
    }

    public void OnBuy()
    {
        continueButton.Select();
    }

    public void ContinuePressed()
    {
        if(ContinueMenu.Continue != null)
        {
            ContinueMenu.Continue();
        }
    }
}
