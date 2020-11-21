using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float closeDamage;
    public float farDamage;
    public Vector2 radius;
    public LayerMask mask;
    public List<string> damagableTags;
    public DamageSource source;

    public float force;
    public float timeout;

    // Start is called before the first frame update
    void Start()
    {
        Explode();
        Invoke("Timeout", timeout);
    }

    public void Explode()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, radius.y, mask);

        foreach(Collider col in cols)
        {
            if(damagableTags.Contains(col.transform.tag))
            {
                float amount = Vector3.Distance(transform.position, col.transform.position) > radius.x ? farDamage : closeDamage;
                Damage damage = new Damage(amount, source, DamageType.Explosion);
                
                
                if(col.transform.tag == "Player")
                {

                    ScreenShake.screenShaker.SetShake(amount / closeDamage * 2.5f);
                    RaycastHit hit;
                    if(Physics.Raycast(transform.position, col.transform.position - transform.position, out hit))
                    {
                        if(hit.collider.tag == "Player")
                        {
                            Health hp = col.GetComponent<Health>();
                            if(hp != null)
                                hp.TakeDamage(damage);
                        }
                        
                    }
                }
                else
                {
                    col.GetComponent<Health>().TakeDamage(damage);
                }
            }

            Rigidbody rb = col.GetComponent<Rigidbody>();
            if(rb != null)
            {
                Vector3 dir = col.transform.position - transform.position;
                dir = dir.normalized + Vector3.up;

                rb.AddForce(dir * force);
            }
        }
    }

    void Timeout()
    {
        Destroy(gameObject);
    }
}
