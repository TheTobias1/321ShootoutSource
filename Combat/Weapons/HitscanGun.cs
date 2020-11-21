using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HitscanGun : Weapon
{
    public bool canReload;
    public int ammoInClip;
    public int clipSize;

    public float coneGrowRate;
    public float coneShrinkRate;
    public Vector2 coneRange;

    public float range;
    public LayerMask mask;

    protected float currentConeSize;

    public GameObject ImpactFX;
    public ParticleSystem TraceFX;
    public TextMeshPro clipCounter;

    public float reloadTime = 1;
    private float lastTakeOutTime = 0;

    public bool startingWeapon;

    void Start()
    {
        if (startingWeapon)
        {
            if(PowerupManager.GetPowerupStatus(Powerups.ExtendedMag))
            {
                clipSize += Mathf.FloorToInt((float)clipSize / 2);
                ammoInClip = clipSize;
            }
            if(PowerupManager.GetPowerupStatus(Powerups.ExtraAmmo))
            {
                ammunition.SetAmmo(120);
            }
        }

        lastTakeOutTime = Time.time;
        if(ImpactFX != null)
            OnHit += SpawnEffect;
        currentConeSize = coneRange.x;
    }

    public override void Shoot()
    {
        if(clipSize > 0)
            ammoInClip--;

        Vector3 dir;

        if(Mathf.Abs(currentConeSize) > 0)
        {
            dir = CalculateConeFire();
        }
        else
        {
            dir = spawner.forward;
        }

        FireRaycast(spawner.transform.position, dir, range, mask);
        currentConeSize = Mathf.Clamp(currentConeSize + coneGrowRate, coneRange.x, coneRange.y);

        PlaySound();

        if(ammoInClip <= 0)
        {
            Reload();
        }
    }

    public override void AddAmmoToClip(int amount = -1)
    {
        Debug.Log("Add: " + amount.ToString());
        if(amount == -1)
        {
            ammoInClip += clipSize;
        }
        else
        {
            ammoInClip += amount;
        }
    }

    protected override DamageDescriptor FireRaycast(Vector3 pos, Vector3 dir, float range, LayerMask mask, float rayThickness = 3f)
    {
        DamageDescriptor d = base.FireRaycast(pos, dir, range, mask);

        if(Vector3.Distance(d.point, pos) > 3)
        {
            /*if(!d.Initialised())
                SpawnTrace(pos + (dir * range));
            else
            {
                SpawnTrace(d.point);
            }*/
            SpawnTrace(d.point);
        }


        return d;
    }

    protected override void Update()
    {
        base.Update();
        currentConeSize = Mathf.Clamp(currentConeSize - coneShrinkRate * Time.deltaTime, coneRange.x, coneRange.y);

        if(clipCounter != null)
        { 
            clipCounter.SetText("" + ammoInClip);
        }
    }

    protected virtual Vector3 CalculateConeFire()
    {
        Vector3 dir = spawner.forward;

        Vector3 hVector = spawner.right * Random.Range(-currentConeSize, currentConeSize);
        Vector3 vVector = spawner.up * Random.Range(-currentConeSize, currentConeSize);

        return dir + (hVector + vVector) * 0.01f;
    }

    protected void SpawnEffect(DamageDescriptor d, Vector3 origin)
    {
        if(ImpactFX != null && d.component == null)
            MF_AutoPool.Spawn(ImpactFX, d.point, Quaternion.LookRotation(d.normal));
    }

    protected void SpawnTrace(Vector3 point)
    {
        if(TraceFX != null)
        {
            TraceFX.transform.LookAt(point);
            TraceFX.Play();
        }
    }

    protected void SpawnTrace()
    {
        if (TraceFX != null)
        {
            TraceFX.Play();
        }
    }

    public override bool CanShoot()
    {
        return isReady && gunEnabled && ammoInClip > 0;
    }

    protected override IEnumerator TakeOutSequence()
    {
        if (OnTakeOut != null)
        {
            OnTakeOut();
        }

        yield return new WaitForSeconds(takeOutTime);

        isReady = true;
        if(ammoInClip == 0)
        {
            Reload();
        }
    }

    public override void Reload()
    {
        if (!isReady || AmmoLeft() == 0 || ammoInClip == clipSize || !canReload)
            return;

        base.Reload();
        reloadRoutine = ReloadSequence();
        StartCoroutine(reloadRoutine);
    }

    protected virtual IEnumerator ReloadSequence()
    {
        isReady = false;
        yield return new WaitForSeconds(reloadTime);
        isReady = true;
        int remainder = ammoInClip;
        ammoInClip = TakeAmmo(clipSize);
        TakeAmmo(-remainder);
    }

    public override int GetAmmo()
    {
        return ammoInClip;
    }

    public override void SetAmmo(int ammo)
    {
        if (PowerupManager.GetPowerupStatus(Powerups.ExtendedMag))
        {
            clipSize += Mathf.FloorToInt((float)clipSize / 2); ;
        }

        if(ammo == -1)
        {
            ammoInClip = clipSize;
        }
        else
        {
            ammoInClip = ammo;
        }
            
    }

    private void OnEnable()
    {
        if(lastTakeOutTime + 7 < Time.time)
        {
            if(canReload)
            {
                if(ammunition.AmmoLeft() > ammoInClip)
                {
                    int remainder = ammoInClip;
                    ammoInClip = TakeAmmo(clipSize);
                    TakeAmmo(-remainder);
                }
            }
            lastTakeOutTime = Time.time;
        }
    }

    private void OnDisable()
    {
        lastTakeOutTime = Time.time;
    }
}
