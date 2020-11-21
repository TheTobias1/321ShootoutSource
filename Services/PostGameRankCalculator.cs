using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostGameRankCalculator : MonoBehaviour
{
    public static PlayerRanks gameRank = PlayerRanks.None;

    public static void CalculatePostGameRank(string leaderboardID)
    {
        PostGameRankCalculator.gameRank = PlayerRanks.None;

        //PlayerLeaderboardData.LoadRankData(leaderboardID, PostGameRankCalculator.LeaderboardRetrieved);
    }

    public static void LeaderboardRetrieved(LeaderboardData rankData)
    {
        if(rankData.init)
        {
            PostGameRankCalculator.gameRank = SessionManager.CalculatePlayerRank(rankData.position);

            SessionManager s = SessionManager.SessionInstance;

            if(s != null)
            {
                switch(gameRank)
                {
                    case PlayerRanks.Top100:
                        SessionManager.currentRank = s.standardRank;
                        break;
                    case PlayerRanks.Top50:
                        SessionManager.currentRank = s.bronzeRank;
                        break;
                    case PlayerRanks.Top10:
                        SessionManager.currentRank = s.silverRank;
                        break;
                    case PlayerRanks.Top3:
                        SessionManager.currentRank = s.goldRank;
                        break;
                }
            }
        }
    }
}
