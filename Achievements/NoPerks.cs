using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NoPerks : MonoBehaviour
{
    public bool boughtPerk = false;
    public bool completed = false;
    SessionManager manager;
    public int target;
    // Start is called before the first frame update
    void Start()
    {
        manager = GetComponent<SessionManager>();
        SceneManager.sceneLoaded += OnLoadLevel;
        PowerupManager.OnBuyPerk += OnBuy;
    }

    void OnBuy()
    {
        boughtPerk = true;
    }

    void OnLoadLevel(Scene s, LoadSceneMode m)
    {
        if (SessionManager.currentRound >= target && !completed && !boughtPerk)
        {
            completed = true;
            //Submit
            GameServices.Instance.SubmitAchievement(AchievementNames.NoPerks);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnLoadLevel;
        PowerupManager.OnBuyPerk -= OnBuy;
    }
}
