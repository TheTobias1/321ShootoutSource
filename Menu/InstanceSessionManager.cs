using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InstanceSessionManager : MonoBehaviour
{
    [Header("Game")]
    public SessionManager facilitySessionManager;
    public SessionManager simulationSessionManager;
    public SessionManager surfaceSessionManager;
    public SessionManager spaceSessionManager;
    public SessionManager ruinSessionManager;

    private enum Worlds {Simulation = 0, Facility = 1, Surface = 2, Space = 3, Ruin = 4}
    private int intendedSessionManager;

    [Header("UI")]
    public Text managerText;
    public Image managerBackground;

    public GameObject successPanel;
    public GameObject failurePanel;
    public GameObject startPanel;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        SetIdealResolution();
        Time.timeScale = 1;
        managerText.CrossFadeAlpha(0, 0, false);
        managerBackground.CrossFadeAlpha(0, 0, false);
    }

    public void SetIdealResolution()
    {
        Resolution closest = new Resolution();

        float m = (float)Screen.width / (float)Screen.height;

        float ScreenWidthInch = Screen.width / Screen.dpi;

        if (ScreenWidthInch < 6f)
        {
            if(PlayerPrefs.HasKey("HD") && PlayerPrefs.GetInt("HD") == 1)
            {
                closest.height = 720;
            }
            else
                closest.height = 480;
        }
            
        else
        {
            if(PlayerPrefs.HasKey("HD") && PlayerPrefs.GetInt("HD") == 1)
            {
                closest.height = 1080;
            }
            else
                closest.height = 640;
        }



        closest.width = Mathf.CeilToInt((float)closest.height * m);


        Screen.SetResolution(closest.width, closest.height, true, 60);
    }

    public void SetSessionManager(int m)
    {
        intendedSessionManager = m;
        Worlds world = (Worlds)m;

        if (managerText != null)
            managerText.text = world.ToString();
    }

    public void PlayGame()
    {
        int chosenWorld = intendedSessionManager;
        Worlds world = (Worlds)chosenWorld;
        SessionManager session = null;
        string t = "";

        switch(world)
        {
            case Worlds.Simulation:
                Debug.Log("SIM");
                session = simulationSessionManager;
                t = "CHAPTER I: COMBAT TRAINING";
                break;
            case Worlds.Facility:
                session = facilitySessionManager;
                t = "CHAPTER II: CONTACT";
                break;
            case Worlds.Surface:
                session = surfaceSessionManager;
                t = "CHAPTER III: WALKING THE SURFACE";
                break;
            case Worlds.Space:
                session = spaceSessionManager;
                t = "CHAPTER IV: JOURNEY TO EARTH";
                break;
            case Worlds.Ruin:
                session = ruinSessionManager;
                t = "CHAPTER V: A LONG RUINED WORLD";
                break;
        }

        if(session == null)
        {
            SceneManager.LoadScene("World0_2");
            return;
        }

        if(PlayerPrefs.HasKey(session.worldName))
        {
            SessionManager newSession = Instantiate(session, Vector3.zero, Quaternion.identity);
            newSession.BeginGame();
        }
        else
        {
            StartCoroutine(ChapterTextSequence(session, t));
        }

    }

    public IEnumerator ChapterTextSequence(SessionManager session, string text)
    {
        managerText.text = text;
        managerText.CrossFadeAlpha(1, 2, false);
        managerBackground.CrossFadeAlpha(1, 2, false);

        yield return new WaitForSeconds(5);

        managerText.CrossFadeAlpha(0, 2, false);

        yield return new WaitForSeconds(2);

        SessionManager newSession = Instantiate(session, Vector3.zero, Quaternion.identity);
        newSession.BeginGame();
    }

    public void OnPayment(bool success)
    {
        startPanel.SetActive(false);
        failurePanel.SetActive(!success);
        successPanel.SetActive(success);
    }

}
