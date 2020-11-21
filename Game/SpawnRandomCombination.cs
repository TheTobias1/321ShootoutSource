using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRandomCombination : MonoBehaviour
{
    public Transform chosenParent;
    public bool zeroPosition;
    public bool zeroRotation;

    public List<GameObject> choices;
    public int numberOfSpawns;
    public bool spawnOnlyOnce;

    public float delay;
    public float delayBetweenSpawns;

    public bool spawnOnStart;

    // Start is called before the first frame update
    void Start()
    {
        if(spawnOnStart)
        {
            if(delay > 0)
            {
                Initiate();
            }
            else
            {
                Spawn();
            }
        }
    }

    public void Initiate()
    {
        Invoke("Spawn", delay);
    }

    public void Spawn()
    {
        StartCoroutine(SpawnSequence());
    }

    public IEnumerator SpawnSequence()
    {
        for(int i = 0; i < numberOfSpawns; ++i)
        {
            InstantiateChoice(choices[Random.Range(0, choices.Count)]);
            yield return new WaitForSeconds(delayBetweenSpawns);
        }
    }

    public void InstantiateChoice(GameObject chosenPrefab)
    {
        Vector3 pos = transform.position;
        Quaternion rot = transform.rotation;

        if (zeroPosition)
            pos = Vector3.zero;

        if (zeroRotation)
            rot = Quaternion.identity;

        if(chosenParent == null)
        {
            Instantiate(chosenPrefab, pos, rot);
        }
        else
        {
            Instantiate(chosenPrefab, pos, rot, chosenParent);
        }

        if(spawnOnlyOnce)
        {
            choices.Remove(chosenPrefab);
        }
    }
}
