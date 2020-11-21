using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SquadAI;

public class SessionManager : MonoBehaviour
{
    public static SessionManager SessionInstance;

    public const int windDownScene = 15;

    public string worldName;
    public static int currentRound;
    public static float currentDifficultyMultiplier = 1;

    public float difficultyStep = 0.3f;
    [HideInInspector]
    public GameObject player;
    public GameObject roundStartUI;
    public GameObject roundEndUI;
    public GameObject gameOverObject;

    public List<int> nonCombatScenes;

    public string[] easyMapPool;
    public string[] mediumMapPool;
    public string[] hardMapPool;

    public int numEasyRounds;
    public int numMediumRounds;
    public int numHardRounds;

    private int currentRoundsOfType = 0;
    private int currentMaxRoundsOfType;
    private Difficulty currentDifficulty;

    private float loadFinishTime;
    private bool gameOver = false;

    private string lastLevel = "";

    public bool levelComplete;

    [Header("Leaderboards and Ranks")]
    public LeaderboardNames leaderboard;
    public string iosLeaderboardID;
    public string androidLeaderboardID;

    public Sprite standardRank;
    public Sprite bronzeRank;
    public Sprite silverRank;
    public Sprite goldRank;

    public static Sprite currentRank;

    public AchievementNames achievement;
    

    public void BeginGame()
    {
        SessionManager.currentRound = 1;
        SessionManager[] m = GameObject.FindObjectsOfType<SessionManager>();
        if (m.Length > 1)
        {
            foreach(SessionManager s in m)
            {
                if(s.gameObject.GetInstanceID() != gameObject.GetInstanceID())
                {
                    Destroy(s.gameObject);
                }
            }
        }
        DontDestroyOnLoad(gameObject);
        SessionManager.SessionInstance = this;

        SceneManager.sceneLoaded += OnLevelLoad;

        currentMaxRoundsOfType = numEasyRounds;
        currentDifficulty = Difficulty.Easy;

        EnemySpawner.OnLevelClear += OnLevelClear;
        ChooseLevel();
    }

    private void ChooseLevel()
    {
        string[] levelPool;

        switch(currentDifficulty)
        {
            case Difficulty.Easy:
                levelPool = easyMapPool;
                break;
            case Difficulty.Medium:
                levelPool = mediumMapPool;
                break;
            case Difficulty.Hard:
                levelPool = hardMapPool;
                break;
            default:
                levelPool = hardMapPool;
                break;
        }

        string chosenLevel = levelPool[Random.Range(0, levelPool.Length)];

        int tries = 0;
        while(chosenLevel == lastLevel && tries++ < 100)
        {
            chosenLevel = levelPool[Random.Range(0, levelPool.Length)];
        }
        lastLevel = chosenLevel;
        spawnLoadScreen();
        SceneManager.LoadSceneAsync(chosenLevel);
    }

    public void OnLevelLoad(Scene scene, LoadSceneMode mode)
    {
        levelComplete = false;
        loadFinishTime = Time.time + 4;
        if(!nonCombatScenes.Contains(scene.buildIndex))
        {
            ConfigureLevel();
        }

        MusicManager.PlayMusic();
    }

    private void ConfigureLevel()
    {
        if (PlayerPrefs.HasKey(worldName))
        {
            int r = PlayerPrefs.GetInt(worldName);

            if (currentRound > r)
            {
                PlayerPrefs.SetInt(worldName, currentRound);
            }
        }
        else
        {
            PlayerPrefs.SetInt(worldName, currentRound);
        }

        MapConfigurationManager mapManager = GameObject.FindObjectOfType<MapConfigurationManager>();
        if (mapManager != null)
            mapManager.ChooseConfiguration(currentDifficulty);

        player = GameObject.FindGameObjectWithTag("Player");
        player.SendMessage("SetFreezeInput", true, SendMessageOptions.RequireReceiver);

        Instantiate(roundStartUI);
        MusicManager.FadeHighpass();
        RoundStartSequence.OnPlay += OnLevelBegin;

    }

    public void OnLevelBegin()
    {
        MusicManager.FadeToNormal();
        RoundStartSequence.OnPlay -= OnLevelBegin;

        MapConfigurationManager mapManager = GameObject.FindObjectOfType<MapConfigurationManager>();
        mapManager.PlayMap();

        if(player == null)
            player = GameObject.FindGameObjectWithTag("Player");

        player.SendMessage("SetFreezeInput", false, SendMessageOptions.RequireReceiver);


    }

    private void Update()
    {
        if(player != null)
        {
            if(!player.activeSelf && !levelComplete)
            {
                if(!gameOver)
                {
                    gameOver = true;
                    GameOver();
                    Invoke("LoadMenu", 3);
                    Invoke("spawnLoadScreen", 2.5f);
                }
            }
        }
    }

