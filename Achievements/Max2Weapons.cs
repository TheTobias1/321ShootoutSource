using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Max2Weapons : MonoBehaviour
{
    public int switches = 0;
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
        switches++;
    }

    void OnLoadLevel(Scene s, LoadSceneMode m)
    {
        if (SessionManager.currentRound >= target && !completed && switches <= 1)
        {
            completed = true;
            //Submit
            GameServices.Instance.SubmitAchievement(AchievementNames.StickToGuns);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnLoadLevel;
        PlayerWeaponManager.OnPickedUpWeapon -= OnSwitchWeapon;
    }
}
