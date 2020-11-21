using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : HitscanGun
{
    public float thickness;
    public Vector2 rangeDistance;

    protected override DamageDescriptor FireRaycast(Vector3 pos, Vector3 dir, float range, LayerMask mask, float rayThickness = 0.3f)
    {
        DamageDescriptor d = new DamageDescriptor();

        SpawnTrace();

        float r = rangeDistance.y;
        RaycastHit h;
        if(Physics.Raycast(pos, dir, out h, r, mask))
        {
            r = h.distance;
        }

        RaycastHit[] hits = Physics.SphereCastAll(pos, thickness, dir, r, mask);
        int chosenIndex = -1;
        float closest = r;
        int i = 0;

        foreach (RaycastHit hit in hits)
        {
            if(hit.distance < closest)
            {
                chosenIndex = i;
            }
            ++i;
        }

        if(chosenIndex != -1)
        {
            RaycastHit hit = hits[chosenIndex];

            if (damagableTags.Contains(hit.collider.tag))
            {
                //find a damagable component
                IDamagable hitComponenet = hit.collider.GetComponent<IDamagable>();
                if (hitComponenet != null)
                {
                    Damage dmg = damage;
                    dmg.pushForce = dir * damage.pushForce.x + Vector3.up * damage.pushForce.y;
                    if (hit.distance > rangeDistance.x)
                    {
                        float x = (hit.distance - rangeDistance.x) / (rangeDistance.y - rangeDistance.x);
                        float multiplier = 1 - x;
                        dmg.amount = Mathf.Max(damage.amount * multiplier, 0);
                    }

                    //Debug.Log("DAMAGE: " + hit.collider.name + " - " + dmg.amount);

                    hitComponenet.TakeDamage(dmg);
                    if (!d.Initialised())
                    {
                        d = new DamageDescriptor(hit, dir, hitComponenet, damage);
                        if (OnHit != null)
                            OnHit(d, pos);

                        return d;
                    }

                }

                return d;
            }
        }


        return new DamageDescriptor();
    }
}
