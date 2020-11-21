using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActivateOnHealthpack : MonoBehaviour
{
    public HealingGun hp;
    public Image image;

    // Update is called once per frame
    void Update()
    {
            image.enabled = hp.ammunition.remainingAmmo == 2;
    }
}
