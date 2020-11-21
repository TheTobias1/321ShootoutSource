using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSpawner : MonoBehaviour
{
    public List<Transform> spawnPoints = new List<Transform>();
    public SpawnableWeapon[] weaponPool;
    public int numberOfWeapons;

    public bool spawnOnStart;

    // Start is called before the first frame update
    void Start()
    {
        numberOfWeapons = Mathf.Min(numberOfWeapons, spawnPoints.Count);
        if (spawnOnStart)
            spawnWeapons();
    }

    public void spawnWeapons()
    {
        for (int i = 0; i < numberOfWeapons; ++i)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
            GameObject dropPrefab = SpawnableWeapon.ChooseRandomWeapon(weaponPool);
            GameObject instance = Instantiate(dropPrefab, spawnPoint.position, spawnPoint.rotation);
            instance.SendMessage("OnWorldSpawn", SendMessageOptions.DontRequireReceiver);
            spawnPoints.Remove(spawnPoint);
        }
    }

}

[System.Serializable]
public struct SpawnableWeapon
{
    public GameObject weaponPrefab;
    public float probability;

    public static GameObject ChooseRandomWeapon(SpawnableWeapon[] pool)
    {
        float sum = 0;
        foreach(SpawnableWeapon s in pool)
        {
            sum += s.probability;
        }

        float r = Random.Range(0, sum);
        sum = 0;
        foreach (SpawnableWeapon s in pool)
        {
            sum += s.probability;
            if(sum >= r)
            {
                return s.weaponPrefab;
            }
        }

        return null;
    }
}