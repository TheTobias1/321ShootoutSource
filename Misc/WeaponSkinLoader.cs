using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSkinLoader : MonoBehaviour
{
    public WeaponNames gunName;
    public WeaponSkinDirectory skinDirectory;
    private Material[] weaponSkins;
    private MeshRenderer mesh;
    public int materialIndex;

    public bool loadOnStart = true;

    // Start is called before the first frame update
    void Start()
    {
        weaponSkins = Resources.Load<WeaponSkinDirectory>("Custom Objects/WeaponSkins").skins;
        mesh = GetComponent<MeshRenderer>();
        string key = WeaponSkinLoader.GetWeaponKey(gunName);

        Material skin = weaponSkins[0];


        if(loadOnStart)
        {
            if(PlayerPrefs.HasKey(key))
            {
                skin = weaponSkins[PlayerPrefs.GetInt(key)];
            }

            Material[] weaponMats = mesh.materials;
            weaponMats[materialIndex] = skin;
            mesh.materials = weaponMats;
        }
    }

    public void ManualLoad(int skin)
    {
        Material s = weaponSkins[skin];
        Material[] weaponMats = mesh.materials;
        weaponMats[materialIndex] = s;
        mesh.materials = weaponMats;
    }

    public static string GetWeaponKey(WeaponNames w)
    {
        return w.ToString() + "Skin";
    }
}
