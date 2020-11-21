using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : Health
{
    public ActionDelegate OnPlayerDead;

    public override float TakeDamage(Damage damage)
    {
        ScreenShake.Shake(0.3f);

        return base.TakeDamage(damage);
    }

    public override void Die(Damage killingBlow)
    {
        if (DeathSpawn != null)
        {
            DeathSpawn = Instantiate(DeathSpawn, transform.position, transform.rotation);
        }

        if (OnDie != null)
        {
            OnDie(killingBlow);
        }

        if (OnPlayerDead != null)
        {
            OnPlayerDead();
        }

        OnDie = null;
        OnPlayerDead = null;

        gameObject.SetActive(false);
    }
}
