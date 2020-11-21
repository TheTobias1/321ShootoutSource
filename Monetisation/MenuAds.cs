using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using System;

public class MenuAds : MonoBehaviour
{
    public static int loadUps = 0;
    private bool adsEnabled = true;
    private const bool testMode = true;

    public GameObject MainMenu;
    public GameObject AdMenu;

#if UNITY_IOS
    string gameID = "3813186";
#else
    string gameID = "3813187";
#endif

    // Start is called before the first frame update
    void Start()
    {
        MenuAds.loadUps += 1;

        if(adsEnabled && !Advertisement.isInitialized)
            Advertisement.Initialize(gameID, testMode);

        if(loadUps > 1)
        {
            if(loadUps % 2 == 0 && CanShowMenuAd())
            {
                MainMenu.SetActive(false);
                AdMenu.SetActive(true);
            }
            else if(CanShowMenuAd())
            {
                ShowMenuAd();
            }
        }

    }

    public void ShowMenuAd() 
    {
        if (Advertisement.IsReady()) 
        {
            Advertisement.Show();
        } 
        else 
        {
            Debug.Log("No ad available");
        }
    }

    bool CanShowMenuAd()
    {
        if(!Advertisement.IsReady() || !Advertisement.isInitialized)
        {
            return false;
        }

        if(RewardAdManager.playerRewarded)
        {
            return false;
        }

        if(PlayerPrefs.HasKey("Full") && PlayerPrefs.GetInt("Full") == 1)
        {
            return false;
        }

        return adsEnabled;
    }
}
