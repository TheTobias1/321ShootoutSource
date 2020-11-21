using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class HealingGun : Weapon
{
    public PlayerMovement movement;
    public Animator anim;
    public PutAwayAnimations putAwayAnimations;
    private Health hp;

    public float healAmount = 34;
    public float healTime = 1;

    public float moveSpeed;
    public float originalPlayerSpeed;

    public AudioClip healSound;

    private bool switchedBack;

    private float failSafeStamp;
    private bool failSafe;

    public static event Action<float> OnHeal;

    private void OnValidate()
    {
        PlayerMovement p = GetComponentInParent<PlayerMovement>();
        if(p != null)
            originalPlayerSpeed = p.speed;
    }

    public void Start()
    {
        hp = GetComponentInParent<Health>();
        hp.OnDamage += OnDamage;

        if(PowerupManager.GetPowerupStatus(Powerups.StrongHealthPacks))
        {
            healAmount += healAmount;
        }

        OnTakeOut += TakingOut;
    }

    public void OnDamage(Damage dmg)
    {
        if(!switchedBack)
            Shoot();

        if(gameObject.activeInHierarchy)
            ScreenShake.Shake(4f);
    }

    public override void Shoot()
    {
        movement.speed = originalPlayerSpeed;
        Debug.Log("BREAK FROM HEAL");
        if(!switchedBack)
        {
            StopAllCoroutines();
            switchedBack = true;
            manager.SwitchWeapon(manager.GetCurrentWeapon(), this);
        }

        gunLocked = false;
    }

    void TakingOut()
    {
        gunLocked = true;
        switchedBack = false;
    }

    protected override IEnumerator TakeOutSequence()
    {
        gunLocked = true;
        switchedBack = false;
        //failSafeStamp = Time.time + takeOutTime + healTime + 0.2f;
        //failSafe = true;

        putAwayAnimations.ResetPosition();

        if (!PowerupManager.GetPowerupStatus(Powerups.MobileHealthPacks))
            movement.speed = moveSpeed;
        anim.SetTrigger("Play");

        if (OnTakeOut != null)
        {
            OnTakeOut();
        }

        yield return new WaitForSeconds(takeOutTime);

        isReady = true;

        yield return new WaitForSeconds(healTime);

        movement.speed = originalPlayerSpeed;
        TakeAmmo(1);
        Health hp = GetComponentInParent<Health>();

        if(hp != null)
        {
            if(OnHeal != null)
                OnHeal(hp.currentHealth / hp.maxHealth);

            hp.AddHealth(healAmount);
        }
       
        SoundFXPlayer.SFX.PlaySound(healSound);
        Shoot();
    }

    protected override void Update()
    {
        base.Update();

        /*if(failSafeStamp < Time.time && failSafe)
        {
            gameObject.SetActive(false);
            movement.speed = originalPlayerSpeed;
            failSafeStamp = 0;
            failSafe = false;
        }*/
    }

    private void OnDisable()
    {
        movement.speed = originalPlayerSpeed;
    }

    public void OnDestroy()
    {
        OnTakeOut -= TakingOut;

        if (hp != null)
            hp.OnDamage -= OnDamage;
    }

}
