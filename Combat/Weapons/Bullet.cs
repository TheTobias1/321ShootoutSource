using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject deathPrefab;
    Damage damage;

    public string damagingTag;
    public float timeout;
    public float rayRange = 3.5f;

    public LayerMask mask;

    Vector3 vel;

    public void Init(Damage dmg, Vector3 force)
    {
        damage = dmg;
        damage.pushForce = Vector3.zero;
        vel = force;

        CancelInvoke("Timeout");
        Invoke("Timeout", timeout);
    }

    private void Update()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward, out hit, rayRange, mask))
        {
            if (hit.collider.tag == damagingTag)
            {
                if(hit.collider != null)
                {
                    Health hp = hit.collider.gameObject.GetComponent<Health>();
                    if(hp != null)
                        hp.TakeDamage(damage);
                }
                    
            }

            if (deathPrefab != null && hit.collider.tag != "Player")
            {
                MF_AutoPool.Spawn(deathPrefab, hit.point, transform.rotation);
                //Instantiate(deathPrefab, hit.point, transform.rotation);
            }

            Kill();
        }
    }

    private void FixedUpdate()
    {
        transform.Translate(vel * Time.fixedDeltaTime, Space.World);
    }

    public void Timeout()
    {
        if (deathPrefab != null)
        {
             Instantiate(deathPrefab, transform.position, transform.rotation);
        }
        Kill();
    }

    public void Kill()
    {
        CancelInvoke("Timeout");
        MF_AutoPool.Despawn(gameObject);
    }
}
