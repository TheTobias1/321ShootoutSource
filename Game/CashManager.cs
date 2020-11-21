using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CashManager : MonoBehaviour
{
    public static CashManager CashSystem;

    public int cash;
    public static int Cash
    {
        get
        {
            if (CashManager.CashSystem != null)
                return CashManager.CashSystem.cash;
            else
                return 0;
        }
    }

    public float currentMultiplier;
    public float multiplierDecreaseRate;
    [HideInInspector]
    public float multiplierImmunityStamp;
    public static int Multiplier
    {
        get
        {
            if (CashManager.CashSystem != null)
                return Mathf.RoundToInt(Mathf.Min(CashManager.CashSystem.currentMultiplier, 3));
            else
                return 1;
        }
    }

    public static event IntegerDelegate OnMultiplierChange;
    public static event IntegerDelegate OnGainScore;

    private void Awake()
    {
        CashManager.CashSystem = this;
    }

    private void Update()
    {
        int preMultiplier = CashManager.Multiplier;

        if(Time.time > multiplierImmunityStamp)
            currentMultiplier = Mathf.Max(currentMultiplier - multiplierDecreaseRate * Time.deltaTime, 1.4f);

        if (CashManager.Multiplier != preMultiplier)
        {
            if (OnMultiplierChange != null)
            {
                OnMultiplierChange(CashManager.Multiplier);
            }
        }
    }

    public static void OnScore(int amount)
    {
        CashManager.AddCash(amount);

        int preMultiplier = CashManager.Multiplier;
        XPManager.AddXP(amount * preMultiplier);

        CashManager.AddMultiplier();
        if(CashManager.Multiplier != preMultiplier)
        {
            if(OnMultiplierChange != null)
            {
                OnMultiplierChange(CashManager.Multiplier);
            }
        }
    }

    public static void AddMultiplier()
    {
        if(CashManager.CashSystem != null)
        {
            CashManager.CashSystem.currentMultiplier = Mathf.Min(CashManager.CashSystem.currentMultiplier + 1, 3.3f);
            CashManager.CashSystem.multiplierImmunityStamp = Time.time + 1f;
        }

    }

    public static void AddCash(int amount)
    {
        if(PowerupManager.GetPowerupStatus(Powerups.Multiplier))
        {
            amount = Mathf.RoundToInt(amount * 1.5f);
        }

        amount *= Multiplier;

        if(CashManager.CashSystem != null)
        {
            CashManager.CashSystem.cash += amount;

            if(CashManager.OnGainScore != null)
            {
                CashManager.OnGainScore(amount);
            }
        }
    }
    
    public static bool TakeCash(int amount)
    {
        if(CashManager.CashSystem == null)
        {
            return false;
        }

        if(CashManager.Cash >= amount)
        {
            CashManager.CashSystem.cash -= amount;
            return true;
        }

        return false;
    }

    public static void ResetCash()
    {
        if (CashManager.CashSystem != null)
        {
            if (PowerupManager.GetPowerupStatus(Powerups.Investment))
            {
                CashManager.CashSystem.cash = PowerupManager.PowerupSystem.investmentBuffer;
                PowerupManager.PowerupSystem.investmentBuffer = 0;
                return;
            }

            CashManager.CashSystem.cash = 0;
        }
    }
}
