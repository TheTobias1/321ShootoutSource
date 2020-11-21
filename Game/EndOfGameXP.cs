using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndOfGameXP : MonoBehaviour
{
    public GameObject XpPanel;
    public LevelupSequence mapLevel;
    public LevelupSequence skinLevel;
    public AnimatedEnd animatedEnding;

    private int xpBeforeGame;
    private int xpAfterGame;
    private int round;

    public GameObject[] MapRewardTeasers;
    public GameObject[] SkinRewardTeasers;

    private AsyncOperation menuLoad;

    // Start is called before the first frame update
    void Start()
    {
        xpBeforeGame = XPManager.GetStoredXP();
        xpAfterGame = xpBeforeGame + XPManager.GetEarnedXP();
        XPManager.StoreEarnedXP();

        round = SessionManager.currentRound;
        SceneManager.sceneLoaded += OnLevelLoad;
        DontDestroyOnLoad(gameObject);
    }

    public void OnLevelLoad(Scene scene, LoadSceneMode mode)
    {
        if(scene.buildIndex == SessionManager.windDownScene)
        {
            AnimatedEnd.OnEndFinished += PlaySequence;
            AnimatedEnd starsAndRound = Instantiate(animatedEnding);
            starsAndRound.Initiate(round);

            mapLevel.InitSlider(xpBeforeGame, XPManager.MAP_XP_BOUNDARIES);
            skinLevel.InitSlider(xpBeforeGame, XPManager.SKIN_XP_BOUNDARIES);
            mapLevel.rootObject.SetActive(false);
            skinLevel.rootObject.SetActive(false);
            LoadMenu();
        }
    }

    public void PlaySequence()
    {
        ActivateXpPanel();
        AnimatedEnd.OnEndFinished -= PlaySequence;
    }

    public void ActivateXpPanel()
    {
        int currentLevel = XPManager.CalculateLevel(xpBeforeGame, XPManager.MAP_XP_BOUNDARIES);
        DisplayMapTeaser(currentLevel);

        XpPanel.SetActive(true);
        mapLevel.OnDone += OnMapLevelUpDone;
        mapLevel.OnLevelUp += DisplayMapTeaser;

        if (xpBeforeGame < XPManager.MAP_LEVEL_CAP)
            Invoke("OnXpPanelActive", 0.4f);
        else
            OnMapLevelUpDone();

        currentLevel = XPManager.CalculateLevel(xpBeforeGame, XPManager.SKIN_XP_BOUNDARIES);
        DisplaySkinTeaser(currentLevel);
    }

    public void OnXpPanelActive()
    {
        mapLevel.rootObject.SetActive(true);
        mapLevel.DoLevelUp(xpBeforeGame, xpAfterGame, XPManager.MAP_XP_BOUNDARIES, XPManager.MAP_LEVEL_CAP);
    }

    public void DisplayMapTeaser(int index)
    {
        for(int i = 0; i < MapRewardTeasers.Length; ++i)
        {
            try
            {
                MapRewardTeasers[i].SetActive(i == index);
            }
            catch
            {
                Debug.Log("Missing? " + i.ToString());
            }
        }
    }

    public void DisplaySkinTeaser(int index)
    {
        for (int i = 0; i < SkinRewardTeasers.Length; ++i)
        {
            SkinRewardTeasers[i].SetActive(i == index);
        }
    }

    public void OnMapLevelUpDone()
    {
        mapLevel.OnDone -= OnMapLevelUpDone;
        mapLevel.OnLevelUp -= DisplayMapTeaser;
        skinLevel.OnDone += OnSkinFinished;
        skinLevel.OnLevelUp += OnSkinLevelUp;

        if (xpBeforeGame < XPManager.SKIN_LEVEL_CAP)
            Invoke("DoSkinLevelUp", 1);
        else
            OnSkinFinished();
    }

    public void DoSkinLevelUp()
    {
        skinLevel.rootObject.SetActive(true);
        skinLevel.DoLevelUp(xpBeforeGame, xpAfterGame, XPManager.SKIN_XP_BOUNDARIES, XPManager.SKIN_LEVEL_CAP);
    }

    public void OnSkinFinished()
    {
        skinLevel.OnDone -= OnSkinFinished;
        skinLevel.OnLevelUp -= OnSkinLevelUp;
        Invoke("FinishLoad", 1);
        Invoke("SpawnLoadScreen", 0.5f);
    }

    public void OnSkinLevelUp(int i)
    {
        DisplaySkinTeaser(i);
    }

    public void SpawnLoadScreen()
    {
        LoadingScreen.SpawnLoadingScreen();
    }

    public void Skip()
    {
        skinLevel.OnDone = null;
        skinLevel.OnLevelUp = null;
        mapLevel.OnDone = null;
        mapLevel.OnLevelUp = null;
        Invoke("FinishLoad", 0.5f);
        SpawnLoadScreen();
    }

    public void LoadMenu()
    {
        menuLoad = SceneManager.LoadSceneAsync(0);
        menuLoad.allowSceneActivation = false;
        SceneManager.sceneLoaded -= OnLevelLoad;

    }

    public void FinishLoad()
    {
        menuLoad.allowSceneActivation = true;
        Destroy(gameObject);
    }

}
