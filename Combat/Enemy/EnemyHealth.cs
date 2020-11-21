using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : Health
{
    public DeadBody enemyBodyPrefab;

    public ActionDelegate OnHitShield;
    public ActionDelegate OnBreakShield;

    public float curShield = 60;
    public float maxShield = 60;

    public float shieldRechargeSpeed = 10;
    public float shieldRechargeDelay = 3;
    private float shieldRechargeTime;

    public int cashWorth = 50;

    public override float TakeDamage(Damage damage)
    {
        if(curShield <= 0)
            return base.TakeDamage(damage);
        else
        {
            shieldRechargeTime = Time.time + shieldRechargeDelay;

            curShield -= damage.amount;

            if (curShield <= 0)
            {
                damage.amount = Mathf.Abs(curShield);
                TakeDamage(damage);
                curShield = 0;

                if (OnBreakShield != null)
                {
                    OnBreakShield();
                }
            }
            else if (OnHitShield != null)
            {
                OnHitShield();
            }

            return currentHealth;
        }
    }

    private void Update()
    {
        if(Time.time > shieldRechargeTime && curShield < maxShield)
        {
            curShield = Mathf.Min(curShield + shieldRechargeSpeed * Time.deltaTime, maxShield);
        }
    }

    public override void Die(Damage killingBlow)
    {
        CashManager.OnScore(cashWorth);

        if(enemyBodyPrefab != null)
        {
            DeadBody instance = Instantiate(enemyBodyPrefab, transform.position, transform.rotation);
            instance.Push(killingBlow.pushForce);
        }
        base.Die(killingBlow);
    }
}
