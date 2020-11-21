using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponInput : MonoBehaviour
{
    private PlayerManager manager;
    public Weapon weapon;

    public bool playerCanShoot = true;
    
    // Start is called before the first frame update
    void Start()
    {
        manager = GetComponentInParent<PlayerManager>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(manager.PlayerInputState != null)
        {
            bool shoot = manager.PlayerInputState.Shoot && playerCanShoot;
            weapon.Shooting = shoot;

            if(manager.PlayerInputState.Reload)
            {
                weapon.Reload();
            }
        }
    }
}