    public void GameOver()
    {
        Instantiate(gameOverObject);
        MusicManager.Fadeout();

        string saveKey = worldName + "HI";

        if(CalculateNumberOfStars(SessionManager.currentRound) >= 5)
        {
            GameServices.Instance.SubmitAchievement(achievement);
        }

        if(!PlayerPrefs.HasKey(saveKey))
            GameServices.Instance.SubmitScore(SessionManager.currentRound, leaderboard, OnLeaderboardSubmitted);

        if (SessionManager.currentRound > PlayerPrefs.GetInt(saveKey))
            GameServices.Instance.SubmitScore(SessionManager.currentRound, leaderboard, OnLeaderboardSubmitted);
        else
            OnLeaderboardSubmitted(GameServices.Instance.IsLoggedIn(), GameServicesError.Success);
    }

    public void OnLeaderboardSubmitted(bool success, GameServicesError e)
    {
        if(success)
        {
            string saveKey = worldName + "HI";
            PlayerPrefs.SetInt(saveKey, SessionManager.currentRound);

#if UNITY_ANDROID && !UNITY_EDITOR
            PostGameRankCalculator.CalculatePostGameRank(androidLeaderboardID);
#endif
#if UNITY_IOS && !UNITY_EDITOR
            PostGameRankCalculator.CalculatePostGameRank(iosLeaderboardID);
#endif
        }
    }

    public void OnLevelClear()
    {
        if(!gameOver)
        {
            levelComplete = true;

            MusicManager.FadeLowpass();
            FreezePlayerInput();
            if (currentMaxRoundsOfType != -1)
                ++currentRoundsOfType;

            if (currentRoundsOfType >= currentMaxRoundsOfType)
            {
                AdvanceDifficulty();
            }

            Instantiate(roundEndUI);
            ContinueMenu.Continue += ContinueToNext;

            PowerupManager.ResetPowerups();
        }
    }

    void FreezePlayerInput()
    {
        player.BroadcastMessage("SetFreezeInput", true, SendMessageOptions.RequireReceiver);
    }

    public void ContinueToNext()
    {
        CashManager.ResetCash();

        ContinueMenu.Continue -= ContinueToNext;
        SessionManager.currentDifficultyMultiplier += difficultyStep;
        SessionManager.currentRound += 1;
        MusicManager.FadeToNormal();
        Invoke("ChooseLevel", 1.5f);
    }
    
    private void AdvanceDifficulty()
    {
        currentRoundsOfType = 0;
        currentDifficultyMultiplier = 1;

        int d = (int)currentDifficulty;
        d = Mathf.Min(d + 1, 4);
        currentDifficulty = (Difficulty)d;
        
        switch(currentDifficulty)
        {
            case Difficulty.Medium:
                currentMaxRoundsOfType = numMediumRounds;
                break;
            case Difficulty.Hard:
                currentMaxRoundsOfType = numHardRounds;
                break;
            default:
                currentMaxRoundsOfType = -1;
                break;
        }
    }

    public void spawnLoadScreen()
    {
        LoadingScreen.SpawnLoadingScreen();
    }

    public void LoadMenu()
    {
        SceneManager.LoadSceneAsync(windDownScene);
        Invoke("KillSessionManager", 4);
    }

    public void KillSessionManager()
    {
        Destroy(gameObject);
    }

    public static GameObject GetPlayer()
    {
        if(SessionManager.SessionInstance == null)
        {
            return null;
        }
        else
        {
            return SessionManager.SessionInstance.player;
        }
    }

    public static int CalculateNumberOfStars(int round)
    {
        if(round < 3)
        {
            return 1;
        }
        else if(round < 6)
        {
            return 2;
        }
        else if(round < 10)
        {
            return 3;
        }
        else if(round < 13)
        {
            return 4;
        }
        else
        {
            return 5;
        }
    }

    public static PlayerRanks CalculatePlayerRank(int leaderboardPosition)
    {
        if(leaderboardPosition > 100)
        {
            return PlayerRanks.None;
        }
        else if(leaderboardPosition > 50)
        {
            return PlayerRanks.Top100;
        }
        else if (leaderboardPosition > 10)
        {
            return PlayerRanks.Top50;
        }
        else if(leaderboardPosition > 3)
        {
            return PlayerRanks.Top10;
        }
        else
        {
            return PlayerRanks.Top3;
        }
    }

    private void OnDestroy()
    {
        EnemySpawner.OnLevelClear -= OnLevelClear;
        SceneManager.sceneLoaded -= OnLevelLoad;
    }
}
public enum PlayerRanks { None, Top100, Top50, Top10, Top3 };