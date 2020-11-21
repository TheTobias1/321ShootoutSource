using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XPManager : MonoBehaviour
{
    public static XPManager XPSystem;

    public const int MAP_XP_BOUNDARIES = 104000;
    public const int SKIN_XP_BOUNDARIES = 11100;

    public const int MAP_LEVEL_CAP = 416000;
    public const int SKIN_LEVEL_CAP = 333000;

    public int earnedXP;

    private void Awake()
    {
        earnedXP = 0;
        XPManager.XPSystem = this;
    }

    public static void AddXP(int amount)
    {
        if (XPManager.XPSystem == null)
        {
            return;
        }

        if(RewardAdManager.playerRewarded)
            amount *= 2;

        #if UNITY_EDITOR
        amount *= 2000;
        #endif

        XPManager.XPSystem.earnedXP += amount;

    }

    public static int GetEarnedXP()
    {
        if(XPManager.XPSystem == null)
        {
            return 0;
        }
        else
        {
            return XPManager.XPSystem.earnedXP;
        }
    }

    public static int GetStoredXP()
    {
        return PlayerPrefs.GetInt("XP");
    }

    public static int StoreEarnedXP()
    {
        int x = GetStoredXP();
        x += XPManager.GetEarnedXP();

        PlayerPrefs.SetInt("XP", x);
        PlayerPrefs.Save();

        return x;
    }

    public static int CalculateLevel(int xp, int boundaries)
    {
        return Mathf.FloorToInt(xp / boundaries);
    }
}

public enum XpTypes { Maps, Skins, Custom };
