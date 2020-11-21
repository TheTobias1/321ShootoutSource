using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeOnShoot : MonoBehaviour
{
    public Weapon weapon;
    public float multiplier = 1;

    // Start is called before the first frame update
    void Start()
    {
        weapon.OnShoot += Shoot;
    }

    void Shoot()
    {
        ScreenShake.Shake(weapon.fireRate * multiplier);
    }

    private void OnDestroy()
    {
        weapon.OnShoot -= Shoot;
    }
}
