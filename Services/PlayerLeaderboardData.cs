using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SocialPlatforms;

#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#endif

#if UNITY_IOS
using UnityEngine.SocialPlatforms.GameCenter;
#endif

public class PlayerLeaderboardData : MonoBehaviour
{
    public static void LoadRankData(string leaderboardID, Action<LeaderboardData> OnComplete)
    {
#if UNITY_ANDROID

        if(GameServices.Instance.IsLoggedIn())
        {
            PlayGamesPlatform.Instance.LoadScores
            (
                leaderboardID, 
                LeaderboardStart.PlayerCentered, 
                1, 
                LeaderboardCollection.Public, 
                LeaderboardTimeSpan.AllTime, 
                (LeaderboardScoreData data) =>
                {
                    OnAndroidDataRetrieved(data, OnComplete);
                }
            );
        }

#elif UNITY_IOS
        bool done = false;
        var leaderboard = Social.CreateLeaderboard();
        leaderboard.range = new Range(1, 100);
        leaderboard.id = leaderboardID;
        leaderboard.timeScope = TimeScope.AllTime;
        leaderboard.userScope = UserScope.Global;
        leaderboard.LoadScores( success =>
        {
            if (leaderboard.scores.Length > 0)
            {
                //OnIOSDataRetrieved(leaderboard.scores, OnComplete);
                OnIOSLocalScoreRetrieved(leaderboard.localUserScore, OnComplete);
                done = true;
            }
            else
            {
                Debug.LogError( "No scores registered" );
                OnComplete(new LeaderboardData());
            }
        } );

        if(!done)
        {
            Debug.Log("No Data Found");
            OnComplete(new LeaderboardData());
        }

        #else
        OnComplete(new LeaderboardData());
        #endif
    }

#if UNITY_ANDROID
    public static void OnAndroidDataRetrieved(LeaderboardScoreData data, Action<LeaderboardData> OnComplete)
    {
        LeaderboardData lb = new LeaderboardData();

        if(data.Valid)
        {
            IScore curScore = data.Scores[0];
            lb.init = true;
            lb.round = curScore.value;
            lb.position = curScore.rank;
            Debug.Log("curScore.rank");
        }

        OnComplete(lb);
    }
#endif

#if UNITY_IOS
    public static void OnIOSDataRetrieved(IScore[] scores, Action<LeaderboardData> OnComplete)
    {
        LeaderboardData lb = new LeaderboardData();

        string userID = Social.localUser.id;

        for (int i = 0; i < scores.Length; ++i)
        {
            IScore curScore = scores[i];

            if (userID == curScore.userID)
            {
                lb.init = true;
                lb.round = curScore.value;
                lb.position = curScore.rank;
            }
        }

        OnComplete(lb);
    }

    public static void OnIOSLocalScoreRetrieved(IScore localScore, Action<LeaderboardData> OnComplete)
    {
        LeaderboardData lb = new LeaderboardData();

        lb.init = true;
        lb.round = localScore.value;
        lb.position = localScore.rank;

        OnComplete(lb);
    }
#endif
}

public struct LeaderboardData
{
    public bool init;
    public long round;
    public int position;
}
