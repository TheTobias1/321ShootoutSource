using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunWalkAnimation : MonoBehaviour
{
    CharacterController controller;
    PlayerMovement movement;
    Weapon weapon;

    public float walkSpeed;
    public Vector2 walkAmount;

    public float idleSpeed;
    public float idleAmount;

    public float rotateAmount;

    public float lerpSpeed;
 
    Vector3 desiredPosition;
    Vector3 originalPosition;

    Vector3 originalEuler;
    Quaternion desiredRotation;

    public Vector3 shootRecoil;

    float t = 0;

    // Start is called before the first frame update
    void Start()
    {
        movement = GetComponentInParent<PlayerMovement>();
        controller = GetComponentInParent<CharacterController>();
        weapon = GetComponentInParent<Weapon>();

        weapon.OnShoot += Shoot;

        originalEuler = transform.localEulerAngles;
        originalPosition = transform.localPosition;
    }

    public void Shoot()
    {
        transform.localPosition += shootRecoil * 0.1f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //position
        Vector3 resultant = new Vector3();

        float s = idleSpeed;
        Vector2 a = new Vector2(0, idleAmount / 10);
        float sinB = 0;
        float mag = controller.velocity.magnitude;
        float relMag = mag / movement.speed;
        
        if (t > 10000)
            t = 0;

        if (mag > 1)
        {
            t += Time.fixedDeltaTime * relMag;
            s = walkSpeed;
            a = walkAmount / 10;
            sinB = Mathf.Sin(t * s * 0.5f);
        }
        else
        {
            t += Time.fixedDeltaTime * idleSpeed;
        }

        float sinA = Mathf.Sin(t * s);

        resultant.y = sinA * a.y;
        resultant.x = sinB * a.x;

        desiredPosition = resultant + originalPosition;

        //rotation
        Vector3 d = new Vector3(0, -movement.InputDirection.z * rotateAmount, 0);
        desiredRotation = Quaternion.Euler(d + originalEuler);

        transform.localRotation = Quaternion.Lerp(transform.localRotation, desiredRotation, lerpSpeed * Time.fixedDeltaTime);
        transform.localPosition = Vector3.Lerp(transform.localPosition, desiredPosition, lerpSpeed * Time.fixedDeltaTime);
    }
}
