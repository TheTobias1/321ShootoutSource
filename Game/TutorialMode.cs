using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialMode : MonoBehaviour
{

    public GameObject endScreen;
    PlayerHealth hp;
    bool tutorialFinished;

    // Start is called before the first frame update
    void Start()
    {
        hp = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!tutorialFinished && hp.currentHealth > hp.maxHealth - 5)
        {
            tutorialFinished = true;
            StartCoroutine(EndTutorial());
        }
    }

    IEnumerator EndTutorial()
    {
        endScreen.SetActive(true);

        yield return new WaitForSeconds(3);

        LoadingScreen.SpawnLoadingScreen();
        yield return null;
        SceneManager.LoadSceneAsync(0);
    }
}
