using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponLoader : MonoBehaviour
{
    public static Dictionary<WeaponNames, WeaponPrefab> WeaponSet;
    public PlayerWeaponDirectory CurrentDirectory;

    private void Awake()
    {
        PlayerWeaponLoader.WeaponSet = new Dictionary<WeaponNames, WeaponPrefab>();

        foreach(WeaponPrefab p in CurrentDirectory.prefabs)
        {
            WeaponSet.Add(p.weapon, p);
        }
    }

    public static WeaponPrefab GetWeapon(WeaponNames name)
    {
        if(WeaponSet.ContainsKey(name))
        {
            return WeaponSet[name];
        }
        return new WeaponPrefab();
    }
}
