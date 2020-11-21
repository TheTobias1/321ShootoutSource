using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Damage
{
    public float amount;
    public DamageSource source;
    public DamageType damageType;
    public Vector3 pushForce;

    public Damage(float dmg, DamageSource src, DamageType typ)
    {
        amount = dmg;
        source = src;
        damageType = typ;
        pushForce = Vector3.zero;
    }
    public Damage(float dmg, Vector3 force, DamageSource src, DamageType typ)
    {
        amount = dmg;
        source = src;
        damageType = typ;
        pushForce = Vector3.zero;
    }
}

public struct DamageDescriptor
{
    public Vector3 point;
    public Vector3 normal;
    public Vector3 direction;
    public float distance;

    public IDamagable component;
    public Damage damage;

    public DamageDescriptor(RaycastHit h, Vector3 dir, IDamagable c, Damage dmg)
    {
        point = h.point;
        normal = h.normal;
        direction = dir;
        component = c;
        damage = dmg;
        distance = h.distance;
    }

    public DamageDescriptor(Vector3 pos, Vector3 norm, Vector3 dir, IDamagable c, Damage dmg)
    {
        point = pos;
        normal = norm;
        direction = dir;
        component = c;
        damage = dmg;
        distance = 0;
    }

    public bool Initialised()
    {
        if(component != null)
        {
            return true;
        }
        return false;
    }
}

public enum DamageSource {Player, Enemy};
public enum DamageType {LightBullet, HeavyBullet, Melee, Explosion};

public interface IDamagable
{
    float TakeDamage(Damage damage);
}

public delegate void DamageDelegate(Damage damage);
