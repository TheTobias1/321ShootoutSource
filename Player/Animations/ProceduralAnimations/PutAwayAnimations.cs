using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PutAwayAnimations : MonoBehaviour
{
    public Weapon weapon;

    public Vector3 normalRotation = new Vector3(0, 0, 0);
    public Vector3 awayRotation = new Vector3(40, -25, 0);
    private Quaternion normalRot;
    private Quaternion awayRot;

    public float putAwaySpeed = 0.1f;
    public float putAwayTime;

    int state = 0;
   
    private void Awake()
    {
        normalRotation = transform.localEulerAngles;
        weapon.OnTakeOut += OnTakeOutGun;
        weapon.OnPutAway += OnPutAwayGun;

        normalRot = Quaternion.Euler(normalRotation);
        awayRot = Quaternion.Euler(awayRotation);
    }

    private void FixedUpdate()
    {
        if(state == -1)
            PutAway();
        else if (state == 1)
            TakeOut();
    }

    public void OnTakeOutGun()
    {
        state = 1;
        Invoke("ResetPosition", putAwayTime);
        transform.localEulerAngles = awayRotation;
    }

    public void OnPutAwayGun()
    {
        transform.localEulerAngles = normalRotation;
        state = -1;
    }

    void PutAway()
    {
        transform.localRotation = Quaternion.Lerp(transform.localRotation, awayRot, putAwaySpeed * Time.fixedDeltaTime);
    }

    void TakeOut()
    {
        transform.localRotation = Quaternion.Lerp(transform.localRotation, normalRot, putAwaySpeed * Time.fixedDeltaTime);
    }

    public void ResetPosition()
    {
        transform.localEulerAngles = normalRotation;
        state = 0;
    }
}
