using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class ServiceLogin : MonoBehaviour
{
    public static bool attemptedLogin = false;
    public static bool loggedIn = false;

    public GameObject leaderboardReal;
    public GameObject leaderboardGreyed;
    public GameObject signIn;
    // Start is called before the first frame update
    void Start()
    {
        if(!ServiceLogin.loggedIn)
        {
            if(!attemptedLogin)
            {
                GameServices.Instance.LogIn(OnSignInComplete);
                ServiceLogin.attemptedLogin = true;
            }
            else
            {
                signIn.SetActive(true);
            }
        }
        else
        {
            leaderboardReal.SetActive(true);
            leaderboardGreyed.SetActive(false);
        }
    }

    public void TryLogin()
    {
        GameServices.Instance.LogIn(OnSignInComplete);
    }

    public void OnSignInComplete(bool s)
    {
        ServiceLogin.loggedIn = s;
        if (s)
        {
            leaderboardReal.SetActive(true);
            leaderboardGreyed.SetActive(false);
        }
        else
        {
            leaderboardReal.SetActive(false);
            leaderboardGreyed.SetActive(true);
            signIn.SetActive(true);
        }
    }

    public void ShowLeaderboards()
    {
        GameServices.Instance.ShowLeaderboadsUI();
    }

    public void ShowAchievment()
    {
        GameServices.Instance.ShowAchievementsUI();
    }

}
