using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoModule : MonoBehaviour
{
    PlayerManager manager;
    HudManager HUD;
    public AmmoType ammoType;
    public bool displayAmmo;

    public int remainingAmmo;

    // Start is called before the first frame update
    void Start()
    {
        manager = GetComponentInParent<PlayerManager>();
        HUD = HudManager.HUD;
        UpdateAmmoHUD();
    }

    public int TakeAmmo(int amount)
    {
        if(remainingAmmo >= amount)
        {
            remainingAmmo -= amount;
            UpdateAmmoHUD();
            return amount;
        }
        else
        {
            int remAmmo = remainingAmmo;
            remainingAmmo = 0;
            UpdateAmmoHUD();
            return remAmmo;
        }
    }

    public int AmmoLeft()
    {
        return remainingAmmo;
    }

    public void SetAmmo(int amount, bool updateCount = true)
    {
        remainingAmmo = amount;
        if(updateCount)
            UpdateAmmoHUD();
    }

    public void UpdateAmmoHUD()
    {
        if (HUD == null)
            return;

        if (displayAmmo)
        {
            if(gameObject.activeSelf)
                HUD.SetAmmoCount(AmmoLeft());
        }
        else
            HUD.DisableAmmoCount();
    }

    public void OnEnable()
    {
        UpdateAmmoHUD();
    }

    public void UpdateHealthPackAmmo()
    {
        if(HUD == null)
            HUD = HudManager.HUD;

        HUD.SetHealthPackCount(remainingAmmo);
    }

}
