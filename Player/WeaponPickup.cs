using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public WeaponNames weaponType;
    [SerializeField]
    public int ammo;
    private int ammoInClip = -1;

    public AudioClip takeSound;

    public bool nonreloadable;

    [HideInInspector]
    public bool worldSpawned;

    public int Ammo
    {
        get
        {
            return ammo;
        }
        set
        {
            ammo = value;
        }
    }

    public int AmmoInClip
    {
        get
        {
            return ammoInClip;
        }
        set
        {
            ammoInClip = value;
        }
    }

    public int TotalAmmo
    {
        get { return Mathf.Max(ammoInClip, 0) + ammo; }
    }

    public void OnWorldSpawn()
    {
        worldSpawned = true;
        if(PowerupManager.GetPowerupStatus(Powerups.ExtraAmmo))
        {
            ammo += ammo;
        }
    }

    public void TakePickup()
    {
        Destroy(gameObject);
    }
    void SetAmmo(int clip, int total)
    {
        ammoInClip = clip;
        ammo = total;
    }

    public void SetAmmo(Vector2Int a)
    {
        Debug.Log("Set ammo " + a);
        ammoInClip = a.x;
        ammo = a.y;
    }

    private void OnDestroy()
    {
        if (takeSound != null && SoundFXPlayer.SFX != null)
        {
            SoundFXPlayer.SFX.PlaySound(takeSound);
        }
    }
}
