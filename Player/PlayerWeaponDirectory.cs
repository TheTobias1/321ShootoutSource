using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerWeaponData", menuName = "ScriptableObjects/PlayerWeaponData")]
public class PlayerWeaponDirectory : ScriptableObject
{

    public WeaponPrefab[] prefabs;
}

[System.Serializable]
public struct WeaponPrefab
{
    public WeaponNames weapon;
    public Weapon weaponPrefab;
    public GameObject dropPrefab;
}

public enum WeaponNames {BattleRifle = 0, Pistol = 1, SubmachineGun = 2, AssaultRifle = 3, LMG = 4, Shotgun = 5, Raygun = 6};
