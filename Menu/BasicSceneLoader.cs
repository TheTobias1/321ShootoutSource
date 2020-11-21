using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BasicSceneLoader : MonoBehaviour
{
    public bool loadOnEscape;
    public int scene = 0;

    private void Update()
    {
        if(loadOnEscape && Input.GetKey(KeyCode.Escape))
        {
            LoadScene(scene);
        }
    }

    public void LoadScene()
    {
        Time.timeScale = 1;
        LoadingScreen.SpawnLoadingScreen();
        SceneManager.LoadSceneAsync(scene);
    }

    public void LoadScene(int index)
    {
        Time.timeScale = 1;
        LoadingScreen.SpawnLoadingScreen();
        SceneManager.LoadSceneAsync(index);
    }

    public void QuitApp()
    {
        Application.Quit();
    }
}
