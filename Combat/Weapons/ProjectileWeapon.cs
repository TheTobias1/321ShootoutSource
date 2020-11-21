using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWeapon : Weapon
{
    public GameObject projectilePrefab;
    public float burstFireRate;
    public int burstSize;
    int currentBurstCount;

    public float coneSize;
    public float projectileForce;
    public float projectileOffset;

    public override void Shoot()
    {
        FireProjectile();
        
        if(++currentBurstCount >= burstSize)
        {
            nextShot += burstFireRate;
            currentBurstCount = 0;
        }
    }

    protected void FireProjectile()
    {
        Vector3 dir = spawner.transform.forward;
        dir += spawner.transform.right * Random.Range(-coneSize, coneSize);
        dir += spawner.transform.up * Random.Range(-coneSize, coneSize);

        Bullet instance = MF_AutoPool.Spawn(projectilePrefab, spawner.position, spawner.transform.rotation).GetComponent<Bullet>();

        //Bullet instance = Instantiate(projectilePrefab, spawner.position, spawner.transform.rotation);
        instance.Init(damage, dir.normalized * projectileForce);

        PlaySound();
    }
}
