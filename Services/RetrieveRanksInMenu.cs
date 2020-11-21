using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RetrieveRanksInMenu : MonoBehaviour
{
    public string iosLeaderboardID;
    public string androidLeaderboardID;

    public GameObject standard;
    public GameObject bronze;
    public GameObject silver;
    public GameObject gold;

    void Start()
    {
        //Invoke("TryGetRank", 1);
    }

    void TryGetRank()
    {
        if(!GameServices.Instance.IsLoggedIn())
        {
            Invoke("TryGetRank", 1);
            return;
        }

        string leaderboardID;

        if(Application.platform == RuntimePlatform.Android)
        {
            leaderboardID = androidLeaderboardID;
        }
        else if(Application.platform == RuntimePlatform.IPhonePlayer)
        {
            leaderboardID = iosLeaderboardID;
        }
        else
        {
            return;
        }

        PlayerLeaderboardData.LoadRankData(leaderboardID, RankRetrieved);
    }

    public void RankRetrieved(LeaderboardData rankData)
    {
        if(rankData.init)
        {
            PlayerRanks rank = SessionManager.CalculatePlayerRank(rankData.position);

            switch (rank)
            {
                case PlayerRanks.Top100:
                    standard.SetActive(true);
                    break;
                case PlayerRanks.Top50:
                    bronze.SetActive(true);
                    break;
                case PlayerRanks.Top10:
                    silver.SetActive(true);
                    break;
                case PlayerRanks.Top3:
                    gold.SetActive(true);
                    break;
            }
        }

    }
}
