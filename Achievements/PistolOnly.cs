using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PistolOnly : MonoBehaviour
{
    public bool switchedWeapon = false;
    public bool completed = false;
    SessionManager manager;
    public int target;
    // Start is called before the first frame update
    void Start()
    {
        manager = GetComponent<SessionManager>();
        SceneManager.sceneLoaded += OnLoadLevel;
        PlayerWeaponManager.OnPickedUpWeapon += OnSwitchWeapon;
    }

    void OnSwitchWeapon()
    {
        switchedWeapon = true;
    }

    void OnLoadLevel(Scene s, LoadSceneMode m)
    {
        if(SessionManager.currentRound >= target && !completed && !switchedWeapon)
        {
            completed = true;
            //Submit
            GameServices.Instance.SubmitAchievement(AchievementNames.PistolOnly);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnLoadLevel;
        PlayerWeaponManager.OnPickedUpWeapon -= OnSwitchWeapon;
    }
}
