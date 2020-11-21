using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponAnimations : MonoBehaviour
{
    public Weapon weapon;
    public Animator controller;

    private Transform clipRoot;
    private Vector3 originalClipPosition;
    private Quaternion originalClipRotation;
    public Transform magazine;
    public Transform hand;

    private void Start()
    {
        if (weapon == null)
            weapon = GetComponentInParent<Weapon>();

        if(weapon != null)
        {
            weapon.OnPutAway += ResetAnim;
            weapon.OnTakeOut += ResetAnim;
            weapon.OnReload += OnReload;
        }

        if(magazine != null)
        {
            Debug.Log("CLIP");
            clipRoot = magazine.parent;
            originalClipPosition = magazine.localPosition;
            originalClipRotation = magazine.localRotation;
        }
    }

    private void OnEnable()
    {
        if(clipRoot != null)
            ClipToRoot();
    }

    public void OnReload()
    {
        controller.SetTrigger("Reload");
    }

    public void ResetAnim()
    {
        controller.SetTrigger("Reset");
    }

    public void ClipToHand()
    {
        Vector3 currentWorldPos = magazine.position;
        magazine.parent = hand;
        magazine.position = currentWorldPos;
    }

    public void ClipToRoot()
    {
        magazine.parent = clipRoot;
        magazine.localPosition = originalClipPosition;
        magazine.localRotation = originalClipRotation;
    }

    private void OnDestroy()
    {
        weapon.OnPutAway -= ResetAnim;
        weapon.OnTakeOut -= ResetAnim;
        weapon.OnReload -= OnReload;
    }
}
