using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen loadScreenPrefab;

    public Image loadingScreen;
    public Text text;
    public int curScene;

    int dots = 0;
    float nextTime = 0;

    private void Awake()
    {
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }

    void Start()
    {
        loadingScreen.CrossFadeAlpha(0, 0, true);
        text.CrossFadeAlpha(0, 0, true);

        loadingScreen.CrossFadeAlpha(1, 0.5f, true);
        text.CrossFadeAlpha(1, 0.75f, true);
    }

    private void Update()
    {
        if(nextTime < Time.time)
        {
            nextTime = Time.time + 0.2f;

            ++dots;
            if(dots >= 3)
            {
                dots = 0;
            }

            string d = "";
            for(int i = 0; i <= dots; ++i)
            {
                d += ".";
            }

            text.text = "LOADING" + d;
        }
    }

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if(arg0.buildIndex == curScene)
        {
            return;
        }

        loadingScreen.CrossFadeAlpha(0, 0.5f, true);
        text.CrossFadeAlpha(0, 0.5f, true);

        SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
        Invoke("Kill", 0.5f);
    }

    void Kill()
    {
        Destroy(gameObject);
    }

    public static void SpawnLoadingScreen()
    {
        if(LoadingScreen.loadScreenPrefab == null)
        {
            LoadingScreen.loadScreenPrefab = Resources.Load<LoadingScreen>("Prefabs/Menus HUD UI/LoadingScreen");
        }

        if(LoadingScreen.loadScreenPrefab != null)
        {
            LoadingScreen l = Instantiate(LoadingScreen.loadScreenPrefab);
            l.curScene = SceneManager.GetActiveScene().buildIndex;
            DontDestroyOnLoad(l.gameObject);
        }
    }
}
