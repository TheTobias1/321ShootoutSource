using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public WeaponNames thisWeapon;
    public Transform spawner;
    public AudioSource audioSource;
    public AmmoModule ammunition;
    protected PlayerWeaponManager manager;

    public delegate void shootDelegate(DamageDescriptor damageDone, Vector3 origin);

    public ActionDelegate OnReload;
    protected IEnumerator reloadRoutine;
    public ActionDelegate OnShoot;
    public ActionDelegate OnTakeOut;
    public ActionDelegate OnPutAway;
    public shootDelegate OnHit;

    private bool shooting;
    protected bool isReady = true;

    protected float nextShot = 0;

    public bool gunLocked = false;
    public bool gunEnabled = true;
    public float fireRate;
    public Damage damage;

    public string[] DamagableTags;
    protected HashSet<string> damagableTags;

    public float takeOutTime;
    public float putAwayTime;

    public LayerMask damagableMask;

    private bool inputFrozen;

    public bool Shooting
    {
        get
        {
            return shooting && isReady && gunEnabled;
        }
        set
        {
            shooting = value;
        }
    }

    public virtual void SetAmmo(int ammo)
    {
        return;
    }

    public virtual int GetAmmo()
    {
        return -1;
    }

    protected virtual void Awake()
    {
        damagableTags = new HashSet<string>();
        foreach(string t in DamagableTags)
        {
            damagableTags.Add(t);
        }

        manager = GetComponentInParent<PlayerWeaponManager>();
        if(manager != null)
        {
            spawner = manager.spawner;
        }
        
    }



    public bool Ready
    {
        get
        {
            return isReady && gunEnabled;
        }
    }

    public abstract void Shoot();

    protected virtual void Update()
    {
        if(Shooting && Time.time > nextShot && CanShoot() && !inputFrozen)
        {
            nextShot = Time.time + fireRate;
            Shoot();
            if (OnShoot != null)
                OnShoot();
        }
    }

    public virtual bool CanShoot()
    {
         return Ready;
    }

    public float TakeOutWeapon()
    {
        isReady = false;
        if (OnTakeOut != null)
        {
            OnTakeOut();
        }
        StartCoroutine(TakeOutSequence());
        return takeOutTime;
    }

    protected virtual IEnumerator TakeOutSequence()
    {
        yield return new WaitForSeconds(takeOutTime);

        isReady = true;
    }

    public float PutAwayWeapon()
    {
        if(reloadRoutine != null)
        {
            StopCoroutine(reloadRoutine);
        }

        isReady = false;
        if(gameObject.activeSelf)
            StartCoroutine(PutAwaySequence());
        return putAwayTime;
    }

    protected virtual IEnumerator PutAwaySequence()
    {
        if (OnPutAway != null)
        {
            OnPutAway();
        }

        yield return new WaitForSeconds(putAwayTime);

        gameObject.SetActive(false);
    }

    public virtual void Reload()
    {
        if (!isReady || AmmoLeft() == 0)
            return;
        if(OnReload != null)
        {
            OnReload();
        }
    }

    public void SetSpawnTransform(Transform spawn)
    {
        spawner = spawn;
    }

    public virtual void AddAmmoToClip(int amount = -1)
    {
        return;
    }

    protected int TakeAmmo(int amount)
    {
        if(ammunition != null)
        {
            return ammunition.TakeAmmo(amount);
        }

        return -1;
    }

    protected int AmmoLeft() 
    {
        return (ammunition != null) ? ammunition.AmmoLeft() : -1;
    }

    private void OnDestroy()
    {
        OnHit = null;
        OnShoot = null;
    }

    //Useful methods
    protected virtual DamageDescriptor FireRaycast(Vector3 pos, Vector3 dir, float range, LayerMask mask, float rayThickness = 0.25f)
    {
        /*RaycastHit hit;
        DamageDescriptor d = new DamageDescriptor();
        if (Physics.Raycast(pos, dir, out hit, range, mask))
        {
            if (damagableTags.Contains(hit.collider.tag))
            {
                //find a damagable component
                IDamagable hitComponenet = hit.collider.GetComponent<IDamagable>();
                if (hitComponenet != null)
                {
                    Damage damageInstance = damage;
                    damageInstance.pushForce = dir * damage.pushForce.magnitude;
                    hitComponenet.TakeDamage(damageInstance);
                    if (!d.Initialised())
                    {
                        d = new DamageDescriptor(hit, dir, hitComponenet, damageInstance);
                        if (OnHit != null)
                            OnHit(d, pos);

                        return d;
                    }

                }
            }
            d = new DamageDescriptor(hit, dir, null, damage);
            if (OnHit != null)
                OnHit(d, pos);

            return d;
        }

        return new DamageDescriptor();*/

        DamageDescriptor d = new DamageDescriptor();


        float r = range;
        Vector3 ogPoint = new Vector3();
        RaycastHit h;
        if (Physics.Raycast(pos, dir, out h, r, mask))
        {
            r = h.distance;
        }

        ogPoint = h.point;

        RaycastHit[] hits = Physics.SphereCastAll(pos, rayThickness, dir, r, damagableMask);
        System.Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance));

        foreach (RaycastHit hit in hits)
        {
            if (damagableTags.Contains(hit.collider.tag))
            {
                //find a damagable component
                IDamagable hitComponenet = hit.collider.GetComponent<IDamagable>();
                if (hitComponenet != null)
                {
                    Damage damageInstance = damage;
                    damageInstance.pushForce = dir * damage.pushForce.magnitude;
                    hitComponenet.TakeDamage(damageInstance);
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

        if(h.collider != null)
        {
            d = new DamageDescriptor(h, dir, null, damage);
            if (OnHit != null)
                OnHit(d, pos);
        }
        else
        {
            d.point = pos + dir;
        }

        return d;
    }

    public virtual void PlaySound()
    {
        if (audioSource == null)
            return;

        if (audioSource.clip == null)
            return;

        audioSource.PlayOneShot(audioSource.clip);
    }

    public void SetFreezeInput(bool i)
    {
        inputFrozen = i;
    }

}
