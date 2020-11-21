using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPainting : MonoBehaviour
{
    public WeaponNames gun;

    public WeaponSkinLoader model;
    private Material[] skins;

    public GameObject skinCamera;
    public GameObject modelObject;

    private void Start()
    {
        skins = Resources.Load<WeaponSkinDirectory>("Custom Objects/WeaponSkins").skins;
    }

    public void StoreGunPaint(int skin)
    {
        model.ManualLoad(skin);
        PlayerPrefs.SetInt(WeaponSkinLoader.GetWeaponKey(gun), skin);
        PlayerPrefs.Save();
    }


    private void OnEnable()
    {
        for(int i = 0; i < skinCamera.transform.childCount; ++i)
        {
            skinCamera.transform.GetChild(i).gameObject.SetActive(false);
        }

        modelObject.SetActive(true);
    }
}
