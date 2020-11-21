using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunRotateOnMove : MonoBehaviour
{
    PlayerMovement movement;

    public float walkAngle;
    public float lerpSpeed;

    Vector3 originalRotation;
    // Start is called before the first frame update
    void Start()
    {
        originalRotation = transform.localEulerAngles;
        movement = GetComponentInParent<PlayerMovement>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 rot = new Vector3(movement.InputDirection.y * walkAngle / 4, 0, -movement.InputDirection.x * walkAngle);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(rot + originalRotation), lerpSpeed * Time.fixedDeltaTime);
    }
}
