using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string dynamicScene;

    public GameObject levelClearObject;

    public Camera deathCamPrefab;
    private Camera playerCamera;
    private Camera currentCamera;

    private GameObject[] enemies;
    private bool checkingEnemies = true;
    private IEnumerator checkRoutine;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ReloadSequence());
    }

    private void Update()
    {
        if(!checkingEnemies)
        {
            checkingEnemies = true;
            checkRoutine = CheckEnemies();
            StartCoroutine(checkRoutine);
        }
    }

    public void PlayerDied()
    {
        currentCamera = Instantiate(deathCamPrefab, playerCamera.transform.position, playerCamera.transform.rotation);

        StopCoroutine(checkRoutine);
        CancelInvoke("ReloadScene");
        Invoke("ReloadScene", 3);
    }

    public void ReloadScene()
    {
        StopCoroutine(checkRoutine);
        checkingEnemies = true;
        CancelInvoke("ReloadScene");
        StartCoroutine(ReloadSequence());
    }

    IEnumerator ReloadSequence()
    {
        if(SceneManager.sceneCount > 1)
        {
            AsyncOperation unloading = SceneManager.UnloadSceneAsync(dynamicScene);

            while (!unloading.isDone)
            {
                yield return null;
            }
        }

        AsyncOperation loading = SceneManager.LoadSceneAsync(dynamicScene, LoadSceneMode.Additive);

        while (!loading.isDone)
        {
            yield return null;
        }

        yield return null;

        if (currentCamera != null)
        {
            Destroy(currentCamera.gameObject);
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        PlayerHealth hp = player.GetComponent<PlayerHealth>();
        if(hp != null)
            hp.OnPlayerDead += PlayerDied;

        playerCamera = Camera.main;
        yield return null;

        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        checkingEnemies = false;
        StopCoroutine(checkRoutine);
    }

    IEnumerator CheckEnemies()
    {
        bool levelClear = true;

        foreach(GameObject enemy in enemies)
        {
            if(enemy != null)
            {
                if (enemy.tag == "Enemy")
                {
                    levelClear = false;
                    break;
                }
            }

            yield return null;
        }

        if(levelClear)
        {
            levelClearObject.SetActive(true);
        }
        else
        {
            yield return new WaitForSeconds(0.1f);
            checkingEnemies = false;
        }
    }
}
