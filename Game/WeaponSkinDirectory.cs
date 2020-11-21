using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponSkinData", menuName = "ScriptableObjects/WeaponSkinData")]
public class WeaponSkinDirectory : ScriptableObject
{
    public Material[] skins;
}
