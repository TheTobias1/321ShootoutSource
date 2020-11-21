using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundOnEnemyHit : MonoBehaviour
{

    private EnemyHealth enemyHealth;
    public AudioClip[] ShieldHitSounds;
    public AudioClip[] FleshHitSounds;
    public AudioClip DieSound;

    private void Start()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        enemyHealth.OnHitShield += OnHitShield;
        enemyHealth.OnDamage += OnHitFlesh;
        enemyHealth.OnDie += OnDieSound;
    }

    public void OnHitFlesh(Damage dmg)
    {
        SoundFXPlayer.SFX.PlaySound(FleshHitSounds[Random.Range(0,FleshHitSounds.Length)]);
    }

    public void OnHitShield()
    {
        SoundFXPlayer.SFX.PlaySound(ShieldHitSounds[Random.Range(0, ShieldHitSounds.Length)]);
    }

    public void OnDieSound(Damage dmg)
    {
        SoundFXPlayer.SFX.PlaySound(DieSound);
    }

    public void OnDestroy()
    {
        enemyHealth.OnHitShield -= OnHitShield;
        enemyHealth.OnDamage -= OnHitFlesh;
        enemyHealth.OnDie -= OnDieSound;
    }
}
