using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SquadAI;

public class MapConfigurationManager : MonoBehaviour
{
    public MapConfiguration[] configPool;
    MapConfiguration currentConfig;

    public void ChooseConfiguration(Difficulty selectedDifficulty)
    {
        List<MapConfiguration> possibleChoices = new List<MapConfiguration>();
        int count = 0;

        foreach(MapConfiguration c in configPool)
        {
            if(c.difficulty == selectedDifficulty)
            {
                possibleChoices.Add(c);
                ++count;
            }
        }

        int selectedConfig = Random.Range(0, count);
        MapConfiguration config = possibleChoices[selectedConfig];

        currentConfig = config;
        if (config.weaponSpawner != null)
            config.weaponSpawner.SetActive(true);

        currentConfig.Activate();
    }

    public void PlayMap()
    {
        currentConfig.spawner.enabled = true;
    }
}

[System.Serializable]
public struct MapConfiguration
{
    public EnemySpawner spawner;
    public GameObject weaponSpawner;
    public Difficulty difficulty;

    public void Activate()
    {
        spawner.gameObject.SetActive(true);
    }
}

public enum Difficulty { Easy = 0, Medium = 1, Hard  = 2, Elite = 3};
