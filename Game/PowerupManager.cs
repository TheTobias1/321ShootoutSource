using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PowerupManager : MonoBehaviour
{
    public static PowerupManager PowerupSystem;

    public bool[] powerupStatus = new bool[8];
    public int investmentBuffer;
    public WeaponNames startingWeaponBuffer;

    public int roundMultiplier;

    public static event Action OnBuyPerk;

    private void Awake()
    {
        PowerupManager.PowerupSystem = this;
    }

    public static bool GetPowerupStatus(Powerups powerup)
    {
        if(PowerupManager.PowerupSystem == null)
        {
            return false;
        }

        else
        {
            return PowerupManager.PowerupSystem.powerupStatus[(int)powerup];
        }
    }

    public static void ActivatePowerup(Powerups powerup)
    {
        if (PowerupManager.PowerupSystem == null)
        {
            return;
        }
        else
        {
            OnBuyPerk?.Invoke();
            PowerupManager.PowerupSystem.powerupStatus[(int)powerup] = true;
        }
    }

    public static int GetInvestmentBuffer()
    {
        if (PowerupManager.PowerupSystem == null)
        {
            return 0;
        }
        else
        {
            return PowerupManager.PowerupSystem.investmentBuffer;
        }
    }

    public static void ResetPowerups()
    {
        if (PowerupManager.PowerupSystem != null)
        {
            int l = PowerupManager.PowerupSystem.powerupStatus.Length;
            for (int i = 0; i < l; ++i)
            {
                PowerupManager.PowerupSystem.powerupStatus[i] = false;
            }
        }
    }

    public static int GetRoundMultiplier()
    {
        if (PowerupManager.PowerupSystem == null)
        {
            return 1;
        }
        else
        {
            return PowerupManager.PowerupSystem.roundMultiplier;
        }
    }
    
}

public enum Powerups { Investment = 0, Multiplier = 1, MaxHealthPacks = 2, StrongHealthPacks = 3, MobileHealthPacks = 4, ExtendedMag = 5, ExtraAmmo = 6, StartingWeapon = 7 };
