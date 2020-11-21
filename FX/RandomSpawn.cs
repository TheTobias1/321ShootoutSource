using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpawn : MonoBehaviour
{
    public GameObject[] objects;

    public bool spawnOnStart = true;
    public bool pooled;
    private bool hasSpawned;

    public float delay;

    public bool keepRotation = true;

    private void Start()
    {
        if(spawnOnStart && !pooled)
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

    private void OnEnable()
    {
        if (spawnOnStart && pooled)
        {
            if (delay > 0)
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
        if(!pooled)
            Instantiate(objects[Random.Range(0, objects.Length)], transform.position, keepRotation ? transform.rotation : Quaternion.identity);
        else
        {
            if(!hasSpawned)
            {
                int i = Random.Range(0, objects.Length);
                objects[i].SetActive(true);

                objects[i].transform.position = transform.position;
                objects[i].transform.rotation = keepRotation ? transform.rotation: Quaternion.identity;

                hasSpawned = true;
            }

        }
    }
}
