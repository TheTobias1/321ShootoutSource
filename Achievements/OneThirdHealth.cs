using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OneThirdHealth : MonoBehaviour
{
    public bool healed = false;
    public bool completed = false;
    SessionManager manager;
    public int target;
    // Start is called before the first frame update
    void Start()
    {
        manager = GetComponent<SessionManager>();
        SceneManager.sceneLoaded += OnLoadLevel;
        HealingGun.OnHeal += OnHealed;
    }

    void OnHealed(float h)
    {
        if(h > 0.333f)
            healed = true;
    }

    void OnLoadLevel(Scene s, LoadSceneMode m)
    {
        if (SessionManager.currentRound >= target && !completed && !healed)
        {
            completed = true;
            //Submit
            GameServices.Instance.SubmitAchievement(AchievementNames.ThirdHealth);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnLoadLevel;
        HealingGun.OnHeal -= OnHealed;
    }
}
