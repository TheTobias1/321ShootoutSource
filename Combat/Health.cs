using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour, IDamagable
{
    public float maxHealth;
    public float currentHealth;

    public GameObject DeathSpawn;
    public DamageDelegate OnDamage;
    public DamageDelegate OnDie;

    public bool dontDestroyOnDie = false;


    public ActionDelegate OnHeal;

    public virtual float TakeDamage(Damage damage)
    {
        currentHealth -= damage.amount;

        if(currentHealth <= 0)
        {
            Die(damage);
        }
        else if(OnDamage != null)
        {
            OnDamage(damage);
        }

        return currentHealth;
    }

    public float AddHealth(float amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        OnHeal?.Invoke();
        return currentHealth;
    }

    public virtual void Die(Damage killingBlow)
    {
        if(DeathSpawn != null)
        {
            DeathSpawn = Instantiate(DeathSpawn, transform.position, transform.rotation);
        }
        
        if(OnDie != null)
        {
            OnDie(killingBlow);
        }

        OnDie = null;

        if(!dontDestroyOnDie)
            Destroy(gameObject);
    }
}
