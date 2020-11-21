using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RewardAdManager : MonoBehaviour
{
    public static RewardAdManager instance;
    public static DateTime rewardEndStamp;
    public static bool playerRewarded;
    public GameObject rewardIndicator;

    void Awake()
    {
        RewardAdManager.instance = this;
    }

    void Start()
    {
        if(PlayerPrefs.HasKey("Full") && PlayerPrefs.GetInt("Full") == 1)
        {
            RewardAdManager.playerRewarded = true;
        }
        else if(!PlayerPrefs.HasKey("RewardStamp"))
        {
            MenuAds.loadUps -= 1;

            RewardAdManager.rewardEndStamp = DateTime.Now;
            PlayerPrefs.SetString("RewardStamp", RewardAdManager.rewardEndStamp.ToString());

            RewardAdManager.playerRewarded = false;
        }
        else
        {
            string stampAsString = PlayerPrefs.GetString("RewardStamp");
            Debug.Log(stampAsString);
            RewardAdManager.rewardEndStamp = DateTime.Parse(stampAsString);

            int comparison = rewardEndStamp.CompareTo(DateTime.Now);

            if(comparison < 0)
            {
                Debug.Log("no reward");
                Debug.Log("Stamp" + RewardAdManager.rewardEndStamp.ToString());
                Debug.Log("Now" + DateTime.Now.ToString() + " c: " + comparison.ToString());
                RewardAdManager.playerRewarded = false;
            }
            else
            {
                RewardAdManager.playerRewarded = true;
                Debug.Log("reward");
            }
        }

        rewardIndicator.SetActive(RewardAdManager.playerRewarded);
    }

    public static void RewardPlayer()
    {
        Debug.Log("REWARD PLAYER");   

        RewardAdManager.rewardEndStamp = DateTime.Now;
        RewardAdManager.rewardEndStamp = RewardAdManager.rewardEndStamp.AddHours(1);
        
        PlayerPrefs.SetString("RewardStamp", RewardAdManager.rewardEndStamp.ToString());

        RewardAdManager.playerRewarded = true;
        RewardAdManager.instance.rewardIndicator.SetActive(true);
    }
}
